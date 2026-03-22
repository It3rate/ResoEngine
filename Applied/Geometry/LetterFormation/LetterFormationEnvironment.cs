using Applied.Geometry.Utils;
using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public sealed record LetterFormationEnvironment(
    PlanarPoint TopLeft,
    PlanarPoint BottomRight,
    Proportion CenterlineX,
    Proportion MidlineY,
    Proportion RandomMotionWeight)
{
    public Proportion Left => TopLeft.Horizontal;
    public Proportion Top => TopLeft.Vertical;
    public Proportion Right => BottomRight.Horizontal;
    public Proportion Bottom => BottomRight.Vertical;
    public Proportion Width => Right - Left;
    public Proportion Height => Bottom - Top;

    public PlanarPoint ResolveRelativePoint(Proportion horizontal, Proportion vertical) =>
        new(Left + (Width * horizontal), Top + (Height * vertical));

    public static LetterFormationEnvironment CreateLetterBox(
        int widthTicks = 10,
        int heightTicks = 14,
        Proportion? midlineRatio = null,
        Proportion? randomMotionWeight = null)
    {
        Proportion width = new(widthTicks);
        Proportion height = new(heightTicks);
        Proportion midlineRatioValue = midlineRatio ?? new Proportion(42, 100);
        return new LetterFormationEnvironment(
            new PlanarPoint(Proportion.Zero, Proportion.Zero),
            new PlanarPoint(width, height),
            width / new Proportion(2),
            height * midlineRatioValue,
            randomMotionWeight ?? Proportion.One);
    }
}
