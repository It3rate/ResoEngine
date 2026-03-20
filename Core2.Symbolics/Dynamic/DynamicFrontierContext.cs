using Core2.Branching;

namespace Core2.Symbolics.Dynamic;

public sealed record DynamicFrontierContext<TState, TEnvironment>(
    BranchId NodeId,
    DynamicContext<TState, TEnvironment> Context);
