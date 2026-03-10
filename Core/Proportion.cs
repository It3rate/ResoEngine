using System;
using ResoEngine.Support;

namespace ResoEngine;
public sealed class Proportion : IProportion, IAlgebraic<long>, ISpace, IValue // can not be a subspace because it is the lowest level.
{
    private long _top;
    private long _bot;
    private Chirality _chirality;

    public int Dims => 2;
    public EndLock TopLock { get; }
    public EndLock BotLock { get; }
    public Proportion(long top, long bot, Chirality chirality = Chirality.Pro)
    {
        _top = top;
        _bot = bot;
        _chirality = chirality;
        TopLock = EndLock.None;
        BotLock = EndLock.None;
    }

    public Proportion(long top, long bot, Chirality chirality, EndLock topLock, EndLock botLock)
    {
        _top = top;
        _bot = bot;
        _chirality = chirality;
        TopLock = topLock;
        BotLock = botLock;
    }
    public long RawTop => _top;
    public long RawBot => _bot;


    // IAlgebraic<long>: fraction multiplication algebra
    // index 0 = numerator, index 1 = denominator
    // (a/b) * (c/d) = (a*c)/(b*d)
    public AlgebraEntry[] Algebra =>
    [
        new AlgebraEntry(0, 0, 0, +1), // num * num -> +num
        new AlgebraEntry(1, 1, 1, +1), // den * den -> +den
    ];

    public long GetElement(int index) => index switch
    {
        0 => GetNumerator(),
        1 => GetDenominator(),
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    public IValue[] ChildValues => [new Scalar(GetNumerator()), new Scalar(GetDenominator())];

    /// <summary>
    /// Collapse 2D (num/den) to Grade 0 scalar. Executes the deferred division.
    /// Returns double: an approximation that is "more precise than the information you had" -
    /// the honest cost of losing resolution information.
    /// </summary>
    public double Fold()
    {
        long den = GetDenominator();
        return den != 0 ? GetNumerator() / (double)den : 0.0;
    }

    public void ForceChirality(Chirality newChirality)
    {
        if (_chirality != newChirality)
        {
            _top = -_top;
            _bot = -_bot;
            _chirality = newChirality;
        }
    }

    public long GetTicksByPerspective(Chirality perspective) => perspective.IsPositive() ? GetNumerator() : GetDenominator();

    public long GetNumerator() => GetTick(_chirality);
    public long GetDenominator() => GetTick(_chirality.Invert());
    public long GetTick(Chirality chirality) => chirality == Chirality.Pro ? TopLock.Value(_top) : BotLock.Value(_bot);
    public double[] GetValues() => [_top / (double)_bot];


    public Proportion WithTop(long newTop) => new Proportion(newTop, _bot, _chirality, TopLock, BotLock);
    public Proportion WithBot(long newBot) => new Proportion(_top, newBot, _chirality, TopLock, BotLock);
    public Proportion InvertChirality() => new Proportion(-_top, -_bot, _chirality.Invert(), TopLock, BotLock);
    public Proportion Unlock() => new Proportion(_top, _bot, _chirality);


    public static Proportion Rational(long ticksPerUnit, long value) =>
        new Proportion(value, ticksPerUnit, Chirality.Pro, EndLock.Fixed, EndLock.None);
    public static Proportion Imaginary(long ticksPerUnit, long value) =>
        new Proportion(value, ticksPerUnit, Chirality.Con, EndLock.None, EndLock.Fixed);
    public static Proportion FixedRational(long ticksPerUnit, long value) =>
        new Proportion(value, ticksPerUnit, Chirality.Pro, EndLock.Fixed, EndLock.Fixed);
    public static Proportion FixedImaginary(long ticksPerUnit, long value) =>
        new Proportion(value, ticksPerUnit, Chirality.Con, EndLock.Fixed, EndLock.Fixed);

    /// <summary>
    /// Additive identity: 0/1.
    /// </summary>
    public static Proportion Zero => new Proportion(0, 1, Chirality.Pro);

    // Operators: make Proportions algebraically composable

    /// <summary>
    /// Fraction multiplication: (a/b) * (c/d) = (a*c)/(b*d)
    /// </summary>
    public static Proportion operator *(Proportion a, Proportion b)
    {
        long newTop = a.GetNumerator() * b.GetNumerator();
        long newBot = a.GetDenominator() * b.GetDenominator();
        return new Proportion(newTop, newBot, Chirality.Pro);
    }

    /// <summary>
    /// Fraction addition: (a/b) + (c/d) = (a*d + c*b) / (b*d)
    /// </summary>
    public static Proportion operator +(Proportion a, Proportion b)
    {
        long aN = a.GetNumerator(), aD = a.GetDenominator();
        long bN = b.GetNumerator(), bD = b.GetDenominator();
        long newTop = aN * bD + bN * aD;
        long newBot = aD * bD;
        return new Proportion(newTop, newBot, Chirality.Pro);
    }

    /// <summary>
    /// Unary negation: -(a/b) = (-a)/b. Used by algebra sign application.
    /// </summary>
    public static Proportion operator -(Proportion p)
    {
        return new Proportion(-p.GetNumerator(), p.GetDenominator(), Chirality.Pro);
    }


    public override string ToString()
    {
        return $"({GetTick(Chirality.Pro)},{GetTick(Chirality.Con)}){((int)_chirality > 0 ? '↑': '↓')}";
    }
}
