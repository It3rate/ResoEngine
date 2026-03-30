namespace Core3.Engine;

/// <summary>
/// Relates a subject to a previously established frame so the frame can lend
/// calibration without being copied into the subject as owned structure.
/// This is closer to measurement or inscription than to containment.
/// </summary>
public sealed record EngineReference(
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

    public EngineElementOutcome ReadWithTension() =>
        Subject.Grade == Frame.Grade
            ? Subject.CommitToCalibrationWithTension(Frame)
            : Subject.CommitToCalibrationWithTension(Calibration);

    public bool TryRead(out GradedElement? read) =>
        EngineExactness.TryProjectExact(
            ReadWithTension(),
            static outcome => outcome.Result,
            out read);

    public CompositeElement GetBoundaryAxis() =>
        TryRead(out var read) && read is not null
            ? EngineBoundary.GetAxis(Calibration, read)
            : EngineBoundary.CreateUnknownAxis(Calibration);

    public EngineElementOutcome MeasureOnCalibrationWithTension()
    {
        var readOutcome = ReadWithTension();

        if (Calibration.Grade == Subject.Grade)
        {
            var measured = new CompositeElement(Calibration, readOutcome.Result);
            return readOutcome.IsExact
                ? EngineElementOutcome.Exact(measured)
                : EngineElementOutcome.WithTension(
                    measured,
                    readOutcome.Tension ?? Subject,
                    readOutcome.Note ?? "Reference measurement preserved unresolved borrowed read.");
        }

        return EngineElementOutcome.WithTension(
            Subject,
            Frame,
            "Reference measurement preserved unresolved structure because calibration and subject grades differed.");
    }

    public bool TryMeasureOnCalibration(out CompositeElement? measured) =>
        EngineExactness.TryProjectExact(
            MeasureOnCalibrationWithTension(),
            static outcome => (CompositeElement)outcome.Result,
            out measured);

    public override string ToString() => $"ref({Frame} <- {Subject})";
}
