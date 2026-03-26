namespace Core3.Engine;

/// <summary>
/// Minimal fold helpers for the independent grade engine.
/// The helpers do not store fold classifications; they derive them from the
/// nested numbers and ordered child structure each time.
/// </summary>
public static class EngineFolding
{
    public static GradedElement Inbound(EnginePin pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        return pin.Inbound;
    }

    public static GradedElement Outbound(EnginePin pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        return pin.Outbound;
    }

    public static bool SharesUnitSpace(EnginePin pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        return pin.SharesUnitSpace;
    }

    public static GradedElement? Add(EnginePin pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        return pin.Add();
    }

    public static CompositeElement Multiply(EnginePin pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        return pin.Contrast();
    }

    public static bool MultiplyRequiresLift(EnginePin pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        return pin.MultiplyRequiresLift();
    }

    public static bool CanBoolean(EnginePin pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        return pin.HasResolvedUnits;
    }
}
