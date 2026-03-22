namespace Core2.Symbolics.Conceptual;

public sealed record ConceptualLexicalMappingDraft(
    string SurfaceForm,
    ConceptualSurfaceKind SurfaceKind,
    string SchemaId,
    ConceptualRelationFamily Family,
    IReadOnlyList<ConceptualTopologyKind> Topology,
    IReadOnlyList<ConceptualLexicalDomain> Domains,
    string EnglishDescription,
    string ApproximateEncoding,
    string ExampleUsage);
