namespace ResoEngine.Core2;

/// <summary>
/// Neutral element in the recursive degree ladder.
/// Elements do not inherently act as values or frames; those roles arise through relation.
/// </summary>
public interface IElement
{
    int Degree { get; }
}
