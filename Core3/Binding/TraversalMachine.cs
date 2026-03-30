namespace Core3.Binding;

/// <summary>
/// One named register carried by a traversal token or trolley. The register's
/// value shape is still expressed as a binding-time template so the machine
/// definition stays declarative rather than materialized.
/// </summary>
public sealed record TraversalRegister(
    string Name,
    BoundTemplate Template)
{
    public string Name { get; } = !string.IsNullOrWhiteSpace(Name)
        ? Name
        : throw new ArgumentException("A traversal register name cannot be empty.", nameof(Name));

    public BoundTemplate Template { get; } =
        Template ?? throw new ArgumentNullException(nameof(Template));
}

/// <summary>
/// Experimental declarative shape for one traversal machine. This is not yet an
/// execution runtime. It is a serializable description of:
/// - which exact iterator-like mover carries the current route position
/// - which token registers the machine carries
/// - where execution first enters
/// - which operation laws are attached to which structural sites
/// </summary>
public sealed record TraversalMachineDefinition
{
    public TraversalMachineDefinition(
        string name,
        string entrySiteName,
        TraversalMover mover,
        IReadOnlyList<TraversalRegister> registers,
        IReadOnlyList<OperationAttachment> attachments)
    {
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new ArgumentException("A traversal machine name cannot be empty.", nameof(name));
        EntrySiteName = !string.IsNullOrWhiteSpace(entrySiteName)
            ? entrySiteName
            : throw new ArgumentException("An entry site name cannot be empty.", nameof(entrySiteName));
        Mover = mover ?? throw new ArgumentNullException(nameof(mover));
        Registers = registers ?? throw new ArgumentNullException(nameof(registers));
        Attachments = attachments ?? throw new ArgumentNullException(nameof(attachments));
    }

    public string Name { get; }
    public string EntrySiteName { get; }
    public TraversalMover Mover { get; }
    public IReadOnlyList<TraversalRegister> Registers { get; }
    public IReadOnlyList<OperationAttachment> Attachments { get; }
}
