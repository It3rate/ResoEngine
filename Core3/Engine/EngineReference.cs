namespace Core3.Engine;

/// <summary>
/// Relates a subject to a previously established frame so the frame can lend
/// calibration without being copied into the subject as owned structure.
/// This is closer to measurement or inscription than to containment.
/// </summary>
public sealed record EngineReference
{
    public EngineReference(CompositeElement frame, GradedElement subject)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(subject);

        Frame = frame;
        Subject = subject;
    }

    public CompositeElement Frame { get; }
    public GradedElement Subject { get; }

    /// <summary>
    /// The frame child whose support is available for reuse.
    /// At grade 1 this is the shared denominator/support side.
    /// </summary>
    public GradedElement Calibration => Frame.Recessive;

    /// <summary>
    /// The frame child that represents the current measured or realized readout.
    /// </summary>
    public GradedElement ExistingReadout => Frame.Dominant;

    public bool TryMeasureOnCalibration(out CompositeElement? measured)
    {
        // Long term when we have explicit support operations, this will become one generic rule on GradedElement, and then the atomic special case will disappear.
        if (Calibration is AtomicElement calibration &&
            Subject is AtomicElement subjectAtomic &&
            calibration.SharesUnitSpace(subjectAtomic) &&
            subjectAtomic.TryCommitToSupport(calibration.Resolution, out var alignedSubject) &&
            alignedSubject is not null)
        {
            measured = new CompositeElement(calibration, alignedSubject);
            return true;
        }

        if (Calibration.Grade == Subject.Grade)
        {
            measured = new CompositeElement(Calibration, Subject);
            return true;
        }

        measured = null;
        return false;
    }

    public override string ToString() => $"ref({Frame} <- {Subject})";
}
