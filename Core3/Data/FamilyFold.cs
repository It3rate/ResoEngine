using Core3.Engine;
using Core3.Operations;

namespace Core3.Data;

/// <summary>
/// GENERATIVE — recursive pairwise reduction of family members.
///
/// Fold is the most clearly generative operation: it produces entirely new
/// structure from the relationships between adjacent members. Each fold pass
/// reduces the family by one, and the intermediate families at each level
/// ARE the derived structure — not intermediate computation to be discarded.
///
/// De Casteljau (Bezier curves): fold with interpolation.
/// Cumulative sum: fold with addition.
/// Running product: fold with multiplication.
/// Difference sequences: fold with subtraction.
///
/// The law applied at each step is a GradedElement operation, so any
/// combination of value-level arithmetic produces valid fold sequences.
/// Tension at any fold level tells you where the structure resists reduction —
/// which is meaningful information, not error.
///
/// Parked for now alongside FamilyInterpolation as generative-layer sketches.
/// The connection to unresolved (zero-unit) frames: a fold could be what
/// a zero-unit frame invokes to resolve its tension — "I don't know what's
/// here yet, so derive it by folding the members."
/// </summary>
public static class FamilyFold
{
    /// <summary>
    /// Single fold pass: apply a pairwise law to adjacent members,
    /// producing a new family with count-1 members.
    /// </summary>
    public static Family FoldOnce(
        Family family,
        Func<GradedElement, GradedElement, EngineElementOutcome> law)
    {
        var result = new Family(family.Frame);

        for (var i = 0; i < family.Count - 1; i++)
        {
            var outcome = law(family.Members[i], family.Members[i + 1]);
            result.AddMember(outcome.Result);
        }

        return result;
    }

    /// <summary>
    /// Full recursive fold: keep folding until one element remains.
    /// Returns all intermediate levels — these are the derived structure.
    ///
    /// levels[0] = first fold (count-1 members)
    /// levels[last] = single result element
    ///
    /// For a Bezier curve, level 0 = linear segments between control points,
    /// level 1 = quadratic sub-curves, ..., last level = the point on the curve.
    /// Reading level N-1 at the same parameter gives you the tangent.
    /// </summary>
    public static List<Family> FoldAll(
        Family family,
        Func<GradedElement, GradedElement, EngineElementOutcome> law)
    {
        var levels = new List<Family>();
        var current = family;

        while (current.Count > 1)
        {
            current = FoldOnce(current, law);
            levels.Add(current);
        }

        return levels;
    }

    /// <summary>
    /// De Casteljau: fold with interpolation at parameter t.
    /// The purest generative operation — creates a curve from control points
    /// by recursively generating intermediate structure.
    /// </summary>
    public static EngineElementOutcome DeCasteljau(Family controlPoints, AtomicElement t)
    {
        var levels = FoldAll(
            controlPoints,
            (left, right) => FamilyInterpolation.Interpolate(left, right, t));

        var final = levels[^1];
        return final.Count == 1
            ? EngineElementOutcome.Exact(final.Members[0])
            : EngineElementOutcome.WithTension(
                final.Members[0],
                final.Members[0].CreateZeroLikePeer(),
                "Fold did not converge to single element.");
    }

    /// <summary>
    /// Cumulative sum: fold with addition.
    /// The value-layer equivalent of fold — no new structure generated,
    /// just accumulated values. Sits at the boundary between value and generative.
    /// </summary>
    public static List<Family> CumulativeSum(Family family) =>
        FoldAll(family, (left, right) => left.Add(right));

    /// <summary>
    /// Cumulative product: fold with multiplication.
    /// </summary>
    public static List<Family> CumulativeProduct(Family family) =>
        FoldAll(family, (left, right) => left.Multiply(right));

    /// <summary>
    /// Evaluate a full curve by sampling de Casteljau at N positions.
    /// Returns a new family of generated points — pure generative output.
    /// </summary>
    public static Family EvaluateCurve(Family controlPoints, long sampleCount)
    {
        var curve = new Family(controlPoints.Frame);

        for (long i = 0; i <= sampleCount; i++)
        {
            var t = new AtomicElement(i, sampleCount);
            var point = DeCasteljau(controlPoints, t);
            curve.AddMember(point.Result);
        }

        return curve;
    }
}
