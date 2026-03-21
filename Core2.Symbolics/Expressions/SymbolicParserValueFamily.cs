using Core2.Boolean;
using Core2.Branching;
using Core2.Elements;
using Core2.Repetition;
using Core2.Symbolics.Repetition;
using System.Globalization;

namespace Core2.Symbolics.Expressions;

public static partial class SymbolicParser
{
    private sealed partial class Parser
    {
        private SymbolicTerm ParseValueLike()
        {
            var left = ParseAtomicValueLike();
            while (true)
            {
                if (Match(TokenKind.Asterisk))
                {
                    var right = ParseAtomicValueLike();
                    if (Match(TokenKind.At))
                    {
                        var position = ParseProportionLiteral();
                        if (right is AnchorReferenceTerm anchor)
                        {
                            left = new PinTerm(
                                ToValueTerm(left),
                                new ValueReferenceTerm(anchor.OwnerName),
                                position,
                                anchor);
                        }
                        else
                        {
                            left = new PinTerm(ToValueTerm(left), ToValueTerm(right), position);
                        }

                        continue;
                    }

                    left = ShouldTreatAsTransformApplication(right)
                        ? new ApplyTransformTerm(ToValueTerm(left), ToTransformTerm(right))
                        : new MultiplyValuesTerm(ToValueTerm(left), ToValueTerm(right));
                    continue;
                }

                if (Match(TokenKind.Slash))
                {
                    var right = ParseAtomicValueLike();
                    left = new DivideValuesTerm(ToValueTerm(left), ToValueTerm(right));
                    continue;
                }

                return left;
            }
        }

        private SymbolicTerm ParseAtomicValueLike()
        {
            if (Match(TokenKind.LeftParen))
            {
                var inner = ParseConstraintOrRelationOrValue();
                Expect(TokenKind.RightParen);
                return inner;
            }

            if (PeekIdentifier("fold") || PeekIdentifier("fold-first") || PeekIdentifier("structure-preserving"))
            {
                return ParseFold();
            }

            if (PeekIdentifier("branch"))
            {
                return ParseBranch();
            }

            if (PeekIdentifier("count"))
            {
                return ParseCount();
            }

            if (PeekIdentifier("position"))
            {
                return ParsePosition();
            }

            if (PeekIdentifier("span"))
            {
                return ParseSpan();
            }

            if (PeekIdentifier("continue"))
            {
                return ParseContinueTerm();
            }

            if (PeekIdentifier("pow"))
            {
                return ParsePower();
            }

            if (PeekIdentifier("inverse") || PeekIdentifier("unfold"))
            {
                return ParseInverseContinuation();
            }

            if (Current.Kind == TokenKind.LeftBracket)
            {
                return new ElementLiteralTerm(ParseAxisLiteral());
            }

            if (TryParseAxisShorthandLiteral(out var shorthandAxis))
            {
                return new ElementLiteralTerm(shorthandAxis);
            }

            if (TryParseSpecialAxisConstant(out var special))
            {
                return new ElementLiteralTerm(special);
            }

            if (TryParseProportionLiteral(out var proportion))
            {
                return new ElementLiteralTerm(proportion);
            }

            if (TryParseScalarLiteral(out var scalar))
            {
                return new ElementLiteralTerm(scalar);
            }

            if (Current.Kind == TokenKind.Identifier)
            {
                return ParseValueReferenceLike();
            }

            throw Error($"Unexpected token '{Current.Text}'.");
        }

    }
}
