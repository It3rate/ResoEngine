namespace ResoEngine.Core2.Support;

/// <summary>
/// Generic 2-slot algebra table used by Proportion, Axis, and Area.
/// Slot 0 is the recessive/opposed slot (encoded with i in dominant notation).
/// Slot 1 is the dominant/identity slot.
/// </summary>
public sealed class AlgebraTable<T>
{
    private static readonly AlgebraEntry[] OppositionEntries =
    [
        new AlgebraEntry(0, 0, 1, -1),
        new AlgebraEntry(0, 1, 0, +1),
        new AlgebraEntry(1, 0, 0, +1),
        new AlgebraEntry(1, 1, 1, +1),
    ];

    private readonly IArithmetic<T> _arithmetic;

    public AlgebraTable(IArithmetic<T> arithmetic, IReadOnlyList<AlgebraEntry>? entries = null)
    {
        _arithmetic = arithmetic;
        Entries = entries ?? OppositionEntries;
    }

    public IReadOnlyList<AlgebraEntry> Entries { get; }

    public (T Recessive, T Dominant) ApplyOpposition(T recessive, T dominant) =>
        (dominant, _arithmetic.Negate(recessive));

    public (T Recessive, T Dominant) Multiply(
        (T Recessive, T Dominant) left,
        (T Recessive, T Dominant) right)
    {
        T[] result = [_arithmetic.Zero, _arithmetic.Zero];
        T[] leftValues = [left.Recessive, left.Dominant];
        T[] rightValues = [right.Recessive, right.Dominant];

        foreach (var entry in Entries)
        {
            T product = _arithmetic.Multiply(leftValues[entry.LeftIndex], rightValues[entry.RightIndex]);
            if (entry.Sign < 0)
            {
                product = _arithmetic.Negate(product);
            }

            result[entry.ResultIndex] = _arithmetic.Add(result[entry.ResultIndex], product);
        }

        return (result[0], result[1]);
    }
}
