namespace Core2.Branching;

public sealed class BranchGraphBuilder<T>
{
    private readonly Dictionary<BranchId, BranchNode<T>> _nodes = [];
    private readonly List<BranchEdge> _edges = [];
    private readonly List<BranchEvent<T>> _events = [];
    private BranchFrontier _frontier = BranchFrontier.Empty;

    public BranchFrontier CurrentFrontier => _frontier;

    public BranchNode<T> GetNode(BranchId id) =>
        _nodes.TryGetValue(id, out var node)
            ? node
            : throw new KeyNotFoundException($"No branch node exists for id {id}.");

    public bool TryGetNode(BranchId id, out BranchNode<T>? node) =>
        _nodes.TryGetValue(id, out node);

    public IReadOnlyList<BranchNode<T>> GetFrontierNodes() =>
        _frontier.ActiveNodeIds
            .Select(GetNode)
            .ToArray();

    public BranchGraphBuilder<T> Seed(
        T value,
        IReadOnlyList<IBranchAnnotation>? annotations = null,
        bool selectAsPrincipal = true)
    {
        EnsureEmpty();

        var member = new BranchMember<T>(
            BranchId.New(),
            value,
            [],
            annotations ?? []);

        var family = BranchFamily<T>.FromMembers(
            BranchOrigin.Continuation,
            BranchSemantics.Alternative,
            BranchDirection.Forward,
            [member],
            selectAsPrincipal ? BranchSelection.Principal(member.Id) : BranchSelection.None);

        return Seed(family);
    }

    public BranchGraphBuilder<T> Seed(BranchFamily<T> family)
    {
        EnsureEmpty();
        return ApplyFamily(family, BranchEventKind.Seed, []);
    }

    public BranchGraphBuilder<T> Continue(
        Func<BranchNode<T>, T> projector,
        BranchSemantics semantics = BranchSemantics.Mixed,
        IReadOnlyList<IBranchAnnotation>? annotations = null)
    {
        ArgumentNullException.ThrowIfNull(projector);

        var members = _frontier.ActiveNodeIds
            .Select(id =>
            {
                var node = _nodes[id];
                return new BranchMember<T>(
                    BranchId.New(),
                    projector(node),
                    [id],
                    annotations ?? []);
            })
            .ToArray();

        var family = BranchFamily<T>.FromMembers(
            BranchOrigin.Continuation,
            semantics,
            BranchDirection.Forward,
            members);

        return ApplyFamily(family);
    }

    public BranchGraphBuilder<T> ApplyFamily(
        BranchFamily<T> family,
        BranchEventKind? eventKind = null,
        IReadOnlyList<BranchId>? defaultParents = null,
        IReadOnlyList<IBranchAnnotation>? eventAnnotations = null)
    {
        ArgumentNullException.ThrowIfNull(family);

        var incoming = _frontier;
        IReadOnlyList<BranchId> fallbackParents = defaultParents ?? incoming.ActiveNodeIds;
        var effectiveParentsByChild = new Dictionary<BranchId, IReadOnlyList<BranchId>>();
        var createdIds = new List<BranchId>(family.Members.Count);
        int eventIndex = _events.Count;

        foreach (var member in family.Members)
        {
            if (_nodes.ContainsKey(member.Id))
            {
                throw new InvalidOperationException($"Branch node {member.Id} already exists in the graph.");
            }

            IReadOnlyList<BranchId> parents = ResolveParents(member, fallbackParents);
            ValidateParents(parents);

            int depth = parents.Count == 0 ? 0 : parents.Max(parent => _nodes[parent].Depth) + 1;
            _nodes[member.Id] = new BranchNode<T>(
                member.Id,
                member.Value,
                depth,
                eventIndex,
                member.Annotations);

            effectiveParentsByChild[member.Id] = parents;
            createdIds.Add(member.Id);
        }

        var childrenPerParent = effectiveParentsByChild
            .SelectMany(kvp => kvp.Value.Select(parentId => (parentId, kvp.Key)))
            .GroupBy(pair => pair.parentId)
            .ToDictionary(group => group.Key, group => group.Select(pair => pair.Key).Distinct().Count());

        foreach (var (childId, parents) in effectiveParentsByChild)
        {
            foreach (var parentId in parents)
            {
                _edges.Add(new BranchEdge(
                    parentId,
                    childId,
                    ResolveEdgeKind(family, parents, parentId, childrenPerParent)));
            }
        }

        var outgoingSelection = ResolveSelection(family, incoming, effectiveParentsByChild);
        _frontier = new BranchFrontier(createdIds, outgoingSelection);

        _events.Add(new BranchEvent<T>(
            BranchEventId.New(),
            eventIndex,
            eventKind ?? ResolveEventKind(family, fallbackParents),
            incoming,
            family,
            _frontier,
            createdIds,
            eventAnnotations ?? []));

        return this;
    }

    public BranchGraphBuilder<T> Select(
        BranchId selectedId,
        string? reason = null,
        IReadOnlyList<IBranchAnnotation>? annotations = null)
    {
        if (!_frontier.Contains(selectedId))
        {
            throw new InvalidOperationException($"Cannot select node {selectedId} because it is not in the current frontier.");
        }

        var incoming = _frontier;
        _frontier = new BranchFrontier(
            incoming.ActiveNodeIds,
            BranchSelection.Explicit(selectedId, reason));

        _events.Add(new BranchEvent<T>(
            BranchEventId.New(),
            _events.Count,
            BranchEventKind.Selection,
            incoming,
            null,
            _frontier,
            [],
            annotations ?? []));

        return this;
    }

    public BranchGraph<T> Build() =>
        new(_nodes.Values.OrderBy(node => node.EventIndex).ThenBy(node => node.Depth).ToArray(), _edges.ToArray(), _events.ToArray());

    private void EnsureEmpty()
    {
        if (_events.Count > 0 || _nodes.Count > 0 || _edges.Count > 0)
        {
            throw new InvalidOperationException("A branch graph can only be seeded once.");
        }
    }

    private static IReadOnlyList<BranchId> ResolveParents(
        BranchMember<T> member,
        IReadOnlyList<BranchId> fallbackParents) =>
        member.Parents.Count > 0 ? member.Parents.Distinct().ToArray() : fallbackParents.Distinct().ToArray();

    private void ValidateParents(IReadOnlyList<BranchId> parentIds)
    {
        foreach (var parentId in parentIds)
        {
            if (!_nodes.ContainsKey(parentId))
            {
                throw new InvalidOperationException($"Branch parent {parentId} does not exist in the graph.");
            }
        }
    }

    private static BranchSelection ResolveSelection(
        BranchFamily<T> family,
        BranchFrontier incoming,
        IReadOnlyDictionary<BranchId, IReadOnlyList<BranchId>> parentsByChild)
    {
        if (family.Selection.HasSelection)
        {
            return family.Selection;
        }

        if (!incoming.SelectedId.HasValue)
        {
            return BranchSelection.None;
        }

        var descendants = parentsByChild
            .Where(kvp => kvp.Value.Contains(incoming.SelectedId.Value))
            .Select(kvp => kvp.Key)
            .Distinct()
            .ToArray();

        return descendants.Length == 1
            ? BranchSelection.Principal(descendants[0], "Preserved from prior frontier selection.")
            : BranchSelection.None;
    }

    private static BranchEventKind ResolveEventKind(
        BranchFamily<T> family,
        IReadOnlyList<BranchId> fallbackParents)
    {
        if (fallbackParents.Count == 0)
        {
            return BranchEventKind.Seed;
        }

        if (family.Origin == BranchOrigin.Extension || family.Semantics == BranchSemantics.Lifted)
        {
            return BranchEventKind.Lift;
        }

        return BranchEventKind.Family;
    }

    private static BranchEdgeKind ResolveEdgeKind(
        BranchFamily<T> family,
        IReadOnlyList<BranchId> parents,
        BranchId parentId,
        IReadOnlyDictionary<BranchId, int> childrenPerParent)
    {
        if (family.Origin == BranchOrigin.Extension || family.Semantics == BranchSemantics.Lifted)
        {
            return BranchEdgeKind.Lift;
        }

        if (parents.Count > 1)
        {
            return family.Semantics == BranchSemantics.CoPresent
                ? BranchEdgeKind.Merge
                : BranchEdgeKind.Rejoin;
        }

        return childrenPerParent.TryGetValue(parentId, out int childCount) && childCount > 1
            ? BranchEdgeKind.Split
            : BranchEdgeKind.Continuation;
    }
}
