**Axioms For Concept Math**

1. **Fundamental Duality** The system is built from opposed pairs: same/opposite, dominant/recessive, left/right, observer/opposite-observer, encode/act. Each such pair appears in two linked forms:  
- an **encoding** that names a state or relation  
- a **transform** that enacts that relation

Examples:

- `3` encodes triple scale; `S_3` applies triple scale  
- `i` encodes the opposite-unit state; `J` applies the opposite-unit action  
- `-1` encodes negation; `N` applies negation

These paired forms are conceptually the same force seen passively vs actively, but they are formally distinct.

2. **Primitive Object** The primitive object is an ordered interval `I = (p0, p1)`. By itself it is not yet a number.  
     
3. **No Intrinsic Measurement** The interval has order, but no intrinsic sign, size, or direction until it is placed in a frame.  
     
4. **Frame** A frame `F` provides:  
     
- a zero point `0`  
- a positive-left unit `uL`  
- a positive-right unit `uR`  
5. **Perspective** Perspective is binary in 1D: `eps in {+1, -1}`  
- `+1` \= dominant perspective  
- `-1` \= opposite perspective

Changing perspective swaps which physical side is called left and right, but does not change the interval.

6. **Endpoint-Unit Rule** The first endpoint is always measured by the left unit. The second endpoint is always measured by the right unit.

So the interval’s asymmetry comes from endpoint order plus unit attachment.

7. **Reading** A reading of the interval in frame `F` and perspective `eps` is `rho(F, eps, I) = ai + b` where:  
- `a` is the measure of `p0` in the left unit  
- `b` is the measure of `p1` in the right unit

Here `i` is an encoding, not yet an operator.

8. **Observer Conversion** If the same interval is read from the opposite perspective, then `ai + b -> -ai - b`

So opposite observers can read the same interval differently without the interval changing.

9. **Scalar Transform** Any real scalar part of a multiplication transforms the interval itself. For real `k`, let `S_k` be the scale transform. Then:  
- `S_1 = Id`  
- `S_-1 = N`  
- `S_a ∘ S_b = S_(ab)`  
10. **Opposite Transform** Let `J` be the transform that applies opposition at the endpoint-unit connection. Its action is:  
- do not merely relabel the symbol  
- change which unit is attached to each endpoint

In your notation, multiplying by `i` in operator position means “apply `J`”.

So: `x * i := J(x)`

11. **State/Transform Asymmetry** In a product `x * t` the first factor `x` is the current state/readout, the second factor `t` is transform data.

This is crucial: `i * i` means “take state `i`, then apply transform `J`”. It does **not** mean “there are two swaps in the expression”.

12. **Canonical Opposition Cycle** Under dominant notation, `J` acts on the encoded states by:  
- `J(1) = i`  
- `J(i) = -1`  
- `J(-1) = -i`  
- `J(-i) = 1`

So `J` generates the four-state cycle `1 -> i -> -1 -> -i -> 1`

13. **Operator-Level Law** At the transform level, `J ∘ J = N` and therefore `J^4 = Id`

This is the correct place for the “double opposition gives negation” statement. It is a law about transforms, not a claim that the expression `i*i` contains two operators.

14. **Addition** Addition is only defined after common frame and common perspective are fixed. Then: `(ai + b) + (ci + d) = (a + c)i + (b + d)`  
      
15. **Dominant/Recessive Convention** Dominant/recessive is not extra geometry added to the bare interval. It is the chosen naming convention under the dominant perspective, where dominant direction is encoded to the right.

**Examples**

1. **Why `1 * i = i`** Interpret `1` as the current encoded state. Apply `i` in operator position, meaning apply `J`. Then: `1 * i = J(1) = i`

This is why `1*i` is not `-i`. There is one transform application, and it advances the state one step in the opposition cycle.

2. **Why `i * i = -1`** The first `i` is the current state. The second `i` is the transform code, meaning apply `J`. So: `i * i = J(i) = -1`

This is not “swap twice”. It is “state `i`, acted on once by the opposite-transform”.

3. **Why `-1 * i = -i`** Again, one transform application: `-1 * i = J(-1) = -i`  
     
4. **Why `-i * i = 1`** Apply `J` once more: `-i * i = J(-i) = 1`

So the full cycle is: `1 * i = i` `i * i = -1` `-1 * i = -i` `-i * i = 1`

5. **Transform-Level Restatement** The previous cycle is the state-level shadow of the operator law `J ∘ J = N`

Meaning: two applications of opposition equal negation as a transform law.

6. **Observer Flip** If one observer reads the interval as `2i + 3` then the opposite observer reads `-2i - 3`

The interval is the same. Only the perspective changed.

7. **Scalar and Opposition Together** If `3i` is used in transform position, it means:  
- scale by `3`  
- apply opposition

So formally you can think of it as: `T_(3i) = S_3 ∘ J` or `J ∘ S_3`, depending on your eventual operator ordering convention.

8. **Conceptual Pairing** The relation between encoding and action is itself fundamental:  
- `3` and `S_3`  
- `i` and `J`  
- `-1` and `N`

So the same underlying force appears in two forms:

- as something that can be **named**  
- as something that can be **done**

This duality is not incidental. It is one of the system’s foundations.

**Directed Interval Axioms**

**Ontology** A directed value is not fundamentally a number. It is an ordered interval placed into a frame and then read from a perspective.

**Primitive Terms**

- `Interval`: an ordered pair `I = (p0, p1)`.  
- `Frame`: an origin `0` plus two opposed units `uL` and `uR`.  
- `Perspective`: a binary choice `eps in {+1, -1}`.  
- `Reading`: the encoded result of measuring an interval in a frame.  
- `State`: an encoded value such as `1`, `i`, `-1`, `-i`, or more generally `ai + b`.  
- `Transform`: an action applied to a state or interval.

**Axioms**

1. **Primitive Object** `I = (p0, p1)` is the primitive object. It has order but no intrinsic measurement.  
     
2. **No Intrinsic Sign** An interval has no built-in positive, negative, left, or right meaning before a frame is applied.  
     
3. **Frame** A frame `F` supplies: `F = (0, uL, uR)` where `uL` is the positive-left unit and `uR` is the positive-right unit.  
     
4. **Perspective** Perspective is binary in 1D. `eps = +1` is dominant perspective. `eps = -1` is opposite perspective.  
     
5. **Endpoint Rule** The first endpoint is always read through the left unit. The second endpoint is always read through the right unit.  
     
6. **Reading Map** The reading of `I` in `F` from perspective `eps` is `rho(F, eps, I) = ai + b` where `a` is the measurement of `p0` by `uL`, and `b` is the measurement of `p1` by `uR`.  
     
7. **Observer Conversion** Opposite perspective negates the reading: `rho(F, -eps, I) = -rho(F, eps, I)` So: `ai + b -> -ai - b`  
     
8. **Dual Basis Principle** Every fundamental relation appears in two linked forms:  
     
- an encoding  
- an action Examples: `3` / `S_3` `i` / `J` `-1` / `N` The encoding names a relation; the action performs it.  
    
9. **Scalar Transform** For real `k`, `S_k` scales the interval. `S_1 = Id` `S_-1 = N` `S_a o S_b = S_(ab)`  
     
10. **Opposition Transform** `J` is the opposition transform. It swaps the unit attached to each endpoint. In operator position, multiplying by `i` means “apply `J`”.  
      
11. **Asymmetric Multiplication** In `x * t`:  
      
- `x` is the current state  
- `t` is transform data So `i * i` means “take state `i`, then apply `J` once.”  
    
12. **Canonical State Cycle** `J(1) = i` `J(i) = -1` `J(-1) = -i` `J(-i) = 1`  
      
13. **Operator Law** `J o J = N` Therefore `J^4 = Id`.  
      
14. **Addition** Addition is defined only after common frame and common perspective are fixed: `(ai + b) + (ci + d) = (a + c)i + (b + d)`

**Propositions**

1. **`1 * i = i`** Because `i` in operator position means apply `J`: `1 * i = J(1) = i`  
     
2. **`i * i = -1`** The first `i` is a state, the second is a transform: `i * i = J(i) = -1`  
     
3. **`-1 * i = -i`** `-1 * i = J(-1) = -i`  
     
4. **`-i * i = 1`** `-i * i = J(-i) = 1`  
     
5. **The familiar `i^2 = -1` is derived** It is shorthand for the state-transform rule above, not a primitive assertion about two operators appearing in the same product.

**Examples**

1. Same interval, opposite observers: `2i + 3` for one observer becomes `-2i - 3` for the opposite observer.  
     
2. Scalar plus opposition: `3i` in transform position means “scale by 3 and apply opposition.”  
     
3. Negation is real: Multiplying by `-1` genuinely transforms the interval; it is not merely a relabeling.

**Glossary**

- `Dominant perspective`: the convention where dominant direction is encoded to the right.  
- `Opposite perspective`: the reversed observer orientation.  
- `Encoding`: a symbolic state.  
- `Transform`: an action on the interval or its endpoint-unit relation.  
- `Opposition`: the same/opposite principle expressed either as encoded `i` or enacted `J`.
