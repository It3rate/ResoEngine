namespace Core3.Engine;

/// <summary>
/// Small explicit support-choice policy for exact alignment.
/// The values intentionally use a two-bit shape so the default policies remain
/// compact and physically suggestive: 00 compose, 01 preserve host,
/// 10 preserve applied, 11 exact common frame.
/// </summary>
public enum ResolutionPolicy
{
    ComposeSupport = 0b00,
    PreserveHost = 0b01,
    PreserveApplied = 0b10,
    ExactCommonFrame = 0b11
}
