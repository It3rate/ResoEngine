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
            SequenceTerm sequence => ElaborateSequence(sequence, environment),
            _ => new SymbolicElaborationResult(environment, term),
        };

    private static SymbolicElaborationResult ElaborateBind(BindTerm bind, SymbolicEnvironment environment)
    {
        var value = ElaborateTerm(bind.Value, environment);
        var next = environment.Bind(bind.Name, value);
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
            ApplyTransformTerm apply => new ApplyTransformTerm(
                (ValueTerm)ElaborateTerm(apply.State, environment),
                (TransformTerm)ElaborateTerm(apply.Transform, environment)),
            PinTerm pin => new PinTerm(
                (ValueTerm)ElaborateTerm(pin.Host, environment),
                (ValueTerm)ElaborateTerm(pin.Applied, environment),
                pin.Position),
            FoldTerm fold => new FoldTerm((ValueTerm)ElaborateTerm(fold.Source, environment), fold.Kind),
            EqualityTerm equality => new EqualityTerm(
                ElaborateTerm(equality.Left, environment),
                ElaborateTerm(equality.Right, environment)),
            SharedCarrierTerm shared => new SharedCarrierTerm(
                (ReferenceTerm)ElaborateTerm(shared.Left, environment),
                (ReferenceTerm)ElaborateTerm(shared.Right, environment)),
            RouteTerm route => new RouteTerm(
                (ReferenceTerm)ElaborateTerm(route.Site, environment),
                (ReferenceTerm)ElaborateTerm(route.From, environment),
                (ReferenceTerm)ElaborateTerm(route.To, environment)),
            RequirementTerm requirement => new RequirementTerm((RelationTerm)ElaborateTerm(requirement.Relation, environment)),
            PreferenceTerm preference => new PreferenceTerm((RelationTerm)ElaborateTerm(preference.Relation, environment), preference.Weight),
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
