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
    public ContainmentTensionMetrics TensionMetrics => Analyze().Metrics;
    public bool HasTension => Tensions.Count > 0;

    public bool HasTensionOf(ContainmentTensionKind kind) =>
        Tensions.Any(tension => tension.Kind == kind);

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
    IReadOnlyList<ContainmentTension> Tensions,
    ContainmentTensionMetrics Metrics);

internal static class ContainmentAnalysis
{
    public static ContainmentAnalysisResult Analyze(
        IElement parent,
        IElement child,
        Perspective perspective)
    {
        var encodedChild = EncodeForPerspective(child, perspective);
        var tensions = new List<ContainmentTension>();
        var metrics = new List<ContainmentTensionMeasure>();

        return parent switch
        {
            Scalar => new ContainmentAnalysisResult(encodedChild, tensions, ContainmentTensionMetrics.None),
            Proportion proportionParent => AnalyzeProportionParent(proportionParent, encodedChild, tensions, metrics),
            Axis axisParent => AnalyzeAxisParent(axisParent, encodedChild, tensions, metrics),
            Area areaParent => AnalyzeAreaParent(areaParent, encodedChild, tensions, metrics),
            _ => Unsupported(encodedChild, tensions, metrics, parent, child),
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
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics)
    {
        if (child is not Scalar and not Proportion)
        {
            return CrossGrade(child, tensions, metrics, parent, child,
                "A proportion can host higher-degree children, but it cannot fully place their orientation or expansion without an additional rule.");
        }

        var normalized = child switch
        {
            Scalar scalar => Proportion.FromRecessiveDominant(parent.Recessive, scalar),
            Proportion proportion => proportion,
            _ => throw new InvalidOperationException("Unreachable child normalization."),
        };

        AnalyzeProportionRangeAndResolution(parent, normalized, tensions, metrics, string.Empty);
        return BuildResult(normalized, tensions, metrics);
    }

    private static ContainmentAnalysisResult AnalyzeAxisParent(
        Axis parent,
        IElement child,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics)
    {
        if (child is Axis axisChild)
        {
            AnalyzeAxisEnvelope(parent, axisChild, tensions, metrics, string.Empty);
            AnalyzeAxisSupport(parent, axisChild, tensions, metrics, string.Empty);
            return BuildResult(axisChild, tensions, metrics);
        }

        return CrossGrade(child, tensions, metrics, parent, child,
            "An Axis parent can host a child of another degree, but the child's placement along the line is underspecified without additional anchoring information.");
    }

    private static ContainmentAnalysisResult AnalyzeAreaParent(
        Area parent,
        IElement child,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics)
    {
        if (child is Area areaChild)
        {
            AnalyzeAreaEnvelope(parent, areaChild, tensions, metrics, string.Empty);
            return BuildResult(areaChild, tensions, metrics);
        }

        return CrossGrade(child, tensions, metrics, parent, child,
            "An Area parent can host a child of another degree, but its location inside the area is underspecified without an additional embedding rule.");
    }

    private static void AnalyzeAreaEnvelope(
        Area parent,
        Area child,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics,
        string pathPrefix)
    {
        AnalyzeAxisEnvelope(parent.Recessive, child.Recessive, tensions, metrics, AppendPath(pathPrefix, "recessive-axis"));
        AnalyzeAxisSupport(parent.Recessive, child.Recessive, tensions, metrics, AppendPath(pathPrefix, "recessive-axis"));
        AnalyzeAxisEnvelope(parent.Dominant, child.Dominant, tensions, metrics, AppendPath(pathPrefix, "dominant-axis"));
        AnalyzeAxisSupport(parent.Dominant, child.Dominant, tensions, metrics, AppendPath(pathPrefix, "dominant-axis"));
    }

    private static void AnalyzeAxisEnvelope(
        Axis parent,
        Axis child,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics,
        string pathPrefix)
    {
        decimal parentLeft = Math.Min(parent.Start.Value, parent.End.Value);
        decimal parentRight = Math.Max(parent.Start.Value, parent.End.Value);
        decimal childLeft = Math.Min(child.Start.Value, child.End.Value);
        decimal childRight = Math.Max(child.Start.Value, child.End.Value);
        decimal validSpan = parentRight - parentLeft;

        if (childLeft < parentLeft)
        {
            string path = AppendPath(pathPrefix, "recessive.boundary");
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.OutsideExpectedRange,
                path,
                $"Child left boundary {childLeft:0.###} falls outside parent left boundary {parentLeft:0.###}."));
            AddMeasure(metrics, ContainmentTensionKind.OutsideExpectedRange, path, parentLeft - childLeft, validSpan, "overflow / valid span");
        }

        if (childRight > parentRight)
        {
            string path = AppendPath(pathPrefix, "dominant.boundary");
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.OutsideExpectedRange,
                path,
                $"Child right boundary {childRight:0.###} falls outside parent right boundary {parentRight:0.###}."));
            AddMeasure(metrics, ContainmentTensionKind.OutsideExpectedRange, path, childRight - parentRight, validSpan, "overflow / valid span");
        }
    }

    private static void AnalyzeAxisSupport(
        Axis parent,
        Axis child,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics,
        string pathPrefix)
    {
        AnalyzeProportionSupport(parent.Recessive, child.Recessive, tensions, metrics, AppendPath(pathPrefix, "recessive.support"));
        AnalyzeProportionSupport(parent.Dominant, child.Dominant, tensions, metrics, AppendPath(pathPrefix, "dominant.support"));
    }

    private static void AnalyzeProportionRangeAndResolution(
        Proportion parent,
        Proportion child,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics,
        string path)
    {
        if (child.Recessive != parent.Recessive)
        {
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.ResolutionMismatch,
                path,
                $"Child recessive support {child.Recessive} does not match parent recessive support {parent.Recessive}."));
            AddMeasure(
                metrics,
                ContainmentTensionKind.ResolutionMismatch,
                path,
                Math.Abs(child.Recessive.Value - parent.Recessive.Value),
                Math.Abs(parent.Recessive.Value),
                "support difference / parent support");
        }

        decimal parentValue = parent.Fold();
        decimal childValue = child.Fold();
        decimal min = Math.Min(0m, parentValue);
        decimal max = Math.Max(0m, parentValue);
        decimal validSpan = max - min;

        if (childValue < min || childValue > max)
        {
            decimal overflow = childValue < min ? min - childValue : childValue - max;
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.OutsideExpectedRange,
                path,
                $"Child value {childValue:0.###} falls outside expected range [{min:0.###}, {max:0.###}]."));
            AddMeasure(metrics, ContainmentTensionKind.OutsideExpectedRange, path, overflow, validSpan, "overflow / valid span");
        }
    }

    private static void AnalyzeProportionSupport(
        Proportion parent,
        Proportion child,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics,
        string path)
    {
        if (child.Recessive != parent.Recessive)
        {
            tensions.Add(new ContainmentTension(
                ContainmentTensionKind.ResolutionMismatch,
                path,
                $"Child recessive support {child.Recessive} does not match parent recessive support {parent.Recessive}."));
            AddMeasure(
                metrics,
                ContainmentTensionKind.ResolutionMismatch,
                path,
                Math.Abs(child.Recessive.Value - parent.Recessive.Value),
                Math.Abs(parent.Recessive.Value),
                "support difference / parent support");
        }
    }

    private static ContainmentAnalysisResult CrossGrade(
        IElement encodedChild,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics,
        IElement parent,
        IElement child,
        string message)
    {
        tensions.Add(new ContainmentTension(
            ContainmentTensionKind.PlacementUnderspecified,
            string.Empty,
            $"{message} Parent degree {parent.Degree}, child degree {child.Degree}."));

        return BuildResult(encodedChild, tensions, metrics);
    }

    private static ContainmentAnalysisResult Unsupported(
        IElement encodedChild,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics,
        IElement parent,
        IElement child)
    {
        tensions.Add(new ContainmentTension(
            ContainmentTensionKind.UnsupportedInterpretation,
            string.Empty,
            $"No containment rule exists for parent degree {parent.Degree} and child degree {child.Degree}."));

        return BuildResult(encodedChild, tensions, metrics);
    }

    private static ContainmentAnalysisResult BuildResult(
        IElement encodedChild,
        List<ContainmentTension> tensions,
        List<ContainmentTensionMeasure> metrics) =>
        new(
            encodedChild,
            tensions,
            metrics.Count == 0 ? ContainmentTensionMetrics.None : new ContainmentTensionMetrics(metrics.ToArray()));

    private static void AddMeasure(
        List<ContainmentTensionMeasure> metrics,
        ContainmentTensionKind kind,
        string path,
        decimal numerator,
        decimal denominator,
        string basis)
    {
        if (numerator <= 0m)
        {
            return;
        }

        metrics.Add(new ContainmentTensionMeasure(
            kind,
            path,
            Proportion.FromRecessiveDominant((Scalar)denominator, (Scalar)numerator),
            basis));
    }

    private static string AppendPath(string prefix, string segment) =>
        string.IsNullOrEmpty(prefix) ? segment : $"{prefix}.{segment}";
}
