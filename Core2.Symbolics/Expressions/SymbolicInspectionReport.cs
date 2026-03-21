namespace Core2.Symbolics.Expressions;

public sealed record SymbolicInspectionReport(
    string SourceText,
    SymbolicEnvironment InitialEnvironment,
    SymbolicTerm? Parsed,
    IReadOnlyList<SymbolicInspectionStep> Steps,
    SymbolicEnvironment FinalEnvironment,
    string? Error,
    string? StructuralContextName = null,
    string? StructuralContextSummary = null)
{
    public bool HasError => !string.IsNullOrWhiteSpace(Error);
    public SymbolicInspectionStep? FinalStep => Steps.Count == 0 ? null : Steps[^1];
}
