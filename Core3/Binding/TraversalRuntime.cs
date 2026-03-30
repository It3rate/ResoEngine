using Core3.Engine;
using Core3.Operations;

namespace Core3.Binding;

/// <summary>
/// One lightweight runtime state for a traversal machine. This keeps the
/// current mover, token/context/result slots, and any held tension together in
/// one inspectable place without yet introducing a full execution engine.
/// </summary>
public sealed record TraversalRuntimeState(
    TraversalMachineDefinition Machine,
    TraversalMover Mover,
    IReadOnlyDictionary<string, GradedElement> Token,
    IReadOnlyDictionary<string, GradedElement> Context,
    IReadOnlyDictionary<string, GradedElement> Result,
    GradedElement? Tension = null,
    string? Note = null)
{
    public bool IsExact => Tension is null;
}

/// <summary>
/// One encountered site firing together with the resolved input values that
/// were available there.
/// </summary>
public sealed record TraversalSiteEncounter(
    OperationAttachment Attachment,
    IReadOnlyDictionary<string, GradedElement> Inputs);

/// <summary>
/// One small execution step over the current runtime state.
/// </summary>
public sealed record TraversalStepResult(
    TraversalRuntimeState State,
    IReadOnlyList<TraversalSiteEncounter> Encounters);

/// <summary>
/// Minimal traversal execution helpers over the current binding schema.
/// This pass intentionally supports only:
/// - literal register materialization
/// - mover-relative family reads
/// - token/context/result storage by name
/// - the small law surface already used in tests and examples
/// </summary>
public static class TraversalRuntime
{
    public static TraversalRuntimeState CreateInitial(TraversalMachineDefinition machine)
    {
        var token = new Dictionary<string, GradedElement>(StringComparer.Ordinal);
        GradedElement? tension = null;
        string? note = null;

        foreach (var register in machine.Registers)
        {
            var outcome = Materialize(register.Template);

            token[register.Name] = outcome.Result;
            tension = CombineTension(tension, outcome.Tension);
            note = CombineNotes(note, outcome.Note);
        }

        return new TraversalRuntimeState(
            machine,
            machine.Mover,
            token,
            new Dictionary<string, GradedElement>(StringComparer.Ordinal),
            new Dictionary<string, GradedElement>(StringComparer.Ordinal),
            tension,
            note);
    }

    public static bool TryStep(
        TraversalRuntimeState state,
        EngineFamily? family,
        out TraversalStepResult? result)
    {
        var token = new Dictionary<string, GradedElement>(state.Token, StringComparer.Ordinal);
        var context = new Dictionary<string, GradedElement>(state.Context, StringComparer.Ordinal);
        var output = new Dictionary<string, GradedElement>(state.Result, StringComparer.Ordinal);
        var encounters = new List<TraversalSiteEncounter>();
        var tension = state.Tension;
        var note = state.Note;
        var mover = state.Mover;

        foreach (var attachment in ResolveActiveAttachments(state.Machine))
        {
            var inputs = new Dictionary<string, GradedElement>(StringComparer.Ordinal);

            foreach (var input in attachment.Inputs)
            {
                if (!TryResolveSelector(
                        state,
                        family,
                        input.Selector,
                        out var selected,
                        out var selectedTension,
                        out var selectedNote))
                {
                    tension = CombineTension(tension, selectedTension);
                    note = CombineNotes(note, selectedNote);
                    continue;
                }

                tension = CombineTension(tension, selectedTension);
                note = CombineNotes(note, selectedNote);

                if (selected is null)
                {
                    continue;
                }

                inputs[input.Name] = selected;

                if (input.Selector.StoreTarget is not null)
                {
                    Store(input.Selector.StoreTarget, selected, token, context, output);
                }
            }

            encounters.Add(new TraversalSiteEncounter(attachment, inputs));

            switch (attachment.Law.Name)
            {
                case "Add":
                    ApplyAdd(attachment, inputs, token, context, output, ref tension, ref note);
                    break;

                case "ContinueWhileNextMemberExists":
                    ApplyContinueWhileNextMemberExists(
                        attachment,
                        inputs,
                        ref mover,
                        token,
                        context,
                        output);
                    break;
            }
        }

        result = new TraversalStepResult(
            new TraversalRuntimeState(
                state.Machine,
                mover,
                token,
                context,
                output,
                tension,
                note),
            encounters);
        return true;
    }

    private static EngineElementOutcome Materialize(BoundTemplate template) =>
        template switch
        {
            BoundScalarTemplate scalar => MaterializeScalar(scalar),
            BoundCompositeTemplate composite => MaterializeComposite(composite),
            _ => EngineElementOutcome.WithTension(
                new AtomicElement(0, 0),
                new AtomicElement(0, 0),
                "Traversal register materialization preserved unresolved structure because the template type was unsupported.")
        };

    private static EngineElementOutcome MaterializeScalar(BoundScalarTemplate scalar)
    {
        var value = scalar.Value.Literal ?? 0;
        var unit = scalar.Unit.Literal ?? 0;

        if (scalar.Value.Binding is null &&
            scalar.Unit.Binding is null &&
            scalar.Value.Literal is not null &&
            scalar.Unit.Literal is not null)
        {
            return EngineElementOutcome.Exact(new AtomicElement(value, unit));
        }

        return EngineElementOutcome.WithTension(
            new AtomicElement(value, unit),
            new AtomicElement(value, unit),
            "Traversal register materialization preserved unresolved structure because literal-only scalar materialization is the only current runtime form.");
    }

    private static EngineElementOutcome MaterializeComposite(BoundCompositeTemplate composite)
    {
        var recessiveOutcome = Materialize(composite.Recessive);
        var dominantOutcome = Materialize(composite.Dominant);
        var materialized = new CompositeElement(recessiveOutcome.Result, dominantOutcome.Result);

        if (recessiveOutcome.IsExact &&
            dominantOutcome.IsExact)
        {
            return EngineElementOutcome.Exact(materialized);
        }

        return EngineElementOutcome.WithTension(
            materialized,
            materialized,
            CombineNotes(
                "Traversal composite register materialization preserved child tension.",
                recessiveOutcome.Note,
                dominantOutcome.Note));
    }

    private static IEnumerable<OperationAttachment> ResolveActiveAttachments(
        TraversalMachineDefinition machine) =>
        machine.Attachments
            .Select((attachment, index) => (attachment, index, order: ResolveLocalStepOrder(attachment.Site.Address)))
            .OrderBy(item => item.order)
            .ThenBy(item => item.index)
            .Select(item => item.attachment);

    private static decimal ResolveLocalStepOrder(BindingAddress? address) =>
        address is BindingAddress.Position { Parameter: AtomicElement atomic } &&
        atomic.Unit > 0
            ? (decimal)atomic.Value / atomic.Unit
            : decimal.MaxValue;

    private static bool TryResolveSelector(
        TraversalRuntimeState state,
        EngineFamily? family,
        BindingSelector selector,
        out GradedElement? value,
        out GradedElement? tension,
        out string? note)
    {
        switch (selector.Domain)
        {
            case BindingDomain.Token:
                return TryResolveNamedOrIndexed(
                    state.Token,
                    selector.Address,
                    out value,
                    out tension,
                    out note);

            case BindingDomain.Context:
                return TryResolveNamedOrIndexed(
                    state.Context,
                    selector.Address,
                    out value,
                    out tension,
                    out note);

            case BindingDomain.Result:
                return TryResolveNamedOrIndexed(
                    state.Result,
                    selector.Address,
                    out value,
                    out tension,
                    out note);

            case BindingDomain.Frame:
                value = family?.Frame;
                tension = null;
                note = value is null
                    ? "Traversal frame binding had no active family frame."
                    : null;
                return true;

            case BindingDomain.Family:
                return TryResolveFamilyRead(
                    state.Mover,
                    family,
                    selector.Address,
                    out value,
                    out tension,
                    out note);

            default:
                value = null;
                tension = null;
                note = $"Traversal selector domain '{selector.Domain}' is not yet supported by the minimal runtime.";
                return false;
        }
    }

    private static bool TryResolveNamedOrIndexed(
        IReadOnlyDictionary<string, GradedElement> values,
        BindingAddress address,
        out GradedElement? value,
        out GradedElement? tension,
        out string? note)
    {
        switch (address)
        {
            case BindingAddress.Name name:
                values.TryGetValue(name.Value, out value);
                tension = null;
                note = value is null
                    ? null
                    : null;
                return true;

            default:
                value = null;
                tension = null;
                note = "Traversal token/context/result lookup currently requires named addressing.";
                return false;
        }
    }

    private static bool TryResolveFamilyRead(
        TraversalMover mover,
        EngineFamily? family,
        BindingAddress address,
        out GradedElement? value,
        out GradedElement? tension,
        out string? note)
    {
        if (family is null)
        {
            value = null;
            tension = null;
            note = "Traversal family binding had no active family.";
            return true;
        }

        if (address is not BindingAddress.Position position ||
            position.Parameter is not AtomicElement parameter ||
            parameter.Unit <= 0 ||
            parameter.Value % parameter.Unit != 0)
        {
            value = null;
            tension = family.Frame;
            note = "Traversal family binding currently requires integer mover-relative position parameters.";
            return false;
        }

        var targetIndex = checked((int)(mover.CurrentTick + (parameter.Value / parameter.Unit)));

        if (targetIndex < 0 || targetIndex >= family.Count)
        {
            value = null;
            tension = null;
            note = null;
            return true;
        }

        var outcome = family.Members[targetIndex].CommitToCalibrationWithTension(family.Frame);
        value = outcome.Result;
        tension = outcome.Tension;
        note = outcome.Note;
        return true;
    }

    private static void ApplyAdd(
        OperationAttachment attachment,
        IReadOnlyDictionary<string, GradedElement> inputs,
        IDictionary<string, GradedElement> token,
        IDictionary<string, GradedElement> context,
        IDictionary<string, GradedElement> result,
        ref GradedElement? tension,
        ref string? note)
    {
        if (!inputs.TryGetValue("accumulator", out var left) ||
            !inputs.TryGetValue("currentItem", out var right))
        {
            note = CombineNotes(note, "Traversal Add encountered missing inputs and left the token unchanged.");
            return;
        }

        var outcome = left.AddWithTension(right);
        tension = CombineTension(tension, outcome.Tension);
        note = CombineNotes(note, outcome.Note);

        foreach (var outputBinding in attachment.Outputs)
        {
            Store(outputBinding.Target, outcome.Result, token, context, result);
        }
    }

    private static void ApplyContinueWhileNextMemberExists(
        OperationAttachment attachment,
        IReadOnlyDictionary<string, GradedElement> inputs,
        ref TraversalMover mover,
        IDictionary<string, GradedElement> token,
        IDictionary<string, GradedElement> context,
        IDictionary<string, GradedElement> result)
    {
        var route = inputs.ContainsKey("nextItem")
            ? new AtomicElement(1, 1)
            : new AtomicElement(0, 1);

        foreach (var outputBinding in attachment.Outputs)
        {
            Store(outputBinding.Target, route, token, context, result);
        }

        if (route.Value > 0 &&
            mover.TryAdvance(out var advanced) &&
            advanced is not null)
        {
            mover = advanced;
        }
    }

    private static void Store(
        BindingStorageTarget target,
        GradedElement value,
        IDictionary<string, GradedElement> token,
        IDictionary<string, GradedElement> context,
        IDictionary<string, GradedElement> result)
    {
        switch (target.Domain)
        {
            case BindingDomain.Token when !string.IsNullOrWhiteSpace(target.Name):
                token[target.Name] = value;
                return;

            case BindingDomain.Context when !string.IsNullOrWhiteSpace(target.Name):
                context[target.Name] = value;
                return;

            case BindingDomain.Result when !string.IsNullOrWhiteSpace(target.Name):
                result[target.Name] = value;
                return;
        }
    }

    private static GradedElement? CombineTension(GradedElement? existing, GradedElement? next) =>
        existing ?? next;

    private static string? CombineNotes(params string?[] notes)
    {
        string? combined = null;

        foreach (var note in notes)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                continue;
            }

            combined = string.IsNullOrWhiteSpace(combined)
                ? note
                : combined == note
                    ? combined
                    : $"{combined} | {note}";
        }

        return combined;
    }
}
