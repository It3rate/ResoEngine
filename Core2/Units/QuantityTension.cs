namespace Core2.Units;

public enum QuantityTensionKind
{
    SignatureMismatch,
    PreferredUnitMismatch,
    UnsupportedExponent,
}

public sealed record QuantityTension(
    QuantityTensionKind Kind,
    string Message);
