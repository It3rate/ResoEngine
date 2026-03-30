namespace Core3.Binding;

/// <summary>
/// One named register carried by a traversal token or trolley. The register's
/// value shape is still expressed as a binding-time template so the machine
/// definition stays declarative rather than materialized.
/// </summary>
public sealed record TraversalRegister(
    string Name,
    BoundTemplate Template);

/// <summary>
/// Experimental declarative shape for one traversal machine. This is not yet an
/// execution runtime. It is a serializable description of:
/// - which exact iterator-like mover carries the current route position
/// - which token registers the machine carries
/// - where execution first enters
/// - which operation laws are attached to which structural sites
/// </summary>
public sealed record TraversalMachineDefinition(
    string Name,
    string EntrySiteName,
    TraversalMover Mover,
    IReadOnlyList<TraversalRegister> Registers,
    IReadOnlyList<OperationAttachment> Attachments);
