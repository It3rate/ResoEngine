namespace ResoEngine.Core2.Support;

public interface IArithmetic<T>
{
    T Zero { get; }
    T Add(T left, T right);
    T Multiply(T left, T right);
    T Negate(T value);
}
