using Core3.Engine;
using Core3.Operations;
using Core3.Runtime;

namespace Tests.Core3;

public sealed class OperationsTests
{
    [Fact]
    public void Family_CanAddRemoveAndClearMembers()
    {
        var family = new Family(new AtomicElement(0, 4));
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
    public void Family_CanCreateOrderedAndUnorderedViews()
    {
        var family = new Family(new AtomicElement(4, 4));
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
    public void Family_CanTemporarilyFocusMemberIntoFrameRole()
    {
        var family = new Family(new AtomicElement(4, 4), isOrdered: false);
        var first = new AtomicElement(1, 2);
        var focused = new AtomicElement(3, 4);
        var third = new AtomicElement(1, 4);
        family.AddMember(first);
        family.AddMember(focused);
        family.AddMember(third);

        Assert.True(family.TryFocusMember(focused, out var focusedFamily));

        var reframed = Assert.IsType<Family>(focusedFamily);
        Assert.False(reframed.IsOrdered);
        Assert.Same(family, reframed.ParentFamily);
        Assert.Equal(1, reframed.ParentFocusIndex);
        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(reframed.Frame));
        Assert.Equal(2, reframed.Count);
        Assert.Equal(first, reframed.Members[0]);
        Assert.Equal(third, reframed.Members[1]);

        Assert.True(reframed.TryAddAllResult(out var sumResult));
        Assert.True(sumResult!.IsExact);
        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(sumResult.Result));
    }

    [Fact]
    public void Family_CanCollapseFocusedFamilyBackToParentFrame()
    {
        var family = new Family(new AtomicElement(4, 4));
        var first = new AtomicElement(1, 2);
        var focused = new AtomicElement(3, 4);
        var third = new AtomicElement(1, 4);
        family.AddMember(first);
        family.AddMember(focused);
        family.AddMember(third);

        Assert.True(family.TryFocusMember(focused, out var focusedFamily));

        var reframed = Assert.IsType<Family>(focusedFamily);
        Assert.True(reframed.TryCollapseToParentFrame(out var collapsedFamily));

        var collapsed = Assert.IsType<Family>(collapsedFamily);
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
    public void Family_CanSortMembersByFrameSlot()
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
        var family = new Family(frame, isOrdered: false);
        family.AddMember(middle);
        family.AddMember(left);
        family.AddMember(right);

        Assert.True(family.TrySortByFrameSlot(0, descending: false, out var xSorted));
        Assert.True(family.TrySortByFrameSlot(1, descending: true, out var ySorted));

        var xAscending = Assert.IsType<Family>(xSorted);
        var yDescending = Assert.IsType<Family>(ySorted);

        Assert.True(xAscending.IsOrdered);
        Assert.Same(family, xAscending.ParentFamily);
        Assert.Equal([right, middle, left], xAscending.Members);

        Assert.True(yDescending.IsOrdered);
        Assert.Same(family, yDescending.ParentFamily);
        Assert.Equal([right, middle, left], yDescending.Members);
    }

    [Fact]
    public void Family_CanCreateDeterministicUnorderedShuffle()
    {
        var family = new Family(new AtomicElement(4, 4));
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
    public void Family_CanReportMemberBoundaryAxis()
    {
        var family = new Family(new AtomicElement(4, 4));
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
    public void Family_TryAddAll_UsesFrameReadouts()
    {
        var family = new Family(new AtomicElement(0, 4));
        family.AddMember(new AtomicElement(1, 2));
        family.AddMember(new AtomicElement(1, 4));

        Assert.True(family.TryAddAllResult(out var sumResult));
        Assert.True(sumResult!.IsExact);
        Assert.Equal(new AtomicElement(3, 4), Assert.IsType<AtomicElement>(sumResult.Result));
    }

    [Fact]
    public void Family_TryReadAllResult_PreservesUnresolvedFamilyReads()
    {
        var family = new Family(new AtomicElement(0, 1));
        family.AddMember(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(1, 0));

        var result = family.ReadAllResult();

        var readResult = Assert.IsType<PieceArcResult>(result);
        Assert.False(readResult.IsExact);
        Assert.Equal("Read", readResult.OriginLawName);
        Assert.True(readResult.HasAny);
        Assert.True(readResult.HasMany);
        Assert.Equal(2, readResult.OutboundPieces.Count);
        Assert.Equal([0], readResult.OutboundPieces[0].SourceMemberIndices);
        Assert.Equal([1], readResult.OutboundPieces[1].SourceMemberIndices);
        Assert.Equal(
            [new AtomicElement(1, 1), new AtomicElement(1, 0)],
            readResult.Results);
    }

    [Fact]
    public void Family_TryAddAllResult_PreservesUnresolvedOperationResult()
    {
        var family = new Family(new AtomicElement(0, 1));
        family.AddMember(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(1, 0));

        Assert.True(family.TryAddAllResult(out var result));

        var operationResult = Assert.IsType<OperationResult>(result);
        Assert.False(operationResult.IsExact);
        Assert.Equal(new AtomicElement(2, 0), operationResult.Result);
        Assert.Equal(new AtomicElement(0, 1), operationResult.ResultFrame);
        Assert.Contains("Addition preserved unresolved support", operationResult.Note);
    }

    [Fact]
    public void Operations_TryAdd_SupportsOneShotFramedAddition()
    {
        var frame = new AtomicElement(0, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(1, 4),
            new AtomicElement(1, 4)
        };

        Assert.True(new Family(OperationContext.Create(frame, members)).TryAddAllResult(out var sumResult));
        Assert.True(sumResult!.IsExact);
        Assert.Equal(new AtomicElement(4, 4), Assert.IsType<AtomicElement>(sumResult.Result));
    }

    [Fact]
    public void Operations_TryAdd_CanRunDirectlyFromRuntimeContext()
    {
        var context = OperationContext.Create(
            new AtomicElement(0, 4),
            [
                new AtomicElement(1, 2),
                new AtomicElement(1, 4),
                new AtomicElement(1, 4)
            ]);

        Assert.True(new Family(context).TryAddAllResult(out var sumResult));
        Assert.True(sumResult!.IsExact);
        Assert.Equal(new AtomicElement(4, 4), Assert.IsType<AtomicElement>(sumResult.Result));
    }

    [Fact]
    public void Operations_TryAddResult_CanReturnNonExactOperationResult()
    {
        var frame = new AtomicElement(0, 1);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 1),
            new AtomicElement(1, 0)
        };

        Assert.True(new Family(OperationContext.Create(frame, members)).TryAddAllResult(out var result));

        var operationResult = Assert.IsType<OperationResult>(result);
        Assert.False(operationResult.IsExact);
        Assert.Equal(new AtomicElement(2, 0), operationResult.Result);
    }

    [Fact]
    public void Operations_TryAdd_CanReturnResultWithBoundaryAxisAndProvenance()
    {
        var frame = new AtomicElement(4, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(1, 4),
            new AtomicElement(1, 2)
        };

        Assert.True(new Family(OperationContext.Create(frame, members)).TryAddAllResult(out var result));

        var operationResult = Assert.IsType<OperationResult>(result);
        Assert.Equal("Add", operationResult.OperationName);
        Assert.Equal(frame, operationResult.Context.Frame);
        Assert.Equal(3, operationResult.Context.Members.Count);
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
    public void Operations_TryAddResult_PreservesRuntimeContext()
    {
        var context = OperationContext.Create(
            new AtomicElement(4, 4),
            [
                new AtomicElement(1, 2),
                new AtomicElement(1, 4),
                new AtomicElement(1, 2)
            ],
            isOrdered: false);

        Assert.True(new Family(context).TryAddAllResult(out var result));

        var operationResult = Assert.IsType<OperationResult>(result);
        Assert.Equal(context.Frame, operationResult.Context.Frame);
        Assert.Equal(context.IsOrdered, operationResult.Context.IsOrdered);
        Assert.Equal(context.Members, operationResult.Context.Members);
    }

    [Fact]
    public void Family_TryMultiplyAll_UsesFrameReadouts()
    {
        var family = new Family(new AtomicElement(4, 4));
        family.AddMember(new AtomicElement(1, 2));
        family.AddMember(new AtomicElement(3, 4));

        Assert.True(family.TryMultiplyAllResult(out var productResult));
        Assert.True(productResult!.IsExact);
        Assert.Equal(new AtomicElement(6, 16), Assert.IsType<AtomicElement>(productResult.Result));
    }

    [Fact]
    public void Family_TryMultiplyAllResult_PreservesUnresolvedOperationResult()
    {
        var family = new Family(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(2, 1));
        family.AddMember(new AtomicElement(4, 0));

        Assert.True(family.TryMultiplyAllResult(out var result));

        var operationResult = Assert.IsType<OperationResult>(result);
        Assert.False(operationResult.IsExact);
        Assert.Equal(new AtomicElement(8, 0), operationResult.Result);
        Assert.Equal(new AtomicElement(1, 1), operationResult.ResultFrame);
        Assert.Contains("Multiplication preserved unresolved support", operationResult.Note);
    }

    [Fact]
    public void Operations_TryMultiply_SupportsOneShotFramedMultiplication()
    {
        var frame = new AtomicElement(4, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(3, 4)
        };

        Assert.True(new Family(OperationContext.Create(frame, members)).TryMultiplyAllResult(out var productResult));
        Assert.True(productResult!.IsExact);
        Assert.Equal(new AtomicElement(6, 16), Assert.IsType<AtomicElement>(productResult.Result));
    }

    [Fact]
    public void Operations_TryMultiplyResult_CanReturnNonExactOperationResult()
    {
        var frame = new AtomicElement(1, 1);
        var members = new GradedElement[]
        {
            new AtomicElement(2, 1),
            new AtomicElement(4, 0)
        };

        Assert.True(new Family(OperationContext.Create(frame, members)).TryMultiplyAllResult(out var result));

        var operationResult = Assert.IsType<OperationResult>(result);
        Assert.False(operationResult.IsExact);
        Assert.Equal(new AtomicElement(8, 0), operationResult.Result);
    }

    [Fact]
    public void Operations_TryMultiply_CanReturnResultWithDerivedFrameProvenance()
    {
        var frame = new AtomicElement(4, 4);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 2),
            new AtomicElement(3, 4)
        };

        Assert.True(new Family(OperationContext.Create(frame, members)).TryMultiplyAllResult(out var result));

        var operationResult = Assert.IsType<OperationResult>(result);
        Assert.Equal("Multiply", operationResult.OperationName);
        Assert.Equal(frame, operationResult.Context.Frame);
        Assert.Equal(2, operationResult.Context.Members.Count);
        Assert.Equal(new AtomicElement(6, 16), operationResult.Result);
        Assert.Equal(new AtomicElement(16, 16), Assert.IsType<AtomicElement>(operationResult.ResultFrame));
        Assert.Equal(
            new CompositeElement(new AtomicElement(0, 16), new AtomicElement(0, 16)),
            operationResult.GetResultBoundaryAxis());
    }

    [Fact]
    public void OperationResult_CanCarryExplicitPreservedStructure()
    {
        var left = new CompositeElement(
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(2, 1)),
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(3, 1)));
        var right = new CompositeElement(
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(4, 1)),
            new CompositeElement(new AtomicElement(1, 1), new AtomicElement(5, 1)));

        var kernelOutcome = left.MultiplyKernel(right);
        var productOutcome = left.Multiply(right);
        Assert.True(kernelOutcome.IsExact);
        Assert.True(productOutcome.IsExact);
        var preservedKernel = Assert.IsType<CompositeElement>(kernelOutcome.Result);
        var reducedProduct = Assert.IsType<CompositeElement>(productOutcome.Result);

        var operationResult = new OperationResult(
            "Multiply",
            OperationContext.Create(left, [left, right]),
            reducedProduct,
            reducedProduct,
            preservedKernel);

        Assert.Equal(reducedProduct, operationResult.Result);
        Assert.Equal(preservedKernel, operationResult.PreservedStructure);
    }

    [Fact]
    public void Operations_TryBoolean_And_UsesSharedFrameOverlap()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);

        Assert.True(new Family(OperationContext.Create(frame, [primary, secondary])).TryBooleanResult(
            BooleanOperation.And,
            out var result));

        var booleanResult = Assert.IsType<PieceArcResult>(result);
        Assert.Single(booleanResult.Pieces);
        Assert.Equal(
            Core3TestHelpers.CreateSegmentFrame(3, 5, 10),
            booleanResult.Pieces[0].Result);
        Assert.Equal(primary, booleanResult.Pieces[0].Carrier);
        Assert.Equal([0, 1], booleanResult.Pieces[0].SourceMemberIndices);
    }

    [Fact]
    public void Operations_TryPieceArcResult_PreservesUnresolvedSegmentProjection()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = new CompositeElement(
            new AtomicElement(3, 10),
            new AtomicElement(5, 0));

        Assert.True(new Family(OperationContext.Create(frame, [primary, secondary])).TryBooleanResult(
            BooleanOperation.And,
            out var result));

        var booleanResult = Assert.IsType<PieceArcResult>(result);
        Assert.False(booleanResult.IsExact);
        Assert.Empty(booleanResult.Pieces);
        Assert.NotNull(booleanResult.Tension);
        Assert.Contains("Boolean projection preserved unresolved support", booleanResult.Note);
    }

    [Fact]
    public void Operations_TryBoolean_Xor_PreservesTwoPieces()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);

        Assert.True(new Family(OperationContext.Create(frame, [primary, secondary])).TryBooleanResult(
            BooleanOperation.Xor,
            out var result));

        var booleanResult = Assert.IsType<PieceArcResult>(result);
        Assert.True(booleanResult.HasAny);
        Assert.True(booleanResult.HasMany);
        Assert.Equal(2, booleanResult.Pieces.Count);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(0, 3, 10), booleanResult.Pieces[0].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(5, 10, 10), booleanResult.Pieces[1].Result);
    }

    [Fact]
    public void Operations_TryBoolean_NotSecondary_UsesFrameAsNegationCarrier()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);

        Assert.True(new Family(OperationContext.Create(frame, [primary, secondary])).TryBooleanResult(
            BooleanOperation.NotSecondary,
            out var result));

        var booleanResult = Assert.IsType<PieceArcResult>(result);
        Assert.Equal(2, booleanResult.Pieces.Count);
        Assert.Equal(frame, booleanResult.Pieces[0].Carrier);
        Assert.Equal(frame, booleanResult.Pieces[1].Carrier);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(0, 3, 10), booleanResult.Pieces[0].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(5, 10, 10), booleanResult.Pieces[1].Result);
    }

    [Fact]
    public void Operations_TryOccupancyBoolean_ExactlyOne_UsesSymmetricFamilyOccupancy()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);
        var third = Core3TestHelpers.CreateSegmentFrame(6, 8, 10);

        Assert.True(new Family(OperationContext.Create(frame, [first, second, third], isOrdered: false)).TryOccupancyBooleanResult(
            OccupancyOperation.ExactlyOne,
            out var result));

        var occupancyResult = Assert.IsType<PieceArcResult>(result);
        Assert.False(occupancyResult.Context.IsOrdered);
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
    public void Operations_TryOccupancyBoolean_All_UsesFrameCarrierForCoPresentTruth()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var third = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);

        Assert.True(new Family(OperationContext.Create(frame, [first, second, third], isOrdered: false)).TryOccupancyBooleanResult(
            OccupancyOperation.All,
            out var result));

        var occupancyResult = Assert.IsType<PieceArcResult>(result);
        Assert.Single(occupancyResult.Pieces);
        Assert.Equal(frame, occupancyResult.Pieces[0].Carrier);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(3, 5, 10), occupancyResult.Pieces[0].Result);
        Assert.Equal([0, 1, 2], occupancyResult.Pieces[0].SourceMemberIndices);
    }

    [Fact]
    public void Operations_TryOccupancyPieceArcResult_PreservesUnresolvedFamilyProjection()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = new CompositeElement(
            new AtomicElement(3, 10),
            new AtomicElement(5, 0));
        var third = Core3TestHelpers.CreateSegmentFrame(6, 8, 10);

        Assert.True(new Family(OperationContext.Create(frame, [first, second, third], isOrdered: false)).TryOccupancyBooleanResult(
            OccupancyOperation.ExactlyOne,
            out var result));

        var occupancyResult = Assert.IsType<PieceArcResult>(result);
        Assert.False(occupancyResult.IsExact);
        Assert.Empty(occupancyResult.Pieces);
        Assert.NotNull(occupancyResult.Tension);
        Assert.Contains("Boolean projection preserved unresolved support", occupancyResult.Note);
    }

    [Fact]
    public void Operations_TryBooleanAdjacentPairs_UsesOrderedFamilyTraversal()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);
        var third = Core3TestHelpers.CreateSegmentFrame(6, 8, 10);

        Assert.True(new Family(OperationContext.Create(frame, [first, second, third], isOrdered: true)).TryBooleanAdjacentPairResults(
            BooleanOperation.Xor,
            out var results));

        var pairwise = Assert.IsAssignableFrom<IReadOnlyList<PieceArcResult>>(results);
        Assert.Equal(2, pairwise.Count);

        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(0, 3, 10), pairwise[0].Pieces[0].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(5, 10, 10), pairwise[0].Pieces[1].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(3, 5, 10), pairwise[1].Pieces[0].Result);
        Assert.Equal(Core3TestHelpers.CreateSegmentFrame(6, 8, 10), pairwise[1].Pieces[1].Result);
    }

    [Fact]
    public void Operations_TryBooleanAdjacentPairResults_PreservesUnresolvedPairReads()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var first = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var second = new CompositeElement(
            new AtomicElement(3, 10),
            new AtomicElement(5, 0));
        var third = Core3TestHelpers.CreateSegmentFrame(6, 8, 10);

        Assert.True(new Family(OperationContext.Create(frame, [first, second, third], isOrdered: true)).TryBooleanAdjacentPairResults(
            BooleanOperation.Xor,
            out var results));

        var pairwise = Assert.IsAssignableFrom<IReadOnlyList<PieceArcResult>>(results);
        Assert.Equal(2, pairwise.Count);
        Assert.All(pairwise, result => Assert.False(result.IsExact));
        Assert.All(pairwise, result => Assert.Empty(result.Pieces));
        Assert.All(pairwise, result => Assert.Contains("Boolean projection preserved unresolved support", result.Note));
    }

    [Fact]
    public void Operations_TryOccupancyBoolean_CanRunDirectlyFromRuntimeContext()
    {
        var context = OperationContext.Create(
            Core3TestHelpers.CreateSegmentFrame(0, 10, 10),
            [
                Core3TestHelpers.CreateSegmentFrame(0, 10, 10),
                Core3TestHelpers.CreateSegmentFrame(3, 5, 10),
                Core3TestHelpers.CreateSegmentFrame(6, 8, 10)
            ],
            isOrdered: false);

        Assert.True(new Family(context).TryOccupancyBooleanResult(
            OccupancyOperation.ExactlyOne,
            out var result));

        var occupancyResult = Assert.IsType<PieceArcResult>(result);
        Assert.Equal(context.Frame, occupancyResult.Context.Frame);
        Assert.Equal(context.IsOrdered, occupancyResult.Context.IsOrdered);
        Assert.Equal(3, occupancyResult.Pieces.Count);
    }

    [Fact]
    public void Family_TryBooleanAdjacentPairs_RequiresOrderedFamily()
    {
        var family = new Family(Core3TestHelpers.CreateSegmentFrame(0, 10, 10), isOrdered: false);
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(0, 10, 10));
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(3, 5, 10));

        Assert.False(family.TryBooleanAdjacentPairResults(BooleanOperation.And, out _));
    }

    [Fact]
    public void Family_TryBoolean_CurrentlyRequiresExactlyTwoMembers()
    {
        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var family = new Family(frame);
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(0, 10, 10));
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(3, 5, 10));
        family.AddMember(Core3TestHelpers.CreateSegmentFrame(6, 8, 10));

        Assert.False(family.TryBooleanResult(BooleanOperation.Or, out _));
    }
}











