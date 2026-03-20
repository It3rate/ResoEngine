using Core2.Branching;

namespace Core2.Symbolics.Branching;

public static class BranchGraphExtensions
{
    public static BranchGraph<T> ToGraph<T>(this BranchFamily<T> family)
    {
        ArgumentNullException.ThrowIfNull(family);

        var builder = new BranchGraphBuilder<T>();
        builder.Seed(family);
        return builder.Build();
    }
}
