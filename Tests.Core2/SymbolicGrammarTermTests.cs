using Core2.Boolean;
using Core2.Elements;
using Core2.Branching;
using Core2.Symbolics.Expressions;

namespace Tests.Core2;

public class SymbolicGrammarTermTests
{
    [Fact]
    public void ElementLiteral_WrapsNativeCore2Value()
    {
        var term = new ElementLiteralTerm(Axis.One);

        Assert.Equal(SymbolicTermSort.Value, term.Sort);
        Assert.Equal(Axis.One, term.Value);
    }

    [Fact]
    public void TransformLiteral_UsesNativeCore2TransformCode()
    {
        var transform = new TransformLiteralTerm(Axis.I);
        var state = new ElementLiteralTerm(Axis.One);
        var applied = new ApplyTransformTerm(state, transform);

        Assert.Equal(SymbolicTermSort.Transform, transform.Sort);
        Assert.Equal(Axis.I, transform.Code);
        Assert.Equal(state, applied.State);
        Assert.Equal(transform, applied.Transform);
        Assert.Equal(SymbolicTermSort.Value, applied.Sort);
    }

    [Fact]
    public void PinTerm_PreservesHostAppliedAndProportionPosition()
    {
        var host = new ElementLiteralTerm(Axis.One);
        var applied = new ElementLiteralTerm(Axis.I);
        var position = new Proportion(1, 2);
        var pin = new PinTerm(host, applied, position);

        Assert.Equal(host, pin.Host);
        Assert.Equal(applied, pin.Applied);
        Assert.Equal(position, pin.Position);
        Assert.Equal(SymbolicTermSort.Value, pin.Sort);
    }

    [Fact]
    public void PreferenceTerm_UsesNativeProportionWeight()
    {
        var relation = new EqualityTerm(new ReferenceTerm("crossbar", SymbolicTermSort.Value), new ReferenceTerm("midline", SymbolicTermSort.Value));
        var preference = new PreferenceTerm(relation, new Proportion(2, 1), "glyph");

        Assert.Equal(SymbolicTermSort.Constraint, preference.Sort);
        Assert.Equal(new Proportion(2, 1), preference.Weight);
        Assert.Equal(relation, preference.Relation);
        Assert.Equal("glyph", preference.ParticipantName);
    }

    [Fact]
    public void ReferenceTerm_PreservesReferencedSort()
    {
        var reference = new ReferenceTerm("P4.u", SymbolicTermSort.Value);

        Assert.Equal("P4.u", reference.Name);
        Assert.Equal(SymbolicTermSort.Value, reference.Sort);
        Assert.Equal(SymbolicTermSort.Value, reference.ReferencedSort);
    }

    [Fact]
    public void FoldTerm_PreservesRequestedFoldKind()
    {
        var source = new ElementLiteralTerm(new Area(Axis.One, Axis.I));
        var fold = new FoldTerm(source, SymbolicFoldKind.StructurePreserving);

        Assert.Equal(source, fold.Source);
        Assert.Equal(SymbolicFoldKind.StructurePreserving, fold.Kind);
        Assert.Equal(SymbolicTermSort.Value, fold.Sort);
    }

    [Fact]
    public void BranchFamilyTerm_WrapsNativeBranchFamilyOfValueTerms()
    {
        var first = new ElementLiteralTerm(Axis.One);
        var second = new ElementLiteralTerm(Axis.I);
        var family = BranchFamily<ValueTerm>.FromValues(
            BranchOrigin.Continuation,
            BranchSemantics.Alternative,
            BranchDirection.Forward,
            [first, second],
            selectedIndex: 1,
            selectionMode: BranchSelectionMode.Principal);
        var term = new BranchFamilyTerm(family);

        Assert.Equal(family, term.Family);
        Assert.Equal(second, term.Family.SelectedValue);
        Assert.Equal(SymbolicTermSort.Value, term.Sort);
    }

    [Fact]
    public void Formatter_UsesCompactCore2NativeShorthand()
    {
        var formatted = SymbolicTermFormatter.Format(
            new ApplyTransformTerm(
                new ElementLiteralTerm(Axis.One),
                new TransformLiteralTerm(Axis.I)));

        Assert.Equal("1 * i", formatted);
    }

    [Fact]
    public void MultiplyValuesTerm_FormatsAsNativeInfixProduct()
    {
        var term = new MultiplyValuesTerm(
            new ElementLiteralTerm(new Axis(new Proportion(3, 1), new Proportion(2, 1))),
            new ElementLiteralTerm(new Axis(new Proportion(4, 1), new Proportion(5, 1))));

        Assert.Equal("[3/1]i + [2/1] * [4/1]i + [5/1]", SymbolicTermFormatter.Format(term));
    }

    [Fact]
    public void SequenceTerm_PreservesProgramOrderAndFormatsCompactly()
    {
        var sequence = new SequenceTerm(
        [
            new BindTerm("stem", new ElementLiteralTerm(Axis.One)),
            new BindTerm("turn", new TransformLiteralTerm(Axis.I)),
        ]);

        Assert.Equal(2, sequence.Steps.Count);
        Assert.Equal("let stem = 1; let turn = i", SymbolicTermFormatter.Format(sequence));
    }

    [Fact]
    public void Parser_AllowsTrailingExpressionAfterProgramSequence()
    {
        var parsed = SymbolicParser.Parse("let A = 1; let B = i; A == B");

        var sequence = Assert.IsType<SequenceTerm>(parsed);
        Assert.Equal(3, sequence.Steps.Count);
        Assert.IsType<EmitTerm>(sequence.Steps[2]);
        Assert.Equal("let A = 1; let B = i; A == B", SymbolicTermFormatter.Format(sequence));
    }

    [Fact]
    public void Parser_ParsesCommitProgramForm()
    {
        var parsed = SymbolicParser.Parse("commit choice = constraints{prefer(glyph, branch{1 | i} == i, 2/1)}");

        var commit = Assert.IsType<CommitTerm>(parsed);
        Assert.Equal("choice", commit.Name);
        Assert.Equal(
            "commit choice = constraints{prefer(glyph, branch{1 | i} == i, 2/1)}",
            SymbolicTermFormatter.Format(commit));
    }

    [Fact]
    public void Parser_ParsesScopedCommitTarget()
    {
        var parsed = SymbolicParser.Parse("commit glyph.choice = 1 * i");

        var commit = Assert.IsType<CommitTerm>(parsed);
        Assert.Equal("glyph.choice", commit.Target.QualifiedName);
        Assert.True(commit.Target.IsScoped);
        Assert.Equal("commit glyph.choice = 1 * i", SymbolicTermFormatter.Format(commit));
    }

    [Fact]
    public void Elaborator_ResolvesBoundReferencesThroughSequence()
    {
        var sequence = new SequenceTerm(
        [
            new BindTerm("stem", new ElementLiteralTerm(Axis.One)),
            new BindTerm("turn", new TransformLiteralTerm(Axis.I)),
        ]);

        var programResult = SymbolicElaborator.Elaborate(sequence);
        var resolved = SymbolicElaborator.Elaborate(
            new ApplyTransformTerm(
                new ValueReferenceTerm("stem"),
                new TransformReferenceTerm("turn")),
            programResult.Environment);

        var applied = Assert.IsType<ApplyTransformTerm>(resolved.Output);
        Assert.Equal(Axis.One, Assert.IsType<ElementLiteralTerm>(applied.State).Value);
        Assert.Equal(Axis.I, Assert.IsType<TransformLiteralTerm>(applied.Transform).Code);
    }

    [Fact]
    public void SharedCarrierAndRouteTerms_FormatAsCarrierNativeRelations()
    {
        var shared = new SharedCarrierTerm(
            new AnchorReferenceTerm("P4", "u"),
            new AnchorReferenceTerm("P3", "u"));
        var route = new RouteTerm(
            new SiteReferenceTerm("P4"),
            new IncidentReferenceTerm(RouteIncidentKind.HostNegative),
            new IncidentReferenceTerm(RouteIncidentKind.DominantSide));

        Assert.Equal("share(P4.u, P3.u)", SymbolicTermFormatter.Format(shared));
        Assert.Equal("route(P4, host-, u)", SymbolicTermFormatter.Format(route));
    }

    [Fact]
    public void CanonicalSerializer_UsesStableStructuredOutput()
    {
        var preference = new PreferenceTerm(
            new SharedCarrierTerm(
                new AnchorReferenceTerm("P4", "u"),
                new AnchorReferenceTerm("P3", "u")),
            new Proportion(2, 1),
            "glyph");

        var serialized = CanonicalSymbolicSerializer.Serialize(preference);

        Assert.Equal(
            "prefer(participant=\"glyph\",relation=share(left=anchor(owner=\"P4\",name=\"u\"),right=anchor(owner=\"P3\",name=\"u\")),weight=proportion(2/1))",
            serialized);
    }

    [Fact]
    public void Parser_ParsesTransformApplication()
    {
        var parsed = SymbolicParser.Parse("1 * i");

        var applied = Assert.IsType<ApplyTransformTerm>(parsed);
        Assert.Equal("1 * i", SymbolicTermFormatter.Format(applied));
    }

    [Fact]
    public void Parser_ParsesAxisShorthandLiteral()
    {
        var parsed = SymbolicParser.Parse("3i+2");

        var literal = Assert.IsType<ElementLiteralTerm>(parsed);
        var axis = Assert.IsType<Axis>(literal.Value);
        Assert.Equal(new Proportion(3, 1), axis.Recessive);
        Assert.Equal(new Proportion(2, 1), axis.Dominant);
    }

    [Fact]
    public void Parser_ParsesAxisMultiplyWithParameters()
    {
        var parsed = SymbolicParser.Parse("(3i+2) * (4i+5)");

        var multiply = Assert.IsType<MultiplyValuesTerm>(parsed);
        Assert.Equal("[3/1]i + [2/1] * [4/1]i + [5/1]", SymbolicTermFormatter.Format(multiply));
    }

    [Fact]
    public void Parser_ParsesAxisDivisionWithParameters()
    {
        var parsed = SymbolicParser.Parse("(3i+2) / (4i+5)");

        var divide = Assert.IsType<DivideValuesTerm>(parsed);
        Assert.Equal("[3/1]i + [2/1] / [4/1]i + [5/1]", SymbolicTermFormatter.Format(divide));
    }

    [Fact]
    public void Parser_ParsesPositionedPinning()
    {
        var parsed = SymbolicParser.Parse("A * B @ 1/2");

        var pin = Assert.IsType<PinTerm>(parsed);
        Assert.Equal("A", Assert.IsType<ValueReferenceTerm>(pin.Host).Name);
        Assert.Equal("B", Assert.IsType<ValueReferenceTerm>(pin.Applied).Name);
        Assert.Equal(new Proportion(1, 2), pin.Position);
    }

    [Fact]
    public void Parser_ParsesPreferenceEquality()
    {
        var parsed = SymbolicParser.Parse("prefer(glyph, P4.u == P3.u, 2/1)");

        var preference = Assert.IsType<PreferenceTerm>(parsed);
        var equality = Assert.IsType<EqualityTerm>(preference.Relation);
        Assert.Equal("P4.u", Assert.IsType<AnchorReferenceTerm>(equality.Left).QualifiedName);
        Assert.Equal("P3.u", Assert.IsType<AnchorReferenceTerm>(equality.Right).QualifiedName);
        Assert.Equal(new Proportion(2, 1), preference.Weight);
        Assert.Equal("glyph", preference.ParticipantName);
    }

    [Fact]
    public void Parser_ParsesAxisLiteral()
    {
        var parsed = SymbolicParser.Parse("[3/1]i + [12/-1]");

        var literal = Assert.IsType<ElementLiteralTerm>(parsed);
        var axis = Assert.IsType<Axis>(literal.Value);
        Assert.Equal(new Proportion(3, 1), axis.Recessive);
        Assert.Equal(new Proportion(12, -1), axis.Dominant);
    }

    [Fact]
    public void Parser_ParsesSequenceOfLets()
    {
        var parsed = SymbolicParser.Parse("let stem = 1; let turn = i");

        var sequence = Assert.IsType<SequenceTerm>(parsed);
        Assert.Equal(2, sequence.Steps.Count);
        Assert.Equal("let stem = 1; let turn = i", SymbolicTermFormatter.Format(sequence));
    }

    [Fact]
    public void Parser_ParsesBranchFamilyShorthand()
    {
        var parsed = SymbolicParser.Parse("branch{1 | i}");

        var branch = Assert.IsType<BranchFamilyTerm>(parsed);
        Assert.Equal(2, branch.Family.Values.Count);
        Assert.Equal("branch{1 | i}", SymbolicTermFormatter.Format(branch));
    }

    [Fact]
    public void Parser_ParsesAddressedAnchorReference()
    {
        var parsed = SymbolicParser.Parse("P4.u");

        var anchor = Assert.IsType<AnchorReferenceTerm>(parsed);
        Assert.Equal("P4", anchor.OwnerName);
        Assert.Equal("u", anchor.AnchorName);
        Assert.Equal(PinSideRole.Dominant, anchor.SideRole);
    }

    [Fact]
    public void Parser_ParsesRouteWithExplicitIncidents()
    {
        var parsed = SymbolicParser.Parse("route(P4, host-, u)");

        var route = Assert.IsType<RouteTerm>(parsed);
        Assert.Equal("P4", route.Site.SiteName);
        Assert.Equal(RouteIncidentKind.HostNegative, route.From.Kind);
        Assert.Equal(RouteIncidentKind.DominantSide, route.To.Kind);
    }

    [Fact]
    public void PinTerm_CanPreserveExplicitAppliedAnchor()
    {
        var pin = new PinTerm(
            new ValueReferenceTerm("A"),
            new ValueReferenceTerm("B"),
            new Proportion(1, 2),
            new AnchorReferenceTerm("B", "P2"));

        Assert.Equal("A * B.P2 @ 1/2", SymbolicTermFormatter.Format(pin));
        Assert.Equal("B.P2", pin.AppliedAnchor?.QualifiedName);
    }

    [Fact]
    public void Parser_ParsesAnchoredAppliedPinning()
    {
        var parsed = SymbolicParser.Parse("A * B.P2 @ 1/2");

        var pin = Assert.IsType<PinTerm>(parsed);
        Assert.Equal("A", Assert.IsType<ValueReferenceTerm>(pin.Host).Name);
        Assert.Equal("B", Assert.IsType<ValueReferenceTerm>(pin.Applied).Name);
        Assert.Equal("B.P2", Assert.IsType<AnchorReferenceTerm>(pin.AppliedAnchor).QualifiedName);
        Assert.Equal(new Proportion(1, 2), pin.Position);
    }

    [Fact]
    public void AxisBooleanTerm_FormatsAsNativeOperationShorthand()
    {
        var term = new AxisBooleanTerm(
            new ValueReferenceTerm("A"),
            new ValueReferenceTerm("B"),
            AxisBooleanOperation.Xor);

        Assert.Equal("xor(A, B)", SymbolicTermFormatter.Format(term));
    }

    [Fact]
    public void Parser_ParsesBooleanOperationWithOptionalFrame()
    {
        var parsed = SymbolicParser.Parse("xor(A, B, F)");

        var boolean = Assert.IsType<AxisBooleanTerm>(parsed);
        Assert.Equal(AxisBooleanOperation.Xor, boolean.Operation);
        Assert.Equal("A", Assert.IsType<ValueReferenceTerm>(boolean.Primary).Name);
        Assert.Equal("B", Assert.IsType<ValueReferenceTerm>(boolean.Secondary).Name);
        Assert.Equal("F", Assert.IsType<ValueReferenceTerm>(boolean.Frame).Name);
    }

    [Fact]
    public void CanonicalSerializer_EncodesAnchoredPinAndBooleanTerms()
    {
        var pin = new PinTerm(
            new ValueReferenceTerm("A"),
            new ValueReferenceTerm("B"),
            new Proportion(1, 2),
            new AnchorReferenceTerm("B", "P2"));
        var boolean = new AxisBooleanTerm(
            new ValueReferenceTerm("A"),
            new ValueReferenceTerm("B"),
            AxisBooleanOperation.Xor,
            new ValueReferenceTerm("F"));

        Assert.Equal(
            "pin(host=ref(sort=Value,name=\"A\"),applied=ref(sort=Value,name=\"B\"),at=proportion(1/2),appliedAnchor=anchor(owner=\"B\",name=\"P2\"))",
            CanonicalSymbolicSerializer.Serialize(pin));
        Assert.Equal(
            "bool(op=xor,primary=ref(sort=Value,name=\"A\"),secondary=ref(sort=Value,name=\"B\"),frame=ref(sort=Value,name=\"F\"))",
            CanonicalSymbolicSerializer.Serialize(boolean));
    }

    [Fact]
    public void PinToPinTerm_FormatsAsExplicitAnchorAttachment()
    {
        var term = new PinToPinTerm(
            new AnchorReferenceTerm("A", "P1"),
            new AnchorReferenceTerm("B", "P2"));

        Assert.Equal("pin(A.P1, B.P2)", SymbolicTermFormatter.Format(term));
    }

    [Fact]
    public void Parser_ParsesExplicitPinToPinAttachment()
    {
        var parsed = SymbolicParser.Parse("pin(A.P1, B.P2)");

        var pin = Assert.IsType<PinToPinTerm>(parsed);
        Assert.Equal("A.P1", pin.HostAnchor.QualifiedName);
        Assert.Equal("B.P2", pin.AppliedAnchor.QualifiedName);
    }

    [Fact]
    public void CanonicalSerializer_EncodesPinToPinAttachment()
    {
        var term = new PinToPinTerm(
            new AnchorReferenceTerm("A", "P1"),
            new AnchorReferenceTerm("B", "P2"));

        Assert.Equal(
            "pin-to-pin(host=anchor(owner=\"A\",name=\"P1\"),applied=anchor(owner=\"B\",name=\"P2\"))",
            CanonicalSymbolicSerializer.Serialize(term));
    }

    [Fact]
    public void ConstraintSetTerm_FormatsAsCoPresentConstraintFamily()
    {
        var term = new ConstraintSetTerm(
        [
            new RequirementTerm(
                new SharedCarrierTerm(
                    new AnchorReferenceTerm("P4", "u"),
                    new AnchorReferenceTerm("P3", "u")),
                "glyph"),
            new PreferenceTerm(
                new EqualityTerm(
                    new ValueReferenceTerm("width"),
                    new ValueReferenceTerm("full")),
                new Proportion(2, 1),
                "box"),
        ]);

        Assert.Equal(
            "constraints{require(glyph, share(P4.u, P3.u)) | prefer(box, width == full, 2/1)}",
            SymbolicTermFormatter.Format(term));
    }

    [Fact]
    public void Parser_ParsesConstraintSetWithParticipants()
    {
        var parsed = SymbolicParser.Parse(
            "constraints{require(glyph, share(P4.u, P3.u)) | prefer(box, width == full, 2/1)}");

        var set = Assert.IsType<ConstraintSetTerm>(parsed);
        Assert.Equal(2, set.Constraints.Count);

        var requirement = Assert.IsType<RequirementTerm>(set.Constraints[0]);
        var preference = Assert.IsType<PreferenceTerm>(set.Constraints[1]);

        Assert.Equal("glyph", requirement.ParticipantName);
        Assert.Equal("box", preference.ParticipantName);
        Assert.Equal(
            "constraints{require(glyph, share(P4.u, P3.u)) | prefer(box, width == full, 2/1)}",
            SymbolicTermFormatter.Format(set));
    }

    [Fact]
    public void Reducer_PreservesConstraintSetWhileReducingInnerReferences()
    {
        var environment = SymbolicEnvironment.Empty
            .Bind("A", new ElementLiteralTerm(Axis.One))
            .Bind("B", new ElementLiteralTerm(Axis.I));

        var reduced = SymbolicReducer.Reduce(
            SymbolicParser.Parse("constraints{prefer(glyph, A == B, 2/1)}"),
            environment);

        var set = Assert.IsType<ConstraintSetTerm>(reduced.Output);
        var preference = Assert.IsType<PreferenceTerm>(set.Constraints[0]);
        var equality = Assert.IsType<EqualityTerm>(preference.Relation);
        Assert.Equal(Axis.One, Assert.IsType<ElementLiteralTerm>(equality.Left).Value);
        Assert.Equal(Axis.I, Assert.IsType<ElementLiteralTerm>(equality.Right).Value);
    }

    [Fact]
    public void Reducer_ReducesLiteralTransformApplication()
    {
        var reduced = SymbolicReducer.Reduce(SymbolicParser.Parse("1 * i"));

        var literal = Assert.IsType<ElementLiteralTerm>(reduced.Output);
        Assert.Equal(Axis.I, literal.Value);
    }

    [Fact]
    public void Reducer_ReducesLiteralAxisMultiplication()
    {
        var reduced = SymbolicReducer.Reduce(SymbolicParser.Parse("(3i+2) * (4i+5)"));

        var literal = Assert.IsType<ElementLiteralTerm>(reduced.Output);
        var axis = Assert.IsType<Axis>(literal.Value);
        Assert.Equal(new Proportion(23, 1), axis.Recessive);
        Assert.Equal(new Proportion(-2, 1), axis.Dominant);
    }

    [Fact]
    public void Reducer_ReducesLiteralAxisDivision()
    {
        var reduced = SymbolicReducer.Reduce(SymbolicParser.Parse("(3i+2) / (4i+5)"));

        var literal = Assert.IsType<ElementLiteralTerm>(reduced.Output);
        var axis = Assert.IsType<Axis>(literal.Value);
        Assert.Equal(new Proportion(7, 41), axis.Recessive);
        Assert.Equal(new Proportion(22, 41), axis.Dominant);
    }

    [Fact]
    public void Reducer_ReducesFoldAcrossExistingDegrees()
    {
        var reduced = SymbolicReducer.Reduce(SymbolicParser.Parse("fold([1/1]i + [2/1])"));

        var literal = Assert.IsType<ElementLiteralTerm>(reduced.Output);
        Assert.Equal(new Proportion(2, 1), Assert.IsType<Proportion>(literal.Value));
    }

    [Fact]
    public void Reducer_ReducesLiteralBooleanProjectionToBranchFamily()
    {
        var reduced = SymbolicReducer.Reduce(
            SymbolicParser.Parse("xor([0/1]i + [10/1], [-3/1]i + [5/1])"));

        var branch = Assert.IsType<BranchFamilyTerm>(reduced.Output);
        Assert.Equal(2, branch.Family.Values.Count);

        var first = Assert.IsType<Axis>(Assert.IsType<ElementLiteralTerm>(branch.Family.Values[0]).Value);
        var second = Assert.IsType<Axis>(Assert.IsType<ElementLiteralTerm>(branch.Family.Values[1]).Value);

        Assert.Equal(0m, first.Left.Value);
        Assert.Equal(3m, first.Right.Value);
        Assert.Equal(5m, second.Left.Value);
        Assert.Equal(10m, second.Right.Value);
    }

    [Fact]
    public void Reducer_BindsReducedValuesIntoEnvironment()
    {
        var reduced = SymbolicReducer.Reduce(
            SymbolicParser.Parse("let turn = 1 * i; let next = turn * i"));

        Assert.True(reduced.Environment.TryResolve("turn", out var turn));
        Assert.True(reduced.Environment.TryResolve("next", out var next));

        Assert.Equal(Axis.I, Assert.IsType<ElementLiteralTerm>(turn).Value);
        Assert.Equal(Axis.NegativeOne, Assert.IsType<ElementLiteralTerm>(next).Value);
        Assert.Equal(Axis.NegativeOne, Assert.IsType<ElementLiteralTerm>(reduced.Output).Value);
    }

    [Fact]
    public void ConstraintEvaluator_AssessesParticipantRequirementsAndPreferences()
    {
        var evaluation = SymbolicConstraintEvaluator.Evaluate(
            SymbolicParser.Parse("constraints{require(glyph, 1 == 1) | prefer(box, 1 == i, 2/1)}"));

        Assert.Equal(2, evaluation.Items.Count);
        Assert.False(evaluation.HasRequirementFailure);
        Assert.True(evaluation.IsFullyResolved);
        Assert.Equal(new Proportion(2, 1), evaluation.UnsatisfiedPreferenceWeight);

        var glyph = Assert.Single(evaluation.ParticipantSummaries, summary => summary.ParticipantName == "glyph");
        var box = Assert.Single(evaluation.ParticipantSummaries, summary => summary.ParticipantName == "box");

        Assert.Equal(1, glyph.SatisfiedRequirements);
        Assert.Equal(new Proportion(2, 1), box.UnsatisfiedPreferenceWeight);
    }

    [Fact]
    public void ConstraintEvaluator_KeepsSharedCarrierClaimsUnresolvedWithoutCarrierContext()
    {
        var evaluation = SymbolicConstraintEvaluator.Evaluate(
            SymbolicParser.Parse("constraints{require(glyph, share(P4.u, P3.u))}"));

        Assert.False(evaluation.IsFullyResolved);
        Assert.True(evaluation.HasUnresolvedRequirements);

        var glyph = Assert.Single(evaluation.ParticipantSummaries);
        Assert.Equal("glyph", glyph.ParticipantName);
        Assert.Equal(1, glyph.UnresolvedRequirements);
        Assert.Equal("Shared-carrier evaluation requires carrier graph context.", evaluation.Items[0].Note);
    }

    [Fact]
    public void ConstraintEvaluator_UsesReducedEnvironmentInsideSequences()
    {
        var evaluation = SymbolicConstraintEvaluator.Evaluate(
            SymbolicParser.Parse("let A = 1; let B = i; constraints{prefer(glyph, A == 1, 1/1) | prefer(glyph, B == 1, 2/1)}"));

        Assert.Equal(new Proportion(1, 1), evaluation.SatisfiedPreferenceWeight);
        Assert.Equal(new Proportion(2, 1), evaluation.UnsatisfiedPreferenceWeight);

        var glyph = Assert.Single(evaluation.ParticipantSummaries);
        Assert.Equal(new Proportion(1, 1), glyph.SatisfiedPreferenceWeight);
        Assert.Equal(new Proportion(2, 1), glyph.UnsatisfiedPreferenceWeight);
    }

    [Fact]
    public void ConstraintEvaluator_PreservesAlternativeEqualityCandidates()
    {
        var evaluation = SymbolicConstraintEvaluator.Evaluate(
            SymbolicParser.Parse("constraints{prefer(glyph, branch{1 | i} == i, 2/1)}"));

        var item = Assert.Single(evaluation.Items);
        Assert.Equal(ConstraintTruthKind.Unresolved, item.Truth);
        Assert.NotNull(item.CandidateFamily);
        var onlyCandidate = Assert.Single(item.CandidateFamily!.Values);
        Assert.Equal(Axis.I, Assert.IsType<ElementLiteralTerm>(onlyCandidate).Value);
    }

    [Fact]
    public void ConstraintEvaluator_UsesSelectedAlternativeBranchWhenPresent()
    {
        var family = BranchFamily<ValueTerm>.FromValues(
            BranchOrigin.Continuation,
            BranchSemantics.Alternative,
            BranchDirection.Forward,
            [new ElementLiteralTerm(Axis.One), new ElementLiteralTerm(Axis.I)],
            selectedIndex: 1,
            selectionMode: BranchSelectionMode.Principal);

        var evaluation = SymbolicConstraintEvaluator.Evaluate(
            new PreferenceTerm(
                new EqualityTerm(
                    new BranchFamilyTerm(family),
                    new ElementLiteralTerm(Axis.I)),
                new Proportion(2, 1),
                "glyph"));

        var item = Assert.Single(evaluation.Items);
        Assert.Equal(ConstraintTruthKind.Satisfied, item.Truth);
        Assert.Equal("Selected branch satisfies the equality.", item.Note);
    }

    [Fact]
    public void ConstraintNegotiator_SelectsUniquePreferredCandidate()
    {
        var negotiation = SymbolicConstraintNegotiator.Negotiate(
            SymbolicParser.Parse("constraints{prefer(glyph, branch{1 | i} == i, 2/1) | prefer(glyph, branch{1 | i} == 1, 1/1)}"));

        Assert.Equal(ConstraintNegotiationStatus.Selected, negotiation.Status);
        Assert.Equal(Axis.I, Assert.IsType<ElementLiteralTerm>(negotiation.SelectedCandidate).Value);
        Assert.NotNull(negotiation.PreservedCandidateFamily);
        Assert.Equal(BranchSelectionMode.Principal, negotiation.PreservedCandidateFamily!.Family.Selection.Mode);
    }

    [Fact]
    public void ConstraintNegotiator_PreservesCandidatesWhenPreferenceWeightsTie()
    {
        var negotiation = SymbolicConstraintNegotiator.Negotiate(
            SymbolicParser.Parse("constraints{prefer(glyph, branch{1 | i} == i, 1/1) | prefer(glyph, branch{1 | i} == 1, 1/1)}"));

        Assert.Equal(ConstraintNegotiationStatus.PreservedCandidates, negotiation.Status);
        Assert.Null(negotiation.SelectedCandidate);
        Assert.NotNull(negotiation.PreservedCandidateFamily);
        Assert.Equal(2, negotiation.Candidates.Count);
    }

    [Fact]
    public void ConstraintNegotiator_BlocksWhenHardRequirementsNeedMissingContext()
    {
        var negotiation = SymbolicConstraintNegotiator.Negotiate(
            SymbolicParser.Parse("constraints{require(glyph, share(P4.u, P3.u)) | prefer(glyph, branch{1 | i} == i, 2/1)}"));

        Assert.Equal(ConstraintNegotiationStatus.Blocked, negotiation.Status);
        Assert.Null(negotiation.SelectedCandidate);
        Assert.Equal("Shared-carrier evaluation requires carrier graph context.", negotiation.Note);
    }

    [Fact]
    public void ConstraintNegotiator_RespectsRequirementCandidateIntersection()
    {
        var negotiation = SymbolicConstraintNegotiator.Negotiate(
            SymbolicParser.Parse("constraints{require(glyph, branch{1 | i} == i) | prefer(glyph, branch{1 | i} == 1, 2/1)}"));

        Assert.Equal(ConstraintNegotiationStatus.Selected, negotiation.Status);
        Assert.Equal(Axis.I, Assert.IsType<ElementLiteralTerm>(negotiation.SelectedCandidate).Value);
    }

    [Fact]
    public void Commit_BindsNegotiatedRepresentativeIntoEnvironment()
    {
        var reduced = SymbolicReducer.Reduce(
            SymbolicParser.Parse("commit choice = constraints{prefer(glyph, branch{1 | i} == i, 2/1) | prefer(glyph, branch{1 | i} == 1, 1/1)}"));

        var output = Assert.IsType<ElementLiteralTerm>(reduced.Output);
        Assert.Equal(Axis.I, output.Value);
        Assert.True(reduced.Environment.TryResolve("choice", out var choice));
        Assert.Equal(Axis.I, Assert.IsType<ElementLiteralTerm>(choice).Value);
    }

    [Fact]
    public void Commit_PreservesCandidateFamilyWhenNoUniqueRepresentativeExists()
    {
        var reduced = SymbolicReducer.Reduce(
            SymbolicParser.Parse("commit choice = constraints{prefer(glyph, branch{1 | i} == i, 1/1) | prefer(glyph, branch{1 | i} == 1, 1/1)}"));

        var family = Assert.IsType<BranchFamilyTerm>(reduced.Output);
        Assert.Equal(2, family.Family.Values.Count);
        Assert.True(reduced.Environment.TryResolve("choice", out var choice));
        Assert.Same(family, choice);
    }

    [Fact]
    public void Commit_BindsReducedNonConstraintValuesDirectly()
    {
        var reduced = SymbolicReducer.Reduce(
            SymbolicParser.Parse("commit turn = 1 * i; turn"));

        var output = Assert.IsType<ElementLiteralTerm>(reduced.Output);
        Assert.Equal(Axis.I, output.Value);
        Assert.True(reduced.Environment.TryResolve("turn", out var turn));
        Assert.Equal(Axis.I, Assert.IsType<ElementLiteralTerm>(turn).Value);
    }

    [Fact]
    public void Commit_BindsScopedTargetsThatCanBeReferencedLater()
    {
        var reduced = SymbolicReducer.Reduce(
            SymbolicParser.Parse("commit glyph.choice = 1 * i; glyph.choice"));

        var output = Assert.IsType<ElementLiteralTerm>(reduced.Output);
        Assert.Equal(Axis.I, output.Value);
        Assert.True(reduced.Environment.TryResolve("glyph.choice", out var choice));
        Assert.Equal(Axis.I, Assert.IsType<ElementLiteralTerm>(choice).Value);
    }

    [Fact]
    public void Commit_BindsAnchorStyleTargetsThatResolveThroughAnchorReferences()
    {
        var reduced = SymbolicReducer.Reduce(
            SymbolicParser.Parse("commit P4.u = 1 * i; P4.u"));

        var output = Assert.IsType<ElementLiteralTerm>(reduced.Output);
        Assert.Equal(Axis.I, output.Value);
        Assert.True(reduced.Environment.TryResolve("P4.u", out var anchorValue));
        Assert.Equal(Axis.I, Assert.IsType<ElementLiteralTerm>(anchorValue).Value);
    }

    [Fact]
    public void SymbolicInspector_ProducesPerStepTraceForProgramSequence()
    {
        var report = SymbolicInspector.Inspect("let stem = 1; let turn = i; turn");

        Assert.False(report.HasError);
        Assert.Equal(3, report.Steps.Count);
        Assert.Equal("let stem", report.Steps[0].Label);
        Assert.Equal("let turn", report.Steps[1].Label);
        Assert.Equal("emit", report.Steps[2].Label);
        Assert.Equal("i", SymbolicTermFormatter.Format(report.FinalStep!.Reduction.Output!));
    }

    [Fact]
    public void SymbolicInspector_EvaluatesConstraintInsideCommitStep()
    {
        var report = SymbolicInspector.Inspect("commit choice = constraints{prefer(glyph, branch{1 | i} == i, 2/1)}; choice");

        Assert.False(report.HasError);
        Assert.Equal(2, report.Steps.Count);

        var commitStep = report.Steps[0];
        Assert.Equal("commit choice", commitStep.Label);
        Assert.NotNull(commitStep.Evaluation);
        Assert.NotNull(commitStep.Negotiation);
        Assert.Equal(ConstraintNegotiationStatus.Selected, commitStep.Negotiation!.Status);
        Assert.Equal("i", SymbolicTermFormatter.Format(commitStep.Negotiation.SelectedCandidate!));

        Assert.True(report.FinalEnvironment.TryResolve("choice", out var choice));
        Assert.NotNull(choice);
        Assert.Equal("i", SymbolicTermFormatter.Format(choice!));
    }

    [Fact]
    public void SymbolicInspector_ReturnsParseErrorsAsReportState()
    {
        var report = SymbolicInspector.Inspect("let stem = ");

        Assert.True(report.HasError);
        Assert.NotNull(report.Error);
        Assert.Empty(report.Steps);
        Assert.Null(report.Parsed);
    }

    [Fact]
    public void SymbolicInspectionExporter_IncludesExpressionAndEnvironment()
    {
        var report = SymbolicInspector.Inspect("let options = branch{1 | i}; commit choice = constraints{prefer(glyph, options == i, 2/1)}; choice");

        var exported = SymbolicInspectionExporter.Export(report);

        Assert.Contains("EXPRESSION", exported);
        Assert.Contains("let options = branch{1 | i}; commit choice = constraints{prefer(glyph, options == i, 2/1)}; choice", exported);
        Assert.Contains("CANONICAL", exported);
        Assert.Contains("STEP 2: commit choice", exported);
        Assert.Contains("ENVIRONMENT", exported);
        Assert.Contains("choice = i", exported);
    }

    [Fact]
    public void SymbolicInspectionDisplayFormatter_ProducesReadableWorkbenchText()
    {
        var report = SymbolicInspector.Inspect("let options = branch{1 | i}; commit choice = constraints{prefer(glyph, options == i, 2/1)}; choice");

        var display = SymbolicInspectionDisplayFormatter.Format(report);

        Assert.Contains("Expression: let options = branch{1 | i}; commit choice = constraints{prefer(glyph, options == i, 2/1)}; choice", display);
        Assert.Contains("Parsed:", display);
        Assert.Contains("Canonical:", display);
        Assert.Contains("Steps:", display);
        Assert.Contains("2. commit choice", display);
        Assert.Contains("Environment:", display);
    }
}
