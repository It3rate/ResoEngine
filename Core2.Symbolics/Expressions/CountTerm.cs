namespace Core2.Symbolics.Expressions;

public sealed record CountTerm : ValueTerm
{
    public CountTerm(SymbolicCountKind kind)
        : this(null, kind)
    {
    }

    public CountTerm(SiteReferenceTerm? site, SymbolicCountKind kind)
    {
        Site = site;
        Kind = kind;
    }

    public SiteReferenceTerm? Site { get; }
    public SymbolicCountKind Kind { get; }
    public bool IsGlobal => Site is null;
}
