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
        bool succeeded,
        TResult? candidate,
        [NotNullWhen(true)]
        out TResult? exact)
        where TResult : class, IExactResult
    {
        if (succeeded &&
            candidate is not null &&
            candidate.IsExact)
        {
            exact = candidate;
            return true;
        }

        exact = null;
        return false;
    }

    public static bool TryProjectExact<TResult, TValue>(
        bool succeeded,
        TResult? candidate,
        Func<TResult, TValue> projection,
        out TValue? value)
        where TResult : class, IExactResult
    {
        if (TryGetExact(succeeded, candidate, out var exact))
        {
            value = projection(exact);
            return true;
        }

        value = default;
        return false;
    }

    public static bool AreAllExact<TResult>(IEnumerable<TResult>? candidates)
        where TResult : IExactResult =>
        candidates is not null &&
        candidates.All(candidate => candidate.IsExact);
}
