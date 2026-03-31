using Core3.Engine;
using Core3.Operations;

namespace Core3.Data;

/// <summary>
/// STRUCTURAL REORGANIZATION within elements — driven by orthogonal path descent.
///
/// Where FamilyStride organizes a family's members along structural axes,
/// FamilyProjection reorganizes the internal structure of each member.
/// Both are the same gesture: imposing orthogonal structure. The difference
/// is scope — stride operates across the family, projection operates
/// within each element's composite tree.
///
/// A projection is itself structural data: a pattern of descent choices
/// (recessive/dominant) that selects which leaves to extract. This pattern
/// could eventually be expressed as a GradedElement too, making projections
/// composable through the same algebra as everything else.
///
/// For a grade-2 member ((rr | rd) | (dr | dd)):
///   Descend [Recessive, Recessive] → rr  (like selecting "X")
///   Descend [Dominant, Dominant]   → dd  (like selecting "W")
///   Two paths [[R,R], [D,D]]      → Composite(rr, dd)  (swizzle to XW)
/// </summary>
public class FamilyProjection
{
    private readonly ProjectionPath[] _paths;

    public FamilyProjection(params ProjectionPath[] paths)
    {
        _paths = paths;
    }

    /// <summary>
    /// Project a single element: extract the specified structural paths
    /// and compose them into a new element.
    /// </summary>
    public GradedElement? Project(GradedElement element)
    {
        if (_paths.Length == 1)
            return Descend(element, _paths[0]);

        // Multiple paths: compose results into a composite tree.
        // Two paths → grade 1, four paths → grade 2, etc.
        var extracted = new List<GradedElement>();
        foreach (var path in _paths)
        {
            var result = Descend(element, path);
            if (result == null) return null;
            extracted.Add(result);
        }

        return BuildTree(extracted);
    }

    /// <summary>
    /// Project all members of a family, returning a new family
    /// with structurally reorganized members.
    ///
    /// The frame is preserved — projection changes what's inside each member,
    /// not how the family itself is organized. That's the key distinction from
    /// stride: stride changes the family's organizational frame, projection
    /// changes each member's internal structure.
    /// </summary>
    public Family ProjectFamily(Family family)
    {
        var result = new Family(family.Frame);

        foreach (var member in family.Members)
        {
            var projected = Project(member);
            if (projected != null)
                result.AddMember(projected);
        }

        return result;
    }

    /// <summary>
    /// Structural descent through a composite tree.
    /// Each step chooses recessive or dominant — this is path selection
    /// through an imposed structure, the within-element form of the same
    /// orthogonal gesture that strides use across elements.
    /// </summary>
    private static GradedElement? Descend(GradedElement element, ProjectionPath path)
    {
        var current = element;

        foreach (var step in path.Steps)
        {
            if (current is not CompositeElement composite)
                return null;

            current = step == DescentStep.Recessive
                ? composite.Recessive
                : composite.Dominant;
        }

        return current;
    }

    /// <summary>
    /// Build a balanced binary tree from extracted leaves.
    /// [a, b]       → Composite(a, b)           (grade 1)
    /// [a, b, c, d] → Composite(C(a,b), C(c,d)) (grade 2)
    /// </summary>
    private static GradedElement BuildTree(List<GradedElement> elements)
    {
        if (elements.Count == 1) return elements[0];

        var pairs = new List<GradedElement>();
        for (var i = 0; i < elements.Count; i += 2)
        {
            if (i + 1 < elements.Count)
                pairs.Add(new CompositeElement(elements[i], elements[i + 1]));
            else
                pairs.Add(elements[i]);
        }

        return BuildTree(pairs);
    }

    // --- Convenience projections for grade-2 elements (4 atomic leaves) ---

    public static FamilyProjection X => new(ProjectionPath.RecessiveRecessive);
    public static FamilyProjection Y => new(ProjectionPath.RecessiveDominant);
    public static FamilyProjection Z => new(ProjectionPath.DominantRecessive);
    public static FamilyProjection W => new(ProjectionPath.DominantDominant);
    public static FamilyProjection XY => new(ProjectionPath.RecessiveRecessive, ProjectionPath.RecessiveDominant);
    public static FamilyProjection ZW => new(ProjectionPath.DominantRecessive, ProjectionPath.DominantDominant);
    public static FamilyProjection XW => new(ProjectionPath.RecessiveRecessive, ProjectionPath.DominantDominant);
}

public enum DescentStep { Recessive, Dominant }

public record ProjectionPath(params DescentStep[] Steps)
{
    public static ProjectionPath RecessiveRecessive => new(DescentStep.Recessive, DescentStep.Recessive);
    public static ProjectionPath RecessiveDominant => new(DescentStep.Recessive, DescentStep.Dominant);
    public static ProjectionPath DominantRecessive => new(DescentStep.Dominant, DescentStep.Recessive);
    public static ProjectionPath DominantDominant => new(DescentStep.Dominant, DescentStep.Dominant);
}
