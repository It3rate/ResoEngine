namespace Core2.Symbolics.Conceptual;

public sealed record ConceptualSchemaDraft(
    string Id,
    ConceptualRelationFamily Family,
    IReadOnlyList<ConceptualTopologyKind> Topology,
    IReadOnlyList<ConceptualLexicalDomain> Domains,
    string Description);
