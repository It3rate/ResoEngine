using System.Collections;
using Core2.Elements;

namespace Core2.Interpretation.Support;

public sealed record ElementGroup<TElement> : IReadOnlyList<TElement> where TElement : IElement
{
    private readonly TElement[] _members;

    public ElementGroup(IEnumerable<TElement> members)
    {
        ArgumentNullException.ThrowIfNull(members);

        _members = members.ToArray();
        Members = Array.AsReadOnly(_members);
    }

    public ElementGroup(params TElement[] members)
        : this((IEnumerable<TElement>)members)
    {
    }

    public IReadOnlyList<TElement> Members { get; }
    public int Count => _members.Length;
    public TElement this[int index] => _members[index];

    public bool Equals(ElementGroup<TElement>? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null || Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < _members.Length; index++)
        {
            if (!EqualityComparer<TElement>.Default.Equals(_members[index], other._members[index]))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var member in _members)
        {
            hash.Add(member);
        }

        return hash.ToHashCode();
    }

    public IEnumerator<TElement> GetEnumerator() =>
        ((IEnumerable<TElement>)_members).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static ElementGroup<TElement> Pair(TElement first, TElement second) =>
        new(first, second);
}
