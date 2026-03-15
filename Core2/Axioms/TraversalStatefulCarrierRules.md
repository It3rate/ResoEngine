# Traversal and Stateful Carrier Rules

These are the additional rules implied by the current Core 2 traversal and equation-driven path execution model.

## Proposed Appendix Rules

1. **Traversal Is Stateful**  
   A traversal is not memoryless.  
   It retains current value, continuation law, and facing or phase between firings.

2. **Traversal Definition**  
   A traversal definition may include:
   - a frame, if bounded continuation is desired
   - a step
   - a continuation law
   - a seed or initial value

3. **Seed Is Part Of The Lawful State**  
   A nonzero seed is not merely display setup.  
   It changes the lawful path of later firings.

4. **Boundary Law Persists Across Firings**  
   Reflect, wrap, clamp, and tension-preserving continuation are not one-time adjustments.  
   They govern every later firing unless an explicit law-change occurs.

5. **Facing Persists In Reflective Traversal**  
   Under reflective continuation, a traversal keeps its current facing until it reaches a boundary and reverses.  
   Later firings therefore depend on where and how earlier firings ended.

6. **Bounded And Unbounded Traversals Differ**  
   A traversal may use its carrier as a frame, or it may continue without a local frame.  
   This distinction is part of the traversal definition, not an afterthought.

7. **Step Scale Is Part Of The Carrier**  
   A traversal may advance by unit step, span step, or another lawful step size.  
   The amount advanced on each firing is part of the carrier definition.

8. **Equation Identity Is Stateful**  
   Two firings of the same named equation need not produce the same result.  
   The equation name identifies the carrier law, but the visible outcome depends on current internal state.

9. **Offset Carrier Rule**  
   When a carrier begins away from zero, that offset belongs to the traversal itself.  
   It is not merely a one-time pre-roll before the real behavior begins.

10. **Hidden Lead Under Path Interpretation**  
    Under a path interpretation, the route from zero to the carrier start may be treated as a hidden lead.  
    A hidden lead repositions the cursor without drawing.

11. **Visible Span Under Path Interpretation**  
    Under the same interpretation, the route from carrier start to carrier end is the visible span unless another law says otherwise.

12. **Hidden Lead Repeats With The Carrier**  
    If a carrier has a hidden lead, that hidden lead is part of every lawful firing.  
    It is not consumed after the first use.

13. **Commit Materializes Visible Accumulation**  
    A commit closes the currently visible accumulation into a path result, but it does not reset the internal state of the traversals that produced it.

14. **Multiple Fires May Share One Commit**  
    Several equations may fire before one commit.  
    Their combined visible effect then constitutes one grouped step or tick.

15. **Final Traversal Principle**  
    Stateful carriers make repetition executable.  
    The same carrier may reflect, wrap, continue, delay, or leave gaps without changing identity, because its current state is part of what the carrier is.

## Quick Examples

- A reflective carrier on `[0, 1]` with unit step alternates direction on every firing.
- A reflective carrier on `[0, 2]` with unit step advances across two ticks before reversing.
- A continuous carrier on `[0, 2]` with span step advances by `2` each firing without local bounce.
- A carrier on `[-1, 1]` may first reposition silently to `-1`, then draw to `1`; on later firings that silent-plus-visible structure remains part of the same equation.
