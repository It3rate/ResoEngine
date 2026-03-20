# Carrier Routing and True Cross Proposal

This document is a focused proposal for the next layer after recursive pinning and shared carrier identity.
It is intended to sit beside:

- [PinningProposal.md](./PinningProposal.md)
- [PositionedPinningProposal.md](./PositionedPinningProposal.md)
- [RecursivePinningProposal.md](./RecursivePinningProposal.md)
- [PromptAxioms.md](../PromptAxioms.md)

This file is a proposal, not final doctrine.
Its purpose is to clarify how a pinned site decides:

- what is the same carrier
- what continues through the site
- what branches away
- what ends at the site
- what counts as a cusp, `T`, or true cross

without requiring Core 2 to identify carriers from geometry alone.

## Core Claim

The central claim of this proposal is:

- carrier identity and drawn geometry are separate
- a pin site should be able to express explicit continuation or routing between incident carrier parts
- same-carrier continuity should be decided by lawful routing at the site, not by parallel shape alone
- a cusp is a same-carrier continuation with a local tangent change
- a branch introduces a distinct carrier incident at the site
- a `T` is a termination or start of one carrier into another carrier that continues through
- a true cross is one site where two carriers both continue through
- unbound orthogonal sides may look cross-shaped locally without yet constituting a true structural cross

In this reading, the first question is not:

- what curve or polyline should we draw?

The first question is:

- which incident parts continue into which other incident parts?
- which of those incident parts belong to the same carrier?
- which of them remain unbound or latent?

## Primitive Terms

### Site

A site is a local pinning event at which one or more carriers become incident.

### Host carrier

The host carrier is the carrier on which the site is positioned.

### Side proposal

A side proposal is the local continuation desire expressed by one side of the applied descriptor.
It may stay on the host carrier, leave orthogonally, oppose, terminate, or remain unresolved.

### Incident part

An incident part is one local half-attachment of a carrier at a site.
Examples:

- the host carrier approaching from one side
- the host carrier leaving on the other side
- a recessive side proposal
- a dominant side proposal

### Carrier binding

Carrier binding is the identification of an incident part with a specific carrier identity.

### Continuation routing

Continuation routing is the lawful relation at a site that determines which incident parts continue into which others.

### Through continuation

Through continuation is routing in which a carrier remains the same carrier on both sides of a site.

### Termination

Termination is routing in which a carrier ends at the site.

### Emission

Emission is routing in which a carrier begins at the site.

### Cusp

A cusp is same-carrier continuation with a discontinuity of local tangent or local directional proposal.

### Branch

A branch is a site at which one carrier continues and at least one distinct carrier is also emitted, admitted, or terminated.

### True cross

A true cross is a site at which two distinct carriers both continue through the site.

### Cross-shaped local proposal

A cross-shaped local proposal is a site whose host and side directions visually resemble a cross, but where the orthogonal side pair has not yet been explicitly unified into one distinct through-carrier.

## Part I. Identity And Geometry

### CR1. Carrier Identity Is Not Geometry

Carrier identity shall not be inferred merely from straightness, parallelism, or visual closeness.
Carrier identity is a structural fact.

### CR2. Same Carrier And Same Tangent Differ

Two supports may be tangent-aligned and still be distinct carriers.
One carrier may also remain the same carrier while changing tangent at a site.

### CR3. Parallelism Does Not By Itself Imply Sameness

Two parallel pieces shall not automatically be treated as the same carrier.
Otherwise lawful separate structures such as parallel stems, rails, or bars would collapse incorrectly.

### CR4. Curved Continuity May Still Be One Carrier

A carrier may remain one carrier through a bend, cusp, bow, or other nonstraight fold.
Curvature does not by itself create a new carrier.

### CR5. The Site Decides Continuity

Whether incident parts are the same carrier across a site is determined by the site's continuation routing, not by later display geometry.

## Part II. Incident Parts

### CR6. A Site May Host Several Incident Parts

A site may have several incident parts present simultaneously.
At minimum this may include:

- host continuation on the host carrier
- recessive side proposal
- dominant side proposal

Later richer sites may include more than this minimum.

### CR7. Incident Parts And Carrier Bindings Differ

An incident part is a local role at a site.
A carrier binding tells which carrier identity that incident part belongs to.

### CR8. Unbound Incident Parts Are Lawful

An incident part may remain unbound.
An unbound incident part is latent, open, or unresolved rather than invalid.

### CR9. Bound Incident Parts May Share A Carrier

Two incident parts may be bound to the same carrier identity.
This is how two sides of one site may belong to one distinct carrier.

### CR10. Host Continuation Is Not The Only Through Carrier

The host carrier may continue through a site, but another distinct carrier may also continue through the same site.
This is the structural basis of a true cross.

## Part III. Continuation Routing

### CR11. Continuation Routing Is Explicit

Continuation routing should be representable explicitly at a site.
The system should not rely only on geometric inference or fallback heuristics.

### CR12. Routing States What Continues Into What

For each incident part, the routing law states whether it:

- continues into another incident part
- terminates at the site
- begins at the site
- remains unbound

### CR13. Same Carrier Continuity Requires Routing

Two incident parts belong to the same continuing carrier across a site only when the routing law says they do.

### CR14. Distinct Carrier Presence Requires Distinct Binding

If a site supports more than one carrier, those carriers shall be represented distinctly even if their local geometry overlaps or crosses.

### CR15. One Site May Route Several Carriers At Once

The same site may simultaneously:

- continue the host carrier
- terminate or emit another carrier
- continue a second nonhost carrier
- preserve unresolved incident parts

## Part IV. Cusp, Branch, `T`, And Cross

### CR16. Cusp

A cusp is a same-carrier through continuation whose local tangent changes at the site.
A cusp does not by itself create a new carrier.

### CR17. Branch

A branch is a site where one carrier continues and a distinct carrier is also incident.
The distinct carrier may be emitted, admitted, or terminated there.

### CR18. One-Sided Branch

A one-sided branch is a branch in which only one distinct side-carrier is present in addition to the continuing carrier.

### CR19. `T`

A `T` is a site where one carrier continues through while another carrier terminates into it or begins from it without itself continuing through the site.

### CR20. True Cross

A true cross is a site where two distinct carriers both continue through the site.
In a planar fold this may look like a plus-sign, but its truth is structural, not pictorial.

### CR21. Cross-Shaped Proposal Is Not Yet A True Cross

If a host carrier continues and two orthogonal side proposals are present but not yet explicitly unified into one distinct through-carrier, the site is not yet a true cross.
It is a cross-shaped local proposal.

### CR22. Orthogonal Side Pair Becomes A Cross Carrier By Shared Binding

If two opposite orthogonal incident parts are bound to the same distinct carrier and routed through the site, they constitute the second through-carrier of a true cross.

### CR23. A `T` And A Cross Must Differ Structurally

A `T` and a true cross may look similar under some local partial views.
They differ because:

- in a `T`, one carrier terminates or begins at the site
- in a cross, both carriers continue through

### CR24. Branch By Cusp Differs From Branch By Through-Continuation

If a site remains same-carrier but changes tangent sharply, that is a cusp.
If a distinct carrier is also present, that is a branch or richer site.
These must not be conflated.

## Part V. New Carriers

### CR25. New Carrier Creation Is Not Equivalent To Curvature

A new carrier is not created merely because a path bends or curves.
Otherwise every smooth contour would fragment into several carriers.

### CR26. New Carrier Creation Comes From Distinct Binding

A new carrier appears when an incident part is explicitly bound as distinct from the continuing carrier at the site.

### CR27. Same Carrier May Traverse Many Local Shapes

One carrier may lawfully pass through:

- straight segments
- cusps
- arcs
- curves
- loops
- folded returns

without becoming several carriers merely because the fold changes shape.

### CR28. Emitted Carrier Need Not Be Straight

An emitted carrier may later fold into a line, curve, bridge, bowl, or lifted path.
Its carrier identity is independent of that later fold.

## Part VI. Options At A Site

### CR29. Routing Determines Available Next Moves

The next lawful moves available at a site are determined by routing, not just by how many rays are visible.

### CR30. Same Visible Geometry May Offer Different Structural Options

Two sites that draw similarly may still differ if one routes through the host and the other routes into a different carrier.

### CR31. Cusp Offers One Same-Carrier Continuation

A pure cusp offers continued travel on the same carrier with changed tangent.

### CR32. Branch Offers Continuation Plus Distinct Carrier Access

A branch may offer:

- continued travel on the current carrier
- transfer into a distinct carrier

depending on the governing continuation law.

### CR33. `T` Offers Through-Carrier Continuation And Side Access

A `T` may offer:

- continued travel on the through-carrier
- access to the terminating or beginning side-carrier

subject to direction and law.

### CR34. True Cross Offers Two Through-Carriers

A true cross may offer:

- travel continuing on carrier `A`
- transfer into carrier `B`
- travel continuing on carrier `B`
- transfer into carrier `A`

depending on how traversal interprets site routing and tension.

### CR35. Display Choices Must Not Erase Structural Multiplicity

Even if one fold shows one preferred route or contour, the full routing multiplicity of the site should remain structurally available.

## Part VII. Consequences For UI

### CR36. Side Sign And Side Binding Differ

Changing the sign of a side and choosing what carrier that side belongs to are different operations.
The UI should not collapse them into one control.

### CR37. Host, New, Link Is A Lawful Side-Binding Vocabulary

A useful initial UI vocabulary for each side is:

- `Host`: this side belongs to the host carrier
- `New`: this side creates or binds to a distinct new carrier
- `Link`: this side binds to an already existing carrier

This is a UI vocabulary, not necessarily the final Core 2 primitive naming.

### CR38. True Cross Needs Shared Binding Plus Through Routing

A true cross cannot be expressed in UI by orthogonal arrows alone.
The UI must let opposite side proposals share one distinct carrier and mark that carrier as through-continuing at the site.

### CR39. Curves Are Fold Choices

When the UI later shows curved contours, those curves are fold choices of carrier routing and identity, not the primitive truth of the site.

## Open Questions

1. Should routing live directly on `CarrierPinSite`, or in a distinct site-topology layer derived from it?
2. Should host through-continuation be explicit always, or defaulted and overridden only when broken?
3. How much routing should be directional, and how much should be neutral structure plus later traversal law?
4. When two unbound orthogonal sides are present, should the UI show them as latent half-branches or as one temporary unresolved orthogonal family?
5. Should a later richer site support more than one nonhost through-carrier, or should that be composed from several simpler sites first?

## Short Recommendation

Keep carrier identity separate from geometry, and add explicit continuation routing at a site.
That lets Core 2 represent cusp, branch, `T`, and true cross cleanly while preserving the current pinning and carrier-identity work.
