# Multiscale Resolution and Layered Reading Rules

These are the additional rules implied by the current Core 2 multiscale resolution implementation.

## Proposed Appendix Rules

1. **Resolution Frame**  
   A resolution frame defines a grain at which a quantity may be read.  
   The frame carries:
   - a signature
   - a grain
   - an optional named unit or symbol

2. **Resolution Ladder**  
   A resolution ladder is an ordered family of resolution frames sharing one signature and descending from coarser grain to finer grain.

3. **Layered Quantity**  
   A layered quantity is an exact quantity represented as a sum of signed counts across one or more resolution frames.

4. **Signed Digits Are Allowed**  
   Resolution counts need not be only nonnegative digits.  
   A layered quantity may use signed counts when that representation is structurally useful.

5. **Exact Fold**  
   Folding a layered quantity means summing all frame-count contributions into one exact quantity in the shared signature.

6. **Exact Decomposition**  
   A quantity may be decomposed across a resolution ladder into frame counts.  
   This creates a place-value-like representation at several grains.

7. **Coarse Reading**  
   Reading at a chosen frame does not require discarding exact structure.  
   The system may compute:
   - the raw tick count at that grain
   - the quantized tick count
   - the representative quantity at that grain
   - the residual detail not captured by that reading

8. **Quantization Rule**  
   A coarse reading depends on a quantization rule such as nearest, floor, ceiling, toward-zero, or away-from-zero.

9. **Fine Detail May Collapse To Zero**  
   When a quantity is read in a coarse frame, contributions from finer frames may collapse to zero at that grain even though they remain present in the exact folded quantity.

10. **Refinement And Collapse**  
    Refinement introduces or preserves finer-frame detail.  
    Collapse re-expresses the same quantity at a coarser frame, possibly with residual detail omitted from the representative reading.

11. **Representative And Residual Differ**  
    A representative reading is the quantity expressed at the chosen grain.  
    A residual is the remaining exact detail not captured by that representative.

12. **Current Core 2 Scope**  
    The current implementation begins with scalar quantities paired with unit signatures.  
    Higher-structure multiscale readings may be added later.

## Quick Examples

- `109 miles` may be decomposed as `1 x 100-mile + 9 x 1-mile`.
- The signed-digit form `1, 0, -2` in base `10` folds to `98`.
- Reading `101 miles` at the `100-mile` frame may yield the representative `100 miles` with residual `1 mile`.
- A town about `100 miles` away and a stop sign `1 mile` outside the town still read as about `100 miles` in the `100-mile` frame.
