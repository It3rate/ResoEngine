using System.Numerics;
using Core2.Elements;
using Core2.Support;

namespace Core2.Repetition;

public static class InverseContinuationEngine
{
    public static InverseContinuationResult<Scalar> InverseContinue(
        Scalar value,
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Scalar? reference = null)
    {
        if (degree <= 0)
        {
            return InvalidDegree<Scalar>(degree);
        }

        List<Scalar> candidates = [];
        decimal raw = value.Value;

        if (degree % 2 == 0)
        {
            if (raw < 0m)
            {
                return NoCandidates<Scalar>($"No real scalar candidate exists for the even inverse continuation degree {degree} of {value}.");
            }

            Scalar root = Root(raw, degree);
            candidates.Add(root);
            if (!root.IsZero)
            {
                candidates.Add(-root);
            }
        }
        else
        {
            Scalar root = raw < 0m ? -Root(-raw, degree) : Root(raw, degree);
            candidates.Add(root);
        }

        candidates = DistinctBy(candidates, candidate => candidate.Value).ToList();
        Scalar principal = SelectScalarPrincipal(candidates, rule, reference);
        return new InverseContinuationResult<Scalar>(candidates, principal, []);
    }

    public static InverseContinuationResult<Proportion> InverseContinue(
        Proportion value,
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Proportion? reference = null)
    {
        if (degree <= 0)
        {
            return InvalidDegree<Proportion>(degree);
        }

        if (!TryExactIntegerRoot(value.Dominant, degree, out long dominantRoot) ||
            !TryExactIntegerRoot(value.Recessive, degree, out long recessiveRoot))
        {
            return new InverseContinuationResult<Proportion>(
                [],
                null,
                [
                    new InverseContinuationTension(
                        InverseContinuationTensionKind.NoCandidates,
                        $"No exact proportion inverse continuation candidates exist for degree {degree} and value {value}.")
                ]);
        }

        List<Proportion> candidates = [new(dominantRoot, recessiveRoot)];
        if (degree % 2 == 0 && dominantRoot != 0)
        {
            candidates.Add(new Proportion(-dominantRoot, recessiveRoot));
        }

        candidates = DistinctBy(candidates, candidate => (candidate.Dominant, candidate.Recessive)).ToList();
        Proportion principal = SelectProportionPrincipal(candidates, rule, reference);
        return new InverseContinuationResult<Proportion>(candidates, principal, []);
    }

    public static InverseContinuationResult<Axis> InverseContinue(
        Axis value,
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? reference = null)
    {
        if (degree <= 0)
        {
            return InvalidDegree<Axis>(degree);
        }

        return value.Basis switch
        {
            AxisBasis.Complex => InverseContinueComplexAxis(value, degree, rule, reference),
            AxisBasis.SplitComplex => InverseContinueSplitComplexAxis(value, degree, rule, reference),
            _ => new InverseContinuationResult<Axis>(
                [],
                null,
                [
                    new InverseContinuationTension(
                        InverseContinuationTensionKind.UnsupportedBasis,
                        $"The basis {value.Basis} does not currently support inverse continuation.")
                ]),
        };
    }

    public static InverseContinuationResult<Axis> InverseContinue(
        Area value,
        int degree,
        AreaInverseContinuationMode mode = AreaInverseContinuationMode.FoldFirst,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? foldedReference = null)
    {
        if (mode == AreaInverseContinuationMode.StructurePreserving)
        {
            return new InverseContinuationResult<Axis>(
                [],
                null,
                [
                    new InverseContinuationTension(
                        InverseContinuationTensionKind.StructurePreservingUnavailable,
                        $"Structure-preserving area inverse continuation is not implemented yet for degree {degree}. Use fold-first mode to invert the folded Axis value.")
                ]);
        }

        return InverseContinue(value.Value, degree, rule, foldedReference);
    }

    private static InverseContinuationResult<Axis> InverseContinueComplexAxis(
        Axis value,
        int degree,
        InverseContinuationRule rule,
        Axis? reference)
    {
        double real = (double)value.Dominant.Fold();
        double imaginary = (double)value.Recessive.Fold();
        Complex complex = new(real, imaginary);

        double magnitude = complex.Magnitude;
        double argument = complex.Phase;
        double rootMagnitude = Math.Pow(magnitude, 1d / degree);

        List<Axis> candidates = [];
        for (int k = 0; k < degree; k++)
        {
            double angle = (argument + (2d * Math.PI * k)) / degree;
            double candidateReal = ClampNearZero(rootMagnitude * Math.Cos(angle));
            double candidateImaginary = ClampNearZero(rootMagnitude * Math.Sin(angle));
            candidates.Add(Axis.FromCoordinates(
                (Scalar)(decimal)(-candidateImaginary),
                (Scalar)(decimal)candidateReal,
                Scalar.One,
                Scalar.One,
                value.Basis));
        }

        candidates = DistinctBy(candidates, candidate => RootKey(candidate)).ToList();
        Axis principal = SelectAxisPrincipal(candidates, rule, reference);
        return new InverseContinuationResult<Axis>(candidates, principal, []);
    }

    private static InverseContinuationResult<Axis> InverseContinueSplitComplexAxis(
        Axis value,
        int degree,
        InverseContinuationRule rule,
        Axis? reference)
    {
        if (value.Recessive != Proportion.Zero)
        {
            return new InverseContinuationResult<Axis>(
                [],
                null,
                [
                    new InverseContinuationTension(
                        InverseContinuationTensionKind.UnsupportedBasis,
                        "Split-complex inverse continuation is currently only supported for purely dominant values.")
                ]);
        }

        var dominantRoots = InverseContinue(value.Dominant, degree, rule, reference?.Dominant);
        if (!dominantRoots.Succeeded)
        {
            return new InverseContinuationResult<Axis>([], null, dominantRoots.Tensions);
        }

        List<Axis> candidates = dominantRoots.Candidates
            .Select(root => new Axis(Proportion.Zero, root, value.Basis))
            .ToList();

        Axis principal = SelectAxisPrincipal(candidates, rule, reference);
        return new InverseContinuationResult<Axis>(candidates, principal, dominantRoots.Tensions);
    }

    private static Scalar Root(decimal value, int degree) =>
        (Scalar)(decimal)Math.Pow((double)value, 1d / degree);

    private static Scalar SelectScalarPrincipal(
        IReadOnlyList<Scalar> candidates,
        InverseContinuationRule rule,
        Scalar? reference)
    {
        return rule switch
        {
            InverseContinuationRule.Principal => candidates.OrderByDescending(candidate => candidate.Value).First(),
            InverseContinuationRule.PreferPositiveDominant => candidates.OrderByDescending(candidate => candidate.Value).First(),
            InverseContinuationRule.NearestToReference when reference.HasValue =>
                candidates.MinBy(candidate => Math.Abs(candidate.Value - reference.Value.Value))!,
            _ => candidates[0],
        };
    }

    private static Proportion SelectProportionPrincipal(
        IReadOnlyList<Proportion> candidates,
        InverseContinuationRule rule,
        Proportion? reference)
    {
        return rule switch
        {
            InverseContinuationRule.Principal => candidates
                .OrderByDescending(candidate => candidate.Recessive > 0)
                .ThenByDescending(candidate => candidate.Dominant)
                .First(),
            InverseContinuationRule.PreferPositiveDominant => candidates
                .OrderByDescending(candidate => candidate.Dominant > 0)
                .ThenByDescending(candidate => candidate.Recessive > 0)
                .ThenByDescending(candidate => candidate.Dominant)
                .First(),
            InverseContinuationRule.NearestToReference when reference is not null => candidates
                .MinBy(candidate =>
                    Math.Abs(candidate.Dominant - reference.Dominant) +
                    Math.Abs(candidate.Recessive - reference.Recessive))!,
            _ => candidates[0],
        };
    }

    private static Axis SelectAxisPrincipal(
        IReadOnlyList<Axis> candidates,
        InverseContinuationRule rule,
        Axis? reference)
    {
        return rule switch
        {
            InverseContinuationRule.Principal => candidates
                .OrderByDescending(candidate => candidate.Dominant.Fold().Value)
                .ThenByDescending(candidate => candidate.Recessive.Fold().Value)
                .First(),
            InverseContinuationRule.PreferPositiveDominant => candidates
                .OrderByDescending(candidate => candidate.Dominant.Fold().Value > 0m)
                .ThenByDescending(candidate => candidate.Dominant.Fold().Value)
                .ThenByDescending(candidate => candidate.Recessive.Fold().Value)
                .First(),
            InverseContinuationRule.NearestToReference when reference is not null => candidates
                .MinBy(candidate =>
                    Math.Abs(candidate.Dominant.Fold().Value - reference.Dominant.Fold().Value) +
                    Math.Abs(candidate.Recessive.Fold().Value - reference.Recessive.Fold().Value))!,
            _ => candidates[0],
        };
    }

    private static InverseContinuationResult<T> InvalidDegree<T>(int degree) =>
        new(
            [],
            default,
            [
                new InverseContinuationTension(
                    InverseContinuationTensionKind.InvalidDegree,
                    $"Inverse continuation degree must be positive. Received {degree}.")
            ]);

    private static InverseContinuationResult<T> NoCandidates<T>(string message) =>
        new(
            [],
            default,
            [
                new InverseContinuationTension(
                    InverseContinuationTensionKind.NoCandidates,
                    message)
            ]);

    private static IEnumerable<T> DistinctBy<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : notnull
    {
        HashSet<TKey> seen = [];
        foreach (var item in source)
        {
            if (seen.Add(keySelector(item)))
            {
                yield return item;
            }
        }
    }

    private static (decimal Recessive, decimal Dominant) RootKey(Axis candidate) =>
        (Math.Round(candidate.Recessive.Fold().Value, 9), Math.Round(candidate.Dominant.Fold().Value, 9));

    private static double ClampNearZero(double value) =>
        Math.Abs(value) < 1e-9d ? 0d : value;

    private static bool TryExactIntegerRoot(long value, int degree, out long root)
    {
        if (value == 0)
        {
            root = 0;
            return true;
        }

        if (value < 0 && degree % 2 == 0)
        {
            root = 0;
            return false;
        }

        if (value == long.MinValue)
        {
            root = 0;
            return false;
        }

        bool negative = value < 0;
        long target = Math.Abs(value);
        long low = 0;
        long high = target;

        while (low <= high)
        {
            long mid = low + ((high - low) / 2);
            BigInteger power = BigInteger.Pow(new BigInteger(mid), degree);

            if (power == target)
            {
                root = negative ? -mid : mid;
                return true;
            }

            if (power < target)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        root = 0;
        return false;
    }
}
