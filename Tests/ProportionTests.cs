using ResoEngine;
using ResoEngine.Support;

namespace Tests;

public class ProportionTests
{
    // --- Fold returns double ---

    [Fact]
    public void Fold_ReturnsDouble_ExactDivision()
    {
        var p = new Proportion(10, 2, Chirality.Pro);
        Assert.Equal(5.0, p.Fold());
    }

    [Fact]
    public void Fold_ReturnsDouble_FractionalResult()
    {
        var p = new Proportion(5, 3, Chirality.Pro);
        Assert.Equal(5.0 / 3.0, p.Fold(), precision: 10);
    }

    [Fact]
    public void Fold_ZeroDenominator_ReturnsZero()
    {
        var p = new Proportion(5, 0, Chirality.Pro);
        Assert.Equal(0.0, p.Fold());
    }

    [Fact]
    public void Fold_UnitDenominator_ReturnsNumerator()
    {
        var p = new Proportion(1920, 1, Chirality.Pro);
        Assert.Equal(1920.0, p.Fold());
    }

    // --- IAlgebraic<long> ---

    [Fact]
    public void Dims_IsTwo()
    {
        var p = new Proportion(5, 3, Chirality.Pro);
        Assert.Equal(2, p.Dims);
    }

    [Fact]
    public void Algebra_HasTwoDiagonalEntries()
    {
        var p = new Proportion(5, 3, Chirality.Pro);
        Assert.Equal(2, p.Algebra.Length);
        // num * num -> +num
        Assert.Equal(new AlgebraEntry(0, 0, 0, +1), p.Algebra[0]);
        // den * den -> +den
        Assert.Equal(new AlgebraEntry(1, 1, 1, +1), p.Algebra[1]);
    }

    [Fact]
    public void GetElement_Pro_Index0IsNumerator_Index1IsDenominator()
    {
        var p = new Proportion(20, 3, Chirality.Pro);
        Assert.Equal(20, p.GetElement(0)); // numerator
        Assert.Equal(3, p.GetElement(1));  // denominator
    }

    [Fact]
    public void GetElement_Con_ChiralityFlipsInterpretation()
    {
        var p = new Proportion(2, 10, Chirality.Con);
        // Con: numerator = bot = 10, denominator = top = 2
        Assert.Equal(10, p.GetElement(0)); // numerator
        Assert.Equal(2, p.GetElement(1));  // denominator
    }

    [Fact]
    public void GetElement_OutOfRange_Throws()
    {
        var p = new Proportion(1, 1, Chirality.Pro);
        Assert.Throws<ArgumentOutOfRangeException>(() => p.GetElement(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => p.GetElement(-1));
    }

    // --- Operators ---

    [Fact]
    public void Multiply_FractionMultiplication()
    {
        var a = new Proportion(3, 4, Chirality.Pro); // 3/4
        var b = new Proportion(5, 7, Chirality.Pro); // 5/7
        var result = a * b;
        Assert.Equal(15, result.GetNumerator()); // 3*5
        Assert.Equal(28, result.GetDenominator()); // 4*7
    }

    [Fact]
    public void Add_FractionAddition()
    {
        var a = new Proportion(1, 2, Chirality.Pro); // 1/2
        var b = new Proportion(1, 3, Chirality.Pro); // 1/3
        var result = a + b;
        // (1*3 + 1*2) / (2*3) = 5/6
        Assert.Equal(5, result.GetNumerator());
        Assert.Equal(6, result.GetDenominator());
    }

    [Fact]
    public void UnaryNegation_NegatesNumerator()
    {
        var p = new Proportion(5, 3, Chirality.Pro);
        var neg = -p;
        Assert.Equal(-5, neg.GetNumerator());
        Assert.Equal(3, neg.GetDenominator());
    }

    // --- Zero ---

    [Fact]
    public void Zero_IsAdditiveIdentity()
    {
        var p = new Proportion(7, 3, Chirality.Pro);
        var result = Proportion.Zero + p;
        // (0*3 + 7*1) / (1*3) = 7/3
        Assert.Equal(7.0 / 3.0, result.Fold(), precision: 10);
    }

    // --- ChildValues ---

    [Fact]
    public void ChildValues_AreScalarsOfNumeratorAndDenominator()
    {
        var p = new Proportion(20, 4, Chirality.Pro);
        var children = p.ChildValues;
        Assert.Equal(2, children.Length);
        Assert.Equal(20, children[0].GetTicksByPerspective(Chirality.Pro)); // numerator
        Assert.Equal(4, children[1].GetTicksByPerspective(Chirality.Pro));  // denominator
    }

    // --- Full fold chain: Proportion -> double ---

    [Fact]
    public void FoldChain_ProportionToDouble()
    {
        // 1920 ticks at 2 ticks/unit
        var p = new Proportion(1920, 2, Chirality.Pro);
        Assert.Equal(960.0, p.Fold());
    }
}
