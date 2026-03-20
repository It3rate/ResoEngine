using Core2.Branching;
using Core2.Elements;
using Core2.Repetition;
using Core2.Support;

namespace Core2.Symbolics.Branching;

public static class BranchGraphResultExtensions
{
    public static BranchGraphBuilder<T> Append<T>(
        this BranchGraphBuilder<T> builder,
        InverseContinuationResult<T> result,
        bool rewireToCurrentFrontier = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(result);

        return builder.ApplyFamily(
            rewireToCurrentFrontier
                ? result.Branches.WithParents(builder.CurrentFrontier.ActiveNodeIds)
                : result.Branches);
    }

    public static BranchGraphBuilder<T> Append<T>(
        this BranchGraphBuilder<T> builder,
        PowerResult<T> result,
        bool rewireToCurrentFrontier = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(result);

        return builder.ApplyFamily(
            rewireToCurrentFrontier
                ? result.Branches.WithParents(builder.CurrentFrontier.ActiveNodeIds)
                : result.Branches);
    }

    public static BranchGraphBuilder<Axis> Append(
        this BranchGraphBuilder<Axis> builder,
        AxisBooleanResult result,
        bool rewireToCurrentFrontier = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(result);

        return builder.ApplyFamily(
            rewireToCurrentFrontier
                ? result.Branches.WithParents(builder.CurrentFrontier.ActiveNodeIds)
                : result.Branches);
    }

    public static BranchGraph<T> ToBranchGraph<T>(
        this InverseContinuationResult<T> result,
        T sourceValue)
    {
        ArgumentNullException.ThrowIfNull(result);

        var builder = new BranchGraphBuilder<T>()
            .Seed(sourceValue, selectAsPrincipal: false)
            .Append(result, rewireToCurrentFrontier: true);

        return builder.Build();
    }

    public static BranchGraph<T> ToBranchGraph<T>(
        this PowerResult<T> result,
        T sourceValue,
        bool rewireToCurrentFrontier = true)
    {
        ArgumentNullException.ThrowIfNull(result);

        var builder = new BranchGraphBuilder<T>()
            .Seed(sourceValue, selectAsPrincipal: false)
            .Append(result, rewireToCurrentFrontier);

        return builder.Build();
    }

    public static BranchGraph<Axis> ToBranchGraph(this AxisBooleanResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.Branches.ToGraph();
    }
}
