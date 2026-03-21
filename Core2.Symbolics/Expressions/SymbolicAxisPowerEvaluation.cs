using Core2.Elements;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

public sealed record SymbolicAxisPowerEvaluation(
    SymbolicEnvironment Environment,
    PowerTerm Term,
    SymbolicTerm? Elaborated,
    SymbolicTerm? Reduced,
    PowerResult<Axis>? Result)
{
    public bool Succeeded => Result?.Succeeded ?? false;
    public IReadOnlyList<Axis> Candidates => Result?.Candidates ?? Array.Empty<Axis>();
    public Axis? PrincipalCandidate => Result?.PrincipalCandidate;
}

public static class SymbolicPowerEvaluator
{
    public static SymbolicAxisPowerEvaluation EvaluateAxis(
        PowerTerm term,
        SymbolicEnvironment? environment = null,
        ISymbolicStructuralContext? structuralContext = null)
    {
        ArgumentNullException.ThrowIfNull(term);

        var current = environment ?? SymbolicEnvironment.Empty;
        var elaborated = SymbolicElaborator.Elaborate(term, current);
        var reduced = SymbolicReducer.Reduce(term, current, structuralContext);
        var result = TryResolveAxisPower(elaborated.Output);

        return new SymbolicAxisPowerEvaluation(
            reduced.Environment,
            term,
            elaborated.Output,
            reduced.Output,
            result);
    }

    private static PowerResult<Axis>? TryResolveAxisPower(SymbolicTerm? term)
    {
        if (term is not PowerTerm power ||
            power.Base is not ElementLiteralTerm { Value: Axis axis })
        {
            return null;
        }

        Axis? reference = null;
        if (power.Reference is not null)
        {
            if (power.Reference is not ElementLiteralTerm { Value: Axis axisReference })
            {
                return null;
            }

            reference = axisReference;
        }

        return PowerEngine.Pow(axis, power.Exponent, power.Rule, reference);
    }
}
