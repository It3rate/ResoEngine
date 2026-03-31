using Core3.Binding;
using Core3.Engine;
using Core3.Operations;

namespace Core3.Data;

/// <summary>
/// Reads a family's members through a frame. The frame's own structure determines
/// what kind of reading happens at each level — no mode enum needed.
///
/// The three reading layers, determined by unit sign in the frame's atomic leaves:
///
///   ALIGNED (positive unit)  → VALUE CALIBRATION
///     The frame shares a carrier with the data. Members are reexpressed
///     to the frame's resolution. This is measurement — turning raw elements
///     into calibrated values. The frame's unit gives virtual capacity.
///
///   ORTHOGONAL (negative unit) → STRUCTURAL ORGANIZATION
///     The frame imposes a dimension lift on the data. Members are organized
///     along the frame's axis — strides, grids, projections, neighbor relations.
///     This is the same gesture as pinning: locate things orthogonal to their nature.
///
///   UNRESOLVED (zero unit) → GENERATIVE (parked for now)
///     The relationship between frame and data demands evaluation.
///     Interpolation, fold-derived curves, emergent area. The law that
///     resolves this needs more design work before implementing.
///
/// A composite frame naturally mixes these. The recessive child might be
/// structural (organizing into columns) while the dominant calibrates values.
/// The view descends the frame tree and each leaf determines its own behavior.
/// </summary>
public class FamilyView
{
    public Family Family { get; }

    public FamilyView(Family family)
    {
        Family = family;
    }

    /// <summary>
    /// The primary read operation. Descends the frame to determine what to do:
    /// - Aligned frame → calibrate member values to frame's resolution
    /// - Orthogonal frame → interpret structurally (stride/projection)
    /// - Composite frame → recurse, each child determines its own layer
    /// </summary>
    public EngineElementOutcome ReadAtMover(TraversalMover mover)
    {
        var frame = Family.Frame;
        return ReadThroughFrame(frame, mover);
    }

    /// <summary>
    /// Parametric read via an AtomicElement (value/unit = fraction).
    /// e.g. AtomicElement(3, 10) means 30% through the family.
    /// </summary>
    public EngineElementOutcome ReadAtT(AtomicElement t)
    {
        var mover = new TraversalMover("t", t);
        return ReadAtMover(mover);
    }

    /// <summary>
    /// The frame-driven dispatch. Inspects the frame structure to decide
    /// how to read members. This is where the unit-sign semantics do their work.
    /// </summary>
    private EngineElementOutcome ReadThroughFrame(GradedElement frame, TraversalMover mover)
    {
        // Atomic frame: the unit sign tells us what kind of read this is.
        if (frame is AtomicElement atomicFrame)
        {
            if (atomicFrame.IsAlignedUnit)
            {
                // ALIGNED → VALUE CALIBRATION
                // The frame's resolution is the virtual capacity.
                // Map mover position into member space, calibrate result to frame's unit.
                return ReadValueCalibrated(atomicFrame, mover);
            }

            if (atomicFrame.IsOrthogonalUnit)
            {
                // ORTHOGONAL → STRUCTURAL ORGANIZATION
                // The frame's value is a stride width along this axis.
                // The mover position selects within that structural dimension.
                return ReadStructural(atomicFrame, mover);
            }

            // UNRESOLVED → GENERATIVE (parked)
            // For now, fall through to basic value read.
            // Eventually this is where interpolation/fold laws would be invoked.
            return ReadValueCalibrated(atomicFrame, mover);
        }

        // Composite frame: each child handles its own layer.
        // The recessive side organizes/calibrates, the dominant side reads/measures.
        // This mirrors EngineView where Recessive = calibration, Dominant = readout.
        if (frame is CompositeElement compositeFrame)
        {
            return ReadThroughCompositeFrame(compositeFrame, mover);
        }

        return EngineElementOutcome.WithTension(
            new AtomicElement(0, 0), frame,
            "Frame type not recognized.");
    }

    /// <summary>
    /// ALIGNED / VALUE CALIBRATION
    /// Frame has positive unit → members are values to be calibrated.
    /// The frame's resolution determines virtual capacity.
    /// Maps mover tick into member index space, reads (and optionally interpolates)
    /// the member at that position, then calibrates to the frame's unit.
    /// </summary>
    private EngineElementOutcome ReadValueCalibrated(AtomicElement frame, TraversalMover mover)
    {
        var count = Family.Count;
        if (count == 0)
            return EngineElementOutcome.WithTension(
                new AtomicElement(0, 0),
                new AtomicElement(0, 0),
                "Empty family.");

        if (count == 1)
            return Family.Members[0].CommitToCalibration(frame);

        // Map mover position into member index space
        var mapped = MapToMemberSpace(mover.CurrentTick, mover.EndTick, count);
        var lowerIndex = (int)mapped.whole;
        var upperIndex = Math.Min(lowerIndex + 1, count - 1);

        var member = Family.Members[lowerIndex];

        if (lowerIndex == upperIndex || mapped.remainderNum == 0)
        {
            // Exact member hit — calibrate to frame's resolution
            return member.CommitToCalibration(frame);
        }

        // Between two members — for now do nearest-neighbor when calibrating.
        // Full interpolation belongs to the generative layer (unresolved units).
        // But we still calibrate the selected member.
        var selected = (mapped.remainderNum * 2 < mapped.remainderDen)
            ? Family.Members[lowerIndex]
            : Family.Members[upperIndex];

        return selected.CommitToCalibration(frame);
    }

    /// <summary>
    /// ORTHOGONAL / STRUCTURAL ORGANIZATION
    /// Frame has negative unit → this axis organizes members into a dimension.
    /// The frame's absolute value is the stride width along this axis.
    /// The mover position selects a position within that stride.
    ///
    /// This is the same gesture as pinning: imposing an axis orthogonal
    /// to the data's own nature. A pin locates one element; a structural
    /// frame locates all members along an organizational dimension.
    /// </summary>
    private EngineElementOutcome ReadStructural(AtomicElement frame, TraversalMover mover)
    {
        var count = Family.Count;
        if (count == 0)
            return EngineElementOutcome.WithTension(
                new AtomicElement(0, 0),
                new AtomicElement(0, 0),
                "Empty family.");

        // The frame's resolution (absolute unit) is the stride width.
        var strideWidth = frame.Resolution;

        // Map mover into this dimension's range.
        // The mover navigates within the stride — position within the structural axis.
        var posInStride = mover.CurrentTick * strideWidth / mover.EndTick;
        posInStride = Math.Min(posInStride, strideWidth - 1);

        // Orthogonal unit sign means wrapping is natural for this dimension.
        // If position exceeds member count, wrap around.
        var memberIndex = posInStride % count;

        return EngineElementOutcome.Exact(Family.Members[(int)memberIndex]);
    }

    /// <summary>
    /// Composite frame: the recessive and dominant children each contribute
    /// their own reading layer. This mirrors EngineView's structure where
    /// Recessive = calibration/organization, Dominant = existing readout.
    ///
    /// In practice: the recessive side often organizes (structural) while
    /// the dominant side measures (value). But both can be anything —
    /// the unit signs at the leaves determine behavior, not the position.
    ///
    /// For multi-dimensional access: a grade-2 frame has 4 atomic leaves,
    /// each potentially a different axis. Two orthogonal leaves = 2D grid.
    /// One orthogonal + one aligned = structured in one axis, calibrated in another.
    /// </summary>
    private EngineElementOutcome ReadThroughCompositeFrame(
        CompositeElement compositeFrame,
        TraversalMover mover)
    {
        // For now: read each member through the composite frame as a whole,
        // using the existing CommitToCalibration which already recurses
        // through matching composite structure.
        //
        // The recessive side provides calibration/organization context.
        // The dominant side provides the existing measurement reference.
        //
        // A more complete version would decompose the mover into per-dimension
        // movers and dispatch to each child frame independently. That's the
        // FamilyStride behavior — but driven by the frame itself rather than
        // by a separate class.

        var count = Family.Count;
        if (count == 0)
            return EngineElementOutcome.WithTension(
                new AtomicElement(0, 0),
                new AtomicElement(0, 0),
                "Empty family.");

        // Map mover to member index
        var mapped = MapToMemberSpace(mover.CurrentTick, mover.EndTick, count);
        var index = (int)Math.Min(mapped.whole, count - 1);
        var member = Family.Members[index];

        // Read through the full composite frame.
        // CommitToCalibration already handles:
        //   - aligned children → reexpress values
        //   - orthogonal children → carrier contrast preserved as tension
        //   - unresolved children → tension preserved
        // The structural interpretation builds on top of this.
        return member.CommitToCalibration(compositeFrame);
    }

    /// <summary>
    /// Materialize: read every virtual position and collect into a new family.
    /// This is "baking" — turning virtual capacity into actual members.
    /// </summary>
    public Family Materialize(long virtualCapacity)
    {
        var result = new Family(Family.Frame);

        for (long tick = 0; tick <= virtualCapacity; tick++)
        {
            var mover = new TraversalMover("bake", new AtomicElement(tick, virtualCapacity));
            var outcome = ReadAtMover(mover);
            result.AddMember(outcome.Result);
        }

        return result;
    }

    /// <summary>
    /// Get structural neighbors of a member at the given index.
    /// Only meaningful when the frame has orthogonal (structural) components.
    /// Neighbors are the members adjacent along each structural axis.
    /// </summary>
    public List<GradedElement> GetNeighbors(int memberIndex)
    {
        var neighbors = new List<GradedElement>();
        var count = Family.Count;

        // Extract structural dimensions from the frame.
        // Each orthogonal atomic leaf in the frame tree = one structural axis.
        var strides = ExtractStructuralStrides(Family.Frame);

        if (strides.Count == 0)
        {
            // No structural axes — neighbors are just adjacent in flat order.
            if (memberIndex > 0)
                neighbors.Add(Family.Members[memberIndex - 1]);
            if (memberIndex < count - 1)
                neighbors.Add(Family.Members[memberIndex + 1]);
            return neighbors;
        }

        // Decompose flat index into multi-dimensional position
        var position = DecomposeIndex(memberIndex, strides);

        // Walk each structural dimension, step +/- 1
        for (var dim = 0; dim < strides.Count; dim++)
        {
            var width = strides[dim].Resolution;

            // Step backward — orthogonal means wrapping
            var back = position[dim] - 1;
            if (back >= 0)
            {
                var idx = RecomposeIndex(position, dim, back, strides);
                if (idx < count) neighbors.Add(Family.Members[(int)idx]);
            }
            else
            {
                // Wrap: orthogonal axes are naturally periodic
                var wrapped = width - 1;
                var idx = RecomposeIndex(position, dim, wrapped, strides);
                if (idx < count) neighbors.Add(Family.Members[(int)idx]);
            }

            // Step forward
            var forward = position[dim] + 1;
            if (forward < width)
            {
                var idx = RecomposeIndex(position, dim, forward, strides);
                if (idx < count) neighbors.Add(Family.Members[(int)idx]);
            }
            else
            {
                var idx = RecomposeIndex(position, dim, 0, strides);
                if (idx < count) neighbors.Add(Family.Members[(int)idx]);
            }
        }

        return neighbors;
    }

    // --- Helpers ---

    private static (long whole, long remainderNum, long remainderDen) MapToMemberSpace(
        long tick, long endTick, int count)
    {
        var maxIndex = count - 1;
        var scaledNumerator = tick * maxIndex;
        var whole = scaledNumerator / endTick;
        var remainderNum = scaledNumerator % endTick;
        return (whole, remainderNum, endTick);
    }

    /// <summary>
    /// Walk the frame tree and collect all orthogonal (structural) atomic leaves.
    /// Each one represents a stride dimension imposed on the data.
    /// </summary>
    private static List<AtomicElement> ExtractStructuralStrides(GradedElement frame)
    {
        var strides = new List<AtomicElement>();
        CollectOrthogonalLeaves(frame, strides);
        return strides;
    }

    private static void CollectOrthogonalLeaves(GradedElement element, List<AtomicElement> strides)
    {
        if (element is AtomicElement atomic)
        {
            // ORTHOGONAL leaf = structural dimension
            if (atomic.IsOrthogonalUnit)
                strides.Add(atomic);
            return;
        }

        if (element is CompositeElement composite)
        {
            CollectOrthogonalLeaves(composite.Recessive, strides);
            CollectOrthogonalLeaves(composite.Dominant, strides);
        }
    }

    private static long[] DecomposeIndex(int flatIndex, List<AtomicElement> strides)
    {
        var position = new long[strides.Count];
        long remaining = flatIndex;

        for (var dim = 0; dim < strides.Count; dim++)
        {
            var width = strides[dim].Resolution;
            position[dim] = remaining % width;
            remaining /= width;
        }

        return position;
    }

    private static long RecomposeIndex(long[] position, int changedDim, long newValue, List<AtomicElement> strides)
    {
        long flat = 0;
        long stride = 1;

        for (var dim = 0; dim < strides.Count; dim++)
        {
            var val = (dim == changedDim) ? newValue : position[dim];
            flat += val * stride;
            stride *= strides[dim].Resolution;
        }

        return flat;
    }
}
