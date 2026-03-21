using Core2.Elements;
using System.Globalization;

namespace Core2.Symbolics.Expressions;

public static partial class SymbolicParser
{
    private sealed partial class Parser
    {
        private Axis ParseAxisLiteral()
        {
            Expect(TokenKind.LeftBracket);
            var recessive = ParseProportionLiteral();
            Expect(TokenKind.RightBracket);

            string marker = ExpectIdentifier();
            if (!string.Equals(marker, "i", StringComparison.Ordinal))
            {
                throw Error("Axis literals must use 'i' between the two proportion slots.");
            }

            Expect(TokenKind.Plus);
            Expect(TokenKind.LeftBracket);
            var dominant = ParseProportionLiteral();
            Expect(TokenKind.RightBracket);

            return new Axis(recessive, dominant);
        }

        private Proportion ParseProportionLiteral()
        {
            if (!TryParseProportionLiteral(out var proportion))
            {
                throw Error("Expected a proportion literal like 2/1.");
            }

            return proportion;
        }

        private bool TryParseProportionLiteral(out Proportion proportion)
        {
            proportion = null!;
            int start = _index;

            if (Current.Kind != TokenKind.Number || !long.TryParse(Current.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numerator))
            {
                return false;
            }

            Advance();
            if (!Match(TokenKind.Slash))
            {
                _index = start;
                return false;
            }

            if (Current.Kind != TokenKind.Number || !long.TryParse(Current.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var denominator))
            {
                _index = start;
                return false;
            }

            Advance();
            proportion = new Proportion(numerator, denominator);
            return true;
        }

        private bool TryParseScalarLiteral(out Scalar scalar)
        {
            scalar = default;
            if (Current.Kind != TokenKind.Number)
            {
                return false;
            }

            if (!decimal.TryParse(Current.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            {
                return false;
            }

            Advance();
            scalar = new Scalar(value);
            return true;
        }

        private bool TryParseSpecialAxisConstant(out Axis axis)
        {
            axis = Axis.Zero;
            if (Current.Kind != TokenKind.Number && Current.Kind != TokenKind.Identifier)
            {
                return false;
            }

            string text = Current.Text;
            axis = text switch
            {
                "1" => Axis.One,
                "i" => Axis.I,
                "-1" => Axis.NegativeOne,
                "-i" => Axis.NegativeI,
                _ => Axis.Zero,
            };

            if (text is not ("1" or "i" or "-1" or "-i"))
            {
                return false;
            }

            Advance();
            return true;
        }

        private bool TryParseAxisShorthandLiteral(out Axis axis)
        {
            int start = _index;
            axis = Axis.Zero;

            if (!TryParseCoordinateProportion(out var recessive))
            {
                return false;
            }

            if (!PeekIdentifier("i"))
            {
                _index = start;
                return false;
            }

            Advance();
            Match(TokenKind.Plus);

            if (!TryParseCoordinateProportion(out var dominant))
            {
                _index = start;
                return false;
            }

            axis = new Axis(recessive, dominant);
            return true;
        }

        private bool TryParseCoordinateProportion(out Proportion proportion)
        {
            int start = _index;
            if (TryParseProportionLiteral(out proportion))
            {
                return true;
            }

            if (TryParseScalarLiteral(out var scalar))
            {
                try
                {
                    proportion = scalar.AsProportion();
                    return true;
                }
                catch (InvalidOperationException)
                {
                }
                catch (OverflowException)
                {
                }
            }

            _index = start;
            proportion = null!;
            return false;
        }
    }
}
