namespace Core2.Symbolics.Expressions;

public sealed record SiteReferenceTerm : ValueTerm
{
    public SiteReferenceTerm(string siteName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteName);

        SiteName = siteName;
    }

    public string SiteName { get; }
}
