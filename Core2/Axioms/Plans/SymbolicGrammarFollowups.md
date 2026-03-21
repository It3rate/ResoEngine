# Symbolic Grammar Follow-Ups

This note records important questions that arose while shaping the first symbolic grammar proposal but should not block the first implementation pass.

It is meant to keep the direction visible without forcing premature closure.

## 1. Higher structural degrees

Open question:

- how explicitly should `Volume` and higher degrees be named before the symbolic layer is implemented?

Current guidance:

- the grammar should be degree-general now
- implementation may still begin with `Scalar`, `Proportion`, `Axis`, `Area`, and quantity wrappers
- pinning and folding should not be coded as if `Area` were the final degree

## 2. Generalized pin raising and fold lowering

Open question:

- should the law "pinning may raise degree, folding may lower degree" be made explicit in Core 2 before expressions land?

Current guidance:

- this should be treated as a real direction of the system
- but the first symbolic layer can proceed without a fully generalized higher-degree runtime

## 3. Boolean symbolic forms

Open question:

- what is the right first-class symbolic form for boolean combination in Core 2 native terms?

Current guidance:

- do not hide boolean work inside generic transform composition
- treat it as structural comparison/partition/fold

## 4. Branch weighting versus execution bias

Open question:

- when the same native parameter is read structurally in one domain and probabilistically or procedurally in another, what is the cleanest common symbolic representation?

Current guidance:

- keep one native parameter vocabulary
- let the active law or fold determine how that vocabulary is realized

## 5. Distributed participant evaluation

Open question:

- what is the best symbolic representation for many structures each contributing their own fixed points, agnosticism, preferences, and certainties?

Current guidance:

- preserve local proposals before compromise
- do not collapse everything into one anonymous objective score too early

## 6. Applied anchor selection

Open question:

- when an applied structure has several internal pins or landmarks, how should the symbolic layer name which one is being attached?

Current guidance:

- first pass may assume host position plus applied origin
- later passes should support explicit anchor naming or pin-to-pin attachment

## 7. Pin-to-pin versus pin-to-carrier

Open question:

- should explicit pin-to-pin attachment be treated as a later refinement of the default pin-to-carrier model, or as an equally primitive symbolic form from the start?

Current guidance:

- keep the first pass simple
- preserve room for explicit pin-to-pin syntax later

## 8. Support growth, resolution, and canonical simplification

Open question:

- when symbolic multiplication, opposition, or powers compose `Proportion` supports, when should the system preserve raw denominator growth, and when should it display or adopt a more canonical resolution?

Current guidance:

- do not silently discard support information just to imitate ordinary scalar arithmetic
- treat denominator growth as a real structural/resolution question, closer to error-bound or precision composition than to ordinary integer multiplication
- revisit whether special canonical transforms such as opposition should preserve semantic simplicity more directly than generic algebra-table multiplication
- defer a full policy decision until the symbolic page conversions have exposed a few more concrete cases
