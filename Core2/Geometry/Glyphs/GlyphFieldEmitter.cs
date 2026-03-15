using Core2.Propagation;

namespace Core2.Geometry.Glyphs;

public abstract record GlyphFieldEmitter(
    string Key,
    IReadOnlyList<CouplingRule> Couplings,
    decimal BaseStrength = 1m,
    GlyphFieldFalloff Falloff = GlyphFieldFalloff.Linear,
    string? Note = null)
{
    public abstract decimal SampleAt(GlyphVector point);

    public IReadOnlyList<GlyphFieldInfluence> EmitAt(GlyphVector point)
    {
        decimal sample = Math.Max(0m, SampleAt(point) * BaseStrength);
        if (sample == 0m)
        {
            return [];
        }

        return Couplings
            .Select(rule => new GlyphFieldInfluence(Key, rule, sample * rule.Strength))
            .Where(influence => influence.Weight != 0m)
            .ToArray();
    }

    protected decimal ApplyFalloff(decimal distance, decimal radius)
    {
        if (radius <= 0m)
        {
            return distance == 0m ? 1m : 0m;
        }

        if (distance >= radius)
        {
            return 0m;
        }

        return Falloff switch
        {
            GlyphFieldFalloff.Constant => 1m,
            GlyphFieldFalloff.Linear => 1m - distance / radius,
            _ => 0m,
        };
    }
}
