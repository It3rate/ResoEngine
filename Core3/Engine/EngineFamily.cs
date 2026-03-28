namespace Core3.Engine;

/// <summary>
/// Operation-scoped grouping of subjects read in one frame.
/// The frame is just an existing graded element in a frame role; it is not a
/// special ontology.
/// </summary>
public sealed class EngineFamily
{
    private readonly List<GradedElement> _members = [];

    public EngineFamily(GradedElement frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        Frame = frame;
    }

    public GradedElement Frame { get; }
    public IReadOnlyList<GradedElement> Members => _members;
    public int Count => _members.Count;

    public void AddMember(GradedElement member)
    {
        ArgumentNullException.ThrowIfNull(member);
        _members.Add(member);
    }

    public bool RemoveMember(GradedElement member)
    {
        ArgumentNullException.ThrowIfNull(member);
        return _members.Remove(member);
    }

    public void ClearMembers() => _members.Clear();

    public bool TryReadMember(GradedElement member, out GradedElement? read)
    {
        ArgumentNullException.ThrowIfNull(member);
        return member.TryReferenceToFrame(Frame, out read);
    }

    public CompositeElement GetMemberBoundaryAxis(GradedElement member)
    {
        ArgumentNullException.ThrowIfNull(member);

        return TryReadMember(member, out var read) && read is not null
            ? EngineBoundary.GetAxis(Frame, read)
            : EngineBoundary.CreateUnknownAxis(Frame);
    }

    public bool TryReadAll(out IReadOnlyList<GradedElement>? reads)
    {
        var resolvedReads = new List<GradedElement>(_members.Count);

        foreach (var member in _members)
        {
            if (!member.TryReferenceToFrame(Frame, out var read) || read is null)
            {
                reads = null;
                return false;
            }

            resolvedReads.Add(read);
        }

        reads = resolvedReads;
        return true;
    }

    public bool TryAddAll(out GradedElement? sum)
    {
        if (!TryReadAll(out var reads) ||
            reads is null ||
            reads.Count == 0)
        {
            sum = null;
            return false;
        }

        var current = reads[0];

        for (var index = 1; index < reads.Count; index++)
        {
            if (!current.TryAdd(reads[index], out var next) || next is null)
            {
                sum = null;
                return false;
            }

            current = next;
        }

        sum = current;
        return true;
    }

    public bool TryAddAllWithProvenance(out EngineOperationResult? result)
    {
        GradedElement? sum;

        if (TryAddAll(out sum) &&
            sum is not null)
        {
            result = new EngineOperationResult("Add", Frame, _members.ToArray(), sum, Frame);
            return true;
        }

        result = null;
        return false;
    }
}
