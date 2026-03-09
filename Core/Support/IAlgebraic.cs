namespace ResoEngine.Support;

/// <summary>
/// A type whose components are elements of TElement, combined via algebra entries.
/// Proportion implements this over longs; Axis implements this over Proportions.
/// </summary>
public interface IAlgebraic<TElement>
{
    int Dims { get; }
    AlgebraEntry[] Algebra { get; }
    TElement GetElement(int index);
}
