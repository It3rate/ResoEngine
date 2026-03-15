using Core2.Dynamic;
using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public sealed class GlyphTipGrowthStrand : IDynamicStrand<GlyphGrowthState, GlyphEnvironment, GlyphGrowthEffect>
{
    public string Name => "GlyphTipGrowth";

    public IReadOnlyList<DynamicProposal<GlyphGrowthEffect>> Propose(
        DynamicStrandContext<GlyphGrowthState, GlyphEnvironment> context)
    {
        var state = context.Current.Context.State;
        var environment = context.Current.Context.Environment;
        var proposals = new List<DynamicProposal<GlyphGrowthEffect>>();

        foreach (var tip in state.ActiveTips.Where(tip => tip.IsActive))
        {
            decimal packetMagnitude = ResolvePacketMagnitude(state, tip);
            decimal step = ResolveStepSize(packetMagnitude);
            GlyphVector direction = BuildGuidedDirection(state, environment, tip);
            GlyphVector projected = tip.Position + direction * step;
            GlyphVector clamped = ClampToBox(environment.Box, projected);

            proposals.Add(BuildProposal(
                context,
                state,
                environment,
                packetMagnitude,
                new GlyphGrowthEffect(
                    GlyphGrowthEffectKind.Grow,
                    tip.Key,
                    clamped,
                    direction,
                    score: 0m,
                    note: "Advance the active tip under current field guidance."),
                projected));

            var splitEffect = TryCreateSplitEffect(state, environment, tip, direction, clamped, step);
            if (splitEffect is not null)
            {
                proposals.Add(BuildProposal(context, state, environment, packetMagnitude, splitEffect, splitEffect.TargetPosition));
            }

            var joinEffect = TryCreateJoinEffect(state, environment, tip, packetMagnitude);
            if (joinEffect is not null)
            {
                proposals.Add(BuildProposal(context, state, environment, packetMagnitude, joinEffect, joinEffect.TargetPosition));
            }

            var stopEffect = TryCreateStopEffect(environment, tip, direction, projected, clamped, step);
            if (stopEffect is not null)
            {
                proposals.Add(BuildProposal(context, state, environment, packetMagnitude, stopEffect, stopEffect.TargetPosition));
            }
        }

        return proposals;
    }

    private DynamicProposal<GlyphGrowthEffect> BuildProposal(
        DynamicStrandContext<GlyphGrowthState, GlyphEnvironment> context,
        GlyphGrowthState state,
        GlyphEnvironment environment,
        decimal packetMagnitude,
        GlyphGrowthEffect effect,
        GlyphVector projected)
    {
        var tensions = BuildTensions(state, environment, effect, projected);
        decimal score = effect.Score + ComputeScore(state, environment, effect.Kind, effect.TargetPosition, packetMagnitude);
        var scoredEffect = new GlyphGrowthEffect(
            effect.Kind,
            effect.SourceTipKey,
            effect.TargetPosition,
            effect.Direction,
            score,
            effect.PartnerTipKey,
            effect.JunctionKey,
            effect.Branches,
            effect.Note);

        return new DynamicProposal<GlyphGrowthEffect>(
            Name,
            context.Current.NodeId,
            scoredEffect,
            tensions,
            [],
            score,
            effect.Note);
    }

    private static IReadOnlyList<DynamicTension> BuildTensions(
        GlyphGrowthState state,
        GlyphEnvironment environment,
        GlyphGrowthEffect effect,
        GlyphVector projected)
    {
        var tensions = new List<DynamicTension>();
        if (!environment.Box.Contains(projected))
        {
            tensions.Add(new DynamicTension("BoundaryPressure", "Projected growth exceeded the glyph box.", 0.35m));
        }

        decimal stopWeight = environment.SampleInfluencesAt(effect.TargetPosition)
            .Where(influence => influence.Rule.Kind == CouplingKind.Stop)
            .Sum(influence => influence.Weight);

        if (effect.Kind == GlyphGrowthEffectKind.Grow && stopWeight > 0m)
        {
            tensions.Add(new DynamicTension("StopField", "A stop field is resisting continued growth.", stopWeight));
        }

        var fieldSample = state.TensionField?.Sample(effect.TargetPosition);
        if (effect.Kind == GlyphGrowthEffectKind.Grow && fieldSample is GlyphTensionFieldSample sample && sample.Stop > 0.08m)
        {
            tensions.Add(new DynamicTension("BitmapStopField", "The ambient bitmap is pushing growth away from this region.", sample.Stop));
        }

        return tensions;
    }

    private static decimal ResolvePacketMagnitude(
        GlyphGrowthState state,
        GlyphTip tip)
    {
        decimal packetMagnitude = state.Packets
            .Where(packet => packet.CarrierKey == tip.Key)
            .Sum(packet => packet.Magnitude);

        return packetMagnitude > 0m ? packetMagnitude : tip.Energy;
    }

    private static decimal ResolveStepSize(decimal packetMagnitude) =>
        GlyphGrowthDefaults.Step * decimal.Clamp(packetMagnitude, 0.75m, 1.25m);

    private static GlyphVector BuildGuidedDirection(
        GlyphGrowthState state,
        GlyphEnvironment environment,
        GlyphTip tip)
    {
        GlyphVector preferred = tip.PreferredDirection.Normalize();
        if (preferred == GlyphVector.Zero)
        {
            preferred = new GlyphVector(0m, 1m);
        }

        GlyphVector bias = preferred * 2.4m;
        foreach (var landmark in environment.Landmarks)
        {
            GlyphVector delta = landmark.Position - tip.Position;
            if (delta == GlyphVector.Zero)
            {
                continue;
            }

            GlyphVector contribution = landmark.Kind switch
            {
                GlyphLandmarkKind.BranchPoint => delta.Normalize() * (0.85m * landmark.Strength),
                GlyphLandmarkKind.StopPoint => delta.Normalize() * (1.15m * landmark.Strength),
                GlyphLandmarkKind.Centerline => new GlyphVector(delta.X, 0m).Normalize() * (0.45m * landmark.Strength),
                GlyphLandmarkKind.Midline => new GlyphVector(0m, delta.Y).Normalize() * (0.15m * landmark.Strength),
                GlyphLandmarkKind.Capline => new GlyphVector(0m, delta.Y).Normalize() * (0.28m * landmark.Strength),
                GlyphLandmarkKind.Baseline => new GlyphVector(0m, delta.Y).Normalize() * (0.28m * landmark.Strength),
                _ => GlyphVector.Zero,
            };

            bias += contribution;
        }

        foreach (var signal in (state.AmbientSignals ?? []).Where(signal => signal.Position.DistanceTo(tip.Position) <= signal.Radius))
        {
            GlyphVector delta = signal.Position - tip.Position;
            if (delta == GlyphVector.Zero)
            {
                continue;
            }

            decimal weight = signal.Magnitude * (1m - decimal.Clamp(tip.Position.DistanceTo(signal.Position) / signal.Radius, 0m, 1m));
            bias += signal.Kind switch
            {
                CouplingKind.Stop or CouplingKind.Repel => delta.Normalize() * (-weight),
                CouplingKind.Grow when signal.Key.StartsWith(tip.Key, StringComparison.Ordinal) => tip.PreferredDirection.Normalize() * (weight * 0.45m),
                CouplingKind.Grow => delta.Normalize() * (weight * 0.16m),
                CouplingKind.Split => delta.Normalize() * (weight * 0.55m),
                CouplingKind.Join => delta.Normalize() * (weight * 0.38m),
                CouplingKind.Align => new GlyphVector(environment.Box.MidX - tip.Position.X, 0m).Normalize() * weight,
                _ => delta.Normalize() * weight,
            };
        }

        if (state.TensionField is not null)
        {
            var fieldSample = state.TensionField.Sample(tip.Position);
            if (fieldSample.Flow != GlyphVector.Zero)
            {
                bias += fieldSample.Flow.Normalize() * (0.18m + fieldSample.Branch * 0.08m);
            }

            bias += tip.PreferredDirection.Normalize() * (fieldSample.Grow * 0.05m);
        }

        decimal leftDistance = tip.Position.X - environment.Box.Left;
        decimal rightDistance = environment.Box.Right - tip.Position.X;
        decimal horizontalGrowth = rightDistance < leftDistance ? 1m : -1m;
        bias += new GlyphVector(horizontalGrowth, 0m).Normalize() * 0.03m;

        foreach (var other in state.ActiveTips.Where(other => other.IsActive && other.Key != tip.Key))
        {
            decimal distance = tip.Position.DistanceTo(other.Position);
            if (distance <= GlyphGrowthDefaults.JoinCaptureRadius)
            {
                bias += (tip.Position - other.Position).Normalize() * 0.2m;
            }
        }

        return bias.Normalize();
    }

    private static GlyphVector ClampToBox(GlyphBox box, GlyphVector point) =>
        new(
            decimal.Clamp(point.X, box.Left, box.Right),
            decimal.Clamp(point.Y, box.Bottom, box.Top));

    private static GlyphGrowthEffect? TryCreateSplitEffect(
        GlyphGrowthState state,
        GlyphEnvironment environment,
        GlyphTip tip,
        GlyphVector direction,
        GlyphVector growTarget,
        decimal step)
    {
        var branchPoint = environment.FindClosestLandmark(tip.Position, GlyphLandmarkKind.BranchPoint);
        if (branchPoint is null || tip.CarrierKey == branchPoint.Key)
        {
            return null;
        }

        decimal distance = tip.Position.DistanceTo(branchPoint.Position);
        bool canReach = distance <= GlyphGrowthDefaults.BranchCaptureRadius ||
            growTarget.DistanceTo(branchPoint.Position) <= GlyphGrowthDefaults.BranchCaptureRadius ||
            IsAhead(tip.Position, branchPoint.Position, direction) && distance <= step * 1.35m;

        if (!canReach)
        {
            return null;
        }

        var stopPoints = environment.GetLandmarks(GlyphLandmarkKind.StopPoint)
            .Where(landmark => landmark.Position.Y >= branchPoint.Position.Y)
            .OrderBy(landmark => landmark.Position.X)
            .ToArray();

        GlyphVector leftDirection;
        GlyphVector rightDirection;
        if (stopPoints.Length >= 2)
        {
            leftDirection = (stopPoints[0].Position - branchPoint.Position).Normalize();
            rightDirection = (stopPoints[^1].Position - branchPoint.Position).Normalize();
        }
        else
        {
            leftDirection = new GlyphVector(-Math.Abs(direction.Y), Math.Max(0.25m, Math.Abs(direction.X))).Normalize();
            rightDirection = new GlyphVector(Math.Abs(direction.Y), Math.Max(0.25m, Math.Abs(direction.X))).Normalize();
        }

        decimal childEnergy = Math.Max(0.45m, tip.Energy * 0.9m);
        var branches = new[]
        {
            new GlyphGrowthBranch($"{tip.Key}:L", leftDirection, childEnergy, "Left split arm."),
            new GlyphGrowthBranch($"{tip.Key}:R", rightDirection, childEnergy, "Right split arm."),
        };

        return new GlyphGrowthEffect(
            GlyphGrowthEffectKind.Split,
            tip.Key,
            branchPoint.Position,
            direction,
            score: 0.85m + Math.Max(0m, GlyphGrowthDefaults.BranchCaptureRadius - decimal.Min(distance, growTarget.DistanceTo(branchPoint.Position))) / GlyphGrowthDefaults.BranchCaptureRadius,
            junctionKey: branchPoint.Key,
            branches: branches,
            note: "Split at the preferred branch landmark.");
    }

    private static GlyphGrowthEffect? TryCreateJoinEffect(
        GlyphGrowthState state,
        GlyphEnvironment environment,
        GlyphTip tip,
        decimal packetMagnitude)
    {
        foreach (var other in state.ActiveTips.Where(other => other.IsActive && other.Key != tip.Key))
        {
            if (!string.IsNullOrEmpty(tip.CarrierKey) &&
                tip.CarrierKey == other.CarrierKey)
            {
                continue;
            }

            decimal distance = tip.Position.DistanceTo(other.Position);
            if (distance < GlyphGrowthDefaults.Step * 1.75m)
            {
                continue;
            }

            var sharedStop = environment.GetLandmarks(GlyphLandmarkKind.StopPoint)
                .Where(stop =>
                    tip.Position.DistanceTo(stop.Position) <= GlyphGrowthDefaults.JoinCaptureRadius * 2m &&
                    other.Position.DistanceTo(stop.Position) <= GlyphGrowthDefaults.JoinCaptureRadius * 2m)
                .OrderBy(stop => stop.Position.DistanceTo(tip.Position) + stop.Position.DistanceTo(other.Position))
                .FirstOrDefault();

            if (sharedStop is null && distance > GlyphGrowthDefaults.JoinCaptureRadius * 1.5m)
            {
                continue;
            }

            GlyphVector target = sharedStop?.Position ?? Midpoint(tip.Position, other.Position);
            GlyphVector direction = (target - tip.Position).Normalize();
            if (direction == GlyphVector.Zero)
            {
                continue;
            }

            decimal closenessBonus = sharedStop is not null
                ? packetMagnitude * 1.15m
                : Math.Max(0m, GlyphGrowthDefaults.JoinCaptureRadius - distance) / GlyphGrowthDefaults.JoinCaptureRadius;

            return new GlyphGrowthEffect(
                GlyphGrowthEffectKind.Join,
                tip.Key,
                target,
                direction,
                score: closenessBonus,
                partnerTipKey: other.Key,
                junctionKey: sharedStop?.Key ?? $"join:{OrderPair(tip.Key, other.Key)}",
                note: "Capture another compatible tip and resolve into a shared junction.");
        }

        return null;
    }

    private static GlyphGrowthEffect? TryCreateStopEffect(
        GlyphEnvironment environment,
        GlyphTip tip,
        GlyphVector direction,
        GlyphVector projected,
        GlyphVector clamped,
        decimal step)
    {
        GlyphVector? target = null;
        string? junctionKey = null;

        var stopLandmark = environment.FindClosestLandmark(
            tip.Position,
            GlyphLandmarkKind.StopPoint,
            GlyphLandmarkKind.Capline,
            GlyphLandmarkKind.Baseline);

        if (stopLandmark is not null)
        {
            decimal distance = tip.Position.DistanceTo(stopLandmark.Position);
            bool canReach = distance <= GlyphGrowthDefaults.StopCaptureRadius ||
                clamped.DistanceTo(stopLandmark.Position) <= GlyphGrowthDefaults.StopCaptureRadius ||
                IsAhead(tip.Position, stopLandmark.Position, direction) && distance <= step * 1.2m;

            if (canReach)
            {
                target = stopLandmark.Position;
                junctionKey = stopLandmark.Key;
            }
        }

        if (target is null && projected != clamped)
        {
            target = clamped;
            junctionKey = $"boundary:{tip.Key}";
        }

        if (target is null)
        {
            target = TryResolveBoundaryStop(environment.Box, tip.Position, direction, step);
            if (target is not null)
            {
                junctionKey = $"boundary:{tip.Key}";
            }
        }

        return target is null
            ? null
            : new GlyphGrowthEffect(
                GlyphGrowthEffectKind.Stop,
                tip.Key,
                target.Value,
                direction,
                score: target == stopLandmark?.Position ? 0.72m : 0.48m,
                junctionKey: junctionKey,
                note: "Terminate growth at a boundary or stop landmark.");
    }

    private static GlyphVector? TryResolveBoundaryStop(
        GlyphBox box,
        GlyphVector position,
        GlyphVector direction,
        decimal step)
    {
        if (direction == GlyphVector.Zero)
        {
            return null;
        }

        GlyphVector projected = position + direction * step;
        GlyphVector clamped = ClampToBox(box, projected);
        if (projected != clamped)
        {
            return clamped;
        }

        if (direction.Y > 0m && box.Top - position.Y <= GlyphGrowthDefaults.StopCaptureRadius)
        {
            return new GlyphVector(position.X, box.Top);
        }

        if (direction.Y < 0m && position.Y - box.Bottom <= GlyphGrowthDefaults.StopCaptureRadius)
        {
            return new GlyphVector(position.X, box.Bottom);
        }

        if (direction.X > 0m && box.Right - position.X <= GlyphGrowthDefaults.StopCaptureRadius)
        {
            return new GlyphVector(box.Right, position.Y);
        }

        if (direction.X < 0m && position.X - box.Left <= GlyphGrowthDefaults.StopCaptureRadius)
        {
            return new GlyphVector(box.Left, position.Y);
        }

        return null;
    }

    private static bool IsAhead(GlyphVector from, GlyphVector to, GlyphVector direction) =>
        direction != GlyphVector.Zero &&
        (to - from).Dot(direction) > 0m;

    private static GlyphVector Midpoint(GlyphVector a, GlyphVector b) =>
        new((a.X + b.X) * 0.5m, (a.Y + b.Y) * 0.5m);

    private static string OrderPair(string left, string right) =>
        string.CompareOrdinal(left, right) <= 0
            ? $"{left}:{right}"
            : $"{right}:{left}";

    private static decimal ComputeScore(
        GlyphGrowthState state,
        GlyphEnvironment environment,
        GlyphGrowthEffectKind effectKind,
        GlyphVector target,
        decimal packetMagnitude)
    {
        decimal score = packetMagnitude;
        foreach (var influence in environment.SampleInfluencesAt(target))
        {
            score += influence.Rule.Kind switch
            {
                CouplingKind.Grow when effectKind == GlyphGrowthEffectKind.Grow => influence.Weight,
                CouplingKind.Attract when effectKind is GlyphGrowthEffectKind.Grow or GlyphGrowthEffectKind.Split or GlyphGrowthEffectKind.Join => influence.Weight * 0.7m,
                CouplingKind.Align when effectKind == GlyphGrowthEffectKind.Grow => influence.Weight * 0.55m,
                CouplingKind.Split when effectKind == GlyphGrowthEffectKind.Split => influence.Weight,
                CouplingKind.Join when effectKind == GlyphGrowthEffectKind.Join => influence.Weight,
                CouplingKind.Stop when effectKind == GlyphGrowthEffectKind.Stop => influence.Weight,
                CouplingKind.Repel => -influence.Weight,
                _ => 0m,
            };
        }

        if (state.TensionField is not null)
        {
            var fieldSample = state.TensionField.Sample(target);
            score += effectKind switch
            {
                GlyphGrowthEffectKind.Grow => fieldSample.Grow * 0.42m - fieldSample.Stop * 0.35m,
                GlyphGrowthEffectKind.Join => fieldSample.Grow * 0.28m + fieldSample.Branch * 0.1m,
                GlyphGrowthEffectKind.Split => fieldSample.Branch * 0.58m + fieldSample.Grow * 0.08m,
                GlyphGrowthEffectKind.Stop => fieldSample.Stop * 0.75m,
                _ => 0m,
            };
        }

        return Math.Max(0.01m, score);
    }
}
