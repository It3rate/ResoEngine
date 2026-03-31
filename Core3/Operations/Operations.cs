using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Friendly one-shot operation shell over the current family pipeline for cases
/// where the family does not need to persist after the read is complete.
/// </summary>
public static class Operations
{
    private delegate bool FamilyAction<TResult>(Family family, out TResult? result)
        where TResult : class;

    private delegate bool CompositeFamilyAction<TResult, in TPayload>(
        Family family,
        TPayload payload,
        out TResult? result)
        where TResult : class;

    public static bool TryAdd(OperationContext context, out GradedElement? sum) =>
        TryWithFamily(
            context,
            static (Family family, out GradedElement? result) => family.TryAddAll(out result),
            out sum);

    public static bool TryAdd(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out GradedElement? sum) =>
        TryWithFamily(
            frame,
            members,
            static (Family family, out GradedElement? result) => family.TryAddAll(out result),
            out sum);

    public static bool TryAddResult(
        OperationContext context,
        out OperationResult? result) =>
        TryWithFamily(
            context,
            static (Family family, out OperationResult? value) => family.TryAddAllResult(out value),
            out result);

    public static bool TryAddResult(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out OperationResult? result) =>
        TryWithFamily(
            frame,
            members,
            static (Family family, out OperationResult? value) => family.TryAddAllResult(out value),
            out result);

    public static bool TryMultiply(
        OperationContext context,
        out GradedElement? product) =>
        TryWithFamily(
            context,
            static (Family family, out GradedElement? result) => family.TryMultiplyAll(out result),
            out product);

    public static bool TryMultiply(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out GradedElement? product) =>
        TryWithFamily(
            frame,
            members,
            static (Family family, out GradedElement? result) => family.TryMultiplyAll(out result),
            out product);

    public static bool TryMultiplyResult(
        OperationContext context,
        out OperationResult? result) =>
        TryWithFamily(
            context,
            static (Family family, out OperationResult? value) => family.TryMultiplyAllResult(out value),
            out result);

    public static bool TryMultiplyResult(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        out OperationResult? result) =>
        TryWithFamily(
            frame,
            members,
            static (Family family, out OperationResult? value) => family.TryMultiplyAllResult(out value),
            out result);

    public static bool TryBoolean(
        OperationContext context,
        BooleanOperation operation,
        out PieceArcResult? result) =>
        TryWithCompositeFamily(
            context,
            static (Family family, BooleanOperation payload, out PieceArcResult? value) => family.TryBoolean(payload, out value),
            operation,
            out result);

    public static bool TryBoolean(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        BooleanOperation operation,
        out PieceArcResult? result) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered: true,
            static (Family family, BooleanOperation payload, out PieceArcResult? value) => family.TryBoolean(payload, out value),
            out result);

    public static bool TryBooleanResult(
        OperationContext context,
        BooleanOperation operation,
        out PieceArcResult? result) =>
        TryWithCompositeFamily(
            context,
            static (Family family, BooleanOperation payload, out PieceArcResult? value) => family.TryBooleanResult(payload, out value),
            operation,
            out result);

    public static bool TryBooleanResult(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        BooleanOperation operation,
        out PieceArcResult? result) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered: true,
            static (Family family, BooleanOperation payload, out PieceArcResult? value) => family.TryBooleanResult(payload, out value),
            out result);

    public static bool TryOccupancyBoolean(
        OperationContext context,
        OccupancyOperation operation,
        out PieceArcResult? result) =>
        TryWithCompositeFamily(
            context,
            static (Family family, OccupancyOperation payload, out PieceArcResult? value) => family.TryOccupancyBoolean(payload, out value),
            operation,
            out result);

    public static bool TryOccupancyBoolean(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        OccupancyOperation operation,
        out PieceArcResult? result,
        bool isOrdered = false) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered,
            static (Family family, OccupancyOperation payload, out PieceArcResult? value) => family.TryOccupancyBoolean(payload, out value),
            out result);

    public static bool TryOccupancyBooleanResult(
        OperationContext context,
        OccupancyOperation operation,
        out PieceArcResult? result) =>
        TryWithCompositeFamily(
            context,
            static (Family family, OccupancyOperation payload, out PieceArcResult? value) => family.TryOccupancyBooleanResult(payload, out value),
            operation,
            out result);

    public static bool TryOccupancyBooleanResult(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        OccupancyOperation operation,
        out PieceArcResult? result,
        bool isOrdered = false) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered,
            static (Family family, OccupancyOperation payload, out PieceArcResult? value) => family.TryOccupancyBooleanResult(payload, out value),
            out result);

    public static bool TryBooleanAdjacentPairs(
        OperationContext context,
        BooleanOperation operation,
        out IReadOnlyList<PieceArcResult>? results) =>
        TryWithCompositeFamily(
            context,
            static (Family family, BooleanOperation payload, out IReadOnlyList<PieceArcResult>? value) => family.TryBooleanAdjacentPairs(payload, out value),
            operation,
            out results);

    public static bool TryBooleanAdjacentPairs(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        BooleanOperation operation,
        out IReadOnlyList<PieceArcResult>? results) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered: true,
            static (Family family, BooleanOperation payload, out IReadOnlyList<PieceArcResult>? value) => family.TryBooleanAdjacentPairs(payload, out value),
            out results);

    public static bool TryBooleanAdjacentPairResults(
        OperationContext context,
        BooleanOperation operation,
        out IReadOnlyList<PieceArcResult>? results) =>
        TryWithCompositeFamily(
            context,
            static (Family family, BooleanOperation payload, out IReadOnlyList<PieceArcResult>? value) => family.TryBooleanAdjacentPairResults(payload, out value),
            operation,
            out results);

    public static bool TryBooleanAdjacentPairResults(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        BooleanOperation operation,
        out IReadOnlyList<PieceArcResult>? results) =>
        TryWithCompositeFamily(
            frame,
            members,
            operation,
            isOrdered: true,
            static (Family family, BooleanOperation payload, out IReadOnlyList<PieceArcResult>? value) => family.TryBooleanAdjacentPairResults(payload, out value),
            out results);

    private static bool TryWithFamily<TResult>(
        GradedElement frame,
        IEnumerable<GradedElement> members,
        FamilyAction<TResult> action,
        out TResult? result)
        where TResult : class =>
        TryWithFamily(OperationContext.Create(frame, members), action, out result);

    private static bool TryWithFamily<TResult>(
        OperationContext context,
        FamilyAction<TResult> action,
        out TResult? result)
        where TResult : class =>
        action(new Family(context), out result);

    private static bool TryWithCompositeFamily<TResult, TPayload>(
        CompositeElement frame,
        IEnumerable<GradedElement> members,
        TPayload payload,
        bool isOrdered,
        CompositeFamilyAction<TResult, TPayload> action,
        out TResult? result)
        where TResult : class =>
        TryWithCompositeFamily(
            OperationContext.Create(frame, members, isOrdered),
            action,
            payload,
            out result);

    private static bool TryWithCompositeFamily<TResult, TPayload>(
        OperationContext context,
        CompositeFamilyAction<TResult, TPayload> action,
        TPayload payload,
        out TResult? result)
        where TResult : class
    {
        if (context.Frame is not CompositeElement)
        {
            result = null;
            return false;
        }

        return action(new Family(context), payload, out result);
    }
}








