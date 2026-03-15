# Glyph Growth Plan

This document maps the next glyph-growth milestone onto concrete Core 2 files.
It covers the first three implementation phases and points forward to the later Page 1 demo.

## Phase 1. Propagation Core

Add a new `Core2.Propagation` layer that models tension as a moving packet rather than a targeted command.

Files:

- `Core2/Propagation/PacketFlowDirection.cs`
- `Core2/Propagation/PropagationBoundaryKind.cs`
- `Core2/Propagation/PropagationResponseMode.cs`
- `Core2/Propagation/CouplingKind.cs`
- `Core2/Propagation/CouplingRule.cs`
- `Core2/Propagation/TensionPacket.cs`
- `Core2/Propagation/PropagationFrame.cs`
- `Core2/Propagation/CarrierState.cs`
- `Core2/Propagation/JunctionState.cs`
- `Core2/Propagation/ResponseTerm.cs`
- `Core2/Propagation/ResponseProfile.cs`
- `Core2/Dynamic/DynamicMachine.cs`

Responsibilities:

- represent propagating tensions
- represent asymmetric response profiles
- scatter packets into branch families
- support mixed response modes such as reflect plus continue
- support stepwise execution for later animation pages

## Phase 2. Glyph Topology Layer

Add a `Core2.Geometry.Glyphs` namespace for glyph-oriented but still general shape growth.

Files:

- `Core2/Geometry/Glyphs/GlyphVector.cs`
- `Core2/Geometry/Glyphs/GlyphBox.cs`
- `Core2/Geometry/Glyphs/GlyphKinds.cs`
- `Core2/Geometry/Glyphs/GlyphSeed.cs`
- `Core2/Geometry/Glyphs/GlyphTip.cs`
- `Core2/Geometry/Glyphs/GlyphCarrier.cs`
- `Core2/Geometry/Glyphs/GlyphJunction.cs`
- `Core2/Geometry/Glyphs/GlyphLandmark.cs`
- `Core2/Geometry/Glyphs/GlyphGrowthState.cs`
- `Core2/Geometry/Glyphs/GlyphLetterSpec.cs`
- `Core2/Geometry/Glyphs/GlyphLetterCatalog.cs`

Responsibilities:

- hold glyph seeds, tips, carriers, and junctions
- distinguish topology from final outline
- encode letter boxes and landmarks such as midline and centerline
- seed future glyph runs for letters like `V`, `Y`, `T`, and `O`

## Phase 3. Field And Coupling Laws

Add the first field emitters and field sampling helpers.

Files:

- `Core2/Geometry/Glyphs/GlyphFieldFalloff.cs`
- `Core2/Geometry/Glyphs/GlyphFieldInfluence.cs`
- `Core2/Geometry/Glyphs/GlyphFieldEmitter.cs`
- `Core2/Geometry/Glyphs/PointGlyphFieldEmitter.cs`
- `Core2/Geometry/Glyphs/HorizontalBandGlyphFieldEmitter.cs`
- `Core2/Geometry/Glyphs/VerticalBandGlyphFieldEmitter.cs`
- `Core2/Geometry/Glyphs/GlyphEnvironment.cs`

Responsibilities:

- emit general couplings such as attract, repel, align, grow, and stop
- provide reusable field sampling inside glyph boxes
- define the first box, midline, centerline, and endpoint-style influences

## Later Phases

Phase 4:
- glyph resolver
- packet-to-growth proposal generation
- dynamic branching and rejoining in glyph state

Phase 5:
- seeded skeleton demos for `V`, `Y`, `T`, and `O`

Phase 6:
- skeleton-to-outline conversion

Phase 7:
- interactive Page 1 glyph-growth demo with fading ambient tension display
