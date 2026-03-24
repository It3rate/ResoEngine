namespace Core3.Elements;

/// <summary>
/// A thing that can be reduced to a scalar readout when needed.
/// Core3 should prefer carrying structure until this reduction is explicitly required.
/// </summary>
public interface IScalar
{
    decimal ToScalar();
}
