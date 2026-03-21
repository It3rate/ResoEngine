using Core2.Elements;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

internal static partial class SymbolicReductionRepetitionFamily
{
    private static bool TryProjectPowerResult<T>(PowerResult<T> result, out SymbolicTerm reduced)
        where T : IElement
    {
        if (!result.Succeeded)
        {
            reduced = null!;
            return false;
        }

        if (result.PrincipalCandidate is not null && result.Candidates.Count == 1)
        {
            reduced = new ElementLiteralTerm(result.PrincipalCandidate);
            return true;
        }

        reduced = new BranchFamilyTerm(
            result.Branches.Map<ValueTerm>(candidate => new ElementLiteralTerm(candidate)));
        return true;
    }

    private static bool TryProjectInverseContinuationResult<T>(
        InverseContinuationResult<T> result,
        out SymbolicTerm reduced)
        where T : IElement
    {
        if (!result.Succeeded)
        {
            reduced = null!;
            return false;
        }

        if (result.PrincipalCandidate is not null && result.Candidates.Count == 1)
        {
            reduced = new ElementLiteralTerm(result.PrincipalCandidate);
            return true;
        }

        reduced = new BranchFamilyTerm(
            result.Branches.Map<ValueTerm>(candidate => new ElementLiteralTerm(candidate)));
        return true;
    }

    private static bool TryGetIntegerDegree(Proportion degree, out int normalizedDegree)
    {
        if (degree.Denominator != 1 || degree.Numerator <= 0 || degree.Numerator > int.MaxValue)
        {
            normalizedDegree = 0;
            return false;
        }

        normalizedDegree = (int)degree.Numerator;
        return true;
    }
}
