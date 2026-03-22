using Applied.Geometry.LetterFormation;
using Applied.Geometry.Utils;
using Core2.Elements;

namespace Tests.Core2;

public class LetterFormationTensionEvaluatorTests
{
    [Fact]
    public void CarrierDirectionDesire_CanIgnoreHorizontalComponent()
    {
        var state = new LetterFormationState(
            "test",
            0,
            LetterFormationEnvironment.CreateLetterBox(),
            [
                new LetterFormationSiteState("A", new PlanarPoint(0, 0), Axis.Zero, PlanarOffset.Zero, []),
                new LetterFormationSiteState("B", new PlanarPoint(3, 3), Axis.Zero, PlanarOffset.Zero, []),
            ],
            [
                new LetterFormationCarrierState(
                    "Stem",
                    "A",
                    "B",
                    [
                        new CarrierDirectionDesire(
                            new Axis(1, 0, 0, 0),
                            Proportion.Zero,
                            Proportion.One,
                            "downward only"),
                    ]),
            ],
            []);

        var evaluated = LetterFormationTensionEvaluator.Evaluate(state);

        Assert.Empty(evaluated.Tensions);
    }

    [Fact]
    public void CarrierDirectionDesire_ProducesTensionWhenDesiredComponentDisagrees()
    {
        var state = new LetterFormationState(
            "test",
            0,
            LetterFormationEnvironment.CreateLetterBox(),
            [
                new LetterFormationSiteState("A", new PlanarPoint(0, 0), Axis.Zero, PlanarOffset.Zero, []),
                new LetterFormationSiteState("B", new PlanarPoint(0, 4), Axis.Zero, PlanarOffset.Zero, []),
            ],
            [
                new LetterFormationCarrierState(
                    "Bar",
                    "A",
                    "B",
                    [
                        new CarrierDirectionDesire(
                            LetterFormationDirections.Horizontal,
                            Proportion.Zero,
                            Proportion.One,
                            "horizontal"),
                    ]),
            ],
            []);

        var evaluated = LetterFormationTensionEvaluator.Evaluate(state);

        Assert.Contains(evaluated.Tensions, tension => tension.ComponentId == "Bar");
    }

    [Fact]
    public void CapitalASeed_EvaluatesToStructuredLocalTensions()
    {
        var state = LetterFormationPresetFactory.CreateCapitalASeed(new Random(1234));

        var evaluated = LetterFormationTensionEvaluator.Evaluate(state);

        Assert.NotEmpty(evaluated.Tensions);
        Assert.Contains(evaluated.Tensions, tension => tension.ComponentId == "LeftApex" || tension.ComponentId == "RightApex");
        Assert.Contains(
            evaluated.Tensions,
            tension => tension.ComponentId == "LeftJoin" ||
                       tension.ComponentId == "RightJoin" ||
                       tension.ComponentId == "CrossLeft" ||
                       tension.ComponentId == "CrossRight");
    }
}
