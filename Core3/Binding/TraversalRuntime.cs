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
    string? Note = null) : IExactResult
{
    public bool IsExact => Tension is null;
}

/// <summary>
/// One encountered site firing together with the resolved input values that
/// were available there.
/// </summary>
public sealed record TraversalSiteEncounter(
    OperationAttachment Attachment,
    IReadOnlyDictionary<string, GradedElement> Inputs,
    IReadOnlyList<TraversalStoredValue> Outputs,
    GradedElement? Tension = null,
    string? Note = null) : IExactResult
{
    public string OriginLawName => Attachment.Law.Name;
    public bool IsExact => Tension is null;
    public bool HasAny => Outputs.Count > 0;
    public bool HasMany => Outputs.Count > 1;
}

/// <summary>
/// One concrete write performed during a traversal site firing.
/// </summary>
public sealed record TraversalStoredValue(
    BindingStorageTarget Target,
    GradedElement Value);

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
    private delegate void TraversalLawHandler(
        OperationAttachment attachment,
        IReadOnlyDictionary<string, GradedElement> inputs,
        EncounterState state);

    private static readonly IReadOnlyDictionary<string, TraversalLawHandler> LawHandlers =
        new Dictionary<string, TraversalLawHandler>(StringComparer.Ordinal)
        {
            ["Add"] = ApplyAdd,
            ["ContinueWhileNextMemberExists"] = ApplyContinueWhileNextMemberExists
        };

    public static TraversalRuntimeState CreateInitial(TraversalMachineDefinition machine)
    {
        var token = new Dictionary<string, GradedElement>(StringComparer.Ordinal);
        GradedElement? tension = null;
        string? note = null;

        foreach (var register in machine.Registers)
        {
            var outcome = Materialize(register.Template);

            token[register.Name] = outcome.Result;
            tension = EngineTension.CombineTension(tension, outcome.Tension);
            note = EngineTension.CombineNotes(note, outcome.Note);
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
        Family? family,
        out TraversalStepResult? result)
    {
        var encounters = new List<TraversalSiteEncounter>();
        var stepState = new StepState(
            state.Mover,
            new Dictionary<string, GradedElement>(state.Token, StringComparer.Ordinal),
            new Dictionary<string, GradedElement>(state.Context, StringComparer.Ordinal),
            new Dictionary<string, GradedElement>(state.Result, StringComparer.Ordinal),
            state.Tension,
            state.Note);

        foreach (var attachment in ResolveActiveAttachments(state.Machine))
        {
            var inputs = new Dictionary<string, GradedElement>(StringComparer.Ordinal);
            var encounterState = new EncounterState(stepState);

            foreach (var input in attachment.Inputs)
            {
                if (!TryResolveSelector(
                        stepState,
                        family,
                        input.Selector,
                        out var selected,
                        out var selectedTension,
                        out var selectedNote))
                {
                    encounterState.Merge(selectedTension, selectedNote);
                    continue;
                }

                encounterState.Merge(selectedTension, selectedNote);

                if (selected is null)
                {
                    continue;
                }

                inputs[input.Name] = selected;

                if (input.Selector.StoreTarget is not null)
                {
                    Store(input.Selector.StoreTarget, selected, encounterState);
                }
            }

            if (LawHandlers.TryGetValue(attachment.Law.Name, out var handler))
            {
                handler(attachment, inputs, encounterState);
            }
            else
            {
                encounterState.Merge(
                    null,
                    $"Traversal runtime has no handler for law '{attachment.Law.Name}'.");
            }

            encounters.Add(new TraversalSiteEncounter(
                attachment,
                inputs,
                encounterState.Outputs,
                encounterState.Tension,
                encounterState.Note));
        }

        result = new TraversalStepResult(
            new TraversalRuntimeState(
                state.Machine,
                stepState.Mover,
                stepState.Token,
                stepState.Context,
                stepState.Result,
                stepState.Tension,
                stepState.Note),
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
            EngineTension.CombineNotes(
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
        StepState state,
        Family? family,
        BindingSelector selector,
        out GradedElement? value,
        out GradedElement? tension,
        out string? note)
    {
        if (state.TryResolveNamed(selector.Domain, selector.Address, out value, out note))
        {
            tension = null;
            return true;
        }

        switch (selector.Domain)
        {
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
                note = note ?? $"Traversal selector domain '{selector.Domain}' is not yet supported by the minimal runtime.";
                return false;
        }
    }

    private static bool TryResolveFamilyRead(
        TraversalMover mover,
        Family? family,
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

        var outcome = family.Members[targetIndex].CommitToCalibration(family.Frame);
        value = outcome.Result;
        tension = outcome.Tension;
        note = outcome.Note;
        return true;
    }

    private static void ApplyAdd(
        OperationAttachment attachment,
        IReadOnlyDictionary<string, GradedElement> inputs,
        EncounterState state)
    {
        if (!inputs.TryGetValue("accumulator", out var left) ||
            !inputs.TryGetValue("currentItem", out var right))
        {
            state.Merge(null, "Traversal Add encountered missing inputs and left the token unchanged.");
            return;
        }

        var outcome = left.Add(right);
        state.Merge(outcome.Tension, outcome.Note);

        foreach (var outputBinding in attachment.Outputs)
        {
            Store(outputBinding.Target, outcome.Result, state);
        }
    }

    private static void ApplyContinueWhileNextMemberExists(
        OperationAttachment attachment,
        IReadOnlyDictionary<string, GradedElement> inputs,
        EncounterState state)
    {
        var route = inputs.ContainsKey("nextItem")
            ? new AtomicElement(1, 1)
            : new AtomicElement(0, 1);

        foreach (var outputBinding in attachment.Outputs)
        {
            Store(outputBinding.Target, route, state);
        }

        if (route.Value > 0 &&
            state.Mover.TryAdvance(out var advanced) &&
            advanced is not null)
        {
            state.Mover = advanced;
        }
    }

    private static void Store(
        BindingStorageTarget target,
        GradedElement value,
        EncounterState state) =>
        state.Store(target, value);

    private sealed class EncounterState(StepState step)
    {
        private readonly List<TraversalStoredValue> _outputs = [];

        public TraversalMover Mover
        {
            get => step.Mover;
            set => step.Mover = value;
        }

        public IReadOnlyList<TraversalStoredValue> Outputs => _outputs;
        public GradedElement? Tension { get; private set; }
        public string? Note { get; private set; }

        public void Merge(GradedElement? tension, string? note)
        {
            step.Tension = EngineTension.CombineTension(step.Tension, tension);
            step.Note = EngineTension.CombineNotes(step.Note, note);
            Tension = EngineTension.CombineTension(Tension, tension);
            Note = EngineTension.CombineNotes(Note, note);
        }

        public void Store(BindingStorageTarget target, GradedElement value)
        {
            step.Store(target, value);
            _outputs.Add(new TraversalStoredValue(target, value));
        }
    }

    private sealed class StepState(
        TraversalMover mover,
        Dictionary<string, GradedElement> token,
        Dictionary<string, GradedElement> context,
        Dictionary<string, GradedElement> result,
        GradedElement? tension,
        string? note)
    {
        public TraversalMover Mover { get; set; } = mover;
        public Dictionary<string, GradedElement> Token { get; } = token;
        public Dictionary<string, GradedElement> Context { get; } = context;
        public Dictionary<string, GradedElement> Result { get; } = result;
        public GradedElement? Tension { get; set; } = tension;
        public string? Note { get; set; } = note;

        public bool TryResolveNamed(
            BindingDomain domain,
            BindingAddress address,
            out GradedElement? value,
            out string? note)
        {
            if (address is not BindingAddress.Name name)
            {
                value = null;
                note = IsStoredDomain(domain)
                    ? "Traversal token/context/result lookup currently requires named addressing."
                    : null;
                return false;
            }

            var values = GetStoredDomain(domain);

            if (values is null)
            {
                value = null;
                note = null;
                return false;
            }

            values.TryGetValue(name.Value, out value);
            note = null;
            return true;
        }

        public void Store(BindingStorageTarget target, GradedElement value)
        {
            if (!string.IsNullOrWhiteSpace(target.Name) &&
                GetStoredDomain(target.Domain) is Dictionary<string, GradedElement> values)
            {
                values[target.Name] = value;
            }
        }

        private Dictionary<string, GradedElement>? GetStoredDomain(BindingDomain domain) =>
            domain switch
            {
                BindingDomain.Token => Token,
                BindingDomain.Context => Context,
                BindingDomain.Result => Result,
                _ => null
            };

        private static bool IsStoredDomain(BindingDomain domain) =>
            domain is BindingDomain.Token or BindingDomain.Context or BindingDomain.Result;
    }
}

