using System;
using System.Collections.Generic;
using System.Text;

namespace ResoEngine.Support;

public interface ISpace
{
    int Dims { get; }
    //AlgebraTable<T> Algebra { get; }
    IValue[] ChildValues { get; }
}
public interface ISubSpace : ISpace
{
    ISpace[] FrameElements { get; }
}
