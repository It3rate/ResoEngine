using Core2.Branching;
using Core2.Elements;
using Core2.Repetition;
using Core2.Units;

namespace Tests.Core2;

public class UnitSignatureTests
{
    private static readonly PhysicalReferent DistanceReferent = new("distance", "Distance");
    private static readonly PhysicalReferent TimeReferent = new("time", "Time");
    private static readonly UnitGenerator Length = new("length", "L", DistanceReferent);
    private static readonly UnitGenerator Time = new("time", "T", TimeReferent);

    [Fact]
    public void UnitSignature_UsesFreeAbelianExponentArithmetic()
    {
        var velocity = UnitSignature.From(Length).Divide(UnitSignature.From(Time));
        var acceleration = velocity.Divide(UnitSignature.From(Time));

        Assert.Equal("L T^-1", velocity.ToString());
        Assert.Equal("L T^-2", acceleration.ToString());
        Assert.Equal(UnitSignature.From(Time).Reciprocal(), UnitSignature.From(Time, -1));
    }

    [Fact]
    public void UnitSignature_Pow_RepeatsMultiplicativeExponent()
    {
        var area = UnitSignature.From(Length).Pow(2);

        Assert.Equal("L^2", area.ToString());
        Assert.Contains(DistanceReferent, area.Referents);
    }

    [Fact]
    public void UnitSignature_Pow_SupportsRationalExponents()
    {
        var rootLength = UnitSignature.From(Length).Pow(new Proportion(1, 2));
        var restored = rootLength.Multiply(rootLength);

        Assert.Equal("L^(1/2)", rootLength.ToString());
        Assert.Equal(UnitSignature.From(Length), restored);
    }

    [Fact]
    public void Quantity_Addition_RequiresMatchingSignatures()
    {
        var left = new Scalar(3).AsQuantity(UnitSignature.From(Length));
        var right = new Scalar(2).AsQuantity(UnitSignature.From(Time));

        var result = left.TryAdd(right);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Tensions, tension => tension.Kind == QuantityTensionKind.SignatureMismatch);
    }

    [Fact]
    public void Quantity_Addition_AllowsSameSignature_AndTracksPreferredUnitMismatch()
    {
        var lengthSignature = UnitSignature.From(Length);
        var meters = new UnitChoice("meter", "m", lengthSignature, Scalar.One, DistanceReferent);
        var feet = new UnitChoice("foot", "ft", lengthSignature, new Scalar(0.3048m), DistanceReferent);

        var left = new Scalar(3).AsQuantity(lengthSignature, meters);
        var right = new Scalar(4).AsQuantity(lengthSignature, feet);

        var result = left.TryAdd(right);

        Assert.True(result.Succeeded);
        Assert.Equal(new Scalar(7), result.Quantity!.Value.Value);
        Assert.Null(result.Quantity.Value.PreferredUnit);
        Assert.Contains(result.Tensions, tension => tension.Kind == QuantityTensionKind.PreferredUnitMismatch);
    }

    [Fact]
    public void Quantity_Multiplication_CombinesStructureAndUnits()
    {
        var left = new Axis(new Proportion(3, 1), new Proportion(5, 1))
            .AsQuantity(UnitSignature.From(Length));
        var right = new Axis(new Proportion(2, 1), new Proportion(4, 1))
            .AsQuantity(UnitSignature.From(Time, -1));

        var product = left.Multiply(right);

        Assert.Equal(left.Value * right.Value, product.Value);
        Assert.Equal("L T^-1", product.Signature.ToString());
    }

    [Fact]
    public void Quantity_Repetition_DistinguishesAdditiveAndMultiplicativePatterns()
    {
        var value = new Scalar(2).AsQuantity(UnitSignature.From(Length));

        var additive = value.RepeatAdditive(4);
        var multiplicative = value.TryPow(4);

        Assert.Equal(new Scalar(8), additive.Value);
        Assert.Equal(UnitSignature.From(Length), additive.Signature);
        Assert.True(multiplicative.Succeeded);
        Assert.Equal(new Scalar(16), multiplicative.Quantity!.Value.Value);
        Assert.Equal("L^4", multiplicative.Quantity.Value.Signature.ToString());
    }

    [Fact]
    public void Quantity_FractionalPower_AppliesToValueAndUnitSignature()
    {
        var area = new Scalar(4).AsQuantity(UnitSignature.From(Length).Pow(2));

        var rooted = area.TryPow(new Proportion(1, 2));

        Assert.True(rooted.Succeeded);
        Assert.Equal(new Scalar(2), rooted.PrincipalCandidate!.Value);
        Assert.Equal(UnitSignature.From(Length), rooted.PrincipalCandidate.Signature);
        Assert.Contains(rooted.Candidates, candidate => candidate.Value == new Scalar(2));
        Assert.Contains(rooted.Candidates, candidate => candidate.Value == new Scalar(-2));
        Assert.Equal(BranchOrigin.Preimage, rooted.Branches.Origin);
        Assert.Equal(BranchDirection.Forward, rooted.Branches.Direction);
        Assert.All(rooted.Branches.Members, member => Assert.Single(member.Parents));
    }

    [Fact]
    public void Quantity_FractionalPower_ReportsShapeChangingAreaPath()
    {
        var quantity = new Area(Axis.I, Axis.I).AsQuantity(UnitSignature.From(Length).Pow(2));

        var result = quantity.TryPow(new Proportion(1, 2));

        Assert.False(result.Succeeded);
        Assert.Contains(result.Tensions, tension => tension.Kind == PowerTensionKind.ShapeChangingPower);
    }
}
