using Core2.Elements;
using ResoEngine.Core2.Support;

namespace Core2.Support;

internal static class ElementArithmeticResolver
{
    public static T Zero<T>() where T : IElement => For<T>().Zero;

    public static T Add<T>(T left, T right) where T : IElement => For<T>().Add(left, right);

    public static T Multiply<T>(T left, T right) where T : IElement => For<T>().Multiply(left, right);

    public static T Negate<T>(T value) where T : IElement => For<T>().Negate(value);

    public static T One<T>() where T : IElement
    {
        object result = typeof(T) switch
        {
            var type when type == typeof(Scalar) => Scalar.One,
            var type when type == typeof(Proportion) => Proportion.One,
            var type when type == typeof(Axis) => Axis.One,
            var type when type == typeof(Area) => Area.One,
            _ => throw new NotSupportedException($"No multiplicative identity is registered for element type {typeof(T).Name}."),
        };

        return (T)result;
    }

    private static IArithmetic<T> For<T>() where T : IElement
    {
        object arithmetic = typeof(T) switch
        {
            var type when type == typeof(Scalar) => Scalar.Arithmetic,
            var type when type == typeof(Proportion) => Proportion.Arithmetic,
            var type when type == typeof(Axis) => Axis.Arithmetic,
            var type when type == typeof(Area) => Area.Arithmetic,
            _ => throw new NotSupportedException($"No arithmetic is registered for element type {typeof(T).Name}."),
        };

        return (IArithmetic<T>)arithmetic;
    }
}
