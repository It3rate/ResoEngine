using ResoEngine;
using ResoEngine.Support;

namespace Tests;

public class ScalarTests
{
    [Fact]
    public void ImplicitConversion_LongToScalar()
    {
        Scalar s = 42L;
        Assert.Equal(42, s.Value);
    }

    [Fact]
    public void ImplicitConversion_ScalarToLong()
    {
        long v = new Scalar(99);
        Assert.Equal(99, v);
    }

    [Fact]
    public void Fold_ReturnsValue()
    {
        var s = new Scalar(7);
        Assert.Equal(7, s.Fold());
    }

    [Fact]
    public void Dims_IsOne()
    {
        var s = new Scalar(1);
        Assert.Equal(1, s.Dims);
    }

    [Fact]
    public void Algebra_SingleSelfMultiply()
    {
        var s = new Scalar(5);
        Assert.Single(s.Algebra);
        var entry = s.Algebra[0];
        Assert.Equal(0, entry.LeftIndex);
        Assert.Equal(0, entry.RightIndex);
        Assert.Equal(0, entry.ResultIndex);
        Assert.Equal(1, entry.Sign);
    }

    [Fact]
    public void GetElement_ReturnsValue()
    {
        var s = new Scalar(13);
        Assert.Equal(13, s.GetElement(0));
        Assert.Equal(13, s.GetElement(999)); // any index returns the single value
    }

    [Fact]
    public void GetTicksByPerspective_AlwaysReturnsValue()
    {
        var s = new Scalar(10);
        Assert.Equal(10, s.GetTicksByPerspective(Chirality.Pro));
        Assert.Equal(10, s.GetTicksByPerspective(Chirality.Con));
    }

    [Fact]
    public void ToString_ShowsValue()
    {
        Assert.Equal("42", new Scalar(42).ToString());
    }
}
