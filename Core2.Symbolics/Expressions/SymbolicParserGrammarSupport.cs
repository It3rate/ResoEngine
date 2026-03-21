using Core2.Boolean;
using Core2.Elements;
using Core2.Repetition;
using Core2.Symbolics.Repetition;

namespace Core2.Symbolics.Expressions;

public static partial class SymbolicParser
{
    private sealed partial class Parser
    {
        private SymbolicJunctionKind ParseJunctionKind()
        {
            string name = ExpectIdentifier();
            return name switch
            {
                "open" => SymbolicJunctionKind.Open,
                "cusp" => SymbolicJunctionKind.Cusp,
                "branch" => SymbolicJunctionKind.Branch,
                "tee" => SymbolicJunctionKind.Tee,
                "cross" => SymbolicJunctionKind.Cross,
                _ => throw Error($"Unknown junction kind '{name}'."),
            };
        }

        private static bool TryParseSiteFlagKind(string name, out SymbolicSiteFlagKind kind)
        {
            kind = name switch
            {
                "host-through" => SymbolicSiteFlagKind.HostThrough,
                "cross-proposal" => SymbolicSiteFlagKind.CrossProposal,
                "true-cross" => SymbolicSiteFlagKind.TrueCross,
                _ => default,
            };

            return name is "host-through" or "cross-proposal" or "true-cross";
        }

        private static bool TryParseGlobalCountKind(string name, out SymbolicCountKind kind)
        {
            kind = name switch
            {
                "carriers" => SymbolicCountKind.Carriers,
                "sites" => SymbolicCountKind.Sites,
                _ => default,
            };

            return name is "carriers" or "sites";
        }

        private static bool TryParseSiteCountKind(string name, out SymbolicCountKind kind)
        {
            kind = name switch
            {
                "participating-carriers" => SymbolicCountKind.ParticipatingCarriers,
                "through-carriers" => SymbolicCountKind.ThroughCarriers,
                _ => default,
            };

            return name is "participating-carriers" or "through-carriers";
        }

        private static bool TryParseCarrierCountKind(string name, out SymbolicCarrierCountKind kind)
        {
            kind = name switch
            {
                "hosted-sites" => SymbolicCarrierCountKind.HostedSites,
                "attachments" => SymbolicCarrierCountKind.Attachments,
                "referencing-hosts" => SymbolicCarrierCountKind.ReferencingHosts,
                "participating-sites" => SymbolicCarrierCountKind.ParticipatingSites,
                "through-sites" => SymbolicCarrierCountKind.ThroughSites,
                _ => default,
            };

            return name is
                "hosted-sites" or
                "attachments" or
                "referencing-hosts" or
                "participating-sites" or
                "through-sites";
        }

        private static bool TryParseCarrierFlagKind(string name, out SymbolicCarrierFlagKind kind)
        {
            kind = name switch
            {
                "shared" => SymbolicCarrierFlagKind.Shared,
                "recursive" => SymbolicCarrierFlagKind.Recursive,
                "span" => SymbolicCarrierFlagKind.Span,
                "hosted" => SymbolicCarrierFlagKind.Hosted,
                "referenced" => SymbolicCarrierFlagKind.Referenced,
                _ => default,
            };

            return name is "shared" or "recursive" or "span" or "hosted" or "referenced";
        }

        private string? TryParseLeadingParticipantName()
        {
            if (Current.Kind == TokenKind.Identifier &&
                Peek(1).Kind == TokenKind.Comma &&
                !IsReservedRelationHead(Current.Text))
            {
                string participant = Advance().Text;
                Expect(TokenKind.Comma);
                return participant;
            }

            return null;
        }

        private AxisBooleanOperation ParseBooleanOperationIdentifier()
        {
            string name = ExpectIdentifier();
            return TryGetBooleanOperation(name, out var operation)
                ? operation
                : throw Error($"Unknown boolean operation '{name}'.");
        }

        private InverseContinuationRule ParseInverseContinuationRule()
        {
            string name = ExpectIdentifier();
            return name switch
            {
                "principal" => InverseContinuationRule.Principal,
                "prefer-positive" => InverseContinuationRule.PreferPositiveDominant,
                "nearest" => InverseContinuationRule.NearestToReference,
                _ => throw Error($"Unknown inverse-continuation rule '{name}'."),
            };
        }

        private BoundaryContinuationLaw ParseBoundaryContinuationLaw()
        {
            string name = ExpectIdentifier();
            return name switch
            {
                "wrap" => BoundaryContinuationLaw.PeriodicWrap,
                "reflect" => BoundaryContinuationLaw.ReflectiveBounce,
                "clamp" => BoundaryContinuationLaw.Clamp,
                "tension" => BoundaryContinuationLaw.TensionPreserving,
                _ => throw Error($"Unknown boundary-continuation law '{name}'."),
            };
        }

        private ValueTerm ToValueTerm(SymbolicTerm term) => term switch
        {
            ValueTerm value => value,
            ReferenceTerm reference => new ValueReferenceTerm(reference.Name),
            _ => throw Error($"Expected a value term, but found {term.GetType().Name}."),
        };

        private TransformTerm ToTransformTerm(SymbolicTerm term) => term switch
        {
            TransformTerm transform => transform,
            ElementLiteralTerm literal => new TransformLiteralTerm(literal.Value),
            ReferenceTerm reference => new TransformReferenceTerm(reference.Name),
            _ => throw Error($"Expected a transform term, but found {term.GetType().Name}."),
        };

        private static bool ShouldTreatAsTransformApplication(SymbolicTerm term) => term switch
        {
            TransformTerm => true,
            ElementLiteralTerm literal => IsTransformShorthandLiteral(literal.Value),
            _ => false,
        };

        private static bool IsTransformShorthandLiteral(IElement element) => element switch
        {
            Scalar => true,
            Proportion => true,
            Axis axis when axis == Axis.One || axis == Axis.I || axis == Axis.NegativeOne || axis == Axis.NegativeI => true,
            _ => false,
        };

        private bool TryPeekBooleanOperation(out AxisBooleanOperation operation)
        {
            if (Current.Kind == TokenKind.Identifier &&
                TryGetBooleanOperation(Current.Text, out operation))
            {
                return true;
            }

            operation = default;
            return false;
        }

        private static bool TryGetBooleanOperation(string name, out AxisBooleanOperation operation)
        {
            operation = name switch
            {
                "false-op" => AxisBooleanOperation.False,
                "true-op" => AxisBooleanOperation.True,
                "transfer-a" => AxisBooleanOperation.TransferA,
                "transfer-b" => AxisBooleanOperation.TransferB,
                "and" => AxisBooleanOperation.And,
                "or" => AxisBooleanOperation.Or,
                "nand" => AxisBooleanOperation.Nand,
                "nor" => AxisBooleanOperation.Nor,
                "not-a" => AxisBooleanOperation.NotA,
                "not-b" => AxisBooleanOperation.NotB,
                "implication" => AxisBooleanOperation.Implication,
                "reverse-implication" => AxisBooleanOperation.ReverseImplication,
                "inhibition" => AxisBooleanOperation.Inhibition,
                "reverse-inhibition" => AxisBooleanOperation.ReverseInhibition,
                "xor" => AxisBooleanOperation.Xor,
                "xnor" => AxisBooleanOperation.Xnor,
                _ => default,
            };

            return name is
                "false-op" or
                "true-op" or
                "transfer-a" or
                "transfer-b" or
                "and" or
                "or" or
                "nand" or
                "nor" or
                "not-a" or
                "not-b" or
                "implication" or
                "reverse-implication" or
                "inhibition" or
                "reverse-inhibition" or
                "xor" or
                "xnor";
        }
    }
}
