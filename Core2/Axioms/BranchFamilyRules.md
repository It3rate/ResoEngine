# Branch Family Rules

These are the additional rules implied by the Phase 1 Core 2 branching implementation.

## Proposed Appendix Rules

1. **Branch Families Are Local Multiplicity Events**  
   A branch family records the valid multiplicity produced by one lawful event.  
   It does not yet encode the full long-range branch graph.

2. **Every Branch Family Carries Metadata**  
   A branch family retains:
   - origin
   - semantics
   - direction
   - member set
   - selection state
   - tensions
   - annotations

3. **Alternative And Co-Present Branches Differ**  
   Alternative branches compete for the same role and may require selection.  
   Co-present branches jointly constitute the result and should remain together unless a later law folds them.

4. **Selection Does Not Erase The Family**  
   Selecting a principal branch marks one member for continuation or display, but the full family remains present.

5. **Branch Members Carry Provenance**  
   Each branch member may retain parent branch identifiers from the prior multiplicity event.  
   This allows later continuations to preserve how candidates were produced and where rejoining occurred.

6. **Projection Preserves Branch Identity When Possible**  
   When a branch family is mapped into another domain, member identity, parent links, selection, and annotations should be preserved unless the law explicitly merges or discards them.

7. **Merging Requires An Explicit Rejoin**  
   If distinct parents yield the same projected result, the projected member may rejoin them into one value while retaining all contributing parent identifiers.

8. **Inverse Continuation Uses Alternative Reverse Families**  
   Inverse continuation produces an alternative branch family in reverse direction.  
   Roots are the basic example.

9. **Boolean Segment Splits Use Co-Present Structural Families**  
   Directed segment boolean splitting produces a co-present branch family in structural direction.  
   The pieces are jointly the result, not competing alternatives.

10. **Fractional Powers May Carry Preimage Provenance Forward**  
    When a fractional power depends on inverse continuation, the resulting forward branch family may preserve the parent roots that generated each candidate.

## Quick Examples

- `x^2 = 4` produces two reverse candidates, `2` and `-2`, in one alternative family.
- Removing a middle segment from a longer segment produces two co-present pieces in one structural family.
- A fractional power such as a square root may select one principal branch for display while still retaining both candidates.
- If two earlier branches later project to the same value, the projected value may record both parents as a rejoin.
