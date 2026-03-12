using System;
using System.Collections.Generic;
using System.Text;

namespace ResoEngine.Support;


/// <summary>
/// Encodes one term of an algebraic multiplication rule.
/// Sign is derivable from chirality via Deferred Opposition: (-1)^(nConPairs).
/// Retained as a field for custom algebras that may not follow chirality rules.
/// </summary>
public readonly record struct AlgebraEntry(int LeftIndex, int RightIndex, int ResultIndex, int Sign)
{
    public override string ToString() =>
        $"c[{LeftIndex}] × c[{RightIndex}] → {(Sign >= 0 ? "+" : "−")}c[{ResultIndex}]";
}