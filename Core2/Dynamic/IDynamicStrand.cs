namespace Core2.Dynamic;

public interface IDynamicStrand<TState, TEnvironment, TEffect>
{
    string Name { get; }

    IReadOnlyList<DynamicProposal<TEffect>> Propose(
        DynamicStrandContext<TState, TEnvironment> context);
}
