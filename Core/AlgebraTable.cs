using System;
using System.Collections.Generic;
using System.Linq;
using ResoEngine.Support;

namespace ResoEngine;

public class AlgebraTable<T>
{
    private readonly List<T> _elements;
    private readonly Func<T, T, T> _accumulator;

    public AlgebraTable(IEnumerable<T> elements, Func<T, T, T> accumulator)
    {
        _elements = elements.ToList();
        _accumulator = accumulator ?? throw new ArgumentNullException(nameof(accumulator));
    }

    public T Operate(int index1, int index2)
    {
        if (index1 < 0 || index1 >= _elements.Count)
            throw new ArgumentOutOfRangeException(nameof(index1));
        if (index2 < 0 || index2 >= _elements.Count)
            throw new ArgumentOutOfRangeException(nameof(index2));

        T input1 = _elements[index1];
        T input2 = _elements[index2];

        return _accumulator(input1, input2);
    }

    public T Operate(T input1, T input2)
    {
        return _accumulator(input1, input2);
    }

    public IReadOnlyList<T> Elements => _elements;

    /// <summary>
    /// Pin: create a table from an algebraic source's elements.
    /// The source's components become the table's elements.
    /// </summary>
    public static AlgebraTable<T> Pin(IAlgebraic<T> source, Func<T, T, T> accumulator)
    {
        var elements = Enumerable.Range(0, source.Dims).Select(i => source.GetElement(i));
        return new AlgebraTable<T>(elements, accumulator);
    }

    /// <summary>
    /// Add an element to the table (accumulate values into the pinned space).
    /// </summary>
    public void Add(T element) => _elements.Add(element);

    /// <summary>
    /// Fold: collapse all elements into one by applying the accumulator left-to-right.
    /// </summary>
    public T Fold()
    {
        if (_elements.Count == 0) throw new InvalidOperationException("No elements to fold.");
        return _elements.Aggregate(_accumulator);
    }

    /// <summary>
    /// Generic algebraic multiply: use algebra entries to combine elements from two IAlgebraic sources.
    /// </summary>
    public static T[] Multiply(IAlgebraic<T> a, IAlgebraic<T> b,
        Func<T, T, T> multiply, Func<T, T, T> add, Func<T, int, T> scale, T zero)
    {
        var result = new T[a.Dims];
        for (int i = 0; i < result.Length; i++) result[i] = zero;
        foreach (var entry in a.Algebra)
        {
            var product = scale(multiply(a.GetElement(entry.LeftIndex), b.GetElement(entry.RightIndex)), entry.Sign);
            result[entry.ResultIndex] = add(result[entry.ResultIndex], product);
        }
        return result;
    }
}

/// <summary>
/// Non-generic convenience methods for algebraic multiply with concrete element types.
/// </summary>
public static class AlgebraOps
{
    /// <summary>
    /// Algebraic multiply for long elements (used by Proportion-level algebra).
    /// </summary>
    public static long[] Multiply(IAlgebraic<long> a, IAlgebraic<long> b)
    {
        return AlgebraTable<long>.Multiply(a, b,
            multiply: (x, y) => x * y,
            add: (x, y) => x + y,
            scale: (x, s) => x * s,
            zero: 0L);
    }

    /// <summary>
    /// Algebraic multiply for Proportion elements (used by Axis-level algebra).
    /// </summary>
    public static Proportion[] Multiply(IAlgebraic<Proportion> a, IAlgebraic<Proportion> b)
    {
        return AlgebraTable<Proportion>.Multiply(a, b,
            multiply: (x, y) => x * y,
            add: (x, y) => x + y,
            scale: (x, s) => s < 0 ? -x : x,
            zero: Proportion.Zero);
    }
}
