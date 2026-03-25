namespace Core3.Engine;

/// <summary>
/// Minimal fold classifier for the independent grade engine.
/// This is a working model rather than a finished algebra.
/// </summary>
public static class EngineFolding
{
    public static NormalizedPin Normalize(EnginePin pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        return pin.Normalize();
    }

    public static PinSpaceKind ClassifySpace(EnginePin pin) => ClassifySpace(Normalize(pin));

    public static PinSpaceKind ClassifySpace(NormalizedPin pin)
    {
        ArgumentNullException.ThrowIfNull(pin);

        if (!pin.HasResolvedUnits)
        {
            return PinSpaceKind.Tension;
        }

        return pin.SharesUnitSpace
            ? PinSpaceKind.Additive
            : PinSpaceKind.Contrastive;
    }

    public static EngineFoldResult Resolve(EnginePin pin, EngineOperation requestedOperation)
    {
        ArgumentNullException.ThrowIfNull(pin);

        var normalized = Normalize(pin);
        var space = ClassifySpace(normalized);
        var supportsBoolean = space != PinSpaceKind.Tension;

        return requestedOperation switch
        {
            EngineOperation.Add => ResolveAdd(normalized, space, supportsBoolean),
            EngineOperation.Multiply => ResolveMultiply(normalized, space, supportsBoolean),
            EngineOperation.Boolean => ResolveBoolean(normalized, space, supportsBoolean),
            _ => throw new InvalidOperationException($"Unsupported operation {requestedOperation}."),
        };
    }

    public static bool TryAdd(GradedElement left, GradedElement right, out GradedElement? result)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        if (left.Signature != right.Signature)
        {
            result = null;
            return false;
        }

        if (left is AtomicElement leftAtom && right is AtomicElement rightAtom)
        {
            result = new AtomicElement(leftAtom.Value + rightAtom.Value, leftAtom.Unit);
            return true;
        }

        if (left is CompositeElement leftComposite && right is CompositeElement rightComposite &&
            TryAdd(leftComposite.Recessive, rightComposite.Recessive, out var recessive) &&
            TryAdd(leftComposite.Dominant, rightComposite.Dominant, out var dominant) &&
            recessive is not null &&
            dominant is not null)
        {
            result = new CompositeElement(recessive, dominant);
            return true;
        }

        result = null;
        return false;
    }

    private static EngineFoldResult ResolveAdd(
        NormalizedPin normalized,
        PinSpaceKind space,
        bool supportsBoolean)
    {
        if (space == PinSpaceKind.Additive &&
            TryAdd(normalized.Inbound, normalized.Outbound, out var sum) &&
            sum is not null)
        {
            return new EngineFoldResult(
                EngineOperation.Add,
                normalized,
                space,
                supportsBoolean,
                EngineFoldDisposition.Reduced,
                EngineLiftKind.None,
                sum,
                "Normalized children share one unit space, so additive folding is natural.");
        }

        return space switch
        {
            PinSpaceKind.Additive => new EngineFoldResult(
                EngineOperation.Add,
                normalized,
                space,
                supportsBoolean,
                EngineFoldDisposition.Preserved,
                EngineLiftKind.None,
                null,
                "The pair is add-compatible, but no reduction law exists for this structure yet."),

            PinSpaceKind.Contrastive => new EngineFoldResult(
                EngineOperation.Add,
                normalized,
                space,
                supportsBoolean,
                EngineFoldDisposition.Tension,
                EngineLiftKind.None,
                null,
                "Addition is not natural here because the normalized children remain in contrastive space."),

            _ => new EngineFoldResult(
                EngineOperation.Add,
                normalized,
                space,
                supportsBoolean,
                EngineFoldDisposition.Tension,
                EngineLiftKind.None,
                null,
                "Addition cannot proceed while unit relation remains unresolved."),
        };
    }

    private static EngineFoldResult ResolveMultiply(
        NormalizedPin normalized,
        PinSpaceKind space,
        bool supportsBoolean)
    {
        return space switch
        {
            PinSpaceKind.Contrastive => new EngineFoldResult(
                EngineOperation.Multiply,
                normalized,
                space,
                supportsBoolean,
                EngineFoldDisposition.Reduced,
                EngineLiftKind.None,
                new CompositeElement(normalized.Inbound, normalized.Outbound),
                "Contrastive space is the natural multiplicative fold in the current working model."),

            PinSpaceKind.Additive => new EngineFoldResult(
                EngineOperation.Multiply,
                normalized,
                space,
                supportsBoolean,
                EngineFoldDisposition.RequiresLift,
                EngineLiftKind.Sequence,
                null,
                "Same-space multiplication is not natural here. Add sequence or phase to separate the inputs first."),

            _ => new EngineFoldResult(
                EngineOperation.Multiply,
                normalized,
                space,
                supportsBoolean,
                EngineFoldDisposition.Tension,
                EngineLiftKind.None,
                null,
                "Multiplication cannot proceed while unit relation remains unresolved."),
        };
    }

    private static EngineFoldResult ResolveBoolean(
        NormalizedPin normalized,
        PinSpaceKind space,
        bool supportsBoolean)
    {
        if (supportsBoolean)
        {
            return new EngineFoldResult(
                EngineOperation.Boolean,
                normalized,
                space,
                supportsBoolean,
                EngineFoldDisposition.Preserved,
                EngineLiftKind.None,
                null,
                "Boolean folding remains available as a support/frame operation, but no explicit partition law is implemented yet.");
        }

        return new EngineFoldResult(
            EngineOperation.Boolean,
            normalized,
            space,
            supportsBoolean,
            EngineFoldDisposition.Tension,
            EngineLiftKind.None,
            null,
            "Boolean folding requires resolved comparable support.");
    }
}
