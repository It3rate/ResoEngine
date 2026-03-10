using ResoEngine;
using ResoEngine.Support;

namespace Tests;

public class AlgebraTableTests
{
    // --- Existing Operate functionality ---

    [Fact]
    public void Operate_ByIndex_UsesAccumulator()
    {
        var table = new AlgebraTable<long>([3, 7], (a, b) => a * b);
        Assert.Equal(21, table.Operate(0, 1));
    }

    [Fact]
    public void Operate_ByValue_UsesAccumulator()
    {
        var table = new AlgebraTable<long>([1], (a, b) => a + b);
        Assert.Equal(15, table.Operate(10L, 5L)); // explicit long to hit the T,T overload
    }

    [Fact]
    public void Operate_OutOfRange_Throws()
    {
        var table = new AlgebraTable<long>([1, 2], (a, b) => a + b);
        Assert.Throws<ArgumentOutOfRangeException>(() => table.Operate(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => table.Operate(0, 5));
    }

    // --- Pin ---

    [Fact]
    public void Pin_FromProportion_ExtractsElements()
    {
        var p = new Proportion(20, 3, Chirality.Pro);
        var table = AlgebraTable<long>.Pin(p, (a, b) => a * b);
        Assert.Equal(2, table.Elements.Count);
        Assert.Equal(20, table.Elements[0]); // numerator
        Assert.Equal(3, table.Elements[1]);  // denominator
    }

    [Fact]
    public void Pin_FromAxis_ExtractsProportions()
    {
        var axis = Axis.Frame(10, 20, 1);
        var table = AlgebraTable<Proportion>.Pin(axis, (a, b) => a * b);
        Assert.Equal(2, table.Elements.Count);
        Assert.Same(axis.Left, table.Elements[0]);
        Assert.Same(axis.Right, table.Elements[1]);
    }

    // --- Add ---

    [Fact]
    public void Add_AppendsElement()
    {
        var table = new AlgebraTable<long>([1, 2], (a, b) => a + b);
        Assert.Equal(2, table.Elements.Count);
        table.Add(3);
        Assert.Equal(3, table.Elements.Count);
        Assert.Equal(3, table.Elements[2]);
    }

    // --- Fold ---

    [Fact]
    public void Fold_LongsWithMultiplication()
    {
        var table = new AlgebraTable<long>([2, 3, 5], (a, b) => a * b);
        Assert.Equal(30, table.Fold()); // 2 * 3 * 5
    }

    [Fact]
    public void Fold_LongsWithAddition()
    {
        var table = new AlgebraTable<long>([10, 20, 30], (a, b) => a + b);
        Assert.Equal(60, table.Fold()); // 10 + 20 + 30
    }

    [Fact]
    public void Fold_ProportionsWithMultiplication()
    {
        var table = new AlgebraTable<Proportion>(
            [new Proportion(3, 1, Chirality.Pro), new Proportion(7, 1, Chirality.Pro)],
            (a, b) => a * b);
        var result = table.Fold();
        Assert.Equal(21.0, result.Fold()); // (3/1) * (7/1) = 21/1
    }

    [Fact]
    public void Fold_SingleElement_ReturnsThatElement()
    {
        var table = new AlgebraTable<long>([42], (a, b) => a * b);
        Assert.Equal(42, table.Fold());
    }

    [Fact]
    public void Fold_Empty_Throws()
    {
        var table = new AlgebraTable<long>([], (a, b) => a * b);
        Assert.Throws<InvalidOperationException>(() => table.Fold());
    }

    // --- Generic Multiply ---

    [Fact]
    public void Multiply_Generic_ProportionLevelAlgebra()
    {
        var a = new Proportion(5, 3, Chirality.Pro);
        var b = new Proportion(7, 2, Chirality.Pro);
        var result = AlgebraTable<long>.Multiply(a, b,
            multiply: (x, y) => x * y,
            add: (x, y) => x + y,
            scale: (x, s) => x * s,
            zero: 0L);
        Assert.Equal(2, result.Length);
        Assert.Equal(35, result[0]); // num: 5 * 7
        Assert.Equal(6, result[1]);  // den: 3 * 2
    }

    // --- AlgebraOps convenience ---

    [Fact]
    public void AlgebraOps_MultiplyLongs()
    {
        var a = new Proportion(4, 5, Chirality.Pro);
        var b = new Proportion(6, 7, Chirality.Pro);
        var result = AlgebraOps.Multiply(a, b);
        Assert.Equal(24, result[0]); // 4*6
        Assert.Equal(35, result[1]); // 5*7
    }

    [Fact]
    public void AlgebraOps_MultiplyProportions()
    {
        var a = Axis.Frame(2, 3, 1);
        var b = Axis.Frame(4, 5, 1);
        var result = AlgebraOps.Multiply(a, b);
        Assert.Equal(2, result.Length);
    }

    // --- Pin + Add + Fold pattern ---

    [Fact]
    public void PinAddFold_WidthTimesHeight()
    {
        var width = new Proportion(1920, 1, Chirality.Pro);
        var height = new Proportion(1080, 1, Chirality.Pro);
        var table = new AlgebraTable<Proportion>([width, height], (a, b) => a * b);
        var area = table.Fold();
        Assert.Equal(1920.0 * 1080.0, area.Fold());
    }

    [Fact]
    public void PinAddFold_AccumulateMultipleValues()
    {
        var table = new AlgebraTable<long>([2, 3], (a, b) => a * b);
        table.Add(5);
        table.Add(7);
        Assert.Equal(210, table.Fold()); // 2 * 3 * 5 * 7
    }
}
