namespace Core2.Symbolics.Expressions;

internal static class SymbolicConstraintTermSupport
{
    public static bool IsClosed(SymbolicTerm term) =>
        term switch
        {
            ReferenceTerm => false,
            SiteReferenceTerm => false,
            AnchorReferenceTerm => false,
            ElementLiteralTerm => true,
            TransformLiteralTerm => true,
            IncidentReferenceTerm => true,
            ApplyTransformTerm apply => IsClosed(apply.State) && IsClosed(apply.Transform),
            PinTerm pin => IsClosed(pin.Host) && IsClosed(pin.Applied) && (pin.AppliedAnchor is null || IsClosed(pin.AppliedAnchor)),
            PinToPinTerm pinToPin => IsClosed(pinToPin.HostAnchor) && IsClosed(pinToPin.AppliedAnchor),
            AxisBooleanTerm boolean => IsClosed(boolean.Primary) && IsClosed(boolean.Secondary) && (boolean.Frame is null || IsClosed(boolean.Frame)),
            FoldTerm fold => IsClosed(fold.Source),
            EqualityTerm equality => IsClosed(equality.Left) && IsClosed(equality.Right),
            SharedCarrierTerm shared => IsClosed(shared.Left) && IsClosed(shared.Right),
            RouteTerm route => IsClosed(route.Site) && IsClosed(route.From) && IsClosed(route.To),
            JunctionTerm junction => IsClosed(junction.Site),
            SiteFlagTerm flag => IsClosed(flag.Site),
            CarrierFlagTerm flag => IsClosed(flag.Carrier),
            RequirementTerm requirement => IsClosed(requirement.Relation),
            PreferenceTerm preference => IsClosed(preference.Relation),
            ConstraintSetTerm set => set.Constraints.All(IsClosed),
            BranchFamilyTerm branch => branch.Family.Values.All(IsClosed),
            _ => false,
        };

    public static string FormatSiteFlagKind(SymbolicSiteFlagKind kind) => kind switch
    {
        SymbolicSiteFlagKind.HostThrough => "host-through",
        SymbolicSiteFlagKind.CrossProposal => "cross-proposal",
        SymbolicSiteFlagKind.TrueCross => "true-cross",
        _ => kind.ToString().ToLowerInvariant(),
    };

    public static string FormatCarrierFlagKind(SymbolicCarrierFlagKind kind) => kind switch
    {
        SymbolicCarrierFlagKind.Shared => "shared",
        SymbolicCarrierFlagKind.Recursive => "recursive",
        SymbolicCarrierFlagKind.Span => "span",
        SymbolicCarrierFlagKind.Hosted => "hosted",
        SymbolicCarrierFlagKind.Referenced => "referenced",
        _ => kind.ToString().ToLowerInvariant(),
    };
}
