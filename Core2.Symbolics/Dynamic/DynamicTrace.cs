using Core2.Branching;

namespace Core2.Symbolics.Dynamic;

public sealed record DynamicTrace<TState, TEnvironment, TEffect>(
    DynamicContext<TState, TEnvironment> Seed,
    IReadOnlyList<DynamicStep<TState, TEnvironment, TEffect>> Steps,
    BranchGraph<DynamicContext<TState, TEnvironment>> Graph)
{
    public IReadOnlyList<DynamicContext<TState, TEnvironment>> CurrentContexts =>
        Graph.CurrentFrontier.ActiveNodeIds
            .Select(id => Graph.GetNode(id).Value)
            .ToArray();

    public DynamicContext<TState, TEnvironment>? SelectedContext =>
        Graph.TryGetSelectedNode(out var selected)
            ? selected!.Value
            : null;
}
