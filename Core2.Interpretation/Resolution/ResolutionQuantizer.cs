using Core2.Elements;

namespace Core2.Interpretation.Resolution;

internal static class ResolutionQuantizer
{
    public static Scalar Quantize(Scalar value, ResolutionQuantizationRule rule) =>
        new(rule switch
        {
            ResolutionQuantizationRule.Nearest => decimal.Round(value.Value, 0, MidpointRounding.AwayFromZero),
            ResolutionQuantizationRule.Floor => decimal.Floor(value.Value),
            ResolutionQuantizationRule.Ceiling => decimal.Ceiling(value.Value),
            ResolutionQuantizationRule.TowardZero => decimal.Truncate(value.Value),
            ResolutionQuantizationRule.AwayFromZero => value.Value >= 0m
                ? decimal.Ceiling(value.Value)
                : decimal.Floor(value.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null),
        });
}
