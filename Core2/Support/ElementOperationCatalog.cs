using Core2.Elements;

namespace Core2.Support;

public enum ElementOperation
{
    Pin,
    Compare,
    Repeat,
    InverseContinue,
    Scale,
    Mirror,
    Conjugate,
    PerspectiveFlip,
    ExpandMultiply,
    BooleanMerge,
    Contain,
    Fold,
}

public enum OperationActivation
{
    Dormant,
    Relational,
    Oriented,
    Expansive,
}

public readonly record struct ElementOperationProfile(
    ElementOperation Operation,
    int MinimumGrade,
    bool RequiresRelation,
    OperationActivation Activation,
    string Witness,
    string Summary)
{
    public bool IsMeaningful => Activation != OperationActivation.Dormant;
}

public static class ElementOperationCatalog
{
    public static IReadOnlyList<ElementOperation> All { get; } = Enum.GetValues<ElementOperation>();

    public static ElementOperationProfile Describe(int grade, ElementOperation operation)
    {
        int minimumGrade = GetMinimumGrade(operation);
        bool requiresRelation = RequiresRelation(operation);
        OperationActivation activation = ResolveActivation(grade, minimumGrade);

        return new ElementOperationProfile(
            operation,
            minimumGrade,
            requiresRelation,
            activation,
            GetWitness(operation),
            GetSummary(operation, activation));
    }

    public static IReadOnlyList<ElementOperationProfile> DescribeAll(int grade) =>
        All.Select(operation => Describe(grade, operation)).ToArray();

    private static int GetMinimumGrade(ElementOperation operation) => operation switch
    {
        ElementOperation.Pin => 0,
        ElementOperation.Repeat => 0,
        ElementOperation.InverseContinue => 0,
        ElementOperation.Scale => 0,
        ElementOperation.Contain => 0,
        ElementOperation.Compare => 1,
        ElementOperation.Mirror => 1,
        ElementOperation.Fold => 1,
        ElementOperation.Conjugate => 2,
        ElementOperation.PerspectiveFlip => 2,
        ElementOperation.ExpandMultiply => 3,
        ElementOperation.BooleanMerge => 3,
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
    };

    private static bool RequiresRelation(ElementOperation operation) => operation switch
    {
        ElementOperation.Pin => true,
        ElementOperation.Compare => true,
        ElementOperation.Scale => true,
        ElementOperation.ExpandMultiply => true,
        ElementOperation.BooleanMerge => true,
        ElementOperation.Contain => true,
        _ => false,
    };

    private static OperationActivation ResolveActivation(int grade, int minimumGrade)
    {
        if (grade < minimumGrade)
        {
            return OperationActivation.Dormant;
        }

        return grade switch
        {
            <= 1 => OperationActivation.Relational,
            2 => OperationActivation.Oriented,
            _ => OperationActivation.Expansive,
        };
    }

    private static string GetWitness(ElementOperation operation) => operation switch
    {
        ElementOperation.Pin => "A second selected element to enter a common context.",
        ElementOperation.Compare => "A dominant/recessive relation that can witness greater, less, or equal.",
        ElementOperation.Repeat => "An index and continuation law that can carry the same rule across steps, space, or recursion.",
        ElementOperation.InverseContinue => "A reverse-path rule that can select one or more valid inverses of a repetition.",
        ElementOperation.Scale => "A transform partner or scale code that can change the amount or extent.",
        ElementOperation.Mirror => "Two asymmetric roles that can be swapped without collapsing the structure.",
        ElementOperation.Conjugate => "An oriented object with separately addressable recessive and dominant sides.",
        ElementOperation.PerspectiveFlip => "A directed reading with an observer-dependent orientation.",
        ElementOperation.ExpandMultiply => "Independent extents that can expand into a higher interaction space.",
        ElementOperation.BooleanMerge => "Distinct regions or occupancy states that can be combined by a rule.",
        ElementOperation.Contain => "A parent-child relation where one element frames another.",
        ElementOperation.Fold => "A richer degree that can collapse into a simpler one.",
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
    };

    private static string GetSummary(ElementOperation operation, OperationActivation activation) => (operation, activation) switch
    {
        (_, OperationActivation.Dormant) => "The structure does not yet have enough distinguishable content for this operation to act nontrivially.",

        (ElementOperation.Pin, OperationActivation.Relational) => "Pins amounts or relations into a common comparison context.",
        (ElementOperation.Pin, OperationActivation.Oriented) => "Pins directed lines into a shared oriented frame.",
        (ElementOperation.Pin, OperationActivation.Expansive) => "Pins oriented frames into a higher interaction space.",

        (ElementOperation.Compare, OperationActivation.Relational) => "Compares dominant amount against recessive support or another normalized relation.",
        (ElementOperation.Compare, OperationActivation.Oriented) => "Compares directed intervals with sign, order, and perspective in play.",
        (ElementOperation.Compare, OperationActivation.Expansive) => "Compares regions, quadrants, and aspect relations across an expanded space.",

        (ElementOperation.Repeat, OperationActivation.Relational) => "Repeats amounts or relations under a continuation law such as additive accumulation or ratio-preserving recurrence.",
        (ElementOperation.Repeat, OperationActivation.Oriented) => "Repeats directed states or transforms, producing cycles, alternation, and observer-sensitive orbits.",
        (ElementOperation.Repeat, OperationActivation.Expansive) => "Repeats regions, quadrants, or whole frames as tilings, wave patterns, or higher-dimensional recurrences.",

        (ElementOperation.InverseContinue, OperationActivation.Relational) => "Reverses a repetition at the amount/support level and may yield multiple valid candidates.",
        (ElementOperation.InverseContinue, OperationActivation.Oriented) => "Reverses a directed orbit and uses a branch rule to choose a principal inverse path.",
        (ElementOperation.InverseContinue, OperationActivation.Expansive) => "Reverses a folded or expanded space and may distinguish fold-first from structure-preserving candidates.",

        (ElementOperation.Scale, OperationActivation.Relational) => "Changes amount relative to support, even before orientation exists.",
        (ElementOperation.Scale, OperationActivation.Oriented) => "Stretches directed extent while preserving or inverting its orientation.",
        (ElementOperation.Scale, OperationActivation.Expansive) => "Scales whole interaction regions rather than only a single directed extent.",

        (ElementOperation.Mirror, OperationActivation.Relational) => "Swaps dominant amount and recessive support; at this grade it appears as reciprocal.",
        (ElementOperation.Mirror, OperationActivation.Oriented) => "Swaps recessive and dominant directed roles on the same line.",
        (ElementOperation.Mirror, OperationActivation.Expansive) => "Reflects or swaps the interacting axes or regions in the expanded space.",

        (ElementOperation.Conjugate, OperationActivation.Oriented) => "Negates one oriented side while leaving the other side fixed.",
        (ElementOperation.Conjugate, OperationActivation.Expansive) => "Negates selected oriented regions or sub-axes while preserving the larger frame.",

        (ElementOperation.PerspectiveFlip, OperationActivation.Oriented) => "Re-reads the same directed object from the opposite observer stance.",
        (ElementOperation.PerspectiveFlip, OperationActivation.Expansive) => "Re-reads a whole interaction space from the opposite observer stance.",

        (ElementOperation.ExpandMultiply, OperationActivation.Expansive) => "Expands two independent extents into a higher interaction space before any fold.",

        (ElementOperation.BooleanMerge, OperationActivation.Expansive) => "Applies region-level true/false or weighted boolean rules across the expanded space.",

        (ElementOperation.Contain, OperationActivation.Relational) => "Lets one element frame another and records support/range tension.",
        (ElementOperation.Contain, OperationActivation.Oriented) => "Lets a directed frame host directed children with oriented expectations.",
        (ElementOperation.Contain, OperationActivation.Expansive) => "Lets an expanded space host nested regions and accumulate higher-order tensions.",

        (ElementOperation.Fold, OperationActivation.Relational) => "Collapses a relation into a scalar-style summary.",
        (ElementOperation.Fold, OperationActivation.Oriented) => "Collapses a directed line into a proportion-style summary.",
        (ElementOperation.Fold, OperationActivation.Expansive) => "Collapses an expanded interaction space back into a directed summary.",

        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
    };
}

public static class ElementOperationExtensions
{
    public static ElementOperationProfile DescribeOperation(this IElement element, ElementOperation operation) =>
        ElementOperationCatalog.Describe(element.Degree, operation);

    public static IReadOnlyList<ElementOperationProfile> DescribeOperations(this IElement element) =>
        ElementOperationCatalog.DescribeAll(element.Degree);
}
