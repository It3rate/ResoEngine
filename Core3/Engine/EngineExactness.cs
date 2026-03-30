using System.Diagnostics.CodeAnalysis;

namespace Core3.Engine;

/// <summary>
/// Small shared protocol and helpers for Core3 result shells that can be exact
/// or tension-bearing.
/// </summary>
public interface IExactResult
{
    bool IsExact { get; }
}

public static class EngineExactness
{
    public static bool TryGetExact<TResult>(
        TResult? candidate,
        [NotNullWhen(true)]
        out TResult? exact)
        where TResult : class, IExactResult
    {
        if (candidate is not null &&
            candidate.IsExact)
        {
            exact = candidate;
            return true;
        }

        exact = null;
        return false;
    }

    public static bool TryGetExact<TResult>(
        bool succeeded,
        TResult? candidate,
        [NotNullWhen(true)]
        out TResult? exact)
        where TResult : class, IExactResult
        => succeeded
            ? TryGetExact(candidate, out exact)
            : ReturnDefault(out exact);

    public static bool TryProjectExact<TResult, TValue>(
        TResult? candidate,
        Func<TResult, TValue> projection,
        out TValue? value)
        where TResult : class, IExactResult
    {
        if (TryGetExact(candidate, out var exact))
        {
            value = projection(exact);
            return true;
        }

        value = default;
        return false;
    }

    public static bool TryProjectExact<TResult, TValue>(
        bool succeeded,
        TResult? candidate,
        Func<TResult, TValue> projection,
        out TValue? value)
        where TResult : class, IExactResult
        => succeeded
            ? TryProjectExact(candidate, projection, out value)
            : ReturnDefault(out value);

    public static bool AreAllExact<TResult>(IEnumerable<TResult>? candidates)
        where TResult : IExactResult =>
        candidates is not null &&
        candidates.All(candidate => candidate.IsExact);

    private static bool ReturnDefault<TValue>(out TValue? value)
    {
        value = default;
        return false;
    }
}
