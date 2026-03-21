using Core2.Elements;
using Core2.Interpretation.Analysis;
using Core2.Symbolics.Expressions;

namespace ResoEngine.Visualizer.Pages;

internal static class SymbolicStructuralContextPresets
{
    public static IReadOnlyDictionary<string, SymbolicStructuralContextPreset> Build()
    {
        var none = new SymbolicStructuralContextPreset("None", "No structural carrier graph is active.", null);
        var shared = BuildSharedBowlContext();
        var cross = BuildCrossContext();
        var tee = BuildTeeContext();

        return new Dictionary<string, SymbolicStructuralContextPreset>(StringComparer.Ordinal)
        {
            [none.Key] = none,
            [shared.Key] = shared,
            [cross.Key] = cross,
            [tee.Key] = tee,
        };
    }

    private static SymbolicStructuralContextPreset BuildSharedBowlContext()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bowl = CarrierIdentity.Create("Bowl");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var p4 = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(new Proportion(3, -1), new Proportion(2, 1)), Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, Proportion.Zero),
            name: "P4");
        var p3 = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(new Proportion(-3, -1), new Proportion(2, 1)), Proportion.One),
            new CarrierSideAttachment(PinSideRole.Recessive, stem, Proportion.One),
            new CarrierSideAttachment(PinSideRole.Dominant, bowl, Proportion.One),
            name: "P3");

        var analysis = new CarrierPinGraph([stem, bowl], [p4, p3]).Analyze();
        var context = new CarrierGraphSymbolicStructuralContext(analysis);

        return new SymbolicStructuralContextPreset(
            "Shared",
            BuildSummary(
                "Shared Bowl",
                analysis,
                [
                    "P4.u and P3.u both resolve onto Bowl.",
                    "Use this with share(P4.u, P3.u).",
                ]),
            context);
    }

    private static SymbolicStructuralContextPreset BuildCrossContext()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bar = CarrierIdentity.Create("Bar");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var p4 = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, -1, 3, -1), new Proportion(5)),
            new CarrierSideAttachment(PinSideRole.Recessive, bar, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bar, Proportion.One),
            name: "P4");

        var analysis = new CarrierPinGraph([stem, bar], [p4]).Analyze();
        var context = new CarrierGraphSymbolicStructuralContext(analysis);

        return new SymbolicStructuralContextPreset(
            "Cross",
            BuildSummary(
                "True Cross",
                analysis,
                [
                    "P4 has host-through and non-host-through simultaneously.",
                    "Use this with route(P4, host-, host+) and route(P4, i, u).",
                ]),
            context);
    }

    private static SymbolicStructuralContextPreset BuildTeeContext()
    {
        var stem = CarrierIdentity.Create("Stem");
        var bar = CarrierIdentity.Create("Bar");
        var host = Axis.FromCoordinates(Proportion.Zero, new Proportion(10));

        var p4 = CarrierPinSite.FromPointPinning(
            stem,
            host.PinAt(new Axis(3, -1, 3, -1), new Proportion(10)),
            new CarrierSideAttachment(PinSideRole.Recessive, bar, Proportion.Zero),
            new CarrierSideAttachment(PinSideRole.Dominant, bar, Proportion.One),
            name: "P4");

        var analysis = new CarrierPinGraph([stem, bar], [p4]).Analyze();
        var context = new CarrierGraphSymbolicStructuralContext(analysis);

        return new SymbolicStructuralContextPreset(
            "Tee",
            BuildSummary(
                "Tee Junction",
                analysis,
                [
                    "P4 has a non-host through carrier without host-through.",
                    "Use this to see route(P4, host-, host+) fail while route(P4, i, u) holds.",
                ]),
            context);
    }

    private static string BuildSummary(
        string title,
        CarrierPinGraphAnalysis analysis,
        IReadOnlyList<string> notes)
    {
        var lines = new List<string>
        {
            title,
            $"Carriers: {string.Join(", ", analysis.Profiles.Select(profile => profile.Carrier.Name ?? profile.Carrier.Id.ToString()))}",
            $"Sites: {string.Join(", ", analysis.SiteProfiles.Select(profile => $"{profile.Name ?? profile.SiteId.ToString()}={profile.Summary}"))}",
        };
        lines.AddRange(notes);
        return string.Join(Environment.NewLine, lines);
    }
}

internal sealed record SymbolicStructuralContextPreset(
    string Key,
    string Summary,
    ISymbolicStructuralContext? Context)
{
    public string DisplayName => Key;
}
