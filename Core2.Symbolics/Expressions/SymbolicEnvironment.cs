namespace Core2.Symbolics.Expressions;

public sealed class SymbolicEnvironment
{
    private readonly IReadOnlyDictionary<string, SymbolicTerm> _bindings;

    public static SymbolicEnvironment Empty { get; } = new();

    public SymbolicEnvironment()
        : this(new Dictionary<string, SymbolicTerm>(StringComparer.Ordinal))
    {
    }

    private SymbolicEnvironment(IReadOnlyDictionary<string, SymbolicTerm> bindings)
    {
        _bindings = bindings;
    }

    public IEnumerable<KeyValuePair<string, SymbolicTerm>> Bindings => _bindings;

    public bool TryResolve(string name, out SymbolicTerm? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_bindings.TryGetValue(name, out var resolved))
        {
            value = resolved;
            return true;
        }

        value = null;
        return false;
    }

    public SymbolicEnvironment Bind(string name, SymbolicTerm value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);

        var next = _bindings.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
        next[name] = value;
        return new SymbolicEnvironment(next);
    }
}
