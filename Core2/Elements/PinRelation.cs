using System.Collections.Generic;

namespace Core2.Elements;

public readonly record struct PinRelation(
    PinRelationMode Mode,
    PinPolarityMode Polarity = PinPolarityMode.Neutral,
    PinHandednessMode Handedness = PinHandednessMode.Neutral,
    PinContactMode Contact = PinContactMode.None,
    int QuarterTurns = 0)
{
    public static PinRelation Ordered => new(PinRelationMode.Ordered);

    public static PinRelation CollinearSame =>
        new(PinRelationMode.Collinear, PinPolarityMode.Same);

    public static PinRelation CollinearOpposed =>
        new(PinRelationMode.Collinear, PinPolarityMode.Opposed);

    public static PinRelation Parallel => CollinearSame;

    public static PinRelation OrthogonalDirect =>
        new(PinRelationMode.Orthogonal, PinPolarityMode.Neutral, PinHandednessMode.Direct);

    public static PinRelation OrthogonalMirrored =>
        new(PinRelationMode.Orthogonal, PinPolarityMode.Neutral, PinHandednessMode.Mirrored);

    public static PinRelation Orthogonal => OrthogonalDirect;

    public static PinRelation Twisted(
        PinContactMode contact = PinContactMode.Point,
        int quarterTurns = 1,
        PinHandednessMode handedness = PinHandednessMode.Direct) =>
        new(PinRelationMode.Twisted, PinPolarityMode.Neutral, handedness, contact, quarterTurns);

    public bool IsCollinear => Mode == PinRelationMode.Collinear;
    public bool IsOrthogonal => Mode == PinRelationMode.Orthogonal;
    public bool IsTwisted => Mode == PinRelationMode.Twisted;

    public override string ToString()
    {
        return Mode switch
        {
            PinRelationMode.Ordered => "Ordered",
            PinRelationMode.Collinear => $"Collinear({Polarity})",
            PinRelationMode.Orthogonal when Handedness == PinHandednessMode.Neutral => "Orthogonal",
            PinRelationMode.Orthogonal => $"Orthogonal({Handedness})",
            PinRelationMode.Twisted => DescribeTwisted(),
            _ => $"{Mode}({Polarity}, {Handedness}, {Contact}, {QuarterTurns}q)",
        };
    }

    private string DescribeTwisted()
    {
        string contact = Contact == PinContactMode.None ? string.Empty : Contact.ToString();
        string handedness = Handedness == PinHandednessMode.Neutral ? string.Empty : Handedness.ToString();
        var parts = new List<string>();
        if (contact.Length > 0)
        {
            parts.Add(contact);
        }

        if (handedness.Length > 0)
        {
            parts.Add(handedness);
        }

        parts.Add($"{QuarterTurns}q");

        return $"Twisted({string.Join(", ", parts)})";
    }
}
