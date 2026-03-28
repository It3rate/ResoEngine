namespace Core3.Engine;

/// <summary>
/// Four-bit binary truth table for boolean occupancy inside a common frame.
/// Bit order is:
/// 00 = neither
/// 01 = primary only
/// 10 = secondary only
/// 11 = both
/// where the numeric index is (inPrimary ? 1 : 0) | (inSecondary ? 2 : 0).
/// </summary>
public readonly record struct EngineBooleanOperation(byte TruthTable)
{
    public static EngineBooleanOperation False => new(0b0000);
    public static EngineBooleanOperation True => new(0b1111);
    public static EngineBooleanOperation TransferPrimary => new(0b1010);
    public static EngineBooleanOperation TransferSecondary => new(0b1100);
    public static EngineBooleanOperation And => new(0b1000);
    public static EngineBooleanOperation Or => new(0b1110);
    public static EngineBooleanOperation Nand => new(0b0111);
    public static EngineBooleanOperation Nor => new(0b0001);
    public static EngineBooleanOperation NotPrimary => new(0b0101);
    public static EngineBooleanOperation NotSecondary => new(0b0011);
    public static EngineBooleanOperation Implication => new(0b1101);
    public static EngineBooleanOperation ReverseImplication => new(0b1011);
    public static EngineBooleanOperation Inhibition => new(0b0010);
    public static EngineBooleanOperation ReverseInhibition => new(0b0100);
    public static EngineBooleanOperation Xor => new(0b0110);
    public static EngineBooleanOperation Xnor => new(0b1001);

    public bool Evaluate(bool inPrimary, bool inSecondary)
    {
        var index = (inPrimary ? 1 : 0) | (inSecondary ? 2 : 0);
        return (TruthTable & (1 << index)) != 0;
    }

    public override string ToString() =>
        TruthTable switch
        {
            0b0000 => nameof(False),
            0b1111 => nameof(True),
            0b1010 => nameof(TransferPrimary),
            0b1100 => nameof(TransferSecondary),
            0b1000 => nameof(And),
            0b1110 => nameof(Or),
            0b0111 => nameof(Nand),
            0b0001 => nameof(Nor),
            0b0101 => nameof(NotPrimary),
            0b0011 => nameof(NotSecondary),
            0b1101 => nameof(Implication),
            0b1011 => nameof(ReverseImplication),
            0b0010 => nameof(Inhibition),
            0b0100 => nameof(ReverseInhibition),
            0b0110 => nameof(Xor),
            0b1001 => nameof(Xnor),
            _ => $"0b{Convert.ToString(TruthTable, 2).PadLeft(4, '0')}"
        };
}
