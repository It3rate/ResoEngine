using Core2.Branching;

namespace Core2.Symbolics.Expressions;

public static class SymbolicConstraintEvaluator
{
    public static ConstraintSetEvaluation Evaluate(SymbolicTerm term, SymbolicEnvironment? environment = null)
        => Evaluate(term, environment, null);

    public static ConstraintSetEvaluation Evaluate(
        SymbolicTerm term,
        SymbolicEnvironment? environment,
        ISymbolicStructuralContext? structuralContext)
    {
        ArgumentNullException.ThrowIfNull(term);

        var reduced = SymbolicReducer.Reduce(term, environment);
        var reducedConstraint = reduced.Output switch
        {
            ConstraintSetTerm set => set,
            ConstraintTerm constraint => new ConstraintSetTerm([constraint]),
            _ => throw new InvalidOperationException("Constraint evaluation requires a constraint term or a program whose final output is a constraint term."),
        };

        var sourceConstraint = term switch
        {
            ConstraintSetTerm set => set,
            ConstraintTerm constraint => new ConstraintSetTerm([constraint]),
            _ => reducedConstraint,
        };

        var items = reducedConstraint.Constraints
            .Select((constraint, index) => EvaluateConstraint(
                sourceConstraint.Constraints.ElementAtOrDefault(index) ?? constraint,
                constraint,
                structuralContext))
            .ToArray();

        var summaries = items
            .GroupBy(item => item.ParticipantName, StringComparer.Ordinal)
            .Select(CreateSummary)
            .OrderBy(summary => summary.ParticipantName is null ? 1 : 0)
            .ThenBy(summary => summary.ParticipantName, StringComparer.Ordinal)
            .ToArray();

        return new ConstraintSetEvaluation(
            reduced.Environment,
            sourceConstraint,
            reducedConstraint,
            items,
            summaries);
    }

    private static ConstraintEvaluationItem EvaluateConstraint(
        ConstraintTerm source,
        ConstraintTerm reduced,
        ISymbolicStructuralContext? structuralContext) =>
        reduced switch
        {
            RequirementTerm requirement => new ConstraintEvaluationItem(
                source,
                reduced,
                EvaluateRelation(requirement.Relation, structuralContext),
                requirement.ParticipantName,
                null),
            PreferenceTerm preference => new ConstraintEvaluationItem(
                source,
                reduced,
                EvaluateRelation(preference.Relation, structuralContext),
                preference.ParticipantName,
                preference.Weight),
            ConstraintSetTerm set => new ConstraintEvaluationItem(
                source,
                reduced,
                set.Constraints.All(inner => EvaluateConstraint(inner, inner, structuralContext).Truth == ConstraintTruthKind.Satisfied)
                    ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
                    : set.Constraints.Any(inner => EvaluateConstraint(inner, inner, structuralContext).Truth == ConstraintTruthKind.Unsatisfied)
                        ? new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied)
                        : new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Nested constraint family remains unresolved."),
                null,
                null),
            _ => new ConstraintEvaluationItem(
                source,
                reduced,
                new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Constraint form is not yet directly evaluable."),
                null,
                null),
        };

    private static ConstraintRelationAssessment EvaluateRelation(
        RelationTerm relation,
        ISymbolicStructuralContext? structuralContext) =>
        relation switch
        {
            EqualityTerm equality => EvaluateEquality(equality),
            SharedCarrierTerm shared => EvaluateSharedCarrier(shared, structuralContext),
            RouteTerm route => EvaluateRoute(route, structuralContext),
            _ => new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Relation form is not yet directly evaluable."),
        };

    private static ConstraintRelationAssessment EvaluateEquality(EqualityTerm equality)
    {
        if (TryEvaluateAlternativeEquality(equality.Left, equality.Right, out var branchAssessment))
        {
            return branchAssessment;
        }

        if (!IsClosed(equality.Left) || !IsClosed(equality.Right))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Equality requires closed terms or explicit branch selection.");
        }

        return Equals(equality.Left, equality.Right)
            ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
            : new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied);
    }

    private static ConstraintRelationAssessment EvaluateSharedCarrier(
        SharedCarrierTerm shared,
        ISymbolicStructuralContext? structuralContext)
    {
        if (Equals(shared.Left, shared.Right))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied);
        }

        if (structuralContext is not null &&
            shared.Left is AnchorReferenceTerm leftAnchor &&
            shared.Right is AnchorReferenceTerm rightAnchor)
        {
            bool hasLeft = structuralContext.TryResolveAnchorCarrier(leftAnchor, out var leftCarrier, out var leftNote);
            bool hasRight = structuralContext.TryResolveAnchorCarrier(rightAnchor, out var rightCarrier, out var rightNote);

            if (hasLeft && hasRight)
            {
                return leftCarrier == rightCarrier
                    ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
                    : new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied, null, "Anchors resolve to different structural carriers.");
            }

            string note = leftNote ?? rightNote ?? "Shared-carrier evaluation requires carrier graph context.";
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, note);
        }

        return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Shared-carrier evaluation requires carrier graph context.");
    }

    private static ConstraintRelationAssessment EvaluateRoute(
        RouteTerm route,
        ISymbolicStructuralContext? structuralContext)
    {
        if (structuralContext is null)
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Route evaluation requires carrier routing context.");
        }

        if (!structuralContext.TryResolveRoute(route.Site, route.From.Kind, route.To.Kind, out bool exists, out var note))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, note ?? "Route evaluation requires carrier routing context.");
        }

        return exists
            ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
            : new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied, null, "No such structural route exists at the named site.");
    }

    private static bool TryEvaluateAlternativeEquality(
        SymbolicTerm left,
        SymbolicTerm right,
        out ConstraintRelationAssessment assessment)
    {
        if (TryEvaluateAlternativeCandidateFamily(left, right, out assessment))
        {
            return true;
        }

        if (TryEvaluateAlternativeCandidateFamily(right, left, out assessment))
        {
            return true;
        }

        assessment = null!;
        return false;
    }

    private static bool TryEvaluateAlternativeCandidateFamily(
        SymbolicTerm branchCandidate,
        SymbolicTerm other,
        out ConstraintRelationAssessment assessment)
    {
        if (branchCandidate is not BranchFamilyTerm branch ||
            branch.Family.Semantics != BranchSemantics.Alternative)
        {
            assessment = null!;
            return false;
        }

        if (branch.Family.Selection.HasSelection &&
            branch.Family.TryGetSelectedMember(out var selectedMember) &&
            selectedMember is not null &&
            IsClosed(selectedMember.Value) &&
            IsClosed(other))
        {
            assessment = Equals(selectedMember.Value, other)
                ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied, null, "Selected branch satisfies the equality.")
                : new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied, null, "Selected branch does not satisfy the equality.");
            return true;
        }

        var matchingMembers = branch.Family.Members
            .Where(member => IsClosed(member.Value) && IsClosed(other) && Equals(member.Value, other))
            .ToArray();

        if (matchingMembers.Length > 0)
        {
            var candidateFamily = BranchFamily<ValueTerm>.FromMembers(
                branch.Family.Origin,
                branch.Family.Semantics,
                branch.Family.Direction,
                matchingMembers,
                BranchSelection.None,
                branch.Family.Tensions,
                branch.Family.Annotations);

            assessment = new ConstraintRelationAssessment(
                ConstraintTruthKind.Unresolved,
                candidateFamily,
                "Alternative branch family contains satisfying candidates but no single candidate is committed.");
            return true;
        }

        if (branch.Family.Values.All(IsClosed) && IsClosed(other))
        {
            assessment = new ConstraintRelationAssessment(
                ConstraintTruthKind.Unsatisfied,
                null,
                "No branch candidate satisfies the equality.");
            return true;
        }

        assessment = new ConstraintRelationAssessment(
            ConstraintTruthKind.Unresolved,
            null,
            "Alternative branch family requires more reduction or selection before equality can be decided.");
        return true;
    }

    private static bool IsClosed(SymbolicTerm term) =>
        term switch
        {
            ReferenceTerm => false,
            SiteReferenceTerm => false,
            AnchorReferenceTerm => false,
            ElementLiteralTerm => true,
            TransformLiteralTerm => true,
            IncidentReferenceTerm => true,
            ApplyTransformTerm apply => IsClosed(apply.State) && IsClosed(apply.Transform),
            PinTerm pin => IsClosed(pin.Host) && IsClosed(pin.Applied) && (pin.AppliedAnchor is null || IsClosed(pin.AppliedAnchor)),
            PinToPinTerm pinToPin => IsClosed(pinToPin.HostAnchor) && IsClosed(pinToPin.AppliedAnchor),
            AxisBooleanTerm boolean => IsClosed(boolean.Primary) && IsClosed(boolean.Secondary) && (boolean.Frame is null || IsClosed(boolean.Frame)),
            FoldTerm fold => IsClosed(fold.Source),
            EqualityTerm equality => IsClosed(equality.Left) && IsClosed(equality.Right),
            SharedCarrierTerm shared => IsClosed(shared.Left) && IsClosed(shared.Right),
            RouteTerm route => IsClosed(route.Site) && IsClosed(route.From) && IsClosed(route.To),
            RequirementTerm requirement => IsClosed(requirement.Relation),
            PreferenceTerm preference => IsClosed(preference.Relation),
            ConstraintSetTerm set => set.Constraints.All(IsClosed),
            BranchFamilyTerm branch => branch.Family.Values.All(IsClosed),
            _ => false,
        };

    private static ConstraintParticipantSummary CreateSummary(IGrouping<string?, ConstraintEvaluationItem> group)
    {
        int satisfiedRequirements = group.Count(item => item.IsRequirement && item.Truth == ConstraintTruthKind.Satisfied);
        int unsatisfiedRequirements = group.Count(item => item.IsRequirement && item.Truth == ConstraintTruthKind.Unsatisfied);
        int unresolvedRequirements = group.Count(item => item.IsRequirement && item.Truth == ConstraintTruthKind.Unresolved);

        var satisfiedPreferenceWeight = group
            .Where(item => item.IsPreference && item.Truth == ConstraintTruthKind.Satisfied)
            .Aggregate(Core2.Elements.Proportion.Zero, (sum, item) => sum + item.WeightOrZero);
        var unsatisfiedPreferenceWeight = group
            .Where(item => item.IsPreference && item.Truth == ConstraintTruthKind.Unsatisfied)
            .Aggregate(Core2.Elements.Proportion.Zero, (sum, item) => sum + item.WeightOrZero);
        var unresolvedPreferenceWeight = group
            .Where(item => item.IsPreference && item.Truth == ConstraintTruthKind.Unresolved)
            .Aggregate(Core2.Elements.Proportion.Zero, (sum, item) => sum + item.WeightOrZero);

        return new ConstraintParticipantSummary(
            group.Key,
            satisfiedRequirements,
            unsatisfiedRequirements,
            unresolvedRequirements,
            satisfiedPreferenceWeight,
            unsatisfiedPreferenceWeight,
            unresolvedPreferenceWeight);
    }
}
