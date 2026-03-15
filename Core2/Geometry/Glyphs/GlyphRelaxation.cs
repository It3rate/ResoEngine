using Core2.Dynamic;
using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

internal static class GlyphRelaxation
{
    public static bool NeedsRelaxation(GlyphGrowthState state) =>
        (state.AmbientSignals?.Count > 0) ||
        state.ResidualTension > GlyphGrowthDefaults.ResidualTensionThreshold ||
        state.LastAdjustment > GlyphGrowthDefaults.RelaxationThreshold;

    public static GlyphGrowthState Relax(
        GlyphGrowthState state,
        GlyphEnvironment environment,
        IReadOnlyList<DynamicProposal<GlyphGrowthEffect>> acceptedProposals)
    {
        var priorAmbient = (state.AmbientSignals ?? []).ToList();
        var leaked = CreateLeakedSignals(acceptedProposals);
        var nextAmbient = priorAmbient
            .Select(signal => AdvanceSignal(signal, environment.Box))
            .Where(signal => signal.Magnitude >= GlyphGrowthDefaults.MinimumAmbientMagnitude && signal.Age <= GlyphGrowthDefaults.MaximumAmbientAge)
            .Concat(leaked)
            .ToArray();

        var movedTips = state.ActiveTips
            .Select(tip => MoveTip(tip, state, environment, nextAmbient))
            .ToArray();
        var movedJunctions = state.Junctions
            .Select(junction => MoveJunction(junction, state, environment, nextAmbient))
            .ToArray();
        var movedCarriers = state.Carriers
            .Select(carrier => MoveCarrier(carrier, state, environment, movedTips, movedJunctions, nextAmbient))
            .ToArray();

        decimal tipAdjustment = SumAdjustment(state.ActiveTips.Select(tip => tip.Position), movedTips.Select(tip => tip.Position));
        decimal junctionAdjustment = SumAdjustment(state.Junctions.Select(junction => junction.Position), movedJunctions.Select(junction => junction.Position));
        decimal carrierAdjustment = state.Carriers.Zip(
                movedCarriers,
                (prior, next) => prior.Start.DistanceTo(next.Start) + prior.End.DistanceTo(next.End))
            .Sum();

        decimal lastAdjustment = tipAdjustment + junctionAdjustment + carrierAdjustment;
        var provisionalState = state with
        {
            ActiveTips = movedTips,
            Junctions = movedJunctions,
            Carriers = movedCarriers,
            AmbientSignals = nextAmbient,
        };
        var field = (state.TensionField ?? GlyphTensionField.CreateSeeded(environment.Box, state.RandomSeed, nextAmbient))
            .Advance(provisionalState, environment);
        decimal residualTension = ComputeResidualTension(state, movedTips, movedJunctions, nextAmbient, field);

        return provisionalState with
        {
            ResidualTension = residualTension,
            LastAdjustment = lastAdjustment,
            TensionField = field,
        };
    }

    private static IReadOnlyList<GlyphAmbientSignal> CreateLeakedSignals(
        IReadOnlyList<DynamicProposal<GlyphGrowthEffect>> acceptedProposals)
    {
        var signals = new List<GlyphAmbientSignal>();
        foreach (var proposal in acceptedProposals)
        {
            var effect = proposal.Effect;
            signals.Add(new GlyphAmbientSignal(
                $"{effect.SourceTipKey}:{effect.Kind}:{signals.Count}",
                effect.TargetPosition,
                effect.Kind switch
                {
                    GlyphGrowthEffectKind.Grow => CouplingKind.Grow,
                    GlyphGrowthEffectKind.Split => CouplingKind.Split,
                    GlyphGrowthEffectKind.Join => CouplingKind.Join,
                    GlyphGrowthEffectKind.Stop => CouplingKind.Stop,
                    _ => CouplingKind.Attract,
                },
                Math.Max(0.06m, proposal.Weight * 0.18m),
                effect.Kind == GlyphGrowthEffectKind.Grow ? 14m : 18m,
                null,
                0m,
                0,
                effect.Note));

            foreach (var tension in proposal.Tensions)
            {
                signals.Add(new GlyphAmbientSignal(
                    $"{effect.SourceTipKey}:{tension.Kind}:{signals.Count}",
                    effect.TargetPosition,
                    tension.Kind switch
                    {
                        "BoundaryPressure" => CouplingKind.Repel,
                        "StopField" => CouplingKind.Stop,
                        _ => CouplingKind.Attract,
                    },
                    Math.Max(0.05m, tension.Magnitude * 0.24m),
                    16m,
                    null,
                    0m,
                    0,
                    tension.Message));
            }
        }

        return signals;
    }

    private static GlyphAmbientSignal AdvanceSignal(
        GlyphAmbientSignal signal,
        GlyphBox box)
    {
        GlyphVector nextPosition = signal.Position;
        if (signal.TargetPosition is GlyphVector target && signal.Drift > 0m)
        {
            GlyphVector delta = target - signal.Position;
            decimal distance = delta.Length;
            if (distance <= signal.Drift)
            {
                nextPosition = target;
            }
            else if (distance > 0m)
            {
                nextPosition = signal.Position + (delta.Normalize() * signal.Drift);
            }
        }

        nextPosition = new GlyphVector(
            decimal.Clamp(nextPosition.X, box.Left, box.Right),
            decimal.Clamp(nextPosition.Y, box.Bottom, box.Top));

        return signal with
        {
            Position = nextPosition,
            Magnitude = signal.Magnitude * GlyphGrowthDefaults.AmbientDecay,
            Radius = signal.Radius * GlyphGrowthDefaults.AmbientSpread,
            Age = signal.Age + 1,
        };
    }

    private static GlyphTip MoveTip(
        GlyphTip tip,
        GlyphGrowthState state,
        GlyphEnvironment environment,
        IReadOnlyList<GlyphAmbientSignal> ambientSignals)
    {
        GlyphVector field = ComputeVectorField(state, environment, tip.Position, ambientSignals, tip.Key);
        decimal maxMove = tip.IsActive ? GlyphGrowthDefaults.RelaxationStep * 0.06m : GlyphGrowthDefaults.RelaxationStep * 0.18m;
        GlyphVector moved = MovePoint(tip.Position, field, maxMove, environment.Box);

        if (!tip.IsActive)
        {
            return tip with { Position = moved };
        }

        GlyphVector preferred = (tip.PreferredDirection.Normalize() + field * 0.35m).Normalize();
        return tip with
        {
            Position = moved,
            PreferredDirection = preferred == GlyphVector.Zero ? tip.PreferredDirection : preferred,
        };
    }

    private static GlyphJunction MoveJunction(
        GlyphJunction junction,
        GlyphGrowthState state,
        GlyphEnvironment environment,
        IReadOnlyList<GlyphAmbientSignal> ambientSignals)
    {
        GlyphVector bias = ComputeVectorField(state, environment, junction.Position, ambientSignals, junction.Key);

        if (junction.Kind == GlyphJunctionKind.Split)
        {
            var branchPoint = environment.FindClosestLandmark(junction.Position, GlyphLandmarkKind.BranchPoint);
            if (branchPoint is not null)
            {
                bias += (branchPoint.Position - junction.Position).Normalize() * 0.75m;
            }
        }

        if (junction.Kind is GlyphJunctionKind.Join or GlyphJunctionKind.Terminal)
        {
            var stop = environment.FindClosestLandmark(junction.Position, GlyphLandmarkKind.StopPoint, GlyphLandmarkKind.Baseline, GlyphLandmarkKind.Capline);
            if (stop is not null)
            {
                bias += (stop.Position - junction.Position).Normalize() * 0.85m;
            }
        }

        GlyphVector moved = MovePoint(junction.Position, bias, GlyphGrowthDefaults.RelaxationStep * 0.45m, environment.Box);
        return junction with { Position = moved };
    }

    private static GlyphCarrier MoveCarrier(
        GlyphCarrier carrier,
        GlyphGrowthState state,
        GlyphEnvironment environment,
        IReadOnlyList<GlyphTip> movedTips,
        IReadOnlyList<GlyphJunction> movedJunctions,
        IReadOnlyList<GlyphAmbientSignal> ambientSignals)
    {
        GlyphVector start = ResolveAnchorPosition(carrier.Start, state, movedTips, movedJunctions);
        GlyphVector end = ResolveAnchorPosition(carrier.End, state, movedTips, movedJunctions);

        if (start == carrier.Start)
        {
            GlyphVector startField = ComputeVectorField(state, environment, carrier.Start, ambientSignals, carrier.Key);
            start = MovePoint(start, startField, GlyphGrowthDefaults.RelaxationStep * 0.12m, environment.Box);
        }

        if (end == carrier.End)
        {
            GlyphVector endField = ComputeVectorField(state, environment, carrier.End, ambientSignals, carrier.Key);
            end = MovePoint(end, endField, GlyphGrowthDefaults.RelaxationStep * 0.12m, environment.Box);
        }

        return carrier with
        {
            Start = start,
            End = end,
            Tension = start.DistanceTo(end),
        };
    }

    private static GlyphVector ResolveAnchorPosition(
        GlyphVector endpoint,
        GlyphGrowthState priorState,
        IReadOnlyList<GlyphTip> movedTips,
        IReadOnlyList<GlyphJunction> movedJunctions)
    {
        var nearestTip = priorState.ActiveTips
            .Zip(movedTips, (prior, moved) => (prior, moved))
            .Where(pair => pair.prior.Position.DistanceTo(endpoint) <= 1.25m)
            .OrderBy(pair => pair.prior.Position.DistanceTo(endpoint))
            .FirstOrDefault();
        if (nearestTip.prior is not null)
        {
            return nearestTip.moved.Position;
        }

        var nearestJunction = priorState.Junctions
            .Zip(movedJunctions, (prior, moved) => (prior, moved))
            .Where(pair => pair.prior.Position.DistanceTo(endpoint) <= 1.25m)
            .OrderBy(pair => pair.prior.Position.DistanceTo(endpoint))
            .FirstOrDefault();
        if (nearestJunction.prior is not null)
        {
            return nearestJunction.moved.Position;
        }

        return endpoint;
    }

    private static GlyphVector ComputeVectorField(
        GlyphGrowthState state,
        GlyphEnvironment environment,
        GlyphVector point,
        IReadOnlyList<GlyphAmbientSignal> ambientSignals,
        string sourceKey)
    {
        GlyphVector vector = GlyphVector.Zero;

        foreach (var influence in environment.SampleInfluencesAt(point))
        {
            vector += influence.Rule.Kind switch
            {
                CouplingKind.Align => ResolveAlignmentVector(environment.Box, point) * (influence.Weight * 0.9m),
                CouplingKind.Grow => ResolveEdgeGrowthVector(environment.Box, point) * (influence.Weight * 0.8m),
                CouplingKind.Attract => ResolveLandmarkVector(environment, point, GlyphLandmarkKind.Midline, GlyphLandmarkKind.Centerline, GlyphLandmarkKind.BranchPoint, GlyphLandmarkKind.StopPoint) * (influence.Weight * 0.7m),
                CouplingKind.Stop => ResolveLandmarkVector(environment, point, GlyphLandmarkKind.StopPoint, GlyphLandmarkKind.Capline, GlyphLandmarkKind.Baseline) * (influence.Weight * 0.55m),
                CouplingKind.Split => ResolveLandmarkVector(environment, point, GlyphLandmarkKind.BranchPoint) * influence.Weight,
                _ => GlyphVector.Zero,
            };
        }

        foreach (var signal in ambientSignals.Where(signal => signal.Position.DistanceTo(point) <= signal.Radius))
        {
            GlyphVector delta = signal.Position - point;
            GlyphVector direction = delta == GlyphVector.Zero ? GlyphVector.Zero : delta.Normalize();
            decimal weight = signal.Magnitude * (1m - decimal.Clamp(point.DistanceTo(signal.Position) / signal.Radius, 0m, 1m));
            vector += signal.Kind switch
            {
                CouplingKind.Repel or CouplingKind.Stop => direction * (-weight),
                CouplingKind.Align => ResolveAlignmentVector(environment.Box, point) * weight,
                _ => direction * weight,
            };
        }

        foreach (var tip in state.ActiveTips.Where(tip => tip.IsActive && tip.Key != sourceKey))
        {
            decimal distance = tip.Position.DistanceTo(point);
            if (distance > 0m && distance < GlyphGrowthDefaults.JoinCaptureRadius)
            {
                vector += (point - tip.Position).Normalize() * ((GlyphGrowthDefaults.JoinCaptureRadius - distance) / GlyphGrowthDefaults.JoinCaptureRadius) * 0.35m;
            }
        }

        if (state.TensionField is not null)
        {
            var fieldSample = state.TensionField.Sample(point);
            if (fieldSample.Flow != GlyphVector.Zero)
            {
                vector += fieldSample.Flow.Normalize() * (0.28m + fieldSample.Energy * 0.08m);
            }
        }

        return vector.Normalize();
    }

    private static GlyphVector ResolveLandmarkVector(
        GlyphEnvironment environment,
        GlyphVector point,
        params GlyphLandmarkKind[] kinds)
    {
        var landmark = environment.FindClosestLandmark(point, kinds);
        if (landmark is null)
        {
            return GlyphVector.Zero;
        }

        var delta = landmark.Position - point;
        return delta == GlyphVector.Zero ? GlyphVector.Zero : delta.Normalize();
    }

    private static GlyphVector ResolveAlignmentVector(GlyphBox box, GlyphVector point)
    {
        decimal horizontal = box.MidX - point.X;
        return new GlyphVector(horizontal, 0m).Normalize();
    }

    private static GlyphVector ResolveEdgeGrowthVector(GlyphBox box, GlyphVector point)
    {
        decimal leftDistance = point.X - box.Left;
        decimal rightDistance = box.Right - point.X;
        decimal horizontal = rightDistance < leftDistance ? 1m : -1m;
        return new GlyphVector(horizontal, 0m).Normalize();
    }

    private static GlyphVector MovePoint(GlyphVector point, GlyphVector field, decimal maxMove, GlyphBox box)
    {
        if (field == GlyphVector.Zero || maxMove <= 0m)
        {
            return point;
        }

        var moved = point + field.Normalize() * maxMove;
        return new GlyphVector(
            decimal.Clamp(moved.X, box.Left, box.Right),
            decimal.Clamp(moved.Y, box.Bottom, box.Top));
    }

    private static decimal SumAdjustment(IEnumerable<GlyphVector> prior, IEnumerable<GlyphVector> next) =>
        prior.Zip(next, (before, after) => before.DistanceTo(after)).Sum();

    private static decimal ComputeResidualTension(
        GlyphGrowthState priorState,
        IReadOnlyList<GlyphTip> movedTips,
        IReadOnlyList<GlyphJunction> movedJunctions,
        IReadOnlyList<GlyphAmbientSignal> ambientSignals,
        GlyphTensionField field)
    {
        decimal ambient = ambientSignals.Sum(signal => signal.Magnitude);
        decimal active = movedTips.Count(tip => tip.IsActive) * 0.4m;
        decimal junctionDrift = priorState.Junctions
            .Zip(movedJunctions, (prior, next) => prior.Position.DistanceTo(next.Position))
            .Sum();
        decimal fieldEnergy = movedTips
            .Where(tip => tip.IsActive)
            .Select(tip => field.Sample(tip.Position).Energy)
            .DefaultIfEmpty(field.Sample(field.Box.Center).Energy)
            .Average() * 0.25m;
        return ambient + active + junctionDrift + fieldEnergy;
    }
}
