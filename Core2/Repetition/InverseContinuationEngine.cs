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

        var numerator = InverseContinue(value.Dominant, degree, rule, reference?.Dominant);
        var denominator = InverseContinue(value.Recessive, degree, rule, reference?.Recessive);
        List<InverseContinuationTension> tensions = [.. numerator.Tensions, .. denominator.Tensions];

        if (!numerator.Succeeded || !denominator.Succeeded)
        {
            if (tensions.Count == 0)
            {
                tensions.Add(new InverseContinuationTension(
                    InverseContinuationTensionKind.NoCandidates,
                    $"No proportion inverse continuation candidates exist for degree {degree} and value {value}."));
            }

            return new InverseContinuationResult<Proportion>([], null, tensions);
        }

        List<Proportion> candidates = [];
        foreach (var dominantRoot in numerator.Candidates)
        {
            foreach (var recessiveRoot in denominator.Candidates)
            {
                candidates.Add(Proportion.FromScalars(dominantRoot, recessiveRoot));
            }
        }

        candidates = DistinctBy(candidates, candidate => (candidate.Dominant.Value, candidate.Recessive.Value)).ToList();
        Proportion principal = SelectProportionPrincipal(candidates, rule, reference);
        return new InverseContinuationResult<Proportion>(candidates, principal, tensions);
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
                .OrderByDescending(candidate => candidate.Recessive.Value > 0m)
                .ThenByDescending(candidate => candidate.Dominant.Value)
                .First(),
            InverseContinuationRule.PreferPositiveDominant => candidates
                .OrderByDescending(candidate => candidate.Dominant.Value > 0m)
                .ThenByDescending(candidate => candidate.Recessive.Value > 0m)
                .ThenByDescending(candidate => candidate.Dominant.Value)
                .First(),
            InverseContinuationRule.NearestToReference when reference is not null => candidates
                .MinBy(candidate =>
                    Math.Abs(candidate.Dominant.Value - reference.Dominant.Value) +
                    Math.Abs(candidate.Recessive.Value - reference.Recessive.Value))!,
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
}
