using Core2.Elements;

namespace Core2.Symbolics.Expressions;

internal static class SymbolicReductionLiterals
{
    public static bool TryGetAxisLiteral(ValueTerm term, out Axis axis)
    {
        if (term is ElementLiteralTerm literal && literal.Value is Axis typed)
        {
            axis = typed;
            return true;
        }

        axis = Axis.Zero;
        return false;
    }

    public static bool TryGetOptionalAxisLiteral(ValueTerm? term, out Axis? axis)
    {
        if (term is null)
        {
            axis = null;
            return true;
        }

        if (TryGetAxisLiteral(term, out var typed))
        {
            axis = typed;
            return true;
        }

        axis = null;
        return false;
    }

    public static bool TryGetScalarLiteral(ValueTerm term, out Scalar scalar)
    {
        if (term is ElementLiteralTerm literal && literal.Value is Scalar typed)
        {
            scalar = typed;
            return true;
        }

        scalar = Scalar.Zero;
        return false;
    }

    public static bool TryGetProportionLiteral(ValueTerm term, out Proportion proportion)
    {
        if (term is ElementLiteralTerm literal && literal.Value is Proportion typed)
        {
            proportion = typed;
            return true;
        }

        proportion = null!;
        return false;
    }
}
