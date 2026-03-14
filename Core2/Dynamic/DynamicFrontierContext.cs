using Core2.Branching;

namespace Core2.Dynamic;

public sealed record DynamicFrontierContext<TState, TEnvironment>(
    BranchId NodeId,
    DynamicContext<TState, TEnvironment> Context);
