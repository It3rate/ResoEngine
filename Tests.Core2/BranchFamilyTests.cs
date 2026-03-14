using Core2.Branching;

namespace Tests.Core2;

public class BranchFamilyTests
{
    private readonly record struct TestAnnotation(string Label) : IBranchAnnotation;

    [Fact]
    public void Map_PreservesIdsSelectionParentsAnnotationsAndTensions()
    {
        var firstId = BranchId.New();
        var secondId = BranchId.New();
        var family = BranchFamily<int>.FromMembers(
            BranchOrigin.Continuation,
            BranchSemantics.Alternative,
            BranchDirection.Forward,
            [
                new BranchMember<int>(firstId, 2, [], [new TestAnnotation("seed")]),
                new BranchMember<int>(secondId, 3, [firstId], [])
            ],
            BranchSelection.Principal(secondId),
            [new BranchTension(BranchTensionKind.UnresolvedMultiplicity, "Keep both candidates visible.")]);

        var mapped = family.Map(value => value * 10);

        Assert.Equal(BranchOrigin.Continuation, mapped.Origin);
        Assert.Equal(BranchSemantics.Alternative, mapped.Semantics);
        Assert.Equal(BranchDirection.Forward, mapped.Direction);
        Assert.Equal(30, mapped.SelectedValue);
        Assert.Equal(secondId, mapped.SelectedMember!.Id);
        Assert.Equal(firstId, mapped.Members[0].Id);
        Assert.Equal(firstId, Assert.Single(mapped.Members[1].Parents));
        Assert.Equal(BranchTensionKind.UnresolvedMultiplicity, Assert.Single(mapped.Tensions).Kind);
        Assert.True(mapped.Members[0].TryGetAnnotation<TestAnnotation>(out var annotation));
        Assert.Equal("seed", annotation.Label);
    }
}
