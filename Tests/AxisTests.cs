using ResoEngine;
using ResoEngine.Support;

namespace Tests;

public class AxisTests
{
    // --- Injectable algebra ---

    [Fact]
    public void DefaultAlgebra_IsComplex()
    {
        var left = new Proportion(1, 0, Chirality.Con);
        var right = new Proportion(1, 1, Chirality.Pro);
        var axis = new Axis(left, right, Chirality.Pro);
        Assert.Equal(Axis.ComplexAlgebra, axis.Algebra);
    }

    [Fact]
    public void CustomAlgebra_IsUsed()
    {
        var customAlgebra = new[]
        {
            new AlgebraEntry(0, 0, 0, +1), // simple pairwise
            new AlgebraEntry(1, 1, 1, +1),
        };
        var left = new Proportion(1, 3, Chirality.Con);
        var right = new Proportion(5, 1, Chirality.Pro);
        var axis = new Axis(left, right, Chirality.Pro, customAlgebra);
        Assert.Equal(customAlgebra, axis.Algebra);
    }

    // --- Frame factory ---

    [Fact]
    public void Frame_CorrectExtentsAndResolution()
    {
        // extent0=10 (height/imaginary), extent1=20 (width/real), resolution=2
        var frame = Axis.Frame(10, 20, 2);
        Assert.Equal(10, frame.Min);  // imaginary extent
        Assert.Equal(20, frame.Max);  // real extent
        Assert.Equal(2, frame.Unot);  // imaginary resolution
        Assert.Equal(2, frame.Unit);  // real resolution
    }

    [Fact]
    public void Frame_WithCustomAlgebra()
    {
        var custom = new[] { new AlgebraEntry(0, 1, 0, +1) };
        var frame = Axis.Frame(10, 20, 1, custom);
        Assert.Equal(custom, frame.Algebra);
    }

    // --- Fold = Left * Right (deferred multiplication) ---

    [Fact]
    public void Fold_ExecutesDeferredMultiplication()
    {
        var frame = Axis.Frame(10, 20, 2);
        var folded = frame.Fold();
        // Left: num=10, den=2. Right: num=20, den=2.
        // Product: Proportion(200, 4, Pro)
        Assert.Equal(200, folded.GetNumerator());
        Assert.Equal(4, folded.GetDenominator());
    }

    [Fact]
    public void Fold_ToDouble_FullChain()
    {
        var frame = Axis.Frame(10, 20, 2);
        // Axis -> Proportion -> double
        // (10/2) * (20/2) = 5 * 10 = 50 square units
        Assert.Equal(50.0, frame.Fold().Fold());
    }

    [Fact]
    public void Fold_UnitResolution_IsSimpleProduct()
    {
        var frame = Axis.Frame(7, 11, 1);
        // (7/1) * (11/1) = 77
        Assert.Equal(77.0, frame.Fold().Fold());
    }

    // --- Operator * at Proportion level ---

    [Fact]
    public void Multiply_PureReal_GivesRealProduct()
    {
        // a = 3 + 0i, b = 5 + 0i → result = 15 + 0i
        var a = Axis.Frame(0, 3, 1);
        var b = Axis.Frame(0, 5, 1);
        var result = a * b;
        Assert.Equal(15, result.Max); // real part = 3*5
        Assert.Equal(0, result.Min);  // imaginary part = 0
    }

    [Fact]
    public void Multiply_ComplexNumbers()
    {
        // (3 + 2i)(1 + 4i) = (3-8) + (12+2)i = -5 + 14i
        // Right=real, Left=imaginary
        var a = new Axis(
            new Proportion(1, 2, Chirality.Con),  // imag=2
            new Proportion(3, 1, Chirality.Pro),   // real=3
            Chirality.Pro);
        var b = new Axis(
            new Proportion(1, 4, Chirality.Con),  // imag=4
            new Proportion(1, 1, Chirality.Pro),   // real=1
            Chirality.Pro);

        var result = a * b;
        Assert.Equal(-5, result.Max);  // real part
        Assert.Equal(14, result.Min);  // imaginary part
    }

    [Fact]
    public void Multiply_InheritsAlgebra()
    {
        var custom = new[]
        {
            new AlgebraEntry(0, 0, 0, +1),
            new AlgebraEntry(1, 1, 1, +1),
        };
        var a = new Axis(new Proportion(1, 3, Chirality.Con), new Proportion(5, 1, Chirality.Pro), Chirality.Pro, custom);
        var b = new Axis(new Proportion(1, 2, Chirality.Con), new Proportion(7, 1, Chirality.Pro), Chirality.Pro, custom);
        var result = a * b;
        Assert.Equal(custom, result.Algebra);
    }

    // --- GetElement ---

    [Fact]
    public void GetElement_0IsLeft_1IsRight()
    {
        var left = new Proportion(1, 5, Chirality.Con);
        var right = new Proportion(10, 1, Chirality.Pro);
        var axis = new Axis(left, right, Chirality.Pro);
        Assert.Same(left, axis.GetElement(0));
        Assert.Same(right, axis.GetElement(1));
    }

    [Fact]
    public void GetElement_OutOfRange_Throws()
    {
        var axis = Axis.Frame(1, 1, 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => axis.GetElement(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => axis.GetElement(-1));
    }

    // --- Long-level backward compat ---

    [Fact]
    public void LongDims_IsFour()
    {
        var axis = Axis.Frame(10, 20, 2);
        Assert.Equal(4, axis.LongDims);
    }

    [Fact]
    public void GetValueByIndex_MapsCorrectly()
    {
        var axis = Axis.Frame(10, 20, 2);
        Assert.Equal(2, axis[0]);  // Left.GetDenominator = Unot
        Assert.Equal(10, axis[1]); // Left.GetNumerator = Min
        Assert.Equal(2, axis[2]);  // Right.GetDenominator = Unit
        Assert.Equal(20, axis[3]); // Right.GetNumerator = Max
    }

    // --- IValue ---

    [Fact]
    public void GetTicksByPerspective_Pro_ReturnsMax()
    {
        var axis = Axis.Frame(10, 20, 1);
        Assert.Equal(20, axis.GetTicksByPerspective(Chirality.Pro));
    }

    [Fact]
    public void GetTicksByPerspective_Con_ReturnsMin()
    {
        var axis = Axis.Frame(10, 20, 1);
        Assert.Equal(10, axis.GetTicksByPerspective(Chirality.Con));
    }
}
