using Core2.Branching;
using Core2.Elements;
using Core2.Support;
using System.Numerics;

namespace Core2.Repetition;

public static class PowerEngine
{
    private const int AxisApproximationDecimals = 2;
    private const long AxisApproximationSupport = 100;

    public static PowerResult<Scalar> Pow(
        Scalar value,
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Scalar? reference = null)
    {
        if (!TryNormalize(exponent, out var normalized, out var error))
        {
            return InvalidExponent<Scalar>(error);
        }

        if (normalized.Numerator < 0)
        {
            return UnsupportedNegativeExponent<Scalar>(exponent);
        }

        if (normalized.IsZero)
        {
            return SingleCandidate(Scalar.One);
        }

        if (normalized.IsInteger)
        {
            var trace = RepetitionEngine.RepeatMultiplicative(value, normalized.Numerator);
            return SingleCandidate(trace.Result);
        }

        var roots = InverseContinuationEngine.InverseContinue(value, normalized.Denominator, rule, reference);
        if (!roots.Succeeded)
        {
            return FromInverseFailure(roots);
        }

        List<Scalar> candidates = roots.Candidates
            .Select(root => RepetitionEngine.RepeatMultiplicative(root, normalized.Numerator).Result)
            .Distinct()
            .ToList();

        Scalar principal = SelectScalarPrincipal(candidates, rule, reference);
        var branches = ProjectRoots(
            roots.Branches,
            root => RepetitionEngine.RepeatMultiplicative(root, normalized.Numerator).Result,
            candidate => candidate.Value,
            principal);

        return new PowerResult<Scalar>(branches, []);
    }

    public static PowerResult<Proportion> Pow(
        Proportion value,
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Proportion? reference = null)
    {
        if (!TryNormalize(exponent, out var normalized, out var error))
        {
            return InvalidExponent<Proportion>(error);
        }

        if (normalized.Numerator < 0)
        {
            return UnsupportedNegativeExponent<Proportion>(exponent);
        }

        if (normalized.IsZero)
        {
            return SingleCandidate(Proportion.One);
        }

        if (normalized.IsInteger)
        {
            var trace = RepetitionEngine.RepeatMultiplicative(value, normalized.Numerator);
            return SingleCandidate(trace.Result);
        }

        var roots = InverseContinuationEngine.InverseContinue(value, normalized.Denominator, rule, reference);
        if (!roots.Succeeded)
        {
            return FromInverseFailure(roots);
        }

        List<Proportion> candidates = roots.Candidates
            .Select(root => RepetitionEngine.RepeatMultiplicative(root, normalized.Numerator).Result)
            .Distinct()
            .ToList();

        Proportion principal = SelectProportionPrincipal(candidates, rule, reference);
        var branches = ProjectRoots(
            roots.Branches,
            root => RepetitionEngine.RepeatMultiplicative(root, normalized.Numerator).Result,
            candidate => (candidate.Dominant, candidate.Recessive),
            principal);

        return new PowerResult<Proportion>(branches, []);
    }

    public static PowerResult<Axis> Pow(
        Axis value,
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? reference = null)
    {
        try
        {
            if (!TryNormalize(exponent, out var normalized, out var error))
            {
                return InvalidExponent<Axis>(error);
            }

            if (normalized.Numerator < 0)
            {
                return UnsupportedNegativeExponent<Axis>(exponent);
            }

            if (normalized.IsZero)
            {
                return SingleCandidate(Axis.One);
            }

            if (normalized.IsInteger)
            {
                var trace = RepetitionEngine.RepeatMultiplicative(value, normalized.Numerator);
                return SingleCandidate(trace.Result);
            }

            var roots = InverseContinuationEngine.InverseContinue(value, normalized.Denominator, rule, reference);
            if (!roots.Succeeded)
            {
                return FromInverseFailure(roots);
            }

            List<Axis> candidates = roots.Candidates
                .Select(root => RaiseAxisCandidate(root, normalized.Numerator))
                .DistinctBy(candidate => RootKey(candidate))
                .ToList();

            Axis principal = SelectAxisPrincipal(candidates, rule, reference);
            var branches = ProjectRoots(
                roots.Branches,
                root => RaiseAxisCandidate(root, normalized.Numerator),
                RootKey,
                principal);

            return new PowerResult<Axis>(branches, []);
        }
        catch (OverflowException exception)
        {
            return ComputationOverflow<Axis>(exponent, exception);
        }
    }

    public static PowerResult<Axis> Pow(
        Area value,
        Proportion exponent,
        AreaInverseContinuationMode mode = AreaInverseContinuationMode.FoldFirst,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? foldedReference = null)
    {
        if (mode == AreaInverseContinuationMode.StructurePreserving)
        {
            return new PowerResult<Axis>(
                [],
                null,
                [
                    new PowerTension(
                        PowerTensionKind.ShapeChangingPower,
                        "Structure-preserving fractional powers for Area are not implemented yet. Use fold-first mode to power the folded Axis value.")
                ]);
        }

        return Pow(value.Value, exponent, rule, foldedReference);
    }

    internal static bool TryNormalize(Proportion exponent, out RationalExponent normalized, out string error)
    {
        if (!RationalExponent.TryFrom(exponent, out normalized))
        {
            error = "Fractional powers require rational exponents with integer numerator and nonzero integer denominator.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static PowerResult<T> InvalidExponent<T>(string message) =>
        new([], default, [new PowerTension(PowerTensionKind.InvalidExponent, message)]);

    private static PowerResult<T> UnsupportedNegativeExponent<T>(Proportion exponent) =>
        new([], default, [new PowerTension(PowerTensionKind.UnsupportedNegativeExponent, $"Negative exponents are not implemented yet for {exponent}.")]);

    private static PowerResult<T> ComputationOverflow<T>(Proportion exponent, OverflowException exception) =>
        new([], default, [new PowerTension(PowerTensionKind.ComputationOverflow, $"Power computation overflowed for exponent {exponent}: {exception.Message}")]);

    private static PowerResult<T> FromInverseFailure<T>(InverseContinuationResult<T> roots) =>
        new(
            BranchFamily<T>.Empty(
                roots.Branches.Origin,
                roots.Branches.Semantics,
                BranchDirection.Forward,
                roots.Branches.Tensions,
                roots.Branches.Annotations),
            roots.Tensions.Select(tension => new PowerTension(PowerTensionKind.InverseContinuationFailed, tension.Message)).ToArray());

    private static PowerResult<T> SingleCandidate<T>(T value) =>
        new(
            BranchFamily<T>.FromValues(
                BranchOrigin.Continuation,
                BranchSemantics.Alternative,
                BranchDirection.Forward,
                [value],
                selectedIndex: 0,
                selectionMode: BranchSelectionMode.Principal),
            []);

    private static BranchFamily<T> ProjectRoots<T, TKey>(
        BranchFamily<T> roots,
        Func<T, T> projector,
        Func<T, TKey> keySelector,
        T principal)
        where TKey : notnull
    {
        Dictionary<TKey, BranchMember<T>> membersByKey = [];
        var keyComparer = EqualityComparer<TKey>.Default;

        foreach (var root in roots.Members)
        {
            T projected = projector(root.Value);
            TKey key = keySelector(projected);
            if (membersByKey.TryGetValue(key, out var existing))
            {
                var parents = existing.Parents
                    .Concat([root.Id])
                    .Distinct()
                    .ToArray();

                membersByKey[key] = existing with { Parents = parents };
                continue;
            }

            membersByKey[key] = new BranchMember<T>(
                BranchId.New(),
                projected,
                [root.Id],
                []);
        }

        var members = membersByKey.Values.ToArray();
        TKey principalKey = keySelector(principal);
        BranchSelection selection = BranchSelection.None;
        foreach (var member in members)
        {
            if (keyComparer.Equals(keySelector(member.Value), principalKey))
            {
                selection = BranchSelection.Principal(member.Id);
                break;
            }
        }

        return BranchFamily<T>.FromMembers(
            roots.Origin,
            roots.Semantics,
            BranchDirection.Forward,
            members,
            selection,
            roots.Tensions,
            roots.Annotations);
    }

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

    private static (decimal Recessive, decimal Dominant) RootKey(Axis candidate) =>
        (Math.Round(candidate.Recessive.Fold().Value, 9), Math.Round(candidate.Dominant.Fold().Value, 9));

    private static Axis RaiseAxisCandidate(Axis root, int numerator)
    {
        if (root.Basis != AxisBasis.Complex)
        {
            return RepetitionEngine.RepeatMultiplicative(root, numerator).Result;
        }

        Complex complex = new((double)root.Dominant.Fold(), (double)root.Recessive.Fold());
        Complex raised = Complex.Pow(complex, numerator);
        return CreateApproximateComplexAxis(raised.Imaginary, raised.Real, root.Basis);
    }

    private static Axis CreateApproximateComplexAxis(double imaginary, double real, AxisBasis basis) =>
        Axis.FromCoordinates(
            QuantizeCoordinate(-imaginary),
            QuantizeCoordinate(real),
            basis);

    private static Proportion QuantizeCoordinate(double value)
    {
        decimal rounded = decimal.Round(
            (decimal)(Math.Abs(value) < 1e-9d ? 0d : value),
            AxisApproximationDecimals,
            MidpointRounding.AwayFromZero);

        if (rounded == 0m)
        {
            return Proportion.Zero;
        }

        if (rounded == decimal.Truncate(rounded))
        {
            return new Proportion((long)rounded, 1);
        }

        long numerator = checked((long)(rounded * AxisApproximationSupport));
        long denominator = AxisApproximationSupport;
        long divisor = GreatestCommonDivisor(Math.Abs(numerator), denominator);
        return new Proportion(numerator / divisor, denominator / divisor);
    }

    private static long GreatestCommonDivisor(long left, long right)
    {
        while (right != 0)
        {
            long remainder = left % right;
            left = right;
            right = remainder;
        }

        return left == 0 ? 1 : Math.Abs(left);
    }
}
