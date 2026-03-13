using Core2.Elements;
using Core2.Support;

namespace Core2.Repetition;

public static class RepetitionEngine
{
    public static RepetitionTrace<T> RepeatAdditive<T>(T value, int count)
        where T : IElement
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        List<T> states = [ElementArithmeticResolver.Zero<T>()];
        T current = states[0];

        for (int i = 0; i < count; i++)
        {
            current = ElementArithmeticResolver.Add(current, value);
            states.Add(current);
        }

        return new RepetitionTrace<T>(RepetitionKind.Additive, states);
    }

    public static RepetitionTrace<T> RepeatMultiplicative<T>(T value, int count)
        where T : IElement
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        List<T> states = [ElementArithmeticResolver.One<T>()];
        T current = states[0];

        for (int i = 0; i < count; i++)
        {
            current = ElementArithmeticResolver.Multiply(current, value);
            states.Add(current);
        }

        return new RepetitionTrace<T>(RepetitionKind.Multiplicative, states);
    }

    public static RepetitionTrace<T> RepeatTransform<T>(T seed, T transform, int count)
        where T : IElement
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        List<T> states = [seed];
        T current = seed;

        for (int i = 0; i < count; i++)
        {
            current = ElementArithmeticResolver.Multiply(current, transform);
            states.Add(current);
        }

        return new RepetitionTrace<T>(RepetitionKind.Transform, states);
    }

    public static RepetitionTrace<T> RepeatRecursive<T>(
        IReadOnlyList<T> seeds,
        int additionalSteps,
        Func<IReadOnlyList<T>, T> next)
    {
        ArgumentNullException.ThrowIfNull(seeds);
        ArgumentNullException.ThrowIfNull(next);
        ArgumentOutOfRangeException.ThrowIfNegative(additionalSteps);

        if (seeds.Count == 0)
        {
            throw new ArgumentException("Recursive repetition requires at least one seed.", nameof(seeds));
        }

        List<T> states = [.. seeds];

        for (int i = 0; i < additionalSteps; i++)
        {
            states.Add(next(states));
        }

        return new RepetitionTrace<T>(RepetitionKind.Recursive, states);
    }
}
