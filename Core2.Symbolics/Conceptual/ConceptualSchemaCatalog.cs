namespace Core2.Symbolics.Conceptual;

public static class ConceptualSchemaCatalog
{
    public static IReadOnlyList<ConceptualSchemaDraft> All { get; } =
    [
        new(
            "adjacency-band",
            ConceptualRelationFamily.Adjacency,
            [ConceptualTopologyKind.Region, ConceptualTopologyKind.Field],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Scalar, ConceptualLexicalDomain.Language],
            "Nearness, beside-ness, or social closeness expressed as a lawful surrounding band rather than direct occupation."),
        new(
            "container-interior",
            ConceptualRelationFamily.Containment,
            [ConceptualTopologyKind.Region, ConceptualTopologyKind.Boundary],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Language],
            "An interior or exterior reading of one thing relative to a containing host, frame, or admissible region."),
        new(
            "boundary-crossing",
            ConceptualRelationFamily.Boundary,
            [ConceptualTopologyKind.Region, ConceptualTopologyKind.Path],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Motion, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Language],
            "A change in lawful status produced by reaching, crossing, or remaining at a boundary."),
        new(
            "between-interval",
            ConceptualRelationFamily.BoundedInterval,
            [ConceptualTopologyKind.Interval, ConceptualTopologyKind.Region],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Language],
            "A bounded relation inside two anchors, limits, or ordered participants rather than simple nearness to one landmark."),
        new(
            "directional-offset",
            ConceptualRelationFamily.DirectionalOffset,
            [ConceptualTopologyKind.Path, ConceptualTopologyKind.Field],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Scalar, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Language],
            "A before-after, above-below, front-back, or left-right offset relative to an active host frame."),
        new(
            "source-path-goal",
            ConceptualRelationFamily.SourceGoal,
            [ConceptualTopologyKind.Path, ConceptualTopologyKind.Interval],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Motion, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Graph, ConceptualLexicalDomain.Language],
            "A directed progression from a source toward a goal, possibly with traversal, delay, or obstruction along the way."),
        new(
            "along-host",
            ConceptualRelationFamily.PathCoincidence,
            [ConceptualTopologyKind.Path, ConceptualTopologyKind.Through],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Motion, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Graph, ConceptualLexicalDomain.Ornamental, ConceptualLexicalDomain.Language],
            "Continuation that remains constrained by a host route, edge, rail, stroke, or carrier."),
        new(
            "crossing-passage",
            ConceptualRelationFamily.Crossing,
            [ConceptualTopologyKind.Path, ConceptualTopologyKind.Region, ConceptualTopologyKind.Through],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Motion, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Graph, ConceptualLexicalDomain.Language],
            "Entry, through-occupation, and exit across a host region, line, or guarded span."),
        new(
            "attachment-link",
            ConceptualRelationFamily.Attachment,
            [ConceptualTopologyKind.Meet, ConceptualTopologyKind.Through],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Graph, ConceptualLexicalDomain.Language],
            "A tie, contact, or attached relation where things are held together, linked, or latched."),
        new(
            "support-cover",
            ConceptualRelationFamily.Support,
            [ConceptualTopologyKind.Region, ConceptualTopologyKind.Path],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Language],
            "One structure holding, carrying, covering, or overshadowing another from above, below, or across a host span."),
        new(
            "part-whole-membership",
            ConceptualRelationFamily.PartWhole,
            [ConceptualTopologyKind.Region, ConceptualTopologyKind.Field],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.Language],
            "Membership, inclusion, and belonging where one part is read relative to a larger organized whole."),
        new(
            "center-periphery-field",
            ConceptualRelationFamily.CenterPeriphery,
            [ConceptualTopologyKind.Field, ConceptualTopologyKind.Region],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Scalar, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Ornamental, ConceptualLexicalDomain.Language],
            "Core, edge, margin, and fringe structure read as a field around a privileged center or spine."),
        new(
            "surrounding-loop",
            ConceptualRelationFamily.SurroundClosure,
            [ConceptualTopologyKind.Loop, ConceptualTopologyKind.Region, ConceptualTopologyKind.Path],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Motion, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.Ornamental, ConceptualLexicalDomain.Language],
            "A surrounding family of paths or an enclosing relation that wraps around a host before or without exact collapse."),
        new(
            "cyclic-return",
            ConceptualRelationFamily.Cycle,
            [ConceptualTopologyKind.Loop, ConceptualTopologyKind.Path],
            [ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Ornamental, ConceptualLexicalDomain.Language],
            "Recurrent return, looping, or phased revisit rather than one-way passage."),
        new(
            "branch-merge-network",
            ConceptualRelationFamily.Branching,
            [ConceptualTopologyKind.Branch, ConceptualTopologyKind.Merge, ConceptualTopologyKind.Path],
            [ConceptualLexicalDomain.Motion, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Graph, ConceptualLexicalDomain.Language],
            "One route becoming many or many routes reconverging, with alternatives or co-present strands preserved."),
        new(
            "closure-return",
            ConceptualRelationFamily.Closure,
            [ConceptualTopologyKind.Closure, ConceptualTopologyKind.Loop, ConceptualTopologyKind.Path],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Language],
            "A path, relation, or dependency closing back into earlier structure rather than ending open."),
        new(
            "scalar-gradient",
            ConceptualRelationFamily.ScalarExtent,
            [ConceptualTopologyKind.Interval, ConceptualTopologyKind.Field],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Scalar, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Language],
            "Extent or measured spread along an active dimension such as height, depth, width, closeness, or rank."),
        new(
            "intensity-weighting",
            ConceptualRelationFamily.ScalarIntensity,
            [ConceptualTopologyKind.Interval, ConceptualTopologyKind.Field],
            [ConceptualLexicalDomain.Scalar, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Language],
            "Degree, emphasis, confidence, or weighted participation layered onto another relation."),
        new(
            "ordered-comparison",
            ConceptualRelationFamily.Ordering,
            [ConceptualTopologyKind.Path, ConceptualTopologyKind.Interval],
            [ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Scalar, ConceptualLexicalDomain.Social, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.Language],
            "Relative earlier-later, higher-lower, or before-after placement in an ordered frame."),
        new(
            "hierarchy-ladder",
            ConceptualRelationFamily.Hierarchy,
            [ConceptualTopologyKind.Path, ConceptualTopologyKind.Field],
            [ConceptualLexicalDomain.Social, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Language],
            "Ranked structure with upper-lower or senior-junior readings that reuse spatial framing in abstract organizational space."),
        new(
            "temporal-window",
            ConceptualRelationFamily.TemporalWindow,
            [ConceptualTopologyKind.Interval, ConceptualTopologyKind.Region],
            [ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Language],
            "An occupied, available, or bounded duration read as an interval-like region on a temporal carrier."),
        new(
            "temporal-sequence",
            ConceptualRelationFamily.TemporalSequence,
            [ConceptualTopologyKind.Path, ConceptualTopologyKind.Interval],
            [ConceptualLexicalDomain.Temporal, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Language],
            "Ordered succession, deadline, lead-up, or aftermath along a temporal carrier."),
        new(
            "obstruction-gate",
            ConceptualRelationFamily.Obstruction,
            [ConceptualTopologyKind.Region, ConceptualTopologyKind.Path, ConceptualTopologyKind.Boundary],
            [ConceptualLexicalDomain.Spatial, ConceptualLexicalDomain.Motion, ConceptualLexicalDomain.Narrative, ConceptualLexicalDomain.ControlFlow, ConceptualLexicalDomain.Circuit, ConceptualLexicalDomain.Language],
            "A blockage, guard, or threshold that interrupts continuation until some condition, force, or detour resolves it.")
    ];

    public static IReadOnlyList<ConceptualSchemaDraft> ForDomain(ConceptualLexicalDomain domain) =>
        All.Where(schema => schema.Domains.Contains(domain)).ToArray();

    public static IReadOnlyList<ConceptualSchemaDraft> ForFamily(ConceptualRelationFamily family) =>
        All.Where(schema => schema.Family == family).ToArray();
}
