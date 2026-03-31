using Core3.Engine;
using Core3.Operations;
using Core3.Runtime;

namespace Tests.Core3;

public sealed class OperationsTests
{
    [Fact]
    public void EngineFamily_CanAddRemoveAndClearMembers()
    {
        var family = new EngineFamily(new AtomicElement(0, 4));
        var first = new AtomicElement(1, 2);
        var second = new AtomicElement(1, 4);

        family.AddMember(first);
        family.AddMember(second);

        Assert.Equal(2, family.Count);
        Assert.True(family.RemoveMember(first));
        Assert.Equal(1, family.Count);

        family.ClearMembers();

        Assert.Equal(0, family.Count);
    }

    [Fact]
    public void EngineFamily_CanCreateOrderedAndUnorderedViews()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        var first = new AtomicElement(1, 4);
        var second = new AtomicElement(2, 4);
        family.AddMember(first);
        family.AddMember(second);

        var unordered = family.CreateUnorderedCopy();
        var reordered = unordered.CreateOrderedCopy();

        Assert.False(unordered.IsOrdered);
        Assert.Same(family, unordered.ParentFamily);
        Assert.Equal(family.Members, unordered.Members);

        Assert.True(reordered.IsOrdered);
        Assert.Same(unordered, reordered.ParentFamily);
        Assert.Equal(unordered.Members, reordered.Members);
    }

    [Fact]
    public void EngineFamily_CanTemporarilyFocusMemberIntoFrameRole()
    {
        var family = new EngineFamily(new AtomicElement(4, 4), isOrdered: false);
        var first = new AtomicElement(1, 2);
        var focused = new AtomicElement(3, 4);
        var third = new AtomicElement(1, 4);
        family.AddMember(first);
        family.AddMember(focused);
        family.AddMember(third);

        Assert.True(family.TryFocusMember(focused, out var focusedFamily));

        var reframed = Assert.IsType<EngineFamily>(focusedFamily);
        Assert.False(reframed.IsOrdered);
        Assert.Same(family, reframed.ParentFamily);
        Assert.Equal(1, reframed.ParentFocusIndex);
        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(reframed.Frame));
        Assert.Equal(2, reframed.Count);
        Assert.Equal(first, reframed.Members[0]);
        Assert.Equal(third, reframed.Members[1]);

        Assert.True(reframed.TryAddAll(out var sum));
        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(sum));
    }

    [Fact]
    public void EngineFamily_CanCollapseFocusedFamilyBackToParentFrame()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        var first = new AtomicElement(1, 2);
        var focused = new AtomicElement(3, 4);
        var third = new AtomicElement(1, 4);
        family.AddMember(first);
        family.AddMember(focused);
        family.AddMember(third);

        Assert.True(family.TryFocusMember(focused, out var focusedFamily));

        var reframed = Assert.IsType<EngineFamily>(focusedFamily);
        Assert.True(reframed.TryCollapseToParentFrame(out var collapsedFamily));

        var collapsed = Assert.IsType<EngineFamily>(collapsedFamily);
        Assert.Equal(family.Frame, collapsed.Frame);
        Assert.True(collapsed.IsOrdered);
        Assert.Null(collapsed.ParentFamily);
        Assert.Null(collapsed.ParentFocusIndex);
        Assert.Equal(3, collapsed.Count);
        Assert.Equal(first, collapsed.Members[0]);
        Assert.Equal(reframed.Frame, collapsed.Members[1]);
        Assert.Equal(third, collapsed.Members[2]);
    }

    [Fact]
    public void EngineFamily_CanSortMembersByFrameSlot()
    {
        var frame = new CompositeElement(
            new AtomicElement(0, 10),
            new AtomicElement(0, 10));
        var left = new CompositeElement(
            new AtomicElement(8, 10),
            new AtomicElement(1, 10));
        var middle = new CompositeElement(
            new AtomicElement(5, 10),
            new AtomicElement(5, 10));
        var right = new CompositeElement(
            new AtomicElement(2, 10),
            new AtomicElement(9, 10));
        var family = new EngineFamily(frame, isOrdered: false);
        family.AddMember(middle);
        family.AddMember(left);
        family.AddMember(right);

        Assert.True(family.TrySortByFrameSlot(0, descending: false, out var xSorted));
        Assert.True(family.TrySortByFrameSlot(1, descending: true, out var ySorted));

        var xAscending = Assert.IsType<EngineFamily>(xSorted);
        var yDescending = Assert.IsType<EngineFamily>(ySorted);

        Assert.True(xAscending.IsOrdered);
        Assert.Same(family, xAscending.ParentFamily);
        Assert.Equal([right, middle, left], xAscending.Members);

        Assert.True(yDescending.IsOrdered);
        Assert.Same(family, yDescending.ParentFamily);
        Assert.Equal([right, middle, left], yDescending.Members);
    }

    [Fact]
    public void EngineFamily_CanCreateDeterministicUnorderedShuffle()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        var first = new AtomicElement(1, 4);
        var second = new AtomicElement(2, 4);
        var third = new AtomicElement(3, 4);
        var fourth = new AtomicElement(4, 4);
        family.AddMember(first);
        family.AddMember(second);
        family.AddMember(third);
        family.AddMember(fourth);

        var shuffled = family.CreateShuffledCopy(seed: 7);

        Assert.False(shuffled.IsOrdered);
        Assert.Same(family, shuffled.ParentFamily);
        Assert.Equal(4, shuffled.Count);
        Assert.Equal([first, fourth, third, second], shuffled.Members);
    }

    [Fact]
    public void EngineFamily_CanReportMemberBoundaryAxis()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        var insideCandidate = new AtomicElement(3, 4);
        var outsideCandidate = new AtomicElement(6, 4);

        var insideAxis = family.GetMemberBoundaryAxis(insideCandidate);
        var outsideAxis = family.GetMemberBoundaryAxis(outsideCandidate);

        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 4), new AtomicElement(0, 4)),
            insideAxis);
        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 4), new AtomicElement(2, 4)),
            outsideAxis);
    }

    [Fact]
    public void EngineFamily_TryAddAll_UsesFrameReadouts()
    {
        var family = new EngineFamily(new AtomicElement(0, 4));
        family.AddMember(new AtomicElement(1, 2));
        family.AddMember(new AtomicElement(1, 4));

        Assert.True(family.TryAddAll(out var sum));
        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(sum));
    }

    [Fact]
    public void EngineFamily_TryReadAllWithTension_PreservesUnresolvedFamilyReads()
    {
        var family = new EngineFamily(new AtomicElement(0, 1));
        family.AddMember(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(1, 0));

        Assert.True(family.TryReadAllWithTension(out var result));

        var readResult = Assert.IsType<EngineReadResult>(result);
        Assert.False(readResult.IsExact);
        Assert.Equal(
            [new AtomicElement(1, 1), new AtomicElement(1, 0)],
            readResult.Reads);
    }

    [Fact]
    public void EngineFamily_TryAddAllWithTension_PreservesUnresolvedOperationResult()
    {
        var family = new EngineFamily(new AtomicElement(0, 1));
        family.AddMember(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(1, 0));

        Assert.True(family.TryAddAllWithTension(out var result));
        Assert.False(family.TryAddAll(out _));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.False(operationResult.IsExact);
        Assert.Equal(new AtomicElement(2, 0), operationResult.Result);
        Assert.Equal(new AtomicElement(0, 1), operationResult.ResultFrame);
        Assert.Contains("Addition preserved unresolved support", operationResult.Note);
    }

    [Fact]
    public void EngineOperations_TryAdd_SupportsOneShotFramedAddition()
    {
        var frame = new AtomicElement(0, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(1, 4),
            new AtomicElement(1, 4)
        };

        Assert.True(EngineOperations.TryAdd(frame, members, out var sum));
        Assert.Equal(new AtomicElement(4, 4), Assert.IsType<AtomicElement>(sum));
    }

    [Fact]
    public void EngineOperations_TryAdd_CanRunDirectlyFromRuntimeContext()
    {
        var context = EngineOperationContext.Create(
            new AtomicElement(0, 4),
            [
                new AtomicElement(1, 2),
                new AtomicElement(1, 4),
                new AtomicElement(1, 4)
            ]);

        Assert.True(EngineOperations.TryAdd(context, out var sum));
        Assert.Equal(new AtomicElement(4, 4), Assert.IsType<AtomicElement>(sum));
    }

    [Fact]
    public void EngineOperations_TryAddWithTension_CanReturnNonExactResultWithProvenance()
    {
        var frame = new AtomicElement(0, 1);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 1),
            new AtomicElement(1, 0)
        };

        Assert.True(EngineOperations.TryAddWithTension(frame, members, out var result));
        Assert.False(EngineOperations.TryAddWithProvenance(frame, members, out _));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.False(operationResult.IsExact);
        Assert.Equal(new AtomicElement(2, 0), operationResult.Result);
    }

    [Fact]
    public void EngineOperations_TryAdd_CanReturnResultWithBoundaryAxisAndProvenance()
    {
        var frame = new AtomicElement(4, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(1, 4),
            new AtomicElement(1, 2)
        };

        Assert.True(EngineOperations.TryAddWithProvenance(frame, members, out var result));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.Equal("Add", operationResult.OperationName);
        Assert.Equal(frame, operationResult.SourceFrame);
        Assert.Equal(3, operationResult.SourceMembers.Count);
        Assert.Equal(new AtomicElement(5, 4), operationResult.Result);
        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 4), new AtomicElement(1, 4)),
            operationResult.GetResultBoundaryAxis());
        Assert.True(operationResult.HasAny);
        Assert.False(operationResult.HasMany);
        Assert.Single(operationResult.OutboundPieces);
        Assert.Equal(operationResult.Result, operationResult.OutboundPiece.Result);
        Assert.Equal(operationResult.ResultFrame, operationResult.OutboundPiece.Carrier);
        Assert.Equal([0, 1, 2], operationResult.OutboundPiece.SourceMemberIndices);
    }

    [Fact]
    public void EngineOperations_TryAddWithProvenance_PreservesRuntimeContext()
    {
        var context = EngineOperationContext.Create(
            new AtomicElement(4, 4),
            [
                new AtomicElement(1, 2),
                new AtomicElement(1, 4),
                new AtomicElement(1, 2)
            ],
            isOrdered: false);

        Assert.True(EngineOperations.TryAddWithProvenance(context, out var result));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.Equal(context.Frame, operationResult.Context.Frame);
        Assert.Equal(context.IsOrdered, operationResult.Context.IsOrdered);
        Assert.Equal(context.Members, operationResult.Context.Members);
    }

    [Fact]
    public void EngineFamily_TryMultiplyAll_UsesFrameReadouts()
    {
        var family = new EngineFamily(new AtomicElement(4, 4));
        family.AddMember(new AtomicElement(1, 2));
        family.AddMember(new AtomicElement(3, 4));

        Assert.True(family.TryMultiplyAll(out var product));
        Assert.Equal(new AtomicElement(6, 16), Assert.IsType<AtomicElement>(product));
    }

    [Fact]
    public void EngineFamily_TryMultiplyAllWithTension_PreservesUnresolvedOperationResult()
    {
        var family = new EngineFamily(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(2, 1));
        family.AddMember(new AtomicElement(4, 0));

        Assert.True(family.TryMultiplyAllWithTension(out var result));
        Assert.False(family.TryMultiplyAll(out _));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.False(operationResult.IsExact);
        Assert.Equal(new AtomicElement(8, 0), operationResult.Result);
        Assert.Equal(new AtomicElement(1, 1), operationResult.ResultFrame);
        Assert.Contains("Multiplication preserved unresolved support", operationResult.Note);
    }

    [Fact]
    public void EngineOperations_TryMultiply_SupportsOneShotFramedMultiplication()
    {
        var frame = new AtomicElement(4, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(3, 4)
        };

        Assert.True(EngineOperations.TryMultiply(frame, members, out var product));
        Assert.Equal(new AtomicElement(6, 16), Assert.IsType<AtomicElement>(product));
    }

    [Fact]
    public void EngineOperations_TryMultiplyWithTension_CanReturnNonExactResultWithProvenance()
    {
        var frame = new AtomicElement(1, 1);
        var members = new GradedElement[]
        {
            new AtomicElement(2, 1),
            new AtomicElement(4, 0)
        };

        Assert.True(EngineOperations.TryMultiplyWithTension(frame, members, out var result));
        Assert.False(EngineOperations.TryMultiplyWithProvenance(frame, members, out _));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.False(operationResult.IsExact);
        Assert.Equal(new AtomicElement(8, 0), operationResult.Result);
    }

    [Fact]
    public void EngineOperations_TryMultiply_CanReturnResultWithDerivedFrameProvenance()
    {
        var frame = new AtomicElement(4, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(3, 4)
        };

        Assert.True(EngineOperations.TryMultiplyWithProvenance(frame, members, out var result));

        var operationResult = Assert.IsType<EngineOperationResult>(result);
        Assert.Equal("Multiply", operationResult.OperationName);
        Assert.Equal(frame, operationResult.SourceFrame);
        Assert.Equal(2, operationResult.SourceMembers.Count);
        Assert.Equal(new AtomicElement(6, 16), operationResult.Result);
        Assert.Equal(new AtomicElement(16, 16), Assert.IsType<AtomicElement>(operationResult.ResultFrame));
        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 16), new AtomicElement(0, 16)),
            operationResult.GetResultBoundaryAxis());
    }

    [Fact]
    public void EngineOperationResult_CanPreserveMultiplyKernelForCompositeMultiply()
    {
        var left = Core3TestHelpers.CreateAxisLikeNumber(1, 1, 2, 1);
        var right = Core3TestHelpers.CreateAxisLikeNumber(1, 1, 4, 1);
        Assert.True(EngineOperations.TryMultiplyWithProvenance(left, [left, right], out var operationResult));

        var result = Assert.IsType<EngineOperationResult>(operationResult);
        Assert.NotNull(result.PreservedStructure);

        Assert.True(result.TryGetRawMultiplyKernel(out var kernel));

        var squares = Assert.IsType<CompositeElement>(Assert.IsType<CompositeElement>(kernel).Recessive);
        var cross = Assert.IsType<CompositeElement>(Assert.IsType<CompositeElement>(kernel).Dominant);

        Assert.Equal(new AtomicElement(1, 1), squares.Recessive);
        Assert.Equal(new AtomicElement(8, 1), squares.Dominant);
        Assert.Equal(new AtomicElement(4, 1), cross.Recessive);
        Assert.Equal(new AtomicElement(2, 1), cross.Dominant);
    }

    [Fact]
    public void EngineOperations_TryBoolean_And_UsesSharedFrameOverlap()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);

        Assert.True(EngineOperations.TryBoolean(
            frame,
            [primary, secondary],
            EngineBooleanOperation.And,
            out var result));

        var booleanResult = Assert.IsType<EngineBooleanResult>(result);
        Assert.Single(booleanResult.Pieces);
        Assert.Equal(
            Core3TestHelpers.CreateSegmentFrame(3, 5, 10),
            booleanResult.Pieces[0].Result);
        Assert.Equal(primary, booleanResult.Pieces[0].Carrier);
        Assert.Equal([0, 1], booleanResult.Pieces[0].SourceMemberIndices);
    }

    [Fact]
    public void EngineOperations_TryBooleanWithTension_PreservesUnresolvedSegmentProjection()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = new CompositeElement(
            new AtomicElement(3, 10),
            new AtomicElement(5, 0));

        Assert.True(EngineOperations.TryBooleanWithTension(
            frame,
            [primary, secondary],
            EngineBooleanOperation.And,
            out var result));
        Assert.False(EngineOperations.TryBoolean(
            frame,
            [primary, secondary],
            EngineBooleanOperation.And,
            out _));

        var booleanResult = Assert.IsType<EngineBooleanResult>(result);
        Assert.False(booleanResult.IsExact);
        Assert.Empty(booleanResult.Pieces);
        Assert.NotNull(booleanResult.Tension);
        Assert.Contains("Boolean projection preserved unresolved support", booleanResult.Note);
    }

    [Fact]
    public void EngineOperations_TryBoolean_Xor_PreservesTwoPieces()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);

        Assert.True(EngineOperations.TryBoolean(
            frame,
            [primary, secondary],
            EngineBooleanOperation.Xor,
            out var result));

        var booleanResult = Assert.IsType<EngineBooleanResult>(result);
        Assert.True(booleanResult.HasAny);
        Assert.True(booleanResult.HasMany);
        Assert.Equal(2, booleanResult.Pieces.Count);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(0, 3, 10), booleanResult.Pieces[0].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(5, 10, 10), booleanResult.Pieces[1].Result);
    }

    [Fact]
    public void EngineOperations_TryBoolean_NotSecondary_UsesFrameAsNegationCarrier()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);

        Assert.True(EngineOperations.TryBoolean(
            frame,
            [primary, secondary],
            EngineBooleanOperation.NotSecondary,
            out var result));

        var booleanResult = Assert.IsType<EngineBooleanResult>(result);
        Assert.Equal(2, booleanResult.Pieces.Count);
        Assert.Equal(frame, booleanResult.Pieces[0].Carrier);
        Assert.Equal(frame, booleanResult.Pieces[1].Carrier);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(0, 3, 10), booleanResult.Pieces[0].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(5, 10, 10), booleanResult.Pieces[1].Result);
    }

    [Fact]
    public void EngineOperations_TryOccupancyBoolean_ExactlyOne_UsesSymmetricFamilyOccupancy()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);
        var third = Core3TestHelpers.CreateSegmentFrame(6, 8, 10);

        Assert.True(EngineOperations.TryOccupancyBoolean(
            frame,
            [first, second, third],
            EngineOccupancyOperation.ExactlyOne,
            out var result));

        var occupancyResult = Assert.IsType<EngineFamilyBooleanResult>(result);
        Assert.False(occupancyResult.IsOrdered);
        Assert.Equal(3, occupancyResult.Pieces.Count);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(0, 3, 10), occupancyResult.Pieces[0].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(5, 6, 10), occupancyResult.Pieces[1].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(8, 10, 10), occupancyResult.Pieces[2].Result);
        Assert.Equal([0], occupancyResult.Pieces[0].SourceMemberIndices);
        Assert.Equal([0], occupancyResult.Pieces[1].SourceMemberIndices);
        Assert.Equal([0], occupancyResult.Pieces[2].SourceMemberIndices);
        Assert.Equal(first, occupancyResult.Pieces[0].Carrier);
    }

    [Fact]
    public void EngineOperations_TryOccupancyBoolean_All_UsesFrameCarrierForCoPresentTruth()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var third = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);

        Assert.True(EngineOperations.TryOccupancyBoolean(
            frame,
            [first, second, third],
            EngineOccupancyOperation.All,
            out var result));

        var occupancyResult = Assert.IsType<EngineFamilyBooleanResult>(result);
        Assert.Single(occupancyResult.Pieces);
        Assert.Equal(frame, occupancyResult.Pieces[0].Carrier);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(3, 5, 10), occupancyResult.Pieces[0].Result);
        Assert.Equal([0, 1, 2], occupancyResult.Pieces[0].SourceMemberIndices);
    }

    [Fact]
    public void EngineOperations_TryOccupancyBooleanWithTension_PreservesUnresolvedFamilyProjection()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = new CompositeElement(
            new AtomicElement(3, 10),
            new AtomicElement(5, 0));
        var third = Core3TestHelpers.CreateSegmentFrame(6, 8, 10);

        Assert.True(EngineOperations.TryOccupancyBooleanWithTension(
            frame,
            [first, second, third],
            EngineOccupancyOperation.ExactlyOne,
            out var result));
        Assert.False(EngineOperations.TryOccupancyBoolean(
            frame,
            [first, second, third],
            EngineOccupancyOperation.ExactlyOne,
            out _));

        var occupancyResult = Assert.IsType<EngineFamilyBooleanResult>(result);
        Assert.False(occupancyResult.IsExact);
        Assert.Empty(occupancyResult.Pieces);
        Assert.NotNull(occupancyResult.Tension);
        Assert.Contains("Boolean projection preserved unresolved support", occupancyResult.Note);
    }

    [Fact]
    public void EngineOperations_TryBooleanAdjacentPairs_UsesOrderedFamilyTraversal()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);
        var third = Core3TestHelpers.CreateSegmentFrame(6, 8, 10);

        Assert.True(EngineOperations.TryBooleanAdjacentPairs(
            frame,
            [first, second, third],
            EngineBooleanOperation.Xor,
            out var results));

        var pairwise = Assert.IsAssignableFrom<IReadOnlyList<EngineBooleanResult>>(results);
        Assert.Equal(2, pairwise.Count);

        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(0, 3, 10), pairwise[0].Pieces[0].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(5, 10, 10), pairwise[0].Pieces[1].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(3, 5, 10), pairwise[1].Pieces[0].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(6, 8, 10), pairwise[1].Pieces[1].Result);
    }

    [Fact]
    public void EngineOperations_TryBooleanAdjacentPairsWithTension_PreservesUnresolvedPairReads()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = new CompositeElement(
            new AtomicElement(3, 10),
            new AtomicElement(5, 0));
        var third = Core3TestHelpers.CreateSegmentFrame(6, 8, 10);

        Assert.True(EngineOperations.TryBooleanAdjacentPairsWithTension(
            frame,
            [first, second, third],
            EngineBooleanOperation.Xor,
            out var results));
        Assert.False(EngineOperations.TryBooleanAdjacentPairs(
            frame,
            [first, second, third],
            EngineBooleanOperation.Xor,
            out _));

        var pairwise = Assert.IsAssignableFrom<IReadOnlyList<EngineBooleanResult>>(results);
        Assert.Equal(2, pairwise.Count);
        Assert.All(pairwise, result => Assert.False(result.IsExact));
        Assert.All(pairwise, result => Assert.Empty(result.Pieces));
        Assert.All(pairwise, result => Assert.Contains("Boolean projection preserved unresolved support", result.Note));
    }

    [Fact]
    public void EngineOperations_TryOccupancyBoolean_CanRunDirectlyFromRuntimeContext()
    {
        var context = EngineOperationContext.Create(
            Core3TestHelpers.CreateSegmentFrame(0, 10, 10),
            [
                Core3TestHelpers.CreateSegmentFrame(0, 10, 10),
                Core3TestHelpers.CreateSegmentFrame(3, 5, 10),
                Core3TestHelpers.CreateSegmentFrame(6, 8, 10)
            ],
            isOrdered: false);

        Assert.True(EngineOperations.TryOccupancyBoolean(
            context,
            EngineOccupancyOperation.ExactlyOne,
            out var result));

        var occupancyResult = Assert.IsType<EngineFamilyBooleanResult>(result);
        Assert.Equal(context.Frame, occupancyResult.Context.Frame);
        Assert.Equal(context.IsOrdered, occupancyResult.Context.IsOrdered);
        Assert.Equal(3, occupancyResult.Pieces.Count);
    }

    [Fact]
    public void EngineFamily_TryBooleanAdjacentPairs_RequiresOrderedFamily()
    {
        var family = new EngineFamily(Core3TestHelpers.CreateSegmentFrame(0, 10, 10), isOrdered: false);
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(0, 10, 10));
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(3, 5, 10));

        Assert.False(family.TryBooleanAdjacentPairs(EngineBooleanOperation.And, out _));
    }

    [Fact]
    public void EngineFamily_TryBoolean_CurrentlyRequiresExactlyTwoMembers()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var family = new EngineFamily(frame);
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(0, 10, 10));
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(3, 5, 10));
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(6, 8, 10));

        Assert.False(family.TryBoolean(EngineBooleanOperation.Or, out _));
    }
}
