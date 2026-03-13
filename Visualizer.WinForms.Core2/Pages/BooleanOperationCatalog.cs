namespace ResoEngine.Visualizer.Pages;

public sealed record BooleanOperationDefinition(string Name, Func<bool, bool, bool> Evaluate);

public static class BooleanOperationCatalog
{
    public static IReadOnlyList<BooleanOperationDefinition> All { get; } =
    [
        new("Null", (a, b) => false),
        new("Identity", (a, b) => true),
        new("Transfer A", (a, b) => a),
        new("Transfer B", (a, b) => b),
        new("Not A", (a, b) => !a),
        new("Not B", (a, b) => !b),
        new("And", (a, b) => a && b),
        new("Nand", (a, b) => !(a && b)),
        new("Or", (a, b) => a || b),
        new("Nor", (a, b) => !(a || b)),
        new("Implication", (a, b) => !a || b),
        new("Rev Implication", (a, b) => !b || a),
        new("Inhibition", (a, b) => a && !b),
        new("Rev Inhibition", (a, b) => !a && b),
        new("Xor", (a, b) => a ^ b),
        new("Xnor", (a, b) => !(a ^ b)),
    ];
}
