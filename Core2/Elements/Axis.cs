using Core2.Repetition;
using Core2.Support;
using ResoEngine.Core2.Support;

namespace Core2.Elements;

/// <summary>
/// Degree 2: a 1D directed reading with four scalar degrees of freedom.
/// Recessive and dominant are each Proportions, where each Proportion is
/// dominant amount over recessive support. Together that gives four scalar degrees:
/// recessive amount/support and dominant amount/support.
/// </summary>
public sealed record Axis(Proportion Recessive, Proportion Dominant, AxisBasis Basis = AxisBasis.Complex)
    : IElement, IPinnedElement<Proportion, Proportion>
{
    private static readonly AlgebraTable<Proportion> ComplexTable = new(Proportion.Arithmetic);
    private static readonly AlgebraTable<Proportion> SplitComplexTable = new(
        Proportion.Arithmetic,
        [
            new AlgebraEntry(0, 0, 1, +1),
            new AlgebraEntry(0, 1, 0, +1),
            new AlgebraEntry(1, 0, 0, +1),
            new AlgebraEntry(1, 1, 1, +1),
        ]);

    public static Axis Zero => new(Proportion.Zero, Proportion.Zero);
    public static Axis One => new(Proportion.Zero, Proportion.One);
    public static Axis I => new(Proportion.One, Proportion.Zero);
    public static Axis NegativeOne => new(Proportion.Zero, -Proportion.One);
    public static Axis NegativeI => new(-Proportion.One, Proportion.Zero);
    public static Axis PinUnit => new(new Proportion(1, 1), new Proportion(1, 1));
    public int Degree => 2;

    internal static IArithmetic<Axis> Arithmetic { get; } = new AxisArithmetic();

    public Axis(long recessiveValue, long recessiveUnit, long dominantValue, long dominantUnit)
        : this(new Proportion(recessiveValue, recessiveUnit), new Proportion(dominantValue, dominantUnit))
    {
    }

    private static Axis FromPair((Proportion Recessive, Proportion Dominant) pair) =>
        new(pair.Recessive, pair.Dominant);

    private static Axis FromPair((Proportion Recessive, Proportion Dominant) pair, AxisBasis basis) =>
        new(pair.Recessive, pair.Dominant, basis);

    private AlgebraTable<Proportion> Table => Basis == AxisBasis.SplitComplex ? SplitComplexTable : ComplexTable;
    public PinAxisResolution PinResolution => PinAxisInterpreter.Resolve(this);
    public PinRelation Relation => PinResolution.Relation;
    Proportion IPinnedElement<Proportion, Proportion>.RecessiveElement => Recessive;
    Proportion IPinnedElement<Proportion, Proportion>.DominantElement => Dominant;
    IElement IPinnedElement.RecessiveElement => Recessive;
    IElement IPinnedElement.DominantElement => Dominant;
    public bool IsSegmentLike => PinResolution.IsDirectedSegment;
    public bool IsSequentialReinforcement => PinResolution.IsSequentialReinforcement;
    public bool IsOrthogonalStructure => PinResolution.IsOrthogonal;
    public int? PreferredCarrierRank => PinResolution.SharedCarrierRank;

    public Proportion StartCoordinate => -Recessive;
    public Proportion EndCoordinate => Dominant;
    public Proportion CoordinateSpan => EndCoordinate - StartCoordinate;
    public Proportion LeftCoordinate => StartCoordinate <= EndCoordinate ? StartCoordinate : EndCoordinate;
    public Proportion RightCoordinate => StartCoordinate <= EndCoordinate ? EndCoordinate : StartCoordinate;
    public Proportion MidpointCoordinate => (StartCoordinate + EndCoordinate) / new Proportion(2);
    public Scalar Start => -Recessive.Fold();
    public Scalar End => Dominant.Fold();
    public Scalar Left => LeftCoordinate.Fold();
    public Scalar Right => RightCoordinate.Fold();
    public Scalar Span => CoordinateSpan.Fold();
    public Scalar Midpoint => MidpointCoordinate.Fold();
    public bool IsDegenerate => StartCoordinate == EndCoordinate;
    public bool HasExtent => !IsDegenerate;
    public bool IsEmptyInterval => IsDegenerate;
    public bool PointsRight => EndCoordinate >= StartCoordinate;

    public static Axis FromCoordinates(
        Proportion start,
        Proportion end,
        AxisBasis basis = AxisBasis.Complex) =>
        new(-start, end, basis);

    public static Axis FromCoordinates(
        Scalar start,
        Scalar end,
        Scalar? recessiveSupport = null,
        Scalar? dominantSupport = null,
        AxisBasis basis = AxisBasis.Complex) =>
        new(
            (-start).AsProportion((recessiveSupport ?? Scalar.One).ToLongExact()),
            end.AsProportion((dominantSupport ?? Scalar.One).ToLongExact()),
            basis);

    public Axis WithCoordinates(Scalar start, Scalar end) =>
        FromCoordinates(start, end, new Scalar(Recessive.Recessive), new Scalar(Dominant.Recessive), Basis);

    public Axis WithCoordinates(Proportion start, Proportion end) =>
        FromCoordinates(start, end, Basis);

    internal Axis WithBounds(Scalar left, Scalar right) =>
        PointsRight ? WithCoordinates(left, right) : WithCoordinates(right, left);

    internal Axis WithBounds(Proportion left, Proportion right) =>
        PointsRight ? WithCoordinates(left, right) : WithCoordinates(right, left);

    internal bool HasCompatibleCarrier(Axis other) =>
        Basis == other.Basis &&
        PointsRight == other.PointsRight &&
        Recessive.Recessive == other.Recessive.Recessive &&
        Dominant.Recessive == other.Dominant.Recessive;

    public Axis ApplyOpposition()
    {
        var result = Table.ApplyOpposition(Recessive, Dominant);
        return FromPair(result, Basis);
    }

    public Axis Oppose() => ApplyOpposition();

    public Axis Mirror() => FromPair(Table.Mirror(Recessive, Dominant), Basis);

    public Axis SwapUnitRoles() => Mirror();

    public Axis FlipPerspective() => -this;

    public Axis ConjugateRecessive() => FromPair(Table.NegateRecessive(Recessive, Dominant), Basis);

    public Axis ConjugateDominant() => FromPair(Table.NegateDominant(Recessive, Dominant), Basis);

    public Axis ProjectRecessiveIntoDominant() =>
        FromPair(Table.ProjectRecessiveIntoDominant(Recessive, Dominant), Basis);

    public Axis ProjectDominantIntoRecessive() =>
        FromPair(Table.ProjectDominantIntoRecessive(Recessive, Dominant), Basis);

    public Area Pin(Axis other) => new(this, other);

    public PointPinning<Axis, Axis> PinAt(Axis other, Proportion position) => new(this, other, position);

    public PositionedAxis PlaceAt(Proportion position) => new(this, position);

    public PinnedPair<Proportion, Proportion> AsPinnedPair() =>
        new(Recessive, Dominant, Relation);

    public InverseContinuationResult<Axis> InverseContinue(
        int degree,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? reference = null) =>
        InverseContinuationEngine.InverseContinue(this, degree, rule, reference);

    public PowerResult<Axis> TryPow(
        Proportion exponent,
        InverseContinuationRule rule = InverseContinuationRule.Principal,
        Axis? reference = null) =>
        PowerEngine.Pow(this, exponent, rule, reference);

    /// <summary>
    /// Returns the geometric overlap clipped into the left operand's direction/support frame.
    /// Use TryIntersect(...) if you need to distinguish "no overlap" from a point result.
    /// </summary>
    public Axis Intersect(Axis other) =>
        TryIntersect(other, out var intersection) ? intersection : Zero;

    public bool TryIntersect(Axis other, out Axis intersection)
    {
        var left = Proportion.Max(LeftCoordinate, other.LeftCoordinate);
        var right = Proportion.Min(RightCoordinate, other.RightCoordinate);
        if (left > right)
        {
            intersection = Zero;
            return false;
        }

        intersection = WithBounds(left, right);
        return true;
    }

    /// <summary>
    /// Returns the convex envelope spanning both segments in the left operand's direction/support frame.
    /// This is not a split-preserving boolean OR; use Boolean(...) for that.
    /// </summary>
    public Axis Envelope(Axis other) =>
        WithBounds(
            Proportion.Min(LeftCoordinate, other.LeftCoordinate),
            Proportion.Max(RightCoordinate, other.RightCoordinate));

    public Axis Union(Axis other) => Envelope(other);

    public bool Contains(Scalar value) => Contains(value.AsProportion());

    public bool Contains(Proportion value) => value >= LeftCoordinate && value <= RightCoordinate;

    public AxisBooleanResult Boolean(
        Axis other,
        AxisBooleanOperation operation,
        Axis? frame = null) =>
        AxisBooleanProjection.Resolve(this, other, operation, frame);

    public BoundaryContinuationResult Continue(Scalar value, BoundaryContinuationLaw law) =>
        BoundaryContinuation.Continue(this, value, law);

    public BoundaryContinuationResult Continue(Scalar value, BoundaryPinPair? boundaryPins) =>
        BoundaryContinuation.Continue(this, value, boundaryPins);

    public BoundaryContinuationResult Continue(Proportion value, BoundaryContinuationLaw law) =>
        BoundaryContinuation.Continue(this, value, law);

    public BoundaryContinuationResult Continue(Proportion value, BoundaryPinPair? boundaryPins) =>
        BoundaryContinuation.Continue(this, value, boundaryPins);

    public static Axis operator +(Axis left, Axis right) =>
        new(left.Recessive + right.Recessive, left.Dominant + right.Dominant, left.Basis);

    public static Axis operator -(Axis value) =>
        new(-value.Recessive, -value.Dominant, value.Basis);

    /// <summary>
    /// Right action: the left operand is the current line-state, the right operand is transform data.
    /// </summary>
    public static Axis operator *(Axis state, Axis transform)
    {
        var result = state.Table.Multiply((state.Recessive, state.Dominant), (transform.Recessive, transform.Dominant));
        return FromPair(result, state.Basis);
    }

    public Proportion Fold() => Recessive * Dominant;

    public override string ToString() => $"[{Recessive}]i + [{Dominant}]";

    private sealed class AxisArithmetic : IArithmetic<Axis>
    {
        public Axis Zero => Axis.Zero;
        public Axis Add(Axis left, Axis right) => left + right;
        public Axis Multiply(Axis left, Axis right) => left * right;
        public Axis Negate(Axis value) => -value;
    }
}
