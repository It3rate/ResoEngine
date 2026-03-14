using Core2.Elements;

namespace Core2.Support;

/// <summary>
/// A relation where one neutral element is interpreted inside another.
/// The parent acts as frame only in this relationship; any tensions are recorded rather than rejected.
/// </summary>
public sealed class Containment
{
    public Containment(ElementNode parent, ElementNode child)
    {
        Parent = parent;
        Child = child;
    }

    public ElementNode Parent { get; }
    public ElementNode Child { get; }
    public Perspective Perspective => Parent.Perspective;
    public IElement ChildInParentContext => Analyze().ChildInParentContext;
    public IReadOnlyList<ContainmentTension> Tensions => Analyze().Tensions;

    public override string ToString() =>
        $"{Parent.Element} [{Perspective}] contains {ChildInParentContext}";

    private ContainmentAnalysisResult Analyze() =>
        ContainmentAnalysis.Analyze(Parent.Element, Child.Element, Perspective);
}

public sealed class ElementNode
{
    private readonly List<Containment> _children = [];

    public ElementNode(IElement element, Perspective perspective = Perspective.Dominant)
    {
        Element = element;
        Perspective = perspective;
    }

    public IElement Element { get; }
    public Perspective Perspective { get; set; }
    public Containment? ParentRelation { get; private set; }
    public IReadOnlyList<Containment> Children => _children;

    public Containment AddChild(IElement child, Perspective childPerspective = Perspective.Dominant)
    {
        var childNode = new ElementNode(child, childPerspective);
        var relation = new Containment(this, childNode);
        childNode.ParentRelation = relation;
        _children.Add(relation);
        return relation;
    }

    public void OpposePerspective() => Perspective = Perspective.Oppose();

    public override string ToString() => $"{Element} [{Perspective}]";
}

public static class ElementNodeExtensions
{
    public static ElementNode AsNode(this IElement element, Perspective perspective = Perspective.Dominant) =>
        new(element, perspective);
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
        decimal parentLeft = Math.Min(parent.Start.Value, parent.End.Value);
        decimal parentRight = Math.Max(parent.Start.Value, parent.End.Value);
        decimal childLeft = Math.Min(child.Start.Value, child.End.Value);
        decimal childRight = Math.Max(child.Start.Value, child.End.Value);

        if (childLeft < parentLeft)
        {
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.OutsideExpectedRange,
                AppendPath(pathPrefix, "recessive.boundary"),
                $"Child left boundary {childLeft:0.###} falls outside parent left boundary {parentLeft:0.###}."));
        }

        if (childRight > parentRight)
        {
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.OutsideExpectedRange,
                AppendPath(pathPrefix, "dominant.boundary"),
                $"Child right boundary {childRight:0.###} falls outside parent right boundary {parentRight:0.###}."));
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
