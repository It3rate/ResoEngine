namespace Core3.Engine;

/// <summary>
/// Relates a subject to a previously established frame so the frame can lend
/// calibration without being copied into the subject as owned structure.
/// This is closer to measurement or inscription than to containment.
/// </summary>
public sealed record EngineView(
    CompositeElement Frame,
    GradedElement Subject)
{

    /// <summary>
    /// The frame child whose support is available for reuse.
    /// At grade 1 this is the shared denominator/support side.
    /// </summary>
    public GradedElement Calibration => Frame.Recessive;

    /// <summary>
    /// The frame child that represents the current measured or realized readout.
    /// </summary>
    public GradedElement ExistingReadout => Frame.Dominant;

    public EngineElementOutcome Read() =>
        Subject.Grade == Frame.Grade
            ? Subject.CommitToCalibration(Frame)
            : Subject.CommitToCalibration(Calibration);

    public CompositeElement GetBoundaryAxis()
    {
        var readOutcome = Read();
        return readOutcome.IsExact
            ? EngineBoundary.GetAxis(Calibration, readOutcome.Result)
            : EngineBoundary.CreateUnknownAxis(Calibration);
    }

    public EngineElementOutcome MeasureOnCalibration()
    {
        var readOutcome = Read();

        if (Calibration.Grade == Subject.Grade)
        {
            var measured = new CompositeElement(Calibration, readOutcome.Result);
            return readOutcome.IsExact
                ? EngineElementOutcome.Exact(measured)
                : EngineElementOutcome.WithTension(
                    measured,
                    readOutcome.Tension ?? Subject,
                    readOutcome.Note ?? "View measurement preserved unresolved borrowed read.");
        }

        return EngineElementOutcome.WithTension(
            Subject,
            Frame,
            "View measurement preserved unresolved structure because calibration and subject grades differed.");
    }

    public override string ToString() => $"view({Frame} <- {Subject})";
}
