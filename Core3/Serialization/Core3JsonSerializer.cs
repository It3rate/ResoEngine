using Core3.Binding;
using Core3.Engine;
using Core3.Operations;
using Core3.Runtime;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace Core3.Serialization;

/// <summary>
/// Manual JSON serialization for the current Core3 surface. This keeps
/// serialization logic explicit and local instead of spreading attributes and
/// policy through the engine and runtime types while the shapes are still
/// evolving.
/// </summary>
public static class Core3JsonSerializer
{
    public static string Serialize(GradedElement element, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, element, Resolve(options)), options);

    public static string Serialize(EngineElementOutcome outcome, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, outcome, Resolve(options)), options);

    public static string Serialize(EngineElementPairOutcome outcome, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, outcome, Resolve(options)), options);

    public static string Serialize(EnginePin pin, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, pin, Resolve(options)), options);

    public static string Serialize(EngineHostedPinResult result, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, result, Resolve(options)), options);

    public static string Serialize(EngineView view, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, view, Resolve(options)), options);

    public static string Serialize(OperationContext context, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, context, Resolve(options)), options);

    public static string Serialize(PieceArcResult result, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, result, Resolve(options)), options);

    public static string Serialize(Family family, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, family, Resolve(options)), options);

    public static string Serialize(OperationResult result, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, result, Resolve(options)), options);

    public static string Serialize(BoundTemplate template, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, template, Resolve(options)), options);

    public static string Serialize(OperationAttachment attachment, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, attachment, Resolve(options)), options);

    public static string Serialize(BindingSelector selector, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, selector, Resolve(options)), options);

    public static string Serialize(BindingProjection projection, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, projection, Resolve(options)), options);

    public static string Serialize(BindingTransform transform, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, transform, Resolve(options)), options);

    public static string Serialize(BindingSignal signal, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, signal, Resolve(options)), options);

    public static string Serialize(BindingAddress address) =>
        Serialize(writer => Write(writer, address), Core3JsonSerializerOptions.Default);

    public static string Serialize(BindingStorageTarget target) =>
        Serialize(writer => Write(writer, target), Core3JsonSerializerOptions.Default);

    public static string Serialize(OperationSite site, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, site, Resolve(options)), options);

    public static string Serialize(TraversalMover mover, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, mover, Resolve(options)), options);

    public static string Serialize(TraversalMachineDefinition machine, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, machine, Resolve(options)), options);

    public static string Serialize(TraversalRuntimeState state, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, state, Resolve(options)), options);

    public static string Serialize(TraversalStepResult result, Core3JsonSerializerOptions? options = null) =>
        Serialize(writer => Write(writer, result, Resolve(options)), options);

    public static void Write(Utf8JsonWriter writer, GradedElement element, Core3JsonSerializerOptions? options = null)
    {
        switch (element)
        {
            case AtomicElement atomic:
                writer.WriteStartObject();
                writer.WriteString("kind", "atomic");
                writer.WriteNumber("grade", atomic.Grade);
                writer.WriteNumber("value", atomic.Value);
                writer.WriteNumber("unit", atomic.Unit);
                writer.WriteEndObject();
                return;

            case CompositeElement composite:
                writer.WriteStartObject();
                writer.WriteString("kind", "composite");
                writer.WriteNumber("grade", composite.Grade);
                writer.WritePropertyName("recessive");
                Write(writer, composite.Recessive, options);
                writer.WritePropertyName("dominant");
                Write(writer, composite.Dominant, options);
                writer.WriteEndObject();
                return;

            default:
                throw new InvalidOperationException($"Unsupported graded element type: {element.GetType().Name}.");
        }
    }

    public static void Write(Utf8JsonWriter writer, EngineElementOutcome outcome, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "elementOutcome");
        writer.WriteBoolean("isExact", outcome.IsExact);
        writer.WritePropertyName("result");
        Write(writer, outcome.Result, actual);

        if (outcome.Tension is not null)
        {
            writer.WritePropertyName("tension");
            Write(writer, outcome.Tension, actual);
        }

        if (!string.IsNullOrWhiteSpace(outcome.Note))
        {
            writer.WriteString("note", outcome.Note);
        }

        if (actual.IncludeDerived)
        {
            writer.WriteNumber("survivorCount", outcome.SurvivorCount);
            writer.WritePropertyName("outboundResults");
            WriteElements(writer, outcome.OutboundResults, actual);

            if (outcome.TryGetRawPair(out var rawPair) &&
                rawPair is not null)
            {
                writer.WritePropertyName("rawPair");
                Write(writer, rawPair, actual);
            }
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, EngineElementPairOutcome outcome, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "elementPairOutcome");
        writer.WriteBoolean("isExact", outcome.IsExact);
        writer.WritePropertyName("left");
        Write(writer, outcome.Left, actual);
        writer.WritePropertyName("right");
        Write(writer, outcome.Right, actual);

        if (outcome.Tension is not null)
        {
            writer.WritePropertyName("tension");
            Write(writer, outcome.Tension, actual);
        }

        if (!string.IsNullOrWhiteSpace(outcome.Note))
        {
            writer.WriteString("note", outcome.Note);
        }

        if (actual.IncludeDerived)
        {
            writer.WriteNumber("survivorCount", outcome.SurvivorCount);
            writer.WritePropertyName("outboundResults");
            WriteElements(writer, outcome.OutboundResults, actual);
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, EnginePin pin, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "pin");
        writer.WriteNumber("grade", pin.Grade);

        if (pin.Host is not null)
        {
            writer.WritePropertyName("host");
            Write(writer, pin.Host, actual);
        }

        if (pin.PinPosition is not null)
        {
            writer.WritePropertyName("pinPosition");
            Write(writer, pin.PinPosition, actual);
        }

        if (pin.ResolvedPosition is not null)
        {
            writer.WritePropertyName("resolvedPosition");
            Write(writer, pin.ResolvedPosition, actual);
        }

        writer.WritePropertyName("inbound");
        Write(writer, pin.Inbound, actual);
        writer.WritePropertyName("outbound");
        Write(writer, pin.Outbound, actual);

        if (actual.IncludeDerived)
        {
            writer.WriteBoolean("hasResolvedUnits", pin.HasResolvedUnits);
            writer.WriteBoolean("sharesUnitSpace", pin.SharesUnitSpace);
            writer.WriteBoolean("hasContrastSpace", pin.HasContrastSpace);

            if (pin.DeclaredSpan is not null)
            {
                writer.WritePropertyName("declaredSpan");
                Write(writer, pin.DeclaredSpan, actual);
            }

            if (pin.InboundTension is not null)
            {
                writer.WritePropertyName("inboundTension");
                Write(writer, pin.InboundTension, actual);
            }

            if (pin.OutboundTension is not null)
            {
                writer.WritePropertyName("outboundTension");
                Write(writer, pin.OutboundTension, actual);
            }
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, EngineHostedPinResult result, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "hostedPinResult");
        if (!result.IsExact)
        {
            writer.WriteBoolean("isExact", false);
        }
        writer.WritePropertyName("host");
        Write(writer, result.Host, actual);
        writer.WritePropertyName("requestedPosition");
        Write(writer, result.RequestedPosition, actual);
        writer.WritePropertyName("resolvedPosition");
        Write(writer, result.ResolvedPosition, actual);
        writer.WritePropertyName("inbound");
        Write(writer, result.Inbound, actual);
        writer.WritePropertyName("outbound");
        Write(writer, result.Outbound, actual);

        if (result.Tension is not null)
        {
            writer.WritePropertyName("tension");
            Write(writer, result.Tension, actual);
        }

        if (!string.IsNullOrWhiteSpace(result.Note))
        {
            writer.WriteString("note", result.Note);
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, EngineView view, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "view");
        writer.WritePropertyName("frame");
        Write(writer, view.Frame, actual);
        writer.WritePropertyName("subject");
        Write(writer, view.Subject, actual);

        if (actual.IncludeDerived)
        {
            writer.WritePropertyName("calibration");
            Write(writer, view.Calibration, actual);
            writer.WritePropertyName("existingReadout");
            Write(writer, view.ExistingReadout, actual);

            var readOutcome = view.Read();

            if (readOutcome.IsExact)
            {
                writer.WritePropertyName("read");
                Write(writer, readOutcome.Result, actual);
            }
            else
            {
                writer.WritePropertyName("readOutcome");
                Write(writer, readOutcome, actual);
            }
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, OperationContext context, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "operationContext");
        writer.WriteBoolean("isOrdered", context.IsOrdered);
        writer.WritePropertyName("frame");
        Write(writer, context.Frame, actual);
        writer.WritePropertyName("members");
        WriteElements(writer, context.Members, actual);

        if (context.ParentContext is not null)
        {
            writer.WritePropertyName("parentContext");
            Write(writer, context.ParentContext, actual);
        }

        if (context.ParentFocusIndex is int focusIndex)
        {
            writer.WriteNumber("parentFocusIndex", focusIndex);
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, Family family, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "family");
        writer.WriteBoolean("isOrdered", family.IsOrdered);
        writer.WritePropertyName("frame");
        Write(writer, family.Frame, actual);
        writer.WritePropertyName("members");
        WriteElements(writer, family.Members, actual);

        if (family.ParentFamily is not null)
        {
            writer.WritePropertyName("parentFamily");
            Write(writer, family.ParentFamily, actual);
        }

        if (family.ParentFocusIndex is int focusIndex)
        {
            writer.WriteNumber("parentFocusIndex", focusIndex);
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, PieceArcResult result, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "pieceArcResult");
        if (!result.IsExact)
        {
            writer.WriteBoolean("isExact", false);
        }
        writer.WritePropertyName("context");
        Write(writer, result.Context, actual);
        writer.WriteString("originLawName", result.OriginLawName);
        writer.WritePropertyName("pieces");
        WritePieces(writer, result.Pieces, actual);

        if (result.Tension is not null)
        {
            writer.WritePropertyName("tension");
            Write(writer, result.Tension, actual);
        }

        if (!string.IsNullOrWhiteSpace(result.Note))
        {
            writer.WriteString("note", result.Note);
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, OperationResult result, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "operationResult");
        if (!result.IsExact)
        {
            writer.WriteBoolean("isExact", false);
        }
        writer.WriteString("operationName", result.OperationName);
        writer.WritePropertyName("context");
        Write(writer, result.Context, actual);
        writer.WritePropertyName("result");
        Write(writer, result.Result, actual);
        writer.WritePropertyName("resultFrame");
        Write(writer, result.ResultFrame, actual);

        if (result.Tension is not null)
        {
            writer.WritePropertyName("tension");
            Write(writer, result.Tension, actual);
        }

        if (!string.IsNullOrWhiteSpace(result.Note))
        {
            writer.WriteString("note", result.Note);
        }

        if (actual.IncludeDerived)
        {
            WriteArcDerived(writer, result, actual);

            if (result.PreservedStructure is not null)
            {
                writer.WritePropertyName("preservedStructure");
                Write(writer, result.PreservedStructure, actual);
            }
        }

        if (actual.IncludeDerived &&
            result.TryReadResult(out var read) &&
            read is not null)
        {
            writer.WritePropertyName("readResult");
            Write(writer, read, actual);
            writer.WritePropertyName("resultBoundaryAxis");
            Write(writer, result.GetResultBoundaryAxis(), actual);
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, BoundTemplate template, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        switch (template)
        {
            case BoundScalarTemplate scalar:
                writer.WriteStartObject();
                writer.WriteString("kind", "boundScalarTemplate");
                writer.WriteString("materialization", scalar.Materialization.ToString());
                writer.WritePropertyName("value");
                Write(writer, scalar.Value, actual);
                writer.WritePropertyName("unit");
                Write(writer, scalar.Unit, actual);
                writer.WritePropertyName("constraints");
                WriteConstraints(writer, scalar.Constraints, actual);
                writer.WriteEndObject();
                return;

            case BoundCompositeTemplate composite:
                writer.WriteStartObject();
                writer.WriteString("kind", "boundCompositeTemplate");
                writer.WriteString("materialization", composite.Materialization.ToString());
                writer.WritePropertyName("recessive");
                Write(writer, composite.Recessive, actual);
                writer.WritePropertyName("dominant");
                Write(writer, composite.Dominant, actual);
                writer.WritePropertyName("constraints");
                WriteConstraints(writer, composite.Constraints, actual);
                writer.WriteEndObject();
                return;

            default:
                throw new InvalidOperationException($"Unsupported bound template type: {template.GetType().Name}.");
        }
    }

    public static void Write(Utf8JsonWriter writer, OperationAttachment attachment, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "operationAttachment");
        writer.WritePropertyName("site");
        Write(writer, attachment.Site, actual);
        writer.WritePropertyName("law");
        Write(writer, attachment.Law);
        writer.WritePropertyName("inputs");
        writer.WriteStartArray();
        foreach (var input in attachment.Inputs)
        {
            Write(writer, input, actual);
        }
        writer.WriteEndArray();
        writer.WritePropertyName("outputs");
        writer.WriteStartArray();
        foreach (var output in attachment.Outputs)
        {
            Write(writer, output, actual);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, TraversalMachineDefinition machine, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "traversalMachine");
        writer.WriteString("name", machine.Name);
        writer.WriteString("entrySiteName", machine.EntrySiteName);
        writer.WritePropertyName("mover");
        Write(writer, machine.Mover, actual);
        writer.WritePropertyName("registers");
        writer.WriteStartArray();
        foreach (var register in machine.Registers)
        {
            Write(writer, register, actual);
        }
        writer.WriteEndArray();
        writer.WritePropertyName("attachments");
        writer.WriteStartArray();
        foreach (var attachment in machine.Attachments)
        {
            Write(writer, attachment, actual);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, TraversalMover mover, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "traversalMover");
        writer.WriteString("name", mover.Name);
        writer.WritePropertyName("position");
        Write(writer, mover.Position, actual);

        if (actual.IncludeDerived)
        {
            writer.WriteNumber("currentTick", mover.CurrentTick);
            writer.WriteNumber("endTick", mover.EndTick);
            writer.WriteBoolean("isAtStop", mover.IsAtStop);
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, TraversalRuntimeState state, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "traversalRuntimeState");
        writer.WriteString("machineName", state.Machine.Name);
        if (!state.IsExact)
        {
            writer.WriteBoolean("isExact", false);
        }
        writer.WritePropertyName("mover");
        Write(writer, state.Mover, actual);
        writer.WritePropertyName("token");
        WriteNamedElements(writer, state.Token, actual);
        writer.WritePropertyName("context");
        WriteNamedElements(writer, state.Context, actual);
        writer.WritePropertyName("result");
        WriteNamedElements(writer, state.Result, actual);

        if (state.Tension is not null)
        {
            writer.WritePropertyName("tension");
            Write(writer, state.Tension, actual);
        }

        if (!string.IsNullOrWhiteSpace(state.Note))
        {
            writer.WriteString("note", state.Note);
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, TraversalStepResult result, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("kind", "traversalStepResult");
        writer.WritePropertyName("state");
        Write(writer, result.State, actual);
        writer.WritePropertyName("encounters");
        writer.WriteStartArray();
        foreach (var encounter in result.Encounters)
        {
            Write(writer, encounter, actual);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, BindingSelector selector, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("domain", selector.Domain.ToString());
        writer.WritePropertyName("address");
        Write(writer, selector.Address);
        writer.WritePropertyName("projection");
        Write(writer, selector.Projection, actual);
        if (selector.StoreTarget is not null)
        {
            writer.WritePropertyName("storeTarget");
            Write(writer, selector.StoreTarget);
        }
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, BindingProjection projection, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        if (!string.IsNullOrWhiteSpace(projection.Note))
        {
            writer.WriteString("note", projection.Note);
        }
        writer.WritePropertyName("signal");
        Write(writer, projection.Signal, actual);
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, BindingTransform transform, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        if (!string.IsNullOrWhiteSpace(transform.Note))
        {
            writer.WriteString("note", transform.Note);
        }
        writer.WritePropertyName("signal");
        Write(writer, transform.Signal, actual);
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, BindingSignal signal, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        if (!string.IsNullOrWhiteSpace(signal.Note))
        {
            writer.WriteString("note", signal.Note);
        }
        writer.WritePropertyName("value");
        Write(writer, signal.Value, actual);
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, BindingStorageTarget target)
    {
        writer.WriteStartObject();
        writer.WriteString("domain", target.Domain.ToString());
        if (!string.IsNullOrWhiteSpace(target.Name))
        {
            writer.WriteString("name", target.Name);
        }
        if (target.Index is int index)
        {
            writer.WriteNumber("index", index);
        }
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, BindingAddress address)
    {
        writer.WriteStartObject();

        switch (address)
        {
            case BindingAddress.Position position:
                writer.WriteString("kind", "position");
                writer.WritePropertyName("parameter");
                Write(writer, position.Parameter);
                break;

            case BindingAddress.Name name:
                writer.WriteString("kind", "name");
                writer.WriteString("value", name.Value);
                break;

            default:
                throw new InvalidOperationException($"Unsupported binding address type: {address.GetType().Name}.");
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, OperationSite site, Core3JsonSerializerOptions? options = null)
    {
        writer.WriteStartObject();
        writer.WriteString("kind", site.Kind.ToString());
        if (!string.IsNullOrWhiteSpace(site.Name))
        {
            writer.WriteString("name", site.Name);
        }
        if (site.Address is not null)
        {
            writer.WritePropertyName("address");
            Write(writer, site.Address);
        }
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, TraversalRegister register, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("name", register.Name);
        writer.WritePropertyName("template");
        Write(writer, register.Template, actual);
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, TraversalSiteEncounter encounter, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WritePropertyName("site");
        Write(writer, encounter.Attachment.Site, actual);
        writer.WritePropertyName("law");
        Write(writer, encounter.Attachment.Law);
        writer.WritePropertyName("inputs");
        WriteNamedElements(writer, encounter.Inputs, actual);

        writer.WritePropertyName("outputs");
        writer.WriteStartArray();
        foreach (var output in encounter.Outputs)
        {
            Write(writer, output, actual);
        }
        writer.WriteEndArray();

        if (encounter.Tension is not null)
        {
            writer.WritePropertyName("tension");
            Write(writer, encounter.Tension, actual);
        }

        if (!string.IsNullOrWhiteSpace(encounter.Note))
        {
            writer.WriteString("note", encounter.Note);
        }

        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, TraversalStoredValue output, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WritePropertyName("target");
        Write(writer, output.Target);
        writer.WritePropertyName("value");
        Write(writer, output.Value, actual);
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, OperationLawReference law)
    {
        writer.WriteStartObject();
        writer.WriteString("name", law.Name);
        if (!string.IsNullOrWhiteSpace(law.Variant))
        {
            writer.WriteString("variant", law.Variant);
        }
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, OperationInputBinding input, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("name", input.Name);
        writer.WriteString("materialization", input.Materialization.ToString());
        writer.WritePropertyName("selector");
        Write(writer, input.Selector, actual);
        writer.WriteEndObject();
    }

    public static void Write(Utf8JsonWriter writer, OperationOutputBinding output, Core3JsonSerializerOptions? options = null)
    {
        var actual = Resolve(options);

        writer.WriteStartObject();
        writer.WriteString("name", output.Name);
        writer.WritePropertyName("target");
        Write(writer, output.Target);
        writer.WritePropertyName("transform");
        Write(writer, output.Transform, actual);
        writer.WriteEndObject();
    }

    public static void Write(TextWriter writer, GradedElement element, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(element, options));
    }

    public static void Write(TextWriter writer, EngineElementOutcome outcome, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(outcome, options));
    }

    public static void Write(TextWriter writer, EngineElementPairOutcome outcome, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(outcome, options));
    }

    public static void Write(TextWriter writer, OperationContext context, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(context, options));
    }

    public static void Write(TextWriter writer, PieceArcResult result, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(result, options));
    }

    public static void Write(TextWriter writer, EnginePin pin, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(pin, options));
    }

    public static void Write(TextWriter writer, EngineHostedPinResult result, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(result, options));
    }

    public static void Write(TextWriter writer, EngineView view, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(view, options));
    }

    public static void Write(TextWriter writer, OperationResult result, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(result, options));
    }

    public static void Write(TextWriter writer, OperationAttachment attachment, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(attachment, options));
    }

    public static void Write(TextWriter writer, TraversalMachineDefinition machine, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(machine, options));
    }

    public static void Write(TextWriter writer, TraversalMover mover, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(mover, options));
    }

    public static void Write(TextWriter writer, TraversalRuntimeState state, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(state, options));
    }

    public static void Write(TextWriter writer, TraversalStepResult result, Core3JsonSerializerOptions? options = null)
    {
        writer.Write(Serialize(result, options));
    }

    private static void Write(Utf8JsonWriter writer, OperationPiece piece, Core3JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("result");
        Write(writer, piece.Result, options);
        writer.WritePropertyName("carrier");
        Write(writer, piece.Carrier, options);
        writer.WritePropertyName("sourceMemberIndices");
        writer.WriteStartArray();
        foreach (var index in piece.SourceMemberIndices)
        {
            writer.WriteNumberValue(index);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    private static void Write(Utf8JsonWriter writer, BoundSlot<long> slot, Core3JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (slot.Literal is long literal)
        {
            writer.WriteNumber("literal", literal);
        }
        if (slot.Binding is not null)
        {
            writer.WritePropertyName("binding");
            Write(writer, slot.Binding, options);
        }
        writer.WritePropertyName("transform");
        Write(writer, slot.Transform, options);
        writer.WriteEndObject();
    }

    private static void WriteConstraints(
        Utf8JsonWriter writer,
        IReadOnlyList<BindingConstraint> constraints,
        Core3JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var constraint in constraints)
        {
            writer.WriteStartObject();
            writer.WriteString("targetPath", constraint.TargetPath);
            writer.WriteString("materialization", constraint.Materialization.ToString());
            writer.WritePropertyName("source");
            Write(writer, constraint.Source, options);
            writer.WritePropertyName("transform");
            Write(writer, constraint.Transform, options);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }

    private static void WriteElements(
        Utf8JsonWriter writer,
        IReadOnlyList<GradedElement> elements,
        Core3JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var element in elements)
        {
            Write(writer, element, options);
        }
        writer.WriteEndArray();
    }

    private static void WritePieces(
        Utf8JsonWriter writer,
        IReadOnlyList<OperationPiece> pieces,
        Core3JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var piece in pieces)
        {
            Write(writer, piece, options);
        }
        writer.WriteEndArray();
    }

    private static void WriteArcDerived(
        Utf8JsonWriter writer,
        ArcResult result,
        Core3JsonSerializerOptions options)
    {
        writer.WriteString("originLawName", result.OriginLawName);
        writer.WritePropertyName("outboundPieces");
        WritePieces(writer, result.OutboundPieces, options);
    }

    private static void WriteNamedElements(
        Utf8JsonWriter writer,
        IReadOnlyDictionary<string, GradedElement> values,
        Core3JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var pair in values)
        {
            writer.WritePropertyName(pair.Key);
            Write(writer, pair.Value, options);
        }
        writer.WriteEndObject();
    }

    private static string Serialize(Action<Utf8JsonWriter> write, Core3JsonSerializerOptions? options)
    {
        var actual = Resolve(options);
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions { Indented = actual.Indented });
        write(writer);
        writer.Flush();
        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    private static Core3JsonSerializerOptions Resolve(Core3JsonSerializerOptions? options) =>
        options ?? Core3JsonSerializerOptions.Default;
}








