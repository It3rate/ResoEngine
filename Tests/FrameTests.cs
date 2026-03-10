using ResoEngine;
using ResoEngine.Support;

namespace Tests;

public class FrameTests
{
    // --- AddSample with inherited resolution ---

    [Fact]
    public void AddSample_CreatesChildWithParentResolution()
    {
        var frame = Axis.Frame(10, 20, 2);
        var child = frame.AddSample(5, 8);

        // Child should have parent's resolution
        Assert.Equal(2, child.Unot); // inherited imaginary resolution
        Assert.Equal(2, child.Unit); // inherited real resolution
        // Child should have the tick values
        Assert.Equal(5, child.Min);  // imaginary tick
        Assert.Equal(8, child.Max);  // real tick
    }

    [Fact]
    public void AddSample_ChildFoldsCorrectly()
    {
        var frame = Axis.Frame(10, 20, 2);
        var child = frame.AddSample(5, 8);

        // Fold: (5/2) * (8/2) = 2.5 * 4 = 10.0
        Assert.Equal(10.0, child.Fold().Fold());
    }

    [Fact]
    public void AddSample_ReturnsChild()
    {
        var frame = Axis.Frame(10, 20, 1);
        var child = frame.AddSample(3, 7);
        Assert.NotNull(child);
        Assert.Single(frame.Children);
        Assert.Same(child, frame.Children[0]);
    }

    [Fact]
    public void AddSample_SetsParentReference()
    {
        var frame = Axis.Frame(10, 20, 1);
        var child = frame.AddSample(3, 7);
        Assert.Same(frame, child.Parent);
    }

    [Fact]
    public void AddSample_InheritsAlgebra()
    {
        var custom = new[]
        {
            new AlgebraEntry(0, 0, 0, +1),
            new AlgebraEntry(1, 1, 1, +1),
        };
        var frame = Axis.Frame(10, 20, 1, custom);
        var child = frame.AddSample(3, 7);
        Assert.Equal(custom, child.Algebra);
    }

    [Fact]
    public void AddSample_MultipleSamples()
    {
        var frame = Axis.Frame(10, 20, 1);
        frame.AddSample(1, 2);
        frame.AddSample(3, 4);
        frame.AddSample(5, 6);
        Assert.Equal(3, frame.Children.Count);
    }

    // --- AddChild with parent ref ---

    [Fact]
    public void AddChild_SetsParentAndAddsToChildren()
    {
        var parent = Axis.Frame(100, 200, 1);
        var child = Axis.Frame(10, 20, 2);
        var returned = parent.AddChild(child);

        Assert.Same(child, returned);
        Assert.Same(parent, child.Parent);
        Assert.Single(parent.Children);
        Assert.Same(child, parent.Children[0]);
    }

    [Fact]
    public void AddChild_CanOverrideResolution()
    {
        var parent = Axis.Frame(100, 200, 1);  // resolution 1
        var child = Axis.Frame(10, 20, 5);     // resolution 5 (override)
        parent.AddChild(child);

        Assert.Equal(5, child.Unit);  // child has its own resolution
        Assert.Equal(1, parent.Unit); // parent unchanged
    }

    // --- FoldChild ---

    [Fact]
    public void FoldChild_FoldsSpecificChild()
    {
        var frame = Axis.Frame(10, 20, 1);
        frame.AddSample(3, 7);   // fold = 3*7 = 21
        frame.AddSample(5, 8);   // fold = 5*8 = 40

        var fold0 = frame.FoldChild(0);
        var fold1 = frame.FoldChild(1);

        Assert.Equal(21.0, fold0.Fold());
        Assert.Equal(40.0, fold1.Fold());
    }

    [Fact]
    public void FoldChild_OutOfRange_Throws()
    {
        var frame = Axis.Frame(10, 20, 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => frame.FoldChild(0));
        frame.AddSample(1, 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => frame.FoldChild(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => frame.FoldChild(-1));
    }

    // --- Fractal nesting ---

    [Fact]
    public void FractalNesting_ChildOfChild()
    {
        var root = Axis.Frame(100, 200, 1);
        var child = root.AddSample(10, 20);
        var grandchild = child.AddSample(3, 5);

        Assert.Same(root, child.Parent);
        Assert.Same(child, grandchild.Parent);
        Assert.Single(root.Children);
        Assert.Single(child.Children);
    }

    [Fact]
    public void FractalNesting_GrandchildInheritsResolution()
    {
        var root = Axis.Frame(100, 200, 4);
        var child = root.AddSample(50, 100);
        var grandchild = child.AddSample(10, 20);

        // Grandchild inherits child's resolution, which inherited from root
        Assert.Equal(4, grandchild.Unit);
        Assert.Equal(4, grandchild.Unot);
    }

    [Fact]
    public void FractalNesting_GrandchildFoldsCorrectly()
    {
        var root = Axis.Frame(100, 200, 2);
        var child = root.AddSample(10, 20);
        var grandchild = child.AddSample(3, 5);

        // Grandchild fold: (3/2) * (5/2) = 1.5 * 2.5 = 3.75
        Assert.Equal(3.75, grandchild.Fold().Fold());
    }

    [Fact]
    public void FractalNesting_NestedFrameWithOverriddenResolution()
    {
        var root = Axis.Frame(100, 200, 1);
        var nestedFrame = Axis.Frame(10, 20, 5); // override resolution to 5
        root.AddChild(nestedFrame);
        var sample = nestedFrame.AddSample(3, 7);

        // Sample inherits resolution 5 from nested frame, not 1 from root
        Assert.Equal(5, sample.Unit);
        Assert.Equal(5, sample.Unot);
        // Fold: (3/5) * (7/5) = 0.6 * 1.4 = 0.84
        Assert.Equal(0.84, sample.Fold().Fold(), precision: 10);
    }

    // --- Soft bounds (overflow is allowed) ---

    [Fact]
    public void SoftBounds_ChildCanExceedParentExtents()
    {
        var frame = Axis.Frame(10, 20, 1);
        // Sample exceeds parent's extents (50 > 20, 30 > 10)
        var child = frame.AddSample(30, 50);
        Assert.Equal(30, child.Min);
        Assert.Equal(50, child.Max);
        // Still folds correctly
        Assert.Equal(1500.0, child.Fold().Fold());
    }

    // --- ChildValues / FrameElements ---

    [Fact]
    public void ChildValues_ReturnsChildrenAsIValue()
    {
        var frame = Axis.Frame(10, 20, 1);
        Assert.Empty(frame.ChildValues);

        frame.AddSample(3, 7);
        frame.AddSample(5, 8);

        Assert.Equal(2, frame.ChildValues.Length);
    }

    [Fact]
    public void FrameElements_ReturnsChildrenAsISpace()
    {
        var frame = Axis.Frame(10, 20, 1);
        Assert.Empty(frame.FrameElements);

        frame.AddSample(3, 7);
        Assert.Single(frame.FrameElements);
    }

    [Fact]
    public void RootFrame_HasNoParent()
    {
        var frame = Axis.Frame(10, 20, 1);
        Assert.Null(frame.Parent);
    }

    // --- Full fold chain: Axis -> Proportion -> double ---

    [Fact]
    public void FullFoldChain_RectangleExample()
    {
        // Rectangle: width=20, height=10, resolution=2
        var frame = Axis.Frame(10, 20, 2);

        // Frame fold: (10/2) * (20/2) = 5 * 10 = 50 square units
        Assert.Equal(50.0, frame.Fold().Fold());

        // Add samples and fold them
        var s1 = frame.AddSample(5, 8);
        Assert.Equal(10.0, s1.Fold().Fold()); // (5/2)*(8/2) = 2.5*4 = 10

        var s2 = frame.AddSample(3, 7);
        Assert.Equal(5.25, s2.Fold().Fold()); // (3/2)*(7/2) = 1.5*3.5 = 5.25

        // Fold children through parent
        Assert.Equal(10.0, frame.FoldChild(0).Fold());
        Assert.Equal(5.25, frame.FoldChild(1).Fold());
    }
}
