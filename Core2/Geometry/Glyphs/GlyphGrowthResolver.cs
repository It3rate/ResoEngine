using Core2.Branching;
using Core2.Dynamic;
using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed class GlyphGrowthResolver : IDynamicResolver<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>
{
    public DynamicResolution<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect> Resolve(
        DynamicResolutionInput<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect> input)
    {
        var resolutions = input.Frontier
            .Select(frontier => ResolveFrontierContext(
                frontier,
                input.Proposals.Where(proposal => proposal.SourceId == frontier.NodeId).ToArray()))
            .ToArray();

        var accepted = resolutions.SelectMany(resolution => resolution.AcceptedProposals).ToArray();
        var tensions = resolutions.SelectMany(resolution => resolution.Tensions).ToArray();
        var members = resolutions.SelectMany(resolution => resolution.Outcomes.Members).ToArray();

        if (members.Length == 0)
        {
            return DynamicResolution<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>.FromFamily(
                DynamicResolutionKind.Commit,
                BranchFamily<DynamicContext<GlyphGrowthState, GlyphEnvironment>>.Empty(
                    BranchOrigin.Continuation,
                    BranchSemantics.Alternative,
                    BranchDirection.Forward),
                accepted,
                tensions,
                "No glyph continuations remained active.");
        }

        var kind = resolutions.Any(resolution => resolution.Kind == DynamicResolutionKind.Branch)
            ? DynamicResolutionKind.Branch
            : DynamicResolutionKind.Commit;

        var family = BranchFamily<DynamicContext<GlyphGrowthState, GlyphEnvironment>>.FromMembers(
            BranchOrigin.Continuation,
            members.Length > 1 ? BranchSemantics.Alternative : BranchSemantics.Mixed,
            BranchDirection.Forward,
            members,
            ResolveSelection(resolutions, members));

        return DynamicResolution<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>.FromFamily(
            kind,
            family,
            accepted,
            tensions,
            "Resolved glyph growth proposals.");
    }

    private static BranchSelection ResolveSelection(
        IReadOnlyList<ContextResolution> resolutions,
        IReadOnlyList<BranchMember<DynamicContext<GlyphGrowthState, GlyphEnvironment>>> members)
    {
        foreach (var resolution in resolutions)
        {
            if (resolution.Outcomes.TryGetSelectedMember(out var selected) && selected is not null)
            {
                return BranchSelection.Principal(selected.Id);
            }
        }

        return members.Count == 1
            ? BranchSelection.Principal(members[0].Id)
            : BranchSelection.None;
    }

    private static ContextResolution ResolveFrontierContext(
        DynamicFrontierContext<GlyphGrowthState, GlyphEnvironment> frontier,
        IReadOnlyList<DynamicProposal<GlyphGrowthEffect>> proposals)
    {
        var context = frontier.Context;
        if (proposals.Count == 0)
        {
            if (GlyphRelaxation.NeedsRelaxation(context.State))
            {
                var relaxed = GlyphRelaxation.Relax(context.State, context.Environment, []);
                var relaxedMember = new BranchMember<DynamicContext<GlyphGrowthState, GlyphEnvironment>>(
                    BranchId.New(),
                    new DynamicContext<GlyphGrowthState, GlyphEnvironment>(relaxed, context.Environment),
                    [frontier.NodeId],
                    []);

                return new ContextResolution(
                    DynamicResolutionKind.Commit,
                    BranchFamily<DynamicContext<GlyphGrowthState, GlyphEnvironment>>.FromMembers(
                        BranchOrigin.Continuation,
                        BranchSemantics.Mixed,
                        BranchDirection.Forward,
                        [relaxedMember],
                        BranchSelection.Principal(relaxedMember.Id)),
                    [],
                    []);
            }

            return ContextResolution.Empty;
        }

        var grouped = proposals
            .GroupBy(proposal => proposal.Effect.SourceTipKey)
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(proposal => proposal.Weight).ToArray(),
                StringComparer.Ordinal);

        var selected = SelectPreferredProposals(grouped);
        var ambiguous = FindAmbiguousTip(grouped);
        if (ambiguous is not null)
        {
            var branchMembers = BuildAmbiguousContexts(frontier.NodeId, context, selected, ambiguous.Value.TipKey, ambiguous.Value.Options);
            return new ContextResolution(
                DynamicResolutionKind.Branch,
                BranchFamily<DynamicContext<GlyphGrowthState, GlyphEnvironment>>.FromMembers(
                    BranchOrigin.Continuation,
                    BranchSemantics.Alternative,
                    BranchDirection.Forward,
                    branchMembers,
                    BranchSelection.Principal(branchMembers[0].Id)),
                selected.Values.Concat(ambiguous.Value.Options).Distinct().ToArray(),
                selected.Values.SelectMany(proposal => proposal.Tensions).ToArray());
        }

        var nextState = ApplyProposals(context.State, context.Environment, selected.Values);
        nextState = GlyphRelaxation.Relax(nextState, context.Environment, selected.Values.ToArray());
        var member = new BranchMember<DynamicContext<GlyphGrowthState, GlyphEnvironment>>(
            BranchId.New(),
            new DynamicContext<GlyphGrowthState, GlyphEnvironment>(nextState, context.Environment),
            [frontier.NodeId],
            []);

        return new ContextResolution(
            DynamicResolutionKind.Commit,
            BranchFamily<DynamicContext<GlyphGrowthState, GlyphEnvironment>>.FromMembers(
                BranchOrigin.Continuation,
                BranchSemantics.Mixed,
                BranchDirection.Forward,
                [member],
                BranchSelection.Principal(member.Id)),
            selected.Values.ToArray(),
            selected.Values.SelectMany(proposal => proposal.Tensions).ToArray());
    }

    private static Dictionary<string, DynamicProposal<GlyphGrowthEffect>> SelectPreferredProposals(
        IReadOnlyDictionary<string, DynamicProposal<GlyphGrowthEffect>[]> grouped)
    {
        var selected = new Dictionary<string, DynamicProposal<GlyphGrowthEffect>>(StringComparer.Ordinal);
        foreach (var (tipKey, ordered) in grouped)
        {
            var preferred = ordered[0];
            if (preferred.Effect.Kind == GlyphGrowthEffectKind.Join &&
                !HasMutualJoin(ordered[0], grouped))
            {
                preferred = ordered.FirstOrDefault(proposal => proposal.Effect.Kind != GlyphGrowthEffectKind.Join) ?? ordered[0];
            }

            selected[tipKey] = preferred;
        }

        foreach (var candidate in selected.Values.Where(proposal => proposal.Effect.Kind == GlyphGrowthEffectKind.Join).ToArray())
        {
            if (!HasMutualJoin(candidate, grouped))
            {
                string tipKey = candidate.Effect.SourceTipKey;
                selected[tipKey] = grouped[tipKey]
                    .FirstOrDefault(proposal => proposal.Effect.Kind != GlyphGrowthEffectKind.Join)
                    ?? candidate;
            }
        }

        return selected;
    }

    private static (string TipKey, DynamicProposal<GlyphGrowthEffect>[] Options)? FindAmbiguousTip(
        IReadOnlyDictionary<string, DynamicProposal<GlyphGrowthEffect>[]> grouped)
    {
        foreach (var (tipKey, ordered) in grouped)
        {
            if (ordered.Length < 2)
            {
                continue;
            }

            var options = ordered
                .Where(proposal => proposal.Effect.Kind != GlyphGrowthEffectKind.Join)
                .Take(2)
                .ToArray();

            if (options.Length < 2)
            {
                continue;
            }

            decimal delta = options[0].Weight - options[1].Weight;
            if (delta <= GlyphGrowthDefaults.BranchAmbiguityThreshold &&
                options[0].Effect.Kind != options[1].Effect.Kind)
            {
                return (tipKey, options);
            }
        }

        return null;
    }

    private static IReadOnlyList<BranchMember<DynamicContext<GlyphGrowthState, GlyphEnvironment>>> BuildAmbiguousContexts(
        BranchId parentId,
        DynamicContext<GlyphGrowthState, GlyphEnvironment> context,
        IReadOnlyDictionary<string, DynamicProposal<GlyphGrowthEffect>> selected,
        string tipKey,
        IReadOnlyList<DynamicProposal<GlyphGrowthEffect>> alternatives)
    {
        var members = new List<BranchMember<DynamicContext<GlyphGrowthState, GlyphEnvironment>>>(alternatives.Count);
        foreach (var alternative in alternatives)
        {
            var proposals = selected.ToDictionary(entry => entry.Key, entry => entry.Value, StringComparer.Ordinal);
            proposals[tipKey] = alternative;
            var nextState = ApplyProposals(context.State, context.Environment, proposals.Values);
            nextState = GlyphRelaxation.Relax(nextState, context.Environment, proposals.Values.ToArray());
            members.Add(new BranchMember<DynamicContext<GlyphGrowthState, GlyphEnvironment>>(
                BranchId.New(),
                new DynamicContext<GlyphGrowthState, GlyphEnvironment>(nextState, context.Environment),
                [parentId],
                []));
        }

        return members;
    }

    private static bool HasMutualJoin(
        DynamicProposal<GlyphGrowthEffect> proposal,
        IReadOnlyDictionary<string, DynamicProposal<GlyphGrowthEffect>[]> grouped)
    {
        var effect = proposal.Effect;
        if (effect.Kind != GlyphGrowthEffectKind.Join || string.IsNullOrEmpty(effect.PartnerTipKey))
        {
            return false;
        }

        if (!grouped.TryGetValue(effect.PartnerTipKey, out var partnerOptions))
        {
            return false;
        }

        return partnerOptions.Any(option =>
            option.Effect.Kind == GlyphGrowthEffectKind.Join &&
            option.Effect.PartnerTipKey == effect.SourceTipKey &&
            option.Effect.TargetPosition.DistanceTo(effect.TargetPosition) <= 1m);
    }

    private static GlyphGrowthState ApplyProposals(
        GlyphGrowthState state,
        GlyphEnvironment environment,
        IEnumerable<DynamicProposal<GlyphGrowthEffect>> proposals)
    {
        var tips = state.ActiveTips.ToDictionary(tip => tip.Key, tip => tip, StringComparer.Ordinal);
        var carriers = state.Carriers.ToList();
        var junctions = state.Junctions.ToDictionary(junction => junction.Key, junction => junction, StringComparer.Ordinal);
        var selected = proposals.ToArray();
        var handledTips = new HashSet<string>(StringComparer.Ordinal);

        foreach (var proposal in selected.Where(proposal => proposal.Effect.Kind == GlyphGrowthEffectKind.Join))
        {
            var effect = proposal.Effect;
            if (handledTips.Contains(effect.SourceTipKey) ||
                string.IsNullOrEmpty(effect.PartnerTipKey) ||
                handledTips.Contains(effect.PartnerTipKey))
            {
                continue;
            }

            var partnerProposal = selected.FirstOrDefault(candidate =>
                candidate.Effect.Kind == GlyphGrowthEffectKind.Join &&
                candidate.Effect.SourceTipKey == effect.PartnerTipKey &&
                candidate.Effect.PartnerTipKey == effect.SourceTipKey &&
                candidate.Effect.TargetPosition.DistanceTo(effect.TargetPosition) <= 1m);

            if (partnerProposal is null ||
                !tips.TryGetValue(effect.SourceTipKey, out var tip) ||
                !tips.TryGetValue(effect.PartnerTipKey, out var partner) ||
                !tip.IsActive ||
                !partner.IsActive)
            {
                continue;
            }

            AddCarrier(carriers, tip.Key, tip.Position, effect.TargetPosition);
            AddCarrier(carriers, partner.Key, partner.Position, effect.TargetPosition);

            tips[tip.Key] = tip with { Position = effect.TargetPosition, IsActive = false };
            tips[partner.Key] = partner with { Position = effect.TargetPosition, IsActive = false };
            AddOrReplaceJunction(
                junctions,
                effect.JunctionKey ?? $"join:{tip.Key}:{partner.Key}",
                effect.TargetPosition,
                GlyphJunctionKind.Join,
                [tip.Key, partner.Key],
                allowsSplit: false,
                allowsJoin: true,
                note: effect.Note);

            handledTips.Add(tip.Key);
            handledTips.Add(partner.Key);
        }

        foreach (var group in selected
            .Where(proposal => proposal.Effect.Kind == GlyphGrowthEffectKind.Stop)
            .GroupBy(proposal => proposal.Effect.JunctionKey ?? $"{proposal.Effect.TargetPosition.X}:{proposal.Effect.TargetPosition.Y}"))
        {
            var arrivals = group
                .Where(proposal =>
                    !handledTips.Contains(proposal.Effect.SourceTipKey) &&
                    tips.TryGetValue(proposal.Effect.SourceTipKey, out var tip) &&
                    tip.IsActive)
                .ToArray();

            if (arrivals.Length < 2)
            {
                continue;
            }

            var target = arrivals[0].Effect.TargetPosition;
            var connectedKeys = arrivals
                .Select(arrival => arrival.Effect.SourceTipKey)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(key => key, StringComparer.Ordinal)
                .ToArray();

            foreach (var arrival in arrivals)
            {
                var effect = arrival.Effect;
                var tip = tips[effect.SourceTipKey];
                AddCarrier(carriers, tip.Key, tip.Position, target);
                tips[tip.Key] = tip with
                {
                    Position = target,
                    PreferredDirection = effect.Direction,
                    IsActive = false,
                };
                handledTips.Add(tip.Key);
            }

            AddOrReplaceJunction(
                junctions,
                arrivals[0].Effect.JunctionKey ?? $"join:{string.Join(":", connectedKeys)}",
                target,
                GlyphJunctionKind.Join,
                connectedKeys,
                allowsSplit: false,
                allowsJoin: true,
                note: arrivals[0].Effect.Note);
        }

        foreach (var proposal in selected.Where(proposal => proposal.Effect.Kind != GlyphGrowthEffectKind.Join))
        {
            var effect = proposal.Effect;
            if (handledTips.Contains(effect.SourceTipKey) ||
                !tips.TryGetValue(effect.SourceTipKey, out var tip) ||
                !tip.IsActive)
            {
                continue;
            }

            switch (effect.Kind)
            {
                case GlyphGrowthEffectKind.Grow:
                    AddCarrier(carriers, tip.Key, tip.Position, effect.TargetPosition);
                    tips[tip.Key] = tip with
                    {
                        Position = effect.TargetPosition,
                        PreferredDirection = effect.Direction,
                        Energy = Math.Max(0.35m, tip.Energy * 0.98m),
                        CarrierKey = tip.CarrierKey ?? tip.Key,
                    };
                    handledTips.Add(tip.Key);
                    break;

                case GlyphGrowthEffectKind.Stop:
                    AddCarrier(carriers, tip.Key, tip.Position, effect.TargetPosition);
                    tips[tip.Key] = tip with
                    {
                        Position = effect.TargetPosition,
                        PreferredDirection = effect.Direction,
                        IsActive = false,
                    };
                    AddOrReplaceJunction(
                        junctions,
                        effect.JunctionKey ?? $"{tip.Key}:terminal",
                        effect.TargetPosition,
                        GlyphJunctionKind.Terminal,
                        [tip.Key],
                        allowsSplit: false,
                        allowsJoin: true,
                        note: effect.Note);
                    handledTips.Add(tip.Key);
                    break;

                case GlyphGrowthEffectKind.Split:
                    AddCarrier(carriers, tip.Key, tip.Position, effect.TargetPosition);
                    tips[tip.Key] = tip with
                    {
                        Position = effect.TargetPosition,
                        IsActive = false,
                    };

                    string junctionKey = effect.JunctionKey ?? $"{tip.Key}:split:{state.MacroStep + 1}";
                    AddOrReplaceJunction(
                        junctions,
                        junctionKey,
                        effect.TargetPosition,
                        GlyphJunctionKind.Split,
                        effect.Branches.Select(branch => branch.Key).ToArray(),
                        allowsSplit: true,
                        allowsJoin: true,
                        note: effect.Note);

                    foreach (var branch in effect.Branches)
                    {
                        tips[branch.Key] = new GlyphTip(
                            branch.Key,
                            effect.TargetPosition,
                            branch.Direction,
                            branch.Energy,
                            IsActive: true,
                            CarrierKey: junctionKey,
                            branch.Note);
                    }

                    handledTips.Add(tip.Key);
                    break;
            }
        }

        var nextTips = tips.Values
            .OrderBy(tip => tip.Key, StringComparer.Ordinal)
            .ToArray();

        var packets = BuildPackets(state, nextTips);
        return new GlyphGrowthState(
            state.LetterKey,
            nextTips,
            carriers.ToArray(),
            junctions.Values.OrderBy(junction => junction.Key, StringComparer.Ordinal).ToArray(),
            packets,
            state.MacroStep + 1,
            state.AmbientSignals,
            state.ResidualTension,
            state.LastAdjustment,
            state.TensionField,
            state.RandomSeed);
    }

    private static IReadOnlyList<TensionPacket> BuildPackets(
        GlyphGrowthState priorState,
        IReadOnlyList<GlyphTip> tips)
    {
        var packets = priorState.Packets
            .Select(packet => packet with
            {
                Position = decimal.Clamp(packet.Position + packet.SignedMobility, 0m, 1m),
                Magnitude = packet.Magnitude * GlyphGrowthDefaults.PacketDecay,
                Dissipation = packet.Dissipation + (1m - GlyphGrowthDefaults.PacketDecay),
            })
            .Where(packet => packet.Magnitude > GlyphGrowthDefaults.MinimumPacketMagnitude)
            .ToList();

        foreach (var tip in tips.Where(tip => tip.IsActive))
        {
            packets.Add(new TensionPacket(
                tip.Key,
                $"{priorState.LetterKey}:{tip.Key}",
                0m,
                PacketFlowDirection.Forward,
                Math.Max(GlyphGrowthDefaults.MinimumPacketMagnitude, tip.Energy)));
        }

        return packets.ToArray();
    }

    private static void AddCarrier(
        IList<GlyphCarrier> carriers,
        string tipKey,
        GlyphVector start,
        GlyphVector end)
    {
        if (start == end)
        {
            return;
        }

        if (carriers.Count > 0)
        {
            var previous = carriers[^1];
            if (previous.Key.StartsWith($"{tipKey}:carrier:", StringComparison.Ordinal) &&
                previous.End.DistanceTo(start) <= 0.6m &&
                AreNearlyCollinear(previous.Start, previous.End, end))
            {
                carriers[^1] = previous with
                {
                    End = end,
                    Tension = previous.Start.DistanceTo(end),
                };
                return;
            }
        }

        carriers.Add(new GlyphCarrier(
            $"{tipKey}:carrier:{carriers.Count}",
            start,
            end,
            GlyphCarrierKind.Segment,
            IsCommitted: true,
            Tension: start.DistanceTo(end)));
    }

    private static bool AreNearlyCollinear(
        GlyphVector a,
        GlyphVector b,
        GlyphVector c)
    {
        GlyphVector ab = (b - a).Normalize();
        GlyphVector bc = (c - b).Normalize();
        if (ab == GlyphVector.Zero || bc == GlyphVector.Zero)
        {
            return false;
        }

        return ab.Dot(bc) >= 0.992m;
    }

    private static void AddOrReplaceJunction(
        IDictionary<string, GlyphJunction> junctions,
        string key,
        GlyphVector position,
        GlyphJunctionKind kind,
        IReadOnlyList<string> connectedKeys,
        bool allowsSplit,
        bool allowsJoin,
        string? note)
    {
        junctions[key] = new GlyphJunction(
            key,
            position,
            kind,
            connectedKeys,
            AllowsSplit: allowsSplit,
            AllowsJoin: allowsJoin,
            note);
    }

    private sealed record ContextResolution(
        DynamicResolutionKind Kind,
        BranchFamily<DynamicContext<GlyphGrowthState, GlyphEnvironment>> Outcomes,
        IReadOnlyList<DynamicProposal<GlyphGrowthEffect>> AcceptedProposals,
        IReadOnlyList<DynamicTension> Tensions)
    {
        public static ContextResolution Empty { get; } = new(
            DynamicResolutionKind.Commit,
            BranchFamily<DynamicContext<GlyphGrowthState, GlyphEnvironment>>.Empty(
                BranchOrigin.Continuation,
                BranchSemantics.Alternative,
                BranchDirection.Forward),
            [],
            []);
    }
}
