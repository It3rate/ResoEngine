using Core2.Elements;

namespace Core2.Support;

/// <summary>
/// A relation where one neutral element is interpreted inside another.
/// The parent acts as frame only in this relationship; any tensions are recorded rather than rejected.
/// </summary>
public sealed class Containment
{
    public Containment(ElementNode parent, ElementNode child, Perspective perspective = Perspective.Dominant)
    {
        Parent = parent;
        Child = child;
        Perspective = perspective;

        var analysis = ContainmentAnalysis.Analyze(parent.Element, child.Element, perspective);
        ChildInParentContext = analysis.ChildInParentContext;
        Tensions = analysis.Tensions;
    }

    public ElementNode Parent { get; }
    public ElementNode Child { get; }
    public Perspective Perspective { get; }
    public IElement ChildInParentContext { get; }
    public IReadOnlyList<ContainmentTension> Tensions { get; }
}

public sealed class ElementNode
{
    private readonly List<Containment> _children = [];

    public ElementNode(IElement element)
    {
        Element = element;
    }

    public IElement Element { get; }
    public Containment? ParentRelation { get; private set; }
    public IReadOnlyList<Containment> Children => _children;

    public Containment AddChild(IElement child, Perspective perspective = Perspective.Dominant)
    {
        var childNode = new ElementNode(child);
        var relation = new Containment(this, childNode, perspective);
        childNode.ParentRelation = relation;
        _children.Add(relation);
        return relation;
    }
}

public static class ElementNodeExtensions
{
    public static ElementNode AsNode(this IElement element) => new(element);
}

internal readonly record struct ContainmentAnalysisResult(
    IElement ChildInParentContext,
    IReadOnlyList<ContainmentTension> Tensions);

internal static class ContainmentAnalysis
{
    public static ContainmentAnalysisResult Analyze(
        IElement parent,
        IElement child,
        Perspective perspective)
    {
        var encodedChild = EncodeForPerspective(child, perspective);
        var tensions = new List<ContainmentTension>();

        return parent switch
        {
            Scalar => new ContainmentAnalysisResult(encodedChild, tensions),
            Proportion proportionParent => AnalyzeProportionParent(proportionParent, encodedChild, tensions),
            Axis axisParent => AnalyzeAxisParent(axisParent, encodedChild, tensions),
            Area areaParent => AnalyzeAreaParent(areaParent, encodedChild, tensions),
            _ => Unsupported(encodedChild, tensions, parent, child),
        };
    }

    private static IElement EncodeForPerspective(IElement element, Perspective perspective)
    {
        if (perspective == Perspective.Dominant)
        {
            return element;
        }

        return element switch
        {
            Axis axis => -axis,
            Area area => -area,
            _ => element,
        };
    }

    private static ContainmentAnalysisResult AnalyzeProportionParent(
        Proportion parent,
        IElement child,
        List<ContainmentTension> tensions)
    {
        if (child is not Scalar and not Proportion)
        {
            return CrossGrade(child, tensions, parent, child,
                "A proportion can host higher-degree children, but it cannot fully place their orientation or expansion without an additional rule.");
        }

        var normalized = child switch
        {
            Scalar scalar => Proportion.FromRecessiveDominant(parent.Recessive, scalar),
            Proportion proportion => proportion,
            _ => throw new InvalidOperationException("Unreachable child normalization."),
        };

        AnalyzeProportionRangeAndResolution(parent, normalized, tensions, string.Empty);
        return new ContainmentAnalysisResult(normalized, tensions);
    }

    private static ContainmentAnalysisResult AnalyzeAxisParent(
        Axis parent,
        IElement child,
        List<ContainmentTension> tensions)
    {
        if (child is Axis axisChild)
        {
            AnalyzeAxisEnvelope(parent, axisChild, tensions, string.Empty);
            AnalyzeAxisSupport(parent, axisChild, tensions, string.Empty);
            return new ContainmentAnalysisResult(axisChild, tensions);
        }

        return CrossGrade(child, tensions, parent, child,
            "An Axis parent can host a child of another degree, but the child's placement along the line is underspecified without additional anchoring information.");
    }

    private static ContainmentAnalysisResult AnalyzeAreaParent(
        Area parent,
        IElement child,
        List<ContainmentTension> tensions)
    {
        if (child is Area areaChild)
        {
            AnalyzeAreaEnvelope(parent, areaChild, tensions, string.Empty);
            return new ContainmentAnalysisResult(areaChild, tensions);
        }

        return CrossGrade(child, tensions, parent, child,
            "An Area parent can host a child of another degree, but its location inside the area is underspecified without an additional embedding rule.");
    }

    private static void AnalyzeAreaEnvelope(
        Area parent,
        Area child,
        List<ContainmentTension> tensions,
        string pathPrefix)
    {
        AnalyzeAxisEnvelope(parent.Recessive, child.Recessive, tensions, AppendPath(pathPrefix, "recessive-axis"));
        AnalyzeAxisSupport(parent.Recessive, child.Recessive, tensions, AppendPath(pathPrefix, "recessive-axis"));
        AnalyzeAxisEnvelope(parent.Dominant, child.Dominant, tensions, AppendPath(pathPrefix, "dominant-axis"));
        AnalyzeAxisSupport(parent.Dominant, child.Dominant, tensions, AppendPath(pathPrefix, "dominant-axis"));
    }

    private static void AnalyzeAxisEnvelope(
        Axis parent,
        Axis child,
        List<ContainmentTension> tensions,
        string pathPrefix)
    {
        decimal parentStart = -parent.Recessive.Fold();
        decimal parentEnd = parent.Dominant.Fold();
        decimal childStart = -child.Recessive.Fold();
        decimal childEnd = child.Dominant.Fold();

        if (childStart < parentStart)
        {
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.OutsideExpectedRange,
                AppendPath(pathPrefix, "recessive.boundary"),
                $"Child start boundary {childStart:0.###} falls outside parent start boundary {parentStart:0.###}."));
        }

        if (childEnd > parentEnd)
        {
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.OutsideExpectedRange,
                AppendPath(pathPrefix, "dominant.boundary"),
                $"Child end boundary {childEnd:0.###} falls outside parent end boundary {parentEnd:0.###}."));
        }
    }

    private static void AnalyzeAxisSupport(
        Axis parent,
        Axis child,
        List<ContainmentTension> tensions,
        string pathPrefix)
    {
        AnalyzeProportionSupport(parent.Recessive, child.Recessive, tensions, AppendPath(pathPrefix, "recessive.support"));
        AnalyzeProportionSupport(parent.Dominant, child.Dominant, tensions, AppendPath(pathPrefix, "dominant.support"));
    }

    private static void AnalyzeProportionRangeAndResolution(
        Proportion parent,
        Proportion child,
        List<ContainmentTension> tensions,
        string path)
    {
        if (child.Recessive != parent.Recessive)
        {
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.ResolutionMismatch,
                path,
                $"Child recessive support {child.Recessive} does not match parent recessive support {parent.Recessive}."));
        }

        decimal parentValue = parent.Fold();
        decimal childValue = child.Fold();
        decimal min = Math.Min(0m, parentValue);
        decimal max = Math.Max(0m, parentValue);

        if (childValue < min || childValue > max)
        {
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.OutsideExpectedRange,
                path,
                $"Child value {childValue:0.###} falls outside expected range [{min:0.###}, {max:0.###}]."));
        }
    }

    private static void AnalyzeProportionSupport(
        Proportion parent,
        Proportion child,
        List<ContainmentTension> tensions,
        string path)
    {
        if (child.Recessive != parent.Recessive)
        {
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.ResolutionMismatch,
                path,
                $"Child recessive support {child.Recessive} does not match parent recessive support {parent.Recessive}."));
        }
    }

    private static ContainmentAnalysisResult CrossGrade(
        IElement encodedChild,
        List<ContainmentTension> tensions,
        IElement parent,
        IElement child,
        string message)
    {
        tensions.Add(new ContainmentTension(
            ContainmentTensionKind.PlacementUnderspecified,
            string.Empty,
            $"{message} Parent degree {parent.Degree}, child degree {child.Degree}."));

        return new ContainmentAnalysisResult(encodedChild, tensions);
    }

    private static ContainmentAnalysisResult Unsupported(
        IElement encodedChild,
        List<ContainmentTension> tensions,
        IElement parent,
        IElement child)
    {
        tensions.Add(new ContainmentTension(
            ContainmentTensionKind.UnsupportedInterpretation,
            string.Empty,
            $"No containment rule exists for parent degree {parent.Degree} and child degree {child.Degree}."));

        return new ContainmentAnalysisResult(encodedChild, tensions);
    }

    private static string AppendPath(string prefix, string segment) =>
        string.IsNullOrEmpty(prefix) ? segment : $"{prefix}.{segment}";
}
