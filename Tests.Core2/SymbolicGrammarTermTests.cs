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
        var preference = new PreferenceTerm(relation, new Proportion(2, 1));

        Assert.Equal(SymbolicTermSort.Constraint, preference.Sort);
        Assert.Equal(new Proportion(2, 1), preference.Weight);
        Assert.Equal(relation, preference.Relation);
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
            new ReferenceTerm("P4.u", SymbolicTermSort.Value),
            new ReferenceTerm("P3.u", SymbolicTermSort.Value));
        var route = new RouteTerm(
            new ReferenceTerm("P4", SymbolicTermSort.Value),
            new ReferenceTerm("host-", SymbolicTermSort.Relation),
            new ReferenceTerm("u+", SymbolicTermSort.Relation));

        Assert.Equal("share(P4.u, P3.u)", SymbolicTermFormatter.Format(shared));
        Assert.Equal("route(P4, host-, u+)", SymbolicTermFormatter.Format(route));
    }

    [Fact]
    public void CanonicalSerializer_UsesStableStructuredOutput()
    {
        var preference = new PreferenceTerm(
            new SharedCarrierTerm(
                new ReferenceTerm("P4.u", SymbolicTermSort.Value),
                new ReferenceTerm("P3.u", SymbolicTermSort.Value)),
            new Proportion(2, 1));

        var serialized = CanonicalSymbolicSerializer.Serialize(preference);

        Assert.Equal(
            "prefer(relation=share(left=ref(sort=Value,name=\"P4.u\"),right=ref(sort=Value,name=\"P3.u\")),weight=proportion(2/1))",
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
        var parsed = SymbolicParser.Parse("prefer(P4.u == P3.u, 2/1)");

        var preference = Assert.IsType<PreferenceTerm>(parsed);
        var equality = Assert.IsType<EqualityTerm>(preference.Relation);
        Assert.Equal("P4.u", Assert.IsType<ReferenceTerm>(equality.Left).Name);
        Assert.Equal("P3.u", Assert.IsType<ReferenceTerm>(equality.Right).Name);
        Assert.Equal(new Proportion(2, 1), preference.Weight);
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
}
