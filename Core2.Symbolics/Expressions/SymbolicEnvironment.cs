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

    public bool TryResolve(SymbolicBindingTarget target, out SymbolicTerm? value)
    {
        ArgumentNullException.ThrowIfNull(target);
        return TryResolve(target.QualifiedName, out value);
    }

    public SymbolicEnvironment Bind(string name, SymbolicTerm value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);

        var next = _bindings.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
        next[name] = value;
        return new SymbolicEnvironment(next);
    }

    public SymbolicEnvironment Bind(SymbolicBindingTarget target, SymbolicTerm value)
    {
        ArgumentNullException.ThrowIfNull(target);
        return Bind(target.QualifiedName, value);
    }

    public SymbolicEnvironmentScope GetScopeTree()
    {
        var root = new MutableScope(string.Empty, string.Empty);
        foreach (var binding in _bindings.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            string[] segments = binding.Key.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
            {
                continue;
            }

            if (segments.Length == 1)
            {
                root.DirectBindings.Add(new KeyValuePair<string, SymbolicTerm>(segments[0], binding.Value));
                continue;
            }

            var current = root;
            string qualifiedName = string.Empty;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                qualifiedName = string.IsNullOrEmpty(qualifiedName)
                    ? segments[i]
                    : $"{qualifiedName}.{segments[i]}";
                current = current.GetOrAddChild(segments[i], qualifiedName);
            }

            current.DirectBindings.Add(new KeyValuePair<string, SymbolicTerm>(segments[^1], binding.Value));
        }

        return root.Freeze();
    }

    private sealed class MutableScope
    {
        private readonly Dictionary<string, MutableScope> _children = new(StringComparer.Ordinal);

        public MutableScope(string name, string qualifiedName)
        {
            Name = name;
            QualifiedName = qualifiedName;
        }

        public string Name { get; }
        public string QualifiedName { get; }
        public List<KeyValuePair<string, SymbolicTerm>> DirectBindings { get; } = [];

        public MutableScope GetOrAddChild(string name, string qualifiedName)
        {
            if (_children.TryGetValue(name, out var existing))
            {
                return existing;
            }

            var created = new MutableScope(name, qualifiedName);
            _children[name] = created;
            return created;
        }

        public SymbolicEnvironmentScope Freeze()
        {
            var bindings = DirectBindings
                .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .ToArray();
            var children = _children.Values
                .OrderBy(child => child.Name, StringComparer.Ordinal)
                .Select(child => child.Freeze())
                .ToArray();

            return new SymbolicEnvironmentScope(Name, QualifiedName, bindings, children);
        }
    }
}
