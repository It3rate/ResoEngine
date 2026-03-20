# Dynamic Rules

These are the additional rules implied by the first Core 2 dynamic execution implementation.

## Proposed Appendix Rules

1. **Dynamic Context Has Two Layers**  
   A dynamic context consists of:
   - a state
   - an environment  
   The environment is not identical to the state. It is the framing or surrounding condition within which the state gains executable meaning.

2. **Strands Generate Proposals**  
   A dynamic strand does not directly determine the next state.  
   It generates one or more proposals for what might happen next.

3. **A Proposal May Be Partial**  
   A proposal need not solve the whole step by itself.  
   It may contribute only part of the next action, transform, or computation.

4. **Resolution Is Required**  
   When several strands act in parallel, the system must resolve their proposals before committing the next context.

5. **Resolution May Return A Branch Family**  
   Dynamic resolution is not limited to a single next state.  
   It may return:
   - one committed next context
   - several co-present next contexts
   - several alternative next contexts
   - preserved tension without commitment
   - a lifted continuation

6. **Tension Is Executable Information**  
   Dynamic tension may come from:
   - conflict between proposals
   - conflict with the environment
   - structural invalidity
   - aesthetic pressure
   - unresolved multiplicity  
   Tension is part of execution, not merely a report after the fact.

7. **Environment Feeds Back**  
   The committed result of one step may alter the environment, and the altered environment may change what later strands propose or what the resolver prefers.

8. **A Dynamic Trace Records Becoming**  
   A dynamic trace is the ordered history of proposal, tension, resolution, and commitment.

9. **The Branch Graph Records Structural Multiplicity**  
   The branch graph is not the dynamic law itself.  
   It is the structural history of how dynamic resolution split, continued, selected, or rejoined.

10. **Convergence Is A Lawful Stop Condition**  
    A dynamic system may stop by:
    - fixed step bound
    - empty frontier
    - stable orbit
    - repeated frontier
    - sufficiently resolved tension  
    The stop condition is part of the dynamic law.

## Quick Examples

- Several geometric strands may jointly propose horizontal drift, vertical pulse, and backtrack.
- A resolver may combine those partial proposals into one committed path step.
- If two equal continuations remain viable, the resolver may return both as a branch family rather than collapsing them immediately.
- A path that occupies space may change the environment so later steps avoid overlap.
