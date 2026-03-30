namespace Core3.Engine;

internal static class EngineBoundary
{
    internal static CompositeElement GetAxis(GradedElement frame, GradedElement read)
    {
        if (frame.Grade != read.Grade)
        {
            return CreateUnknownAxis(frame);
        }

        if (frame is AtomicElement atomicFrame &&
            read is AtomicElement atomicRead)
        {
            return GetAtomicAxis(atomicFrame, atomicRead);
        }

        if (frame is CompositeElement compositeFrame &&
            read is CompositeElement compositeRead)
        {
            return new CompositeElement(
                GetAxis(compositeFrame.Recessive, compositeRead.Recessive),
                GetAxis(compositeFrame.Dominant, compositeRead.Dominant));
        }

        return CreateUnknownAxis(frame);
    }

    internal static CompositeElement CreateUnknownAxis(GradedElement frame)
    {
        if (frame is AtomicElement)
        {
            return new CompositeElement(
                new AtomicElement(0, 0),
                new AtomicElement(0, 0));
        }

        if (frame is CompositeElement compositeFrame)
        {
            return new CompositeElement(
                CreateUnknownAxis(compositeFrame.Recessive),
                CreateUnknownAxis(compositeFrame.Dominant));
        }

        throw new InvalidOperationException("Unsupported frame shape for boundary derivation.");
    }

    private static CompositeElement GetAtomicAxis(AtomicElement frame, AtomicElement read)
    {
        if (!TryCommitRead(frame, read, out var committedRead))
        {
            return CreateUnknownAxis(frame);
        }

        var lower = Math.Min(0, frame.Value);
        var upper = Math.Max(0, frame.Value);
        var lowerOverflow = committedRead.Value < lower
            ? checked(lower - committedRead.Value)
            : 0;
        var upperOverflow = committedRead.Value > upper
            ? checked(committedRead.Value - upper)
            : 0;

        return new CompositeElement(
            new AtomicElement(lowerOverflow, frame.Unit),
            new AtomicElement(upperOverflow, frame.Unit));
    }

    private static bool TryCommitRead(
        AtomicElement frame,
        AtomicElement read,
        out AtomicElement committedRead)
    {
        if (frame.HasResolvedUnits &&
            frame.SharesUnitSpace(read))
        {
            committedRead = read;
            return true;
        }

        if (read.TryCommitToCalibration(frame, out var committed) &&
            committed is AtomicElement committedAtomic)
        {
            committedRead = committedAtomic;
            return true;
        }

        committedRead = new AtomicElement(0, 0);
        return false;
    }
}
