using System.Text;

namespace Core2.Symbolics.Expressions;

public static class SymbolicInspectionDisplayFormatter
{
    public static string Format(SymbolicInspectionReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var builder = new StringBuilder();
        builder.AppendLine($"Expression: {report.SourceText}");

        if (report.HasError)
        {
            builder.AppendLine();
            builder.AppendLine($"Error: {report.Error ?? "Unknown symbolic inspection error."}");
            return builder.ToString().TrimEnd();
        }

        if (report.Parsed is not null)
        {
            builder.AppendLine($"Parsed: {SymbolicTermFormatter.Format(report.Parsed)}");
            builder.AppendLine($"Canonical: {CanonicalSymbolicSerializer.Serialize(report.Parsed)}");
        }

        if (report.FinalStep?.Reduction.Output is not null)
        {
            builder.AppendLine($"Final: {SymbolicTermFormatter.Format(report.FinalStep.Reduction.Output)}");
        }

        if (report.Steps.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Steps:");
            foreach (var step in report.Steps)
            {
                builder.AppendLine($"  {step.Index}. {step.Label}");
                builder.AppendLine($"     source: {SymbolicTermFormatter.Format(step.Source)}");
                builder.AppendLine($"     elaborated: {FormatOptional(step.Elaboration.Output)}");
                builder.AppendLine($"     reduced: {FormatOptional(step.Reduction.Output)}");

                if (step.Evaluation is not null)
                {
                    builder.AppendLine(
                        $"     evaluation: items={step.Evaluation.Items.Count}, sat={step.Evaluation.SatisfiedPreferenceWeight}, unsat={step.Evaluation.UnsatisfiedPreferenceWeight}, unresolved={step.Evaluation.UnresolvedPreferenceWeight}");

                    foreach (var item in step.Evaluation.Items)
                    {
                        builder.AppendLine(
                            $"       - {item.Truth} [{item.ParticipantName ?? "(unscoped)"}] {SymbolicTermFormatter.Format(item.Reduced)}{FormatNote(item.Note)}");
                    }
                }

                if (step.Negotiation is not null)
                {
                    builder.AppendLine($"     negotiation: {step.Negotiation.Status}");
                    if (step.Negotiation.SelectedCandidate is not null)
                    {
                        builder.AppendLine($"       selected: {SymbolicTermFormatter.Format(step.Negotiation.SelectedCandidate)}");
                    }

                    if (step.Negotiation.PreservedCandidateFamily is not null)
                    {
                        builder.AppendLine($"       preserved: {SymbolicTermFormatter.Format(step.Negotiation.PreservedCandidateFamily)}");
                    }

                    if (!string.IsNullOrWhiteSpace(step.Negotiation.Note))
                    {
                        builder.AppendLine($"       note: {step.Negotiation.Note}");
                    }
                }
            }
        }

        builder.AppendLine();
        builder.AppendLine("Environment:");
        var bindings = report.FinalEnvironment.Bindings.OrderBy(pair => pair.Key, StringComparer.Ordinal).ToArray();
        if (bindings.Length == 0)
        {
            builder.AppendLine("  (no bindings)");
        }
        else
        {
            foreach (var binding in bindings)
            {
                builder.AppendLine($"  {binding.Key} = {SymbolicTermFormatter.Format(binding.Value)}");
            }
        }

        return builder.ToString().TrimEnd();
    }

    private static string FormatOptional(SymbolicTerm? term) =>
        term is null ? "(none)" : SymbolicTermFormatter.Format(term);

    private static string FormatNote(string? note) =>
        string.IsNullOrWhiteSpace(note) ? string.Empty : $" ({note})";
}
