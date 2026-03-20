namespace Core2.Symbolics.Expressions;

public static class SymbolicConstraintEvaluator
{
    public static ConstraintSetEvaluation Evaluate(SymbolicTerm term, SymbolicEnvironment? environment = null)
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
                constraint))
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

    private static ConstraintEvaluationItem EvaluateConstraint(ConstraintTerm source, ConstraintTerm reduced) =>
        reduced switch
        {
            RequirementTerm requirement => new ConstraintEvaluationItem(
                source,
                reduced,
                EvaluateRelation(requirement.Relation),
                requirement.ParticipantName,
                null),
            PreferenceTerm preference => new ConstraintEvaluationItem(
                source,
                reduced,
                EvaluateRelation(preference.Relation),
                preference.ParticipantName,
                preference.Weight),
            ConstraintSetTerm set => new ConstraintEvaluationItem(
                source,
                reduced,
                set.Constraints.All(inner => EvaluateConstraint(inner, inner).Truth == ConstraintTruthKind.Satisfied)
                    ? ConstraintTruthKind.Satisfied
                    : set.Constraints.Any(inner => EvaluateConstraint(inner, inner).Truth == ConstraintTruthKind.Unsatisfied)
                        ? ConstraintTruthKind.Unsatisfied
                        : ConstraintTruthKind.Unresolved,
                null,
                null),
            _ => new ConstraintEvaluationItem(source, reduced, ConstraintTruthKind.Unresolved, null, null),
        };

    private static ConstraintTruthKind EvaluateRelation(RelationTerm relation) =>
        relation switch
        {
            EqualityTerm equality => EvaluateEquality(equality),
            SharedCarrierTerm shared => EvaluateSharedCarrier(shared),
            RouteTerm => ConstraintTruthKind.Unresolved,
            _ => ConstraintTruthKind.Unresolved,
        };

    private static ConstraintTruthKind EvaluateEquality(EqualityTerm equality)
    {
        if (!IsClosed(equality.Left) || !IsClosed(equality.Right))
        {
            return ConstraintTruthKind.Unresolved;
        }

        return Equals(equality.Left, equality.Right)
            ? ConstraintTruthKind.Satisfied
            : ConstraintTruthKind.Unsatisfied;
    }

    private static ConstraintTruthKind EvaluateSharedCarrier(SharedCarrierTerm shared)
    {
        if (Equals(shared.Left, shared.Right))
        {
            return ConstraintTruthKind.Satisfied;
        }

        return ConstraintTruthKind.Unresolved;
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
