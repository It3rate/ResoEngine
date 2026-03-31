using Core3.Binding;
using Core3.Engine;

namespace Core3.Data;

/// <summary>
/// Blends between multiple FamilyViews.
///
/// Blending sits at the boundary between structural and generative:
/// - The selection of which two views are active is STRUCTURAL (orthogonal
///   organization across the view collection — same pattern as stride)
/// - The interpolation between them is GENERATIVE (unresolved, needs evaluation)
///
/// For now we handle both in one place. As the generative layer matures,
/// the interpolation part should delegate to whatever law-resolution
/// mechanism emerges for unresolved units.
///
/// Think of it as a family-of-families: the outer position picks which
/// two inner families are structurally adjacent, and the inner position
/// reads within each family through its own frame.
/// </summary>
public class FamilyBlend
{
    private readonly List<FamilyView> _views = [];

    public IReadOnlyList<FamilyView> Views => _views;

    public void AddView(FamilyView view) => _views.Add(view);

    /// <summary>
    /// Read at a data position, blended at a blend position.
    /// blendWeight: value/unit ratio. 0/n = first view, n/n = last view.
    /// Intermediate = interpolated between structurally adjacent views.
    /// </summary>
    public EngineElementOutcome ReadBlended(TraversalMover dataMover, AtomicElement blendWeight)
    {
        if (_views.Count == 0)
            return EngineElementOutcome.WithTension(
                new AtomicElement(0, 0),
                new AtomicElement(0, 0),
                "No views to blend.");

        if (_views.Count == 1)
            return _views[0].ReadAtMover(dataMover);

        // STRUCTURAL part: map blend weight into view index space
        // Same pattern as stride — organizing views along an axis.
        var maxIndex = _views.Count - 1;
        var scaled = blendWeight.Value * maxIndex;
        var lowerIndex = (int)(scaled / blendWeight.Unit);
        lowerIndex = Math.Min(lowerIndex, maxIndex - 1);
        var upperIndex = lowerIndex + 1;

        var remainder = scaled - (lowerIndex * blendWeight.Unit);

        // Read from both structurally adjacent views
        var leftResult = _views[lowerIndex].ReadAtMover(dataMover);
        var rightResult = _views[upperIndex].ReadAtMover(dataMover);

        if (remainder == 0)
            return leftResult;

        // GENERATIVE part: interpolate between the two readings.
        // This is the unresolved/evaluative gesture — producing new structure
        // that wasn't in either view alone.
        var interpWeight = new AtomicElement(remainder, blendWeight.Unit);
        return FamilyInterpolation.Interpolate(
            leftResult.Result,
            rightResult.Result,
            interpWeight);
    }
}
