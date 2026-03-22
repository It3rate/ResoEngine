using Core2.Elements;

namespace Applied.Geometry.LetterFormation;

public static class LetterFormationDirections
{
    public static Axis Horizontal => Axis.One;
    public static Axis Vertical => Axis.NegativeI;
    public static Axis UpRight => Axis.NegativeI + Axis.One;
    public static Axis UpLeft => Axis.NegativeI + Axis.NegativeOne;
    public static Axis DownRight => Axis.I + Axis.One;
    public static Axis DownLeft => Axis.I + Axis.NegativeOne;
}
