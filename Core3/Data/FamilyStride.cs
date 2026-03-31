using Core3.Binding;
using Core3.Engine;
using Core3.Operations;

namespace Core3.Data;

/// <summary>
/// STRUCTURAL ORGANIZATION — driven by orthogonal frame axes.
///
/// This is a helper that builds structural frames for families.
/// Rather than being a separate "sampler" class, it constructs the
/// orthogonal frame that FamilyView then reads through.
///
/// The key idea: a stride IS a pin pattern. Pinning locates one element
/// at an origin orthogonal to its nature. A stride locates ALL members
/// along an organizational axis — the same gesture repeated as a pattern.
///
/// Orthogonal (negative) units in the frame = structural axes.
/// The absolute value of each orthogonal unit = the width of that dimension.
/// Multiple orthogonal leaves in a composite frame = multi-dimensional grid.
///
/// Examples:
///   AtomicElement(10, -10)  → 1D stride of width 10, wrapping
///   Composite(AE(5, -5), AE(4, -4)) → 2D grid, 5 columns × 4 rows
///   Composite(AE(3, -3), AE(0, 1)) → 3 structural columns, calibrated rows
/// </summary>
public static class FamilyStride
{
    /// <summary>
    /// Create a 1D structural frame: a single orthogonal axis.
    /// The family's members will be organized into groups of 'width'.
    /// Negative unit signals structural/organizational reading.
    /// </summary>
    public static AtomicElement CreateStride(long width)
    {
        // Negative unit = orthogonal = structural
        return new AtomicElement(width, -width);
    }

    /// <summary>
    /// Create a 2D structural frame (grid): two orthogonal axes composed
    /// into a grade-1 frame. Recessive = column stride, Dominant = row stride.
    /// </summary>
    public static CompositeElement CreateGrid(long columns, long rows)
    {
        // Both children are orthogonal → both are structural axes
        var columnStride = new AtomicElement(columns, -columns);
        var rowStride = new AtomicElement(rows, -rows);
        return new CompositeElement(columnStride, rowStride);
    }

    /// <summary>
    /// Create a mixed frame: structural columns with calibrated values.
    /// Recessive is orthogonal (organizes into columns).
    /// Dominant is aligned (calibrates member values to the given resolution).
    /// </summary>
    public static CompositeElement CreateStructuredWithCalibration(
        long strideWidth,
        long valueResolution)
    {
        // Orthogonal recessive = structural organization
        var structuralAxis = new AtomicElement(strideWidth, -strideWidth);
        // Aligned dominant = value calibration
        var calibrationAxis = new AtomicElement(0, valueResolution);
        return new CompositeElement(structuralAxis, calibrationAxis);
    }

    /// <summary>
    /// Create an N-dimensional structural frame by nesting composites.
    /// Each dimension width becomes an orthogonal leaf.
    /// [5, 4, 3] → ((5/-5 | 4/-4) | 3/-3) → 5×4×3 grid
    /// </summary>
    public static GradedElement CreateNDimensional(params long[] widths)
    {
        if (widths.Length == 0)
            return new AtomicElement(0, 0);

        if (widths.Length == 1)
            return CreateStride(widths[0]);

        // Build bottom-up: pair adjacent dimensions
        var elements = widths.Select(w => (GradedElement)CreateStride(w)).ToList();

        while (elements.Count > 1)
        {
            var paired = new List<GradedElement>();
            for (var i = 0; i < elements.Count; i += 2)
            {
                if (i + 1 < elements.Count)
                    paired.Add(new CompositeElement(elements[i], elements[i + 1]));
                else
                    paired.Add(elements[i]);
            }
            elements = paired;
        }

        return elements[0];
    }

    /// <summary>
    /// Apply a structural frame to a family, returning a new FamilyView
    /// that reads through structural organization.
    /// </summary>
    public static FamilyView ApplyTo(Family family, GradedElement structuralFrame)
    {
        var structuredFamily = new Family(structuralFrame);
        foreach (var member in family.Members)
            structuredFamily.AddMember(member);

        return new FamilyView(structuredFamily);
    }
}
