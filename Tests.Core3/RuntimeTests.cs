using Core3.Engine;
using Core3.Runtime;

namespace Tests.Core3;

public sealed class RuntimeTests
{
    [Fact]
    public void EngineOperationContext_CanFocusSortShuffleAndCollapse()
    {
        var context = EngineOperationContext.Create(
            new AtomicElement(4, 4),
            [
                new AtomicElement(1, 2),
                new AtomicElement(3, 4),
                new AtomicElement(1, 4)
            ],
            isOrdered: false);

        Assert.True(context.TryFocusMember(1, out var focused));

        var focusedContext = Assert.IsType<EngineOperationContext>(focused);
        Assert.Equal(new AtomicElement(3, 4), focusedContext.Frame);
        Assert.False(focusedContext.IsOrdered);
        Assert.NotNull(focusedContext.ParentContext);
        Assert.Equal(1, focusedContext.ParentFocusIndex);
        Assert.Equal(2, focusedContext.Count);

        Assert.True(focusedContext.TryCollapseToParentFrame(out var collapsed));

        var collapsedContext = Assert.IsType<EngineOperationContext>(collapsed);
        Assert.Equal(context.Frame, collapsedContext.Frame);
        Assert.False(collapsedContext.IsOrdered);
        Assert.Equal(3, collapsedContext.Count);
        Assert.Equal(focusedContext.Frame, collapsedContext.Members[1]);

        Assert.True(context.TrySortByFrameSlot(0, descending: false, out var sorted));

        var sortedContext = Assert.IsType<EngineOperationContext>(sorted);
        Assert.True(sortedContext.IsOrdered);
        Assert.Equal(
            [new AtomicElement(1, 4), new AtomicElement(1, 2), new AtomicElement(3, 4)],
            sortedContext.Members);

        var shuffled = context.CreateShuffledCopy(seed: 7);
        Assert.False(shuffled.IsOrdered);
        Assert.Equal(3, shuffled.Count);
        Assert.NotEqual(context.Members, shuffled.Members);
    }

    [Fact]
    public void EngineOperationContext_CanConvertBetweenOrderedAndUnorderedViews()
    {
        var ordered = EngineOperationContext.Create(
            new AtomicElement(4, 4),
            [new AtomicElement(1, 4), new AtomicElement(2, 4)],
            isOrdered: true);

        var unordered = ordered.AsUnordered();
        var reordered = unordered.AsOrdered();

        Assert.True(ordered.IsOrdered);
        Assert.False(unordered.IsOrdered);
        Assert.True(reordered.IsOrdered);
        Assert.Equal(ordered.Frame, unordered.Frame);
        Assert.Equal(ordered.Members, unordered.Members);
        Assert.Equal(unordered.Members, reordered.Members);
    }
}
