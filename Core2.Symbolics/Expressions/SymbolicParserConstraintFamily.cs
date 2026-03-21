namespace Core2.Symbolics.Expressions;

public static partial class SymbolicParser
{
    private sealed partial class Parser
    {
        private SymbolicTerm ParseConstraintOrRelationOrValue()
        {
            if (PeekIdentifier("require"))
            {
                return ParseRequire();
            }

            if (PeekIdentifier("prefer"))
            {
                return ParsePrefer();
            }

            if (PeekIdentifier("constraints"))
            {
                return ParseConstraintSet();
            }

            if (PeekIdentifier("share"))
            {
                return ParseShare();
            }

            if (PeekIdentifier("route"))
            {
                return ParseRoute();
            }

            if (PeekIdentifier("junction"))
            {
                return ParseJunction();
            }

            if (PeekIdentifier("has"))
            {
                return ParseSiteFlag();
            }

            if (TryPeekBooleanOperation(out _))
            {
                return ParseBoolean();
            }

            var left = ParseValueLike();
            if (Match(TokenKind.EqualEqual))
            {
                var right = ParseValueLike();
                return new EqualityTerm(left, right);
            }

            return left;
        }

        private RequirementTerm ParseRequire()
        {
            ConsumeIdentifier("require");
            Expect(TokenKind.LeftParen);
            string? participantName = TryParseLeadingParticipantName();
            var relation = ParseRelationInsideFunction();
            Expect(TokenKind.RightParen);
            return new RequirementTerm(relation, participantName);
        }

        private PreferenceTerm ParsePrefer()
        {
            ConsumeIdentifier("prefer");
            Expect(TokenKind.LeftParen);
            string? participantName = TryParseLeadingParticipantName();
            var relation = ParseRelationInsideFunction();
            Expect(TokenKind.Comma);
            var weight = ParseProportionLiteral();
            Expect(TokenKind.RightParen);
            return new PreferenceTerm(relation, weight, participantName);
        }

        private ConstraintSetTerm ParseConstraintSet()
        {
            ConsumeIdentifier("constraints");
            Expect(TokenKind.LeftBrace);

            List<ConstraintTerm> constraints = [ParseConstraintTerm()];
            while (Match(TokenKind.Pipe))
            {
                constraints.Add(ParseConstraintTerm());
            }

            Expect(TokenKind.RightBrace);
            return new ConstraintSetTerm(constraints);
        }

        private SharedCarrierTerm ParseShare()
        {
            ConsumeIdentifier("share");
            Expect(TokenKind.LeftParen);
            var left = ParseValueReferenceLike();
            Expect(TokenKind.Comma);
            var right = ParseValueReferenceLike();
            Expect(TokenKind.RightParen);
            return new SharedCarrierTerm(left, right);
        }

        private RouteTerm ParseRoute()
        {
            ConsumeIdentifier("route");
            Expect(TokenKind.LeftParen);
            var site = ParseSiteReference();
            Expect(TokenKind.Comma);
            var from = ParseIncidentReference();
            Expect(TokenKind.Comma);
            var to = ParseIncidentReference();
            Expect(TokenKind.RightParen);
            return new RouteTerm(site, from, to);
        }

        private JunctionTerm ParseJunction()
        {
            ConsumeIdentifier("junction");
            Expect(TokenKind.LeftParen);
            var site = ParseSiteReference();
            Expect(TokenKind.Comma);
            var kind = ParseJunctionKind();
            Expect(TokenKind.RightParen);
            return new JunctionTerm(site, kind);
        }

        private RelationTerm ParseSiteFlag()
        {
            ConsumeIdentifier("has");
            Expect(TokenKind.LeftParen);
            string subjectName = ExpectIdentifier();
            Expect(TokenKind.Comma);
            string kindName = ExpectIdentifier();
            Expect(TokenKind.RightParen);

            if (TryParseSiteFlagKind(kindName, out var siteKind))
            {
                return new SiteFlagTerm(new SiteReferenceTerm(subjectName), siteKind);
            }

            if (TryParseCarrierFlagKind(kindName, out var carrierKind))
            {
                return new CarrierFlagTerm(new CarrierReferenceTerm(subjectName), carrierKind);
            }

            throw Error($"Unknown has(...) flag '{kindName}'.");
        }

        private AxisBooleanTerm ParseBoolean()
        {
            var operation = ParseBooleanOperationIdentifier();
            Expect(TokenKind.LeftParen);
            var primary = ToValueTerm(ParseValueLike());
            Expect(TokenKind.Comma);
            var secondary = ToValueTerm(ParseValueLike());

            ValueTerm? frame = null;
            if (Match(TokenKind.Comma))
            {
                frame = ToValueTerm(ParseValueLike());
            }

            Expect(TokenKind.RightParen);
            return new AxisBooleanTerm(primary, secondary, operation, frame);
        }

        private RelationTerm ParseRelationInsideFunction()
        {
            if (PeekIdentifier("share"))
            {
                return ParseShare();
            }

            if (PeekIdentifier("route"))
            {
                return ParseRoute();
            }

            if (PeekIdentifier("junction"))
            {
                return ParseJunction();
            }

            if (PeekIdentifier("has"))
            {
                return ParseSiteFlag();
            }

            var left = ParseValueLike();
            if (!Match(TokenKind.EqualEqual))
            {
                throw Error("Expected a relation inside function arguments.");
            }

            var right = ParseValueLike();
            return new EqualityTerm(left, right);
        }

        private ConstraintTerm ParseConstraintTerm()
        {
            if (PeekIdentifier("require"))
            {
                return ParseRequire();
            }

            if (PeekIdentifier("prefer"))
            {
                return ParsePrefer();
            }

            if (PeekIdentifier("constraints"))
            {
                return ParseConstraintSet();
            }

            throw Error("Expected a constraint term.");
        }
    }
}
