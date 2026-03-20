using Core2.Branching;
using Core2.Elements;
using System.Globalization;

namespace Core2.Symbolics.Expressions;

public static class SymbolicParser
{
    public static SymbolicTerm Parse(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var parser = new Parser(text);
        return parser.Parse();
    }

    private sealed class Parser
    {
        private readonly List<Token> _tokens;
        private int _index;

        public Parser(string text)
        {
            _tokens = Tokenize(text);
        }

        public SymbolicTerm Parse()
        {
            var term = ParseTopLevel();
            Expect(TokenKind.End);
            return term;
        }

        private SymbolicTerm ParseTopLevel()
        {
            var first = ParseStatementOrTerm();
            if (!Match(TokenKind.Semicolon))
            {
                return first;
            }

            if (first is not ProgramTerm firstProgram)
            {
                throw Error("Only program terms can participate in top-level ';' sequences.");
            }

            List<ProgramTerm> steps = [firstProgram];
            do
            {
                var next = ParseStatementOrTerm();
                if (next is not ProgramTerm program)
                {
                    throw Error("Only program terms can participate in top-level ';' sequences.");
                }

                steps.Add(program);
            }
            while (Match(TokenKind.Semicolon));

            return new SequenceTerm(steps);
        }

        private SymbolicTerm ParseStatementOrTerm()
        {
            if (PeekIdentifier("let"))
            {
                return ParseBind();
            }

            return ParseConstraintOrRelationOrValue();
        }

        private ProgramTerm ParseBind()
        {
            ConsumeIdentifier("let");
            string name = ExpectIdentifier();
            Expect(TokenKind.Assign);
            var value = ParseConstraintOrRelationOrValue();
            return new BindTerm(name, value);
        }

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

            if (PeekIdentifier("share"))
            {
                return ParseShare();
            }

            if (PeekIdentifier("route"))
            {
                return ParseRoute();
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
            var relation = ParseRelationInsideFunction();
            Expect(TokenKind.RightParen);
            return new RequirementTerm(relation);
        }

        private PreferenceTerm ParsePrefer()
        {
            ConsumeIdentifier("prefer");
            Expect(TokenKind.LeftParen);
            var relation = ParseRelationInsideFunction();
            Expect(TokenKind.Comma);
            var weight = ParseProportionLiteral();
            Expect(TokenKind.RightParen);
            return new PreferenceTerm(relation, weight);
        }

        private SharedCarrierTerm ParseShare()
        {
            ConsumeIdentifier("share");
            Expect(TokenKind.LeftParen);
            var left = ParseGenericReference();
            Expect(TokenKind.Comma);
            var right = ParseGenericReference();
            Expect(TokenKind.RightParen);
            return new SharedCarrierTerm(left, right);
        }

        private RouteTerm ParseRoute()
        {
            ConsumeIdentifier("route");
            Expect(TokenKind.LeftParen);
            var site = ParseGenericReference();
            Expect(TokenKind.Comma);
            var from = ParseGenericReference();
            Expect(TokenKind.Comma);
            var to = ParseGenericReference();
            Expect(TokenKind.RightParen);
            return new RouteTerm(site, from, to);
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

            var left = ParseValueLike();
            if (!Match(TokenKind.EqualEqual))
            {
                throw Error("Expected a relation inside function arguments.");
            }

            var right = ParseValueLike();
            return new EqualityTerm(left, right);
        }

        private SymbolicTerm ParseValueLike()
        {
            var left = ParseAtomicValueLike();
            if (!Match(TokenKind.Asterisk))
            {
                return left;
            }

            var right = ParseAtomicValueLike();
            if (Match(TokenKind.At))
            {
                var position = ParseProportionLiteral();
                return new PinTerm(ToValueTerm(left), ToValueTerm(right), position);
            }

            return new ApplyTransformTerm(ToValueTerm(left), ToTransformTerm(right));
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

            if (Current.Kind == TokenKind.LeftBracket)
            {
                return new ElementLiteralTerm(ParseAxisLiteral());
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
                string name = Advance().Text;
                return new ReferenceTerm(name, SymbolicTermSort.Value);
            }

            throw Error($"Unexpected token '{Current.Text}'.");
        }

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

        private ReferenceTerm ParseGenericReference()
        {
            string name = ExpectIdentifier();
            return new ReferenceTerm(name, SymbolicTermSort.Value);
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

        private bool PeekIdentifier(string text) =>
            Current.Kind == TokenKind.Identifier &&
            string.Equals(Current.Text, text, StringComparison.Ordinal);

        private void ConsumeIdentifier(string text)
        {
            if (!PeekIdentifier(text))
            {
                throw Error($"Expected identifier '{text}'.");
            }

            Advance();
        }

        private string ExpectIdentifier()
        {
            if (Current.Kind != TokenKind.Identifier)
            {
                throw Error($"Expected identifier, but found '{Current.Text}'.");
            }

            return Advance().Text;
        }

        private bool Match(TokenKind kind)
        {
            if (Current.Kind != kind)
            {
                return false;
            }

            Advance();
            return true;
        }

        private void Expect(TokenKind kind)
        {
            if (!Match(kind))
            {
                throw Error($"Expected {kind}, but found '{Current.Text}'.");
            }
        }

        private Token Advance() => _tokens[_index++];

        private Token Current => _tokens[_index];

        private InvalidOperationException Error(string message) =>
            new($"{message} At token index {_index}.");

        private static List<Token> Tokenize(string text)
        {
            List<Token> tokens = [];
            int index = 0;

            while (index < text.Length)
            {
                char ch = text[index];
                if (char.IsWhiteSpace(ch))
                {
                    index++;
                    continue;
                }

                if (ch == '=' && index + 1 < text.Length && text[index + 1] == '=')
                {
                    tokens.Add(new Token(TokenKind.EqualEqual, "=="));
                    index += 2;
                    continue;
                }

                if (ch == '-' && index + 1 < text.Length && char.IsLetter(text[index + 1]))
                {
                    int start = index++;
                    while (index < text.Length && IsIdentifierChar(text[index]))
                    {
                        index++;
                    }

                    tokens.Add(new Token(TokenKind.Identifier, text[start..index]));
                    continue;
                }

                if ((ch == '+' || ch == '-') && index + 1 < text.Length && char.IsDigit(text[index + 1]))
                {
                    int start = index++;
                    while (index < text.Length && (char.IsDigit(text[index]) || text[index] == '.'))
                    {
                        index++;
                    }

                    tokens.Add(new Token(TokenKind.Number, text[start..index]));
                    continue;
                }

                if (char.IsDigit(ch))
                {
                    int start = index++;
                    while (index < text.Length && (char.IsDigit(text[index]) || text[index] == '.'))
                    {
                        index++;
                    }

                    tokens.Add(new Token(TokenKind.Number, text[start..index]));
                    continue;
                }

                if (char.IsLetter(ch) || ch == '_')
                {
                    int start = index++;
                    while (index < text.Length && IsIdentifierChar(text[index]))
                    {
                        index++;
                    }

                    tokens.Add(new Token(TokenKind.Identifier, text[start..index]));
                    continue;
                }

                TokenKind kind = ch switch
                {
                    '(' => TokenKind.LeftParen,
                    ')' => TokenKind.RightParen,
                    '{' => TokenKind.LeftBrace,
                    '}' => TokenKind.RightBrace,
                    '[' => TokenKind.LeftBracket,
                    ']' => TokenKind.RightBracket,
                    ',' => TokenKind.Comma,
                    ';' => TokenKind.Semicolon,
                    '|' => TokenKind.Pipe,
                    '*' => TokenKind.Asterisk,
                    '@' => TokenKind.At,
                    '=' => TokenKind.Assign,
                    '+' => TokenKind.Plus,
                    '/' => TokenKind.Slash,
                    _ => throw new InvalidOperationException($"Unexpected character '{ch}' in symbolic input."),
                };

                tokens.Add(new Token(kind, ch.ToString()));
                index++;
            }

            tokens.Add(new Token(TokenKind.End, string.Empty));
            return tokens;
        }

        private static bool IsIdentifierChar(char ch) =>
            char.IsLetterOrDigit(ch) || ch is '_' or '.' or '+' or '-';
    }

    private readonly record struct Token(TokenKind Kind, string Text);

    private enum TokenKind
    {
        Identifier,
        Number,
        LeftParen,
        RightParen,
        LeftBrace,
        RightBrace,
        LeftBracket,
        RightBracket,
        Comma,
        Semicolon,
        Pipe,
        Asterisk,
        At,
        EqualEqual,
        Assign,
        Plus,
        Slash,
        End,
    }
}
