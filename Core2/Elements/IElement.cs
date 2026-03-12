namespace Core2.Elements;

/// <summary>
/// Neutral element in the recursive degree ladder.
/// Elements do not inherently act as values or frames; those roles arise through relation.
/// Operation activation is described through ElementOperationCatalog and the related extensions.
/// </summary>
public interface IElement
{
    int Degree { get; }
}
