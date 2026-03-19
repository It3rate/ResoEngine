# Carrier Routing Implementation Plan

This is a rough migration plan for bringing explicit carrier continuity, branching, `T`, and true cross behavior into Core 2 and then exposing it in the shape editor UI.

It assumes the direction proposed in:

- [CarrierRoutingProposal.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/CarrierRoutingProposal.md)
- [RecursivePinningProposal.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/RecursivePinningProposal.md)
- [PositionedPinningProposal.md](C:/Users/Robin/source/repos/ResoEngine/Core2/Axioms/PositionedPinningProposal.md)

## Goal

Treat a pin site as more than:

- a host carrier
- an applied descriptor
- optional side attachments

and let it also say:

- which incident parts belong to which carriers
- which incident parts continue into which others
- which incident parts terminate or begin
- which local configuration should be summarized as cusp, branch, `T`, or true cross

without identifying carriers from geometry alone.

## Design Principles

1. Keep carrier identity separate from fold geometry.
2. Do not infer sameness merely from parallelism or straightness.
3. Preserve current pinning and carrier-graph APIs through compatibility layers where possible.
4. Add routing structure before adding more display heuristics.
5. Keep unbound sides lawful.
6. Let UI binding controls map onto real Core 2 concepts rather than display-only flags.
7. Defer rich curve styling until structural routing is explicit.

## Recommended Phases

### Phase 1. Add Site Routing Primitives

Introduce a small routing layer for pin sites.

Likely new types:

- `CarrierIncident`
- `CarrierIncidentKind`
- `CarrierSiteRouting`
- `CarrierRoute`
- `CarrierRouteKind`
- `CarrierJunctionSummary`

Responsibilities:

- name the local incident parts at a site
- identify which carrier each incident part belongs to
- express through-continuation, emission, and termination
- provide a derived summary such as `Cusp`, `Branch`, `Tee`, or `Cross`

This phase should not change traversal yet.
It should mostly add inspectable structure.

### Phase 2. Bridge Existing `CarrierPinSite` Into Routing

Teach [CarrierPinSite.cs](C:/Users/Robin/source/repos/ResoEngine/Core2/Elements/CarrierPinSite.cs) to expose a derived routing view.

Options:

- enrich `CarrierPinSite` directly
- or keep `CarrierPinSite` minimal and derive `CarrierSiteRouting` beside it

Recommendation:

- keep `CarrierPinSite` as the local hosted descriptor
- derive routing in a separate layer first

This keeps current code stable while the new model settles.

### Phase 3. Represent True Cross Explicitly

Add the minimum structure needed for a true cross:

- host carrier through-continuation
- second carrier through-continuation
- shared site

The key test is:

- two orthogonal incident parts bound to the same distinct carrier
- that carrier is marked as through, not just emitted on one side

This is the first place where "cross-shaped local proposal" and "true cross" must diverge clearly in code.

### Phase 4. Add Junction Summaries

Derive explicit summaries from routing:

- `Open`
- `Cusp`
- `Branch`
- `Tee`
- `Cross`
- later richer summaries if needed

These summaries should be:

- compatibility views
- display helpers
- testing conveniences

but not the only stored source of truth.

### Phase 5. Update Carrier Graph Analysis

Extend [CarrierPinGraph.cs](C:/Users/Robin/source/repos/ResoEngine/Core2/Elements/CarrierPinGraph.cs) and [CarrierPinGraphAnalysis.cs](C:/Users/Robin/source/repos/ResoEngine/Core2/Elements/CarrierPinGraphAnalysis.cs) so analysis can report:

- which sites are through-sites for a carrier
- where a carrier terminates
- where a carrier branches
- where a true cross exists
- whether a carrier is same-carrier continuous through several sites even when the fold bends sharply

This phase is important before changing the editor UI, because the page should ask Core 2 for these facts rather than re-derive them visually.

### Phase 6. Teach Traversal To Respect Routing

Update repetition/traversal layers so they can read site routing instead of only host-side encounter and side attachment presence.

Likely affected files:

- [LocatedPin.cs](C:/Users/Robin/source/repos/ResoEngine/Core2/Repetition/LocatedPin.cs)
- [AxisTraversalDefinition.cs](C:/Users/Robin/source/repos/ResoEngine/Core2/Repetition/AxisTraversalDefinition.cs)
- [PinBoundaryTraversal.cs](C:/Users/Robin/source/repos/ResoEngine/Core2/Repetition/PinBoundaryTraversal.cs)

Add behavior for:

- continuing straight through one carrier
- switching into another carrier at a branch
- recognizing `T` versus cross
- preserving unresolved routing as tension where needed

This should still avoid display-specific curve rules.

### Phase 7. Expose Side Binding In The UI

Once Core 2 can express routing cleanly, add per-side binding controls to [SharedCarrierShapesPage.cs](C:/Users/Robin/source/repos/ResoEngine/Visualizer.WinForms.Core2/Pages/SharedCarrierShapesPage.cs).

Recommended first vocabulary:

- `Host`
- `New`
- `Link`

Per side, not per whole pin.

This lets a user say:

- cusp: side stays `Host`
- branch: one side `New`
- `T`: one carrier through, one carrier terminating/beginning
- cross: two sides `Link` to one shared nonhost carrier and mark that carrier through

### Phase 8. Add Routing Controls In The UI

Binding alone is not enough for `T` versus cross.
The UI also needs a simple continuation control telling whether a bound carrier:

- passes through
- begins here
- ends here
- remains open

This should be small and local to the selected site.
It can likely be derived to a friendlier presentation later.

### Phase 9. Extend Scene Serialization

Update the scene copy format in [SharedCarrierShapesPage.cs](C:/Users/Robin/source/repos/ResoEngine/Visualizer.WinForms.Core2/Pages/SharedCarrierShapesPage.cs) so pasted scene descriptions include:

- site axes
- side binding choices
- shared carrier links
- routing choices
- custom pin positions

This is important because the user specifically wants to author letters and send them back for recreation as presets.

### Phase 10. Add Canonical Preset Shapes From Routing

Once routing is explicit, rebuild the editor presets so `D`, `H`, `T`, `A`, `Y`, `L`, `M`, and later `X`, `O`, and `E` are defined by routing structure first and fold preview second.

That is the right point to harden them as true defaults.

## Suggested File Additions

- [CarrierSiteRouting.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/CarrierSiteRouting.cs)
- [CarrierIncident.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/CarrierIncident.cs)
- [CarrierIncidentKind.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/CarrierIncidentKind.cs)
- [CarrierRoute.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/CarrierRoute.cs)
- [CarrierRouteKind.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/CarrierRouteKind.cs)
- [CarrierJunctionSummary.cs](/Users/Robin/source/repos/ResoEngine/Core2/Elements/CarrierJunctionSummary.cs)

Possible later UI helper additions:

- [CarrierBindingMode.cs](/Users/Robin/source/repos/ResoEngine/Visualizer.WinForms.Core2/Pages/CarrierBindingMode.cs)
- [CarrierRoutingDisplayState.cs](/Users/Robin/source/repos/ResoEngine/Visualizer.WinForms.Core2/Pages/CarrierRoutingDisplayState.cs)

## Suggested Test Areas

### Core 2 routing tests

- same-carrier cusp remains one carrier with changed tangent
- branch creates one continuing carrier plus one distinct carrier
- `T` differs from cross structurally
- true cross requires one second through-carrier, not just two independent orthogonal half-branches
- parallel geometry alone does not merge carriers

Likely test file:

- [RecursivePinningTests.cs](/Users/Robin/source/repos/ResoEngine/Tests.Core2/RecursivePinningTests.cs)

### Carrier graph analysis tests

- carrier-through count at a site
- branch site detection
- `T` site detection
- cross site detection
- preserved carrier identity through nonstraight fold

Likely test file:

- [RepetitionTests.cs](/Users/Robin/source/repos/ResoEngine/Tests.Core2/RepetitionTests.cs)

### UI-facing tests or manual checks

- add a custom pin and bind its sides to host/new/link
- build a cusp without creating a new carrier
- build a `T`
- build a true cross
- copy the scene and verify the routing structure is preserved

## Open Questions

1. Should host through-continuation be explicit on every site, or implicit until broken?
2. Should a true cross be represented as one site with two through-carriers, or should richer sites be decomposed into several simpler sub-sites internally?
3. How much should routing be directional versus neutral structure plus later traversal choice?
4. Should `Link` in the UI create the target carrier if none exists yet, or require an existing carrier selection always?
5. When a user binds two side proposals to the same new carrier, should the UI immediately summarize that as a possible true cross or only after through-routing is set?

## Short Recommendation

First add explicit site routing to Core 2.
Then derive cusp, branch, `T`, and true cross from that routing.
Only after that should the UI expose per-side host/new/link and through/terminate/begin controls, so the editor is describing real carrier structure instead of inventing display-only exceptions.
