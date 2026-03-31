namespace Core3.Operations;

/// <summary>
/// Four-bit binary truth table for boolean occupancy inside a common frame.
/// Bit order is:
/// 00 = neither
/// 01 = primary only
/// 10 = secondary only
/// 11 = both
/// where the numeric index is (inPrimary ? 1 : 0) | (inSecondary ? 2 : 0).
/// </summary>
public readonly record struct BooleanOperation(byte TruthTable)
{
    public static BooleanOperation False => new(0b0000);
    public static BooleanOperation True => new(0b1111);
    public static BooleanOperation TransferPrimary => new(0b1010);
    public static BooleanOperation TransferSecondary => new(0b1100);
    public static BooleanOperation And => new(0b1000);
    public static BooleanOperation Or => new(0b1110);
    public static BooleanOperation Nand => new(0b0111);
    public static BooleanOperation Nor => new(0b0001);
    public static BooleanOperation NotPrimary => new(0b0101);
    public static BooleanOperation NotSecondary => new(0b0011);
    public static BooleanOperation Implication => new(0b1101);
    public static BooleanOperation ReverseImplication => new(0b1011);
    public static BooleanOperation Inhibition => new(0b0010);
    public static BooleanOperation ReverseInhibition => new(0b0100);
    public static BooleanOperation Xor => new(0b0110);
    public static BooleanOperation Xnor => new(0b1001);

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

