# Branch Graph Rules

These are the additional rules implied by the Phase 2 Core 2 branch graph implementation.

## Proposed Appendix Rules

1. **A Branch Graph Is History, Not Only Multiplicity**  
   A branch graph records the ordered history of branch families under continuation.  
   It preserves how branching, continuation, and rejoining unfold across events.

2. **A Frontier Marks Current Activity**  
   The frontier is the set of currently active branch nodes after an event.  
   Selection may mark one frontier node without deleting the others.

3. **Events And Nodes Are Distinct**  
   An event is a branching moment.  
   A node is a concrete member produced at some event.

4. **Edges Preserve Structural Relation**  
   Parent-child edges record how later nodes arise from earlier ones.  
   Edge kinds distinguish continuation, split, rejoin, merge, and lift.

5. **Rejoin Preserves Provenance**  
   When several parents lead to one child, the graph preserves all parents even if display later shows one node.

6. **Selection Is A Graph Event**  
   Selection may update the active continuation without creating a new structural node.

7. **Crossing Does Not Imply Joining**  
   Two paths may cross in display without forming a branch node.  
   A join must be explicit in the graph.

8. **Standalone Graphs May Rewire To A Source Frontier**  
   When a result is displayed without its full prior history, the graph may attach the result family to an explicit source frontier for standalone use.

## Quick Examples

- A seed value followed by square-root candidates forms one root node and two split children.
- Two active branches may later rejoin into one node with two incoming edges.
- A boolean segment split yields several co-present frontier nodes at once.
- A manual selection event may change the preferred continuation while leaving the full frontier intact.
