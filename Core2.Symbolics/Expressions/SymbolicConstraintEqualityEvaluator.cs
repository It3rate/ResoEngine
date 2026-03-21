using Core2.Branching;

namespace Core2.Symbolics.Expressions;

internal static partial class SymbolicConstraintRelationEvaluator
{
    private static ConstraintRelationAssessment EvaluateEquality(
        EqualityTerm equality,
        ISymbolicStructuralContext? structuralContext)
    {
        if (Equals(equality.Left, equality.Right))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied);
        }

        if (TryEvaluateStructuralAnchorEquality(equality.Left, equality.Right, structuralContext, out var structuralAssessment))
        {
            return structuralAssessment;
        }

        if (TryEvaluateAlternativeEquality(equality.Left, equality.Right, out var branchAssessment))
        {
            return branchAssessment;
        }

        if (!SymbolicConstraintTermSupport.IsClosed(equality.Left) || !SymbolicConstraintTermSupport.IsClosed(equality.Right))
        {
            return new ConstraintRelationAssessment(ConstraintTruthKind.Unresolved, null, "Equality requires closed terms or explicit branch selection.");
        }

        return Equals(equality.Left, equality.Right)
            ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied)
            : new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied);
    }

    private static bool TryEvaluateStructuralAnchorEquality(
        SymbolicTerm left,
        SymbolicTerm right,
        ISymbolicStructuralContext? structuralContext,
        out ConstraintRelationAssessment assessment)
    {
        if (left is not AnchorReferenceTerm leftAnchor || right is not AnchorReferenceTerm rightAnchor)
        {
            assessment = null!;
            return false;
        }

        if (structuralContext is null)
        {
            assessment = new ConstraintRelationAssessment(
                ConstraintTruthKind.Unresolved,
                null,
                "Anchor equality requires structural carrier context or explicit binding.");
            return true;
        }

        bool hasLeft = structuralContext.TryResolveAnchorCarrier(leftAnchor, out var leftCarrier, out var leftNote);
        bool hasRight = structuralContext.TryResolveAnchorCarrier(rightAnchor, out var rightCarrier, out var rightNote);

        if (hasLeft && hasRight)
        {
            assessment = leftCarrier == rightCarrier
                ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied, null, "Anchors resolve to the same structural carrier.")
                : new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied, null, "Anchors resolve to different structural carriers.");
            return true;
        }

        assessment = new ConstraintRelationAssessment(
            ConstraintTruthKind.Unresolved,
            null,
            leftNote ?? rightNote ?? "Anchor equality requires structural carrier context or explicit binding.");
        return true;
    }

    private static bool TryEvaluateAlternativeEquality(
        SymbolicTerm left,
        SymbolicTerm right,
        out ConstraintRelationAssessment assessment)
    {
        if (TryEvaluateAlternativeCandidateFamily(left, right, out assessment))
        {
            return true;
        }

        if (TryEvaluateAlternativeCandidateFamily(right, left, out assessment))
        {
            return true;
        }

        assessment = null!;
        return false;
    }

    private static bool TryEvaluateAlternativeCandidateFamily(
        SymbolicTerm branchCandidate,
        SymbolicTerm other,
        out ConstraintRelationAssessment assessment)
    {
        if (branchCandidate is not BranchFamilyTerm branch ||
            branch.Family.Semantics != BranchSemantics.Alternative)
        {
            assessment = null!;
            return false;
        }

        if (branch.Family.Selection.HasSelection &&
            branch.Family.TryGetSelectedMember(out var selectedMember) &&
            selectedMember is not null &&
            SymbolicConstraintTermSupport.IsClosed(selectedMember.Value) &&
            SymbolicConstraintTermSupport.IsClosed(other))
        {
            assessment = Equals(selectedMember.Value, other)
                ? new ConstraintRelationAssessment(ConstraintTruthKind.Satisfied, null, "Selected branch satisfies the equality.")
                : new ConstraintRelationAssessment(ConstraintTruthKind.Unsatisfied, null, "Selected branch does not satisfy the equality.");
            return true;
        }

        var matchingMembers = branch.Family.Members
            .Where(member => SymbolicConstraintTermSupport.IsClosed(member.Value) && SymbolicConstraintTermSupport.IsClosed(other) && Equals(member.Value, other))
            .ToArray();

        if (matchingMembers.Length > 0)
        {
            var candidateFamily = BranchFamily<ValueTerm>.FromMembers(
                branch.Family.Origin,
                branch.Family.Semantics,
                branch.Family.Direction,
                matchingMembers,
                BranchSelection.None,
                branch.Family.Tensions,
                branch.Family.Annotations);

            assessment = new ConstraintRelationAssessment(
                ConstraintTruthKind.Unresolved,
                candidateFamily,
                "Alternative branch family contains satisfying candidates but no single candidate is committed.");
            return true;
        }

        if (branch.Family.Values.All(SymbolicConstraintTermSupport.IsClosed) && SymbolicConstraintTermSupport.IsClosed(other))
        {
            assessment = new ConstraintRelationAssessment(
                ConstraintTruthKind.Unsatisfied,
                null,
                "No branch candidate satisfies the equality.");
            return true;
        }

        assessment = new ConstraintRelationAssessment(
            ConstraintTruthKind.Unresolved,
            null,
            "Alternative branch family requires more reduction or selection before equality can be decided.");
        return true;
    }
}
