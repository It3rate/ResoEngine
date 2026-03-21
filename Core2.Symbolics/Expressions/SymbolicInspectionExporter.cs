using System.Text;

namespace Core2.Symbolics.Expressions;

public static class SymbolicInspectionExporter
{
    public static string Export(SymbolicInspectionReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var builder = new StringBuilder();
        builder.AppendLine("EXPRESSION");
        builder.AppendLine(report.SourceText);
        builder.AppendLine();

        if (!string.IsNullOrWhiteSpace(report.StructuralContextName))
        {
            builder.AppendLine("STRUCTURAL CONTEXT");
            builder.AppendLine(report.StructuralContextName);
            if (!string.IsNullOrWhiteSpace(report.StructuralContextSummary))
            {
                builder.AppendLine(report.StructuralContextSummary);
            }

            builder.AppendLine();
        }

        if (report.HasError)
        {
            builder.AppendLine("ERROR");
            builder.AppendLine(report.Error ?? "Unknown symbolic inspection error.");
            return builder.ToString().TrimEnd();
        }

        if (report.Parsed is not null)
        {
            builder.AppendLine("PARSED");
            builder.AppendLine(SymbolicTermFormatter.Format(report.Parsed));
            builder.AppendLine();

            builder.AppendLine("CANONICAL");
            builder.AppendLine(CanonicalSymbolicSerializer.Serialize(report.Parsed));
            builder.AppendLine();
        }

        if (report.FinalStep?.Reduction.Output is not null)
        {
            builder.AppendLine("FINAL");
            builder.AppendLine(SymbolicTermFormatter.Format(report.FinalStep.Reduction.Output));
            builder.AppendLine();
        }

        foreach (var step in report.Steps)
        {
            builder.AppendLine($"STEP {step.Index}: {step.Label}");
            builder.AppendLine($"source: {SymbolicTermFormatter.Format(step.Source)}");
            builder.AppendLine($"elaborated: {FormatOptional(step.Elaboration.Output)}");
            builder.AppendLine($"reduced: {FormatOptional(step.Reduction.Output)}");

            if (step.Evaluation is not null)
            {
                builder.AppendLine(
                    $"evaluation: items={step.Evaluation.Items.Count}, satisfied-pref={step.Evaluation.SatisfiedPreferenceWeight}, unsatisfied-pref={step.Evaluation.UnsatisfiedPreferenceWeight}, unresolved-pref={step.Evaluation.UnresolvedPreferenceWeight}");

                foreach (var item in step.Evaluation.Items)
                {
                    builder.AppendLine(
                        $"  item: {item.Truth}, participant={item.ParticipantName ?? "(unscoped)"}, relation={SymbolicTermFormatter.Format(item.Reduced)}{FormatNote(item.Note)}");
                }
            }

            if (step.Negotiation is not null)
            {
                builder.AppendLine($"negotiation: {step.Negotiation.Status}");
                if (step.Negotiation.SelectedCandidate is not null)
                {
                    builder.AppendLine($"  selected: {SymbolicTermFormatter.Format(step.Negotiation.SelectedCandidate)}");
                }

                if (step.Negotiation.PreservedCandidateFamily is not null)
                {
                    builder.AppendLine($"  preserved: {SymbolicTermFormatter.Format(step.Negotiation.PreservedCandidateFamily)}");
                }

                if (!string.IsNullOrWhiteSpace(step.Negotiation.Note))
                {
                    builder.AppendLine($"  note: {step.Negotiation.Note}");
                }
            }

            builder.AppendLine();
        }

        builder.AppendLine("ENVIRONMENT");
        var scopeTree = report.FinalEnvironment.GetScopeTree();
        if (scopeTree.DirectBindings.Count == 0 && scopeTree.Children.Count == 0)
        {
            builder.AppendLine("(no bindings)");
        }
        else
        {
            AppendScope(builder, scopeTree, string.Empty);
        }

        return builder.ToString().TrimEnd();
    }

    private static string FormatOptional(SymbolicTerm? term) =>
        term is null ? "(none)" : SymbolicTermFormatter.Format(term);

    private static string FormatNote(string? note) =>
        string.IsNullOrWhiteSpace(note) ? string.Empty : $", note={note}";

    private static void AppendScope(StringBuilder builder, SymbolicEnvironmentScope scope, string indent)
    {
        foreach (var binding in scope.DirectBindings)
        {
            builder.AppendLine($"{indent}{binding.Key} = {SymbolicTermFormatter.Format(binding.Value)}");
        }

        foreach (var child in scope.Children)
        {
            builder.AppendLine($"{indent}{child.Name}:");
            AppendScope(builder, child, indent + "  ");
        }
    }
}
