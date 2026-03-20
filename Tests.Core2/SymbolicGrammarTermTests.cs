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
}
