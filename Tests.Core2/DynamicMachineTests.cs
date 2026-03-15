using Core2.Dynamic;

namespace Tests.Core2;

public class DynamicMachineTests
{
    private sealed record CounterState(int Value);

    private sealed record CounterEnvironment(int Ceiling);

    private sealed class IncrementStrand : IDynamicStrand<CounterState, CounterEnvironment, int>
    {
        public string Name => "Increment";

        public IReadOnlyList<DynamicProposal<int>> Propose(
            DynamicStrandContext<CounterState, CounterEnvironment> context) =>
            [
                new DynamicProposal<int>(Name, context.Current.NodeId, 1)
            ];
    }

    private sealed class CounterResolver : IDynamicResolver<CounterState, CounterEnvironment, int>
    {
        public DynamicResolution<CounterState, CounterEnvironment, int> Resolve(
            DynamicResolutionInput<CounterState, CounterEnvironment, int> input)
        {
            var current = input.Frontier[0].Context;
            int nextValue = current.State.Value + input.Proposals.Sum(proposal => proposal.Effect);
            var nextContext = new DynamicContext<CounterState, CounterEnvironment>(
                new CounterState(nextValue),
                current.Environment);

            return DynamicResolution<CounterState, CounterEnvironment, int>.Commit(nextContext, input.Proposals);
        }
    }

    [Fact]
    public void Machine_StepsIncrementally_AndSnapshotsTrace()
    {
        var machine = new DynamicMachine<CounterState, CounterEnvironment, int>(
            new DynamicContext<CounterState, CounterEnvironment>(
                new CounterState(0),
                new CounterEnvironment(10)),
            [new IncrementStrand()],
            new CounterResolver(),
            new FixedStepConvergencePolicy<CounterState, CounterEnvironment, int>(3));

        Assert.True(machine.Step());
        Assert.Equal(1, machine.StepCount);
        Assert.False(machine.IsCompleted);

        Assert.True(machine.Step());
        Assert.True(machine.Step());
        Assert.False(machine.Step());
        Assert.True(machine.IsCompleted);

        var trace = machine.Snapshot();
        Assert.Equal(3, trace.Steps.Count);
        Assert.Equal(3, trace.SelectedContext!.State.Value);
        Assert.Equal(4, trace.Graph.Nodes.Count);
    }
}
