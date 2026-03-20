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
}
