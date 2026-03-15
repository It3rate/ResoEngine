using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed class GlyphTensionField
{
    private const float MaximumChannelValue = 1.6f;
    private const float MaximumFlowValue = 3.2f;

    private readonly float[] _grow;
    private readonly float[] _stop;
    private readonly float[] _branch;

    private GlyphTensionField(
        GlyphBox box,
        int width,
        int height,
        int seed,
        int stepIndex,
        float[] grow,
        float[] stop,
        float[] branch)
    {
        Box = box;
        Width = width;
        Height = height;
        Seed = seed;
        StepIndex = stepIndex;
        _grow = grow;
        _stop = stop;
        _branch = branch;
    }

    public GlyphBox Box { get; }

    public int Width { get; }

    public int Height { get; }

    public int Seed { get; }

    public int StepIndex { get; }

    public ReadOnlyMemory<float> GrowChannel => _grow;

    public ReadOnlyMemory<float> StopChannel => _stop;

    public ReadOnlyMemory<float> BranchChannel => _branch;

    public static GlyphTensionField CreateSeeded(
        GlyphBox box,
        int seed,
        IReadOnlyList<GlyphAmbientSignal>? signals = null,
        int width = 84,
        int height = 84)
    {
        var field = new GlyphTensionField(
            box,
            width,
            height,
            seed,
            0,
            new float[width * height],
            new float[width * height],
            new float[width * height]);

        if (signals is not null)
        {
            field.StampSignals(signals);
        }

        return field;
    }

    public GlyphTensionField Advance(
        GlyphGrowthState state,
        GlyphEnvironment environment)
    {
        var obstacleMap = ComputeObstacleMap(state);
        var grow = DiffuseChannel(_grow, obstacleMap, channelIndex: 0);
        var stop = DiffuseChannel(_stop, obstacleMap, channelIndex: 1);
        var branch = DiffuseChannel(_branch, obstacleMap, channelIndex: 2);

        var next = new GlyphTensionField(
            Box,
            Width,
            Height,
            Seed,
            StepIndex + 1,
            grow,
            stop,
            branch);

        next.StampSignals(state.AmbientSignals ?? []);
        next.StampTips(state.ActiveTips);
        next.StampJunctions(state.Junctions);
        return next;
    }

    public GlyphTensionFieldSample Sample(GlyphVector point)
    {
        (float fx, float fy) = ToField(point);

        float grow = SampleChannel(_grow, fx, fy);
        float stop = SampleChannel(_stop, fx, fy);
        float branch = SampleChannel(_branch, fx, fy);

        float flowX =
            ComputeGradient(_grow, fx, fy, axisX: true) * 1.10f +
            ComputeGradient(_branch, fx, fy, axisX: true) * 0.65f -
            ComputeGradient(_stop, fx, fy, axisX: true) * 1.35f;
        float flowY =
            ComputeGradient(_grow, fx, fy, axisX: false) * 1.10f +
            ComputeGradient(_branch, fx, fy, axisX: false) * 0.65f -
            ComputeGradient(_stop, fx, fy, axisX: false) * 1.35f;

        return new GlyphTensionFieldSample(
            (decimal)Math.Clamp(grow, 0f, MaximumChannelValue),
            (decimal)Math.Clamp(stop, 0f, MaximumChannelValue),
            (decimal)Math.Clamp(branch, 0f, MaximumChannelValue),
            new GlyphVector(
                (decimal)Math.Clamp(flowX, -MaximumFlowValue, MaximumFlowValue),
                (decimal)Math.Clamp(flowY, -MaximumFlowValue, MaximumFlowValue)));
    }

    private void StampSignals(IReadOnlyList<GlyphAmbientSignal> signals)
    {
        foreach (var signal in signals)
        {
            float growWeight = 0f;
            float stopWeight = 0f;
            float branchWeight = 0f;

            switch (signal.Kind)
            {
                case CouplingKind.Grow:
                case CouplingKind.Attract:
                case CouplingKind.Join:
                    growWeight = 1f;
                    break;

                case CouplingKind.Align:
                    growWeight = 0.42f;
                    branchWeight = 0.28f;
                    break;

                case CouplingKind.Split:
                    growWeight = 0.18f;
                    branchWeight = 1f;
                    break;

                case CouplingKind.Stop:
                case CouplingKind.Repel:
                    stopWeight = 1f;
                    break;
            }

            Stamp(signal.Position, (float)signal.Radius, (float)signal.Magnitude, growWeight, stopWeight, branchWeight);
        }
    }

    private void StampTips(IReadOnlyList<GlyphTip> tips)
    {
        foreach (var tip in tips.Where(tip => tip.IsActive))
        {
            Stamp(
                tip.Position,
                (float)(GlyphGrowthDefaults.JoinCaptureRadius * 2.8m),
                (float)(tip.Energy * 0.12m),
                1f,
                0f,
                0.15f);
        }
    }

    private void StampJunctions(IReadOnlyList<GlyphJunction> junctions)
    {
        foreach (var junction in junctions)
        {
            switch (junction.Kind)
            {
                case GlyphJunctionKind.Split:
                    Stamp(junction.Position, (float)(GlyphGrowthDefaults.JoinCaptureRadius * 2.1m), 0.10f, 0.15f, 0f, 1f);
                    break;
                case GlyphJunctionKind.Join:
                    Stamp(junction.Position, (float)(GlyphGrowthDefaults.JoinCaptureRadius * 1.8m), 0.11f, 0.75f, 0f, 0.2f);
                    break;
                case GlyphJunctionKind.Terminal:
                    Stamp(junction.Position, (float)(GlyphGrowthDefaults.StopCaptureRadius * 2.2m), 0.11f, 0f, 1f, 0f);
                    break;
            }
        }
    }

    private float[] DiffuseChannel(
        float[] source,
        float[] obstacleMap,
        int channelIndex)
    {
        var next = new float[source.Length];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int index = y * Width + x;
                float center = source[index];
                float left = source[ToIndex(x - 1, y)];
                float right = source[ToIndex(x + 1, y)];
                float down = source[ToIndex(x, y - 1)];
                float up = source[ToIndex(x, y + 1)];
                float diag =
                    source[ToIndex(x - 1, y - 1)] +
                    source[ToIndex(x + 1, y - 1)] +
                    source[ToIndex(x - 1, y + 1)] +
                    source[ToIndex(x + 1, y + 1)];

                float driftX = SignedNoise(channelIndex, x, y);
                float driftY = SignedNoise(channelIndex + 7, x, y);
                float advection =
                    (right - left) * driftX * 0.065f +
                    (up - down) * driftY * 0.065f;

                float obstacle = obstacleMap[index];
                float mixed =
                    center * 0.62f +
                    (left + right + down + up) * 0.075f +
                    diag * 0.02f +
                    advection;

                next[index] = Math.Clamp(mixed * (0.965f - obstacle * 0.18f), 0f, MaximumChannelValue);
            }
        }

        return next;
    }

    private float[] ComputeObstacleMap(GlyphGrowthState state)
    {
        var obstacleMap = new float[Width * Height];
        if (state.Carriers.Count == 0)
        {
            return obstacleMap;
        }

        int startIndex = Math.Max(0, state.Carriers.Count - 16);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                GlyphVector point = ToGlyphVector(x + 0.5f, y + 0.5f);
                float obstacle = 0f;
                for (int index = state.Carriers.Count - 1; index >= startIndex; index--)
                {
                    var carrier = state.Carriers[index];
                    float distance = (float)DistanceToSegment(point, carrier.Start, carrier.End);
                    if (distance > 2.6f)
                    {
                        continue;
                    }

                    obstacle = MathF.Max(obstacle, (2.6f - distance) / 2.6f);
                    if (obstacle >= 1f)
                    {
                        break;
                    }
                }

                obstacleMap[y * Width + x] = obstacle;
            }
        }

        return obstacleMap;
    }

    private void Stamp(
        GlyphVector center,
        float radius,
        float magnitude,
        float growWeight,
        float stopWeight,
        float branchWeight)
    {
        if (radius <= 0f || magnitude <= 0f)
        {
            return;
        }

        (float fx, float fy) = ToField(center);
        int minX = Math.Max(0, (int)MathF.Floor(fx - radius));
        int maxX = Math.Min(Width - 1, (int)MathF.Ceiling(fx + radius));
        int minY = Math.Max(0, (int)MathF.Floor(fy - radius));
        int maxY = Math.Min(Height - 1, (int)MathF.Ceiling(fy + radius));
        float radiusSquared = radius * radius;

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = x - fx;
                float dy = y - fy;
                float distanceSquared = dx * dx + dy * dy;
                if (distanceSquared > radiusSquared)
                {
                    continue;
                }

                float falloff = 1f - (distanceSquared / radiusSquared);
                falloff = falloff * falloff;
                int index = y * Width + x;
                _grow[index] = Math.Clamp(_grow[index] + magnitude * growWeight * falloff, 0f, MaximumChannelValue);
                _stop[index] = Math.Clamp(_stop[index] + magnitude * stopWeight * falloff, 0f, MaximumChannelValue);
                _branch[index] = Math.Clamp(_branch[index] + magnitude * branchWeight * falloff, 0f, MaximumChannelValue);
            }
        }
    }

    private (float X, float Y) ToField(GlyphVector point)
    {
        float fx = (float)((point.X - Box.Left) / Box.Width) * (Width - 1);
        float fy = (float)((point.Y - Box.Bottom) / Box.Height) * (Height - 1);
        return (Math.Clamp(fx, 0f, Width - 1), Math.Clamp(fy, 0f, Height - 1));
    }

    private GlyphVector ToGlyphVector(float x, float y)
    {
        decimal gx = Box.Left + (decimal)x / (Width - 1) * Box.Width;
        decimal gy = Box.Bottom + (decimal)y / (Height - 1) * Box.Height;
        return new GlyphVector(gx, gy);
    }

    private int ToIndex(int x, int y) =>
        Math.Clamp(y, 0, Height - 1) * Width + Math.Clamp(x, 0, Width - 1);

    private float SampleChannel(float[] channel, float fx, float fy)
    {
        int x0 = (int)MathF.Floor(fx);
        int y0 = (int)MathF.Floor(fy);
        int x1 = Math.Min(Width - 1, x0 + 1);
        int y1 = Math.Min(Height - 1, y0 + 1);
        float tx = fx - x0;
        float ty = fy - y0;

        float a = channel[ToIndex(x0, y0)];
        float b = channel[ToIndex(x1, y0)];
        float c = channel[ToIndex(x0, y1)];
        float d = channel[ToIndex(x1, y1)];

        float ab = a + (b - a) * tx;
        float cd = c + (d - c) * tx;
        return ab + (cd - ab) * ty;
    }

    private float ComputeGradient(float[] channel, float fx, float fy, bool axisX)
    {
        if (axisX)
        {
            return SampleChannel(channel, fx + 1f, fy) - SampleChannel(channel, fx - 1f, fy);
        }

        return SampleChannel(channel, fx, fy + 1f) - SampleChannel(channel, fx, fy - 1f);
    }

    private float SignedNoise(int channelIndex, int x, int y)
    {
        uint hash = (uint)Seed;
        hash ^= (uint)(StepIndex * 374761393);
        hash ^= (uint)((channelIndex + 11) * 668265263);
        hash ^= (uint)((x + 37) * 2246822519);
        hash ^= (uint)((y + 71) * 3266489917);
        hash ^= hash >> 13;
        hash *= 1274126177u;
        hash ^= hash >> 16;
        float normalized = (hash & 0x00FFFFFF) / 16777215f;
        return normalized * 2f - 1f;
    }

    private static decimal DistanceToSegment(GlyphVector point, GlyphVector start, GlyphVector end)
    {
        GlyphVector segment = end - start;
        decimal lengthSquared = segment.LengthSquared;
        if (lengthSquared <= 0m)
        {
            return point.DistanceTo(start);
        }

        decimal projection = decimal.Clamp((point - start).Dot(segment) / lengthSquared, 0m, 1m);
        GlyphVector nearest = start + (segment * projection);
        return point.DistanceTo(nearest);
    }
}
