using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record PinTerm : ValueTerm
{
    public PinTerm(ValueTerm host, ValueTerm applied, Proportion position)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(applied);
        ArgumentNullException.ThrowIfNull(position);

        Host = host;
        Applied = applied;
        Position = position;
    }

    public ValueTerm Host { get; }
    public ValueTerm Applied { get; }
    public Proportion Position { get; }
}
