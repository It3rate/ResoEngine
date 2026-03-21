using Core2.Branching;

namespace Core2.Symbolics.Expressions;

public static class SymbolicElaborator
{
    public static SymbolicElaborationResult Elaborate(SymbolicTerm term, SymbolicEnvironment? environment = null)
    {
        ArgumentNullException.ThrowIfNull(term);

        var current = environment ?? SymbolicEnvironment.Empty;
        return term switch
        {
            ProgramTerm program => ElaborateProgram(program, current),
            _ => new SymbolicElaborationResult(current, ElaborateTerm(term, current)),
        };
    }

    private static SymbolicElaborationResult ElaborateProgram(ProgramTerm term, SymbolicEnvironment environment) =>
        term switch
        {
            BindTerm bind => ElaborateBind(bind, environment),
            CommitTerm commit => ElaborateCommit(commit, environment),
            EmitTerm emit => new SymbolicElaborationResult(environment, ElaborateTerm(emit.Value, environment)),
            SequenceTerm sequence => ElaborateSequence(sequence, environment),
            _ => new SymbolicElaborationResult(environment, term),
        };

    private static SymbolicElaborationResult ElaborateBind(BindTerm bind, SymbolicEnvironment environment)
    {
        var value = ElaborateTerm(bind.Value, environment);
        var next = environment.Bind(bind.Target, value);
        return new SymbolicElaborationResult(next, value);
    }

    private static SymbolicElaborationResult ElaborateCommit(CommitTerm commit, SymbolicEnvironment environment)
    {
        var value = ElaborateTerm(commit.Value, environment);
        var next = environment.Bind(commit.Target, value);
        return new SymbolicElaborationResult(next, value);
    }

    private static SymbolicElaborationResult ElaborateSequence(SequenceTerm sequence, SymbolicEnvironment environment)
    {
        var current = environment;
        SymbolicTerm? output = null;

        foreach (var step in sequence.Steps)
        {
            var result = ElaborateProgram(step, current);
            current = result.Environment;
            output = result.Output;
        }

        return new SymbolicElaborationResult(current, output);
    }

    private static SymbolicTerm ElaborateTerm(SymbolicTerm term, SymbolicEnvironment environment) =>
        term switch
        {
            ReferenceTerm reference when environment.TryResolve(reference.Name, out var resolved) && resolved is not null => resolved,
            ValueReferenceTerm reference when environment.TryResolve(reference.Name, out var resolved) && resolved is ValueTerm value => value,
            TransformReferenceTerm reference when environment.TryResolve(reference.Name, out var resolved) && resolved is TransformTerm transform => transform,
            RelationReferenceTerm reference when environment.TryResolve(reference.Name, out var resolved) && resolved is RelationTerm relation => relation,
            SiteReferenceTerm reference when environment.TryResolve(reference.SiteName, out var resolved) && resolved is ValueTerm value => value,
            AnchorReferenceTerm reference when environment.TryResolve(reference.QualifiedName, out var resolved) && resolved is ValueTerm value => value,
            ApplyTransformTerm apply => new ApplyTransformTerm(
                (ValueTerm)ElaborateTerm(apply.State, environment),
                (TransformTerm)ElaborateTerm(apply.Transform, environment)),
            MultiplyValuesTerm multiply => new MultiplyValuesTerm(
                (ValueTerm)ElaborateTerm(multiply.Left, environment),
                (ValueTerm)ElaborateTerm(multiply.Right, environment)),
            DivideValuesTerm divide => new DivideValuesTerm(
                (ValueTerm)ElaborateTerm(divide.Left, environment),
                (ValueTerm)ElaborateTerm(divide.Right, environment)),
            PowerTerm power => new PowerTerm(
                (ValueTerm)ElaborateTerm(power.Base, environment),
                power.Exponent,
                power.Rule,
                power.Reference is null ? null : (ValueTerm)ElaborateTerm(power.Reference, environment)),
            InverseContinueTerm inverse => new InverseContinueTerm(
                (ValueTerm)ElaborateTerm(inverse.Source, environment),
                inverse.Degree,
                inverse.Rule,
                inverse.Reference is null ? null : (ValueTerm)ElaborateTerm(inverse.Reference, environment)),
            PinTerm pin => new PinTerm(
                (ValueTerm)ElaborateTerm(pin.Host, environment),
                (ValueTerm)ElaborateTerm(pin.Applied, environment),
                pin.Position,
                pin.AppliedAnchor is null ? null : (AnchorReferenceTerm)ElaborateTerm(pin.AppliedAnchor, environment)),
            PinToPinTerm pinToPin => new PinToPinTerm(
                (AnchorReferenceTerm)ElaborateTerm(pinToPin.HostAnchor, environment),
                (AnchorReferenceTerm)ElaborateTerm(pinToPin.AppliedAnchor, environment)),
            AxisBooleanTerm boolean => new AxisBooleanTerm(
                (ValueTerm)ElaborateTerm(boolean.Primary, environment),
                (ValueTerm)ElaborateTerm(boolean.Secondary, environment),
                boolean.Operation,
                boolean.Frame is null ? null : (ValueTerm)ElaborateTerm(boolean.Frame, environment)),
            FoldTerm fold => new FoldTerm((ValueTerm)ElaborateTerm(fold.Source, environment), fold.Kind),
            EqualityTerm equality => new EqualityTerm(
                ElaborateTerm(equality.Left, environment),
                ElaborateTerm(equality.Right, environment)),
            SharedCarrierTerm shared => new SharedCarrierTerm(
                (ValueTerm)ElaborateTerm(shared.Left, environment),
                (ValueTerm)ElaborateTerm(shared.Right, environment)),
            RouteTerm route => new RouteTerm(
                (SiteReferenceTerm)ElaborateTerm(route.Site, environment),
                route.From,
                route.To),
            RequirementTerm requirement => new RequirementTerm(
                (RelationTerm)ElaborateTerm(requirement.Relation, environment),
                requirement.ParticipantName),
            PreferenceTerm preference => new PreferenceTerm(
                (RelationTerm)ElaborateTerm(preference.Relation, environment),
                preference.Weight,
                preference.ParticipantName),
            ConstraintSetTerm set => new ConstraintSetTerm(
                set.Constraints.Select(constraint => (ConstraintTerm)ElaborateTerm(constraint, environment)).ToArray()),
            BranchFamilyTerm branchFamily => new BranchFamilyTerm(ElaborateBranchFamily(branchFamily.Family, environment)),
            _ => term,
        };

    private static BranchFamily<ValueTerm> ElaborateBranchFamily(
        BranchFamily<ValueTerm> family,
        SymbolicEnvironment environment)
    {
        var members = family.Members
            .Select(member => new BranchMember<ValueTerm>(
                member.Id,
                (ValueTerm)ElaborateTerm(member.Value, environment),
                member.Parents,
                member.Annotations))
            .ToArray();

        return BranchFamily<ValueTerm>.FromMembers(
            family.Origin,
            family.Semantics,
            family.Direction,
            members,
            family.Selection,
            family.Tensions,
            family.Annotations);
    }
}
