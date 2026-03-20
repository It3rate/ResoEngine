using Core2.Elements;

namespace Core2.Interpretation.Support;

public enum ContainmentTensionKind
{
    OutsideExpectedRange,
    ResolutionMismatch,
    PlacementUnderspecified,
    UnsupportedInterpretation,
}

public readonly record struct ContainmentTension(
    ContainmentTensionKind Kind,
    string Path,
    string Message);

public readonly record struct ContainmentTensionMeasure(
    ContainmentTensionKind Kind,
    string Path,
    Proportion Amount,
    string Basis)
{
    public decimal Magnitude => Math.Abs((decimal)Amount.Fold());

    public override string ToString() =>
        string.IsNullOrWhiteSpace(Path)
            ? $"{Kind}: {Amount} ({Basis})"
            : $"{Kind} ({Path}): {Amount} ({Basis})";
}

public sealed class ContainmentTensionMetrics(IReadOnlyList<ContainmentTensionMeasure> measures)
{
    public static ContainmentTensionMetrics None { get; } = new([]);

    public IReadOnlyList<ContainmentTensionMeasure> Measures { get; } = measures;
    public bool HasAny => Measures.Count > 0;
    public decimal TotalMagnitude => Measures.Sum(measure => measure.Magnitude);

    public bool HasKind(ContainmentTensionKind kind) =>
        Measures.Any(measure => measure.Kind == kind);

    public IReadOnlyList<ContainmentTensionMeasure> OfKind(ContainmentTensionKind kind) =>
        Measures.Where(measure => measure.Kind == kind).ToArray();

    public ContainmentTensionMeasure? Find(ContainmentTensionKind kind, string path)
    {
        foreach (var measure in Measures)
        {
            if (measure.Kind == kind && measure.Path == path)
            {
                return measure;
            }
        }

        return null;
    }

    public Proportion? GetAmount(ContainmentTensionKind kind, string path) =>
        Find(kind, path)?.Amount;

    public ContainmentTensionMeasure? StartRange =>
        Find(ContainmentTensionKind.OutsideExpectedRange, "recessive.boundary");

    public ContainmentTensionMeasure? EndRange =>
        Find(ContainmentTensionKind.OutsideExpectedRange, "dominant.boundary");

    public ContainmentTensionMeasure? RecessiveSupport =>
        Find(ContainmentTensionKind.ResolutionMismatch, "recessive.support");

    public ContainmentTensionMeasure? DominantSupport =>
        Find(ContainmentTensionKind.ResolutionMismatch, "dominant.support");
}
