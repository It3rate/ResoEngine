using Core3.Binding;
using Core3.Engine;

namespace Core3.Data;

/// <summary>
/// Accumulates across multiple FamilyViews at corresponding positions.
///
/// This is primarily VALUE-layer work: each view is read at the same
/// mover position (getting calibrated values), then the results are
/// combined through an arithmetic law (Add, Multiply, etc.).
///
/// The combine law is a GradedElement operation, not an enum.
/// This means custom laws naturally participate in the same algebra —
/// you can combine with any operation that takes two elements and
/// returns an outcome, including ones that produce tension.
///
/// Compare with EngineFamily.TryAccumulateAll — same pattern but
/// across separate families at corresponding parametric positions
/// rather than across members within one family.
/// </summary>
public class FamilyCombine
{
    private readonly List<FamilyView> _views = [];
    private readonly Func<GradedElement, GradedElement, EngineElementOutcome> _law;

    public FamilyCombine(Func<GradedElement, GradedElement, EngineElementOutcome> law)
    {
        _law = law;
    }

    public void AddView(FamilyView view) => _views.Add(view);

    /// <summary>
    /// Read all views at the same mover position and accumulate through the law.
    /// Each view reads through its own frame (value/structural/generative),
    /// then the results are combined at the value level.
    /// </summary>
    public EngineElementOutcome ReadCombined(TraversalMover mover)
    {
        if (_views.Count == 0)
            return EngineElementOutcome.WithTension(
                new AtomicElement(0, 0),
                new AtomicElement(0, 0),
                "No views to combine.");

        var current = _views[0].ReadAtMover(mover);

        for (var i = 1; i < _views.Count; i++)
        {
            var next = _views[i].ReadAtMover(mover);
            current = _law(current.Result, next.Result);
        }

        return current;
    }

    // Convenience factories — the law is just a GradedElement operation
    public static FamilyCombine Additive() =>
        new((a, b) => a.Add(b));

    public static FamilyCombine Multiplicative() =>
        new((a, b) => a.Multiply(b));

    public static FamilyCombine Subtractive() =>
        new((a, b) => a.Subtract(b));
}
