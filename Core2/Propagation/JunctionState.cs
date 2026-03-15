namespace Core2.Propagation;

public sealed record JunctionState(
    string Key,
    IReadOnlyList<string> ConnectedCarrierKeys,
    ResponseProfile Response,
    IReadOnlyList<CouplingRule>? Couplings = null,
    bool AllowsCapture = true,
    bool AllowsSplit = true);
