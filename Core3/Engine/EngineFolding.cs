namespace Core3.Engine;

/// <summary>
/// Minimal fold helpers for the independent grade engine.
/// The helpers do not store fold classifications; they derive them from the
/// nested numbers and ordered child structure each time.
/// </summary>
public static class EngineFolding
{
    public static GradedElement Inbound(EnginePin pin) => pin.Inbound;

    public static GradedElement Outbound(EnginePin pin) => pin.Outbound;

    public static bool SharesUnitSpace(EnginePin pin) => pin.SharesUnitSpace;

    public static GradedElement? ResolvedPosition(EnginePin pin) => pin.ResolvedPosition;

    public static GradedElement? DeclaredSpan(EnginePin pin) => pin.DeclaredSpan;

    public static GradedElement? InboundTension(EnginePin pin) => pin.InboundTension;

    public static GradedElement? OutboundTension(EnginePin pin) => pin.OutboundTension;

    public static GradedElement? Add(EnginePin pin) => pin.Add();

    public static GradedElement? Multiply(EnginePin pin) => pin.Multiply();

    public static bool MultiplyRequiresLift(EnginePin pin) => pin.MultiplyRequiresLift();

    public static bool CanBoolean(EnginePin pin) => pin.HasResolvedUnits;
}
