using Core2.Branching;
using Core2.Elements;
using Core2.Repetition;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

public static partial class SymbolicParser
{
    private sealed partial class Parser
    {
        private FoldTerm ParseFold()
        {
            var name = Advance().Text;
            SymbolicFoldKind kind = name switch
            {
                "fold" => SymbolicFoldKind.Canonical,
                "fold-first" => SymbolicFoldKind.FoldFirst,
                "structure-preserving" => SymbolicFoldKind.StructurePreserving,
                _ => throw Error($"Unknown fold form '{name}'."),
            };

            Expect(TokenKind.LeftParen);
            var source = ParseValueLike();
            Expect(TokenKind.RightParen);
            return new FoldTerm(ToValueTerm(source), kind);
        }

        private BranchFamilyTerm ParseBranch()
        {
            ConsumeIdentifier("branch");
            Expect(TokenKind.LeftBrace);

            List<ValueTerm> values = [ToValueTerm(ParseValueLike())];
            while (Match(TokenKind.Pipe))
            {
                values.Add(ToValueTerm(ParseValueLike()));
            }

            Expect(TokenKind.RightBrace);

            var family = BranchFamily<ValueTerm>.FromValues(
                BranchOrigin.Continuation,
                BranchSemantics.Alternative,
                BranchDirection.Forward,
                values);

            return new BranchFamilyTerm(family);
        }

        private ValueTerm ParseCount()
        {
            ConsumeIdentifier("count");
            Expect(TokenKind.LeftParen);

            if (Current.Kind == TokenKind.Identifier &&
                Peek(1).Kind == TokenKind.RightParen &&
                TryParseGlobalCountKind(Current.Text, out var globalKind))
            {
                Advance();
                Expect(TokenKind.RightParen);
                return new CountTerm(globalKind);
            }

            string subjectName = ExpectIdentifier();
            Expect(TokenKind.Comma);
            string kindName = ExpectIdentifier();
            Expect(TokenKind.RightParen);

            if (TryParseSiteCountKind(kindName, out var siteKind))
            {
                return new CountTerm(new SiteReferenceTerm(subjectName), siteKind);
            }

            if (TryParseCarrierCountKind(kindName, out var carrierKind))
            {
                return new CarrierCountTerm(new CarrierReferenceTerm(subjectName), carrierKind);
            }

            throw Error($"Unknown count kind '{kindName}'.");
        }

        private AnchorPositionTerm ParsePosition()
        {
            ConsumeIdentifier("position");
            Expect(TokenKind.LeftParen);
            var anchor = ParseAnchorReference();
            Expect(TokenKind.RightParen);
            return new AnchorPositionTerm(anchor);
        }

        private CarrierSpanTerm ParseSpan()
        {
            ConsumeIdentifier("span");
            Expect(TokenKind.LeftParen);
            var carrier = ParseCarrierReference();
            Expect(TokenKind.RightParen);
            return new CarrierSpanTerm(carrier);
        }

        private ContinueTerm ParseContinueTerm()
        {
            ConsumeIdentifier("continue");
            Expect(TokenKind.LeftParen);
            var frame = ToValueTerm(ParseValueLike());
            Expect(TokenKind.Comma);
            var value = ToValueTerm(ParseValueLike());
            Expect(TokenKind.Comma);
            var law = ParseBoundaryContinuationLaw();
            Expect(TokenKind.RightParen);
            return new ContinueTerm(frame, value, law);
        }

        private PowerTerm ParsePower()
        {
            ConsumeIdentifier("pow");
            Expect(TokenKind.LeftParen);
            var @base = ToValueTerm(ParseValueLike());
            Expect(TokenKind.Comma);
            var exponent = ParseProportionLiteral();
            var rule = InverseContinuationRule.Principal;
            ValueTerm? reference = null;

            if (Match(TokenKind.Comma))
            {
                rule = ParseInverseContinuationRule();
                if (Match(TokenKind.Comma))
                {
                    reference = ToValueTerm(ParseValueLike());
                }
            }

            Expect(TokenKind.RightParen);
            return new PowerTerm(@base, exponent, rule, reference);
        }

        private InverseContinueTerm ParseInverseContinuation()
        {
            Advance();
            Expect(TokenKind.LeftParen);
            var source = ToValueTerm(ParseValueLike());
            Expect(TokenKind.Comma);
            var degree = ParseProportionLiteral();
            var rule = InverseContinuationRule.Principal;
            ValueTerm? reference = null;

            if (Match(TokenKind.Comma))
            {
                rule = ParseInverseContinuationRule();
                if (Match(TokenKind.Comma))
                {
                    reference = ToValueTerm(ParseValueLike());
                }
            }

            Expect(TokenKind.RightParen);
            return new InverseContinueTerm(source, degree, rule, reference);
        }

        private ValueTerm ParseValueReferenceLike()
        {
            string name = ExpectIdentifier();
            if (TryParseAnchorName(name, out var ownerName, out var anchorName))
            {
                return new AnchorReferenceTerm(ownerName, anchorName);
            }

            if (IsSiteName(name))
            {
                return new SiteReferenceTerm(name);
            }

            return new ValueReferenceTerm(name);
        }

        private SiteReferenceTerm ParseSiteReference()
        {
            string name = ExpectIdentifier();
            return new SiteReferenceTerm(name);
        }

        private SymbolicBindingTarget ParseBindingTarget()
        {
            string name = ExpectIdentifier();
            return new SymbolicBindingTarget(name);
        }

        private AnchorReferenceTerm ParseAnchorReference()
        {
            string name = ExpectIdentifier();
            if (!TryParseAnchorName(name, out var ownerName, out var anchorName))
            {
                throw Error($"Expected anchor reference like A.P1, but found '{name}'.");
            }

            return new AnchorReferenceTerm(ownerName, anchorName);
        }

        private CarrierReferenceTerm ParseCarrierReference()
        {
            string name = ExpectIdentifier();
            return new CarrierReferenceTerm(name);
        }

        private IncidentReferenceTerm ParseIncidentReference()
        {
            string name = ExpectIdentifier();
            return name switch
            {
                "host-" => new IncidentReferenceTerm(RouteIncidentKind.HostNegative),
                "host+" => new IncidentReferenceTerm(RouteIncidentKind.HostPositive),
                "i" => new IncidentReferenceTerm(RouteIncidentKind.RecessiveSide),
                "u" => new IncidentReferenceTerm(RouteIncidentKind.DominantSide),
                _ => throw Error($"Unknown route incident '{name}'."),
            };
        }
    }
}
