using Core2.Boolean;
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
            while (true)
            {
                var next = ParseStatementOrTerm();
                bool hasMore = Match(TokenKind.Semicolon);
                if (next is ProgramTerm program)
                {
                    steps.Add(program);
                    if (!hasMore)
                    {
                        break;
                    }

                    continue;
                }

                if (hasMore)
                {
                    throw Error("Only the final term in a top-level ';' sequence may be non-programmatic.");
                }

                steps.Add(new EmitTerm(next));
                break;
            }

            return new SequenceTerm(steps);
        }

        private SymbolicTerm ParseStatementOrTerm()
        {
            if (PeekIdentifier("let"))
            {
                return ParseBind();
            }

            if (PeekIdentifier("commit"))
            {
                return ParseCommit();
            }

            if (PeekIdentifier("pin"))
            {
                return ParsePinToPin();
            }

            return ParseConstraintOrRelationOrValue();
        }

        private ProgramTerm ParseBind()
        {
            ConsumeIdentifier("let");
            var target = ParseBindingTarget();
            Expect(TokenKind.Assign);
            var value = ParseConstraintOrRelationOrValue();
            return new BindTerm(target, value);
        }

        private ProgramTerm ParseCommit()
        {
            ConsumeIdentifier("commit");
            var target = ParseBindingTarget();
            Expect(TokenKind.Assign);
            var value = ParseConstraintOrRelationOrValue();
            return new CommitTerm(target, value);
        }

        private PinToPinTerm ParsePinToPin()
        {
            ConsumeIdentifier("pin");
            Expect(TokenKind.LeftParen);
            var hostAnchor = ParseAnchorReference();
            Expect(TokenKind.Comma);
            var appliedAnchor = ParseAnchorReference();
            Expect(TokenKind.RightParen);
            return new PinToPinTerm(hostAnchor, appliedAnchor);
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

        private PowerTerm ParsePower()
        {
            ConsumeIdentifier("pow");
            Expect(TokenKind.LeftParen);
            var @base = ToValueTerm(ParseValueLike());
            Expect(TokenKind.Comma);
            var exponent = ParseProportionLiteral();
            Expect(TokenKind.RightParen);
            return new PowerTerm(@base, exponent);
        }

        private InverseContinueTerm ParseInverseContinuation()
        {
            Advance();
            Expect(TokenKind.LeftParen);
            var source = ToValueTerm(ParseValueLike());
            Expect(TokenKind.Comma);
            var degree = ParseProportionLiteral();
            Expect(TokenKind.RightParen);
            return new InverseContinueTerm(source, degree);
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
            ValueReferenceTerm => true,
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
        private Token Peek(int offset) => _tokens[Math.Min(_index + offset, _tokens.Count - 1)];

        private InvalidOperationException Error(string message) =>
            new($"{message} At token index {_index}.");

        private static bool IsSiteName(string name) =>
            name.Length >= 2 &&
            name[0] == 'P' &&
            name[1..].All(char.IsDigit);

        private static bool TryParseAnchorName(string name, out string ownerName, out string anchorName)
        {
            int separator = name.IndexOf('.');
            if (separator <= 0 || separator >= name.Length - 1)
            {
                ownerName = string.Empty;
                anchorName = string.Empty;
                return false;
            }

            ownerName = name[..separator];
            anchorName = name[(separator + 1)..];
            return true;
        }

        private static bool IsReservedRelationHead(string name) =>
            name is "share" or "route" or "require" or "prefer" or "constraints";

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
                    while (index < text.Length)
                    {
                        char current = text[index];
                        if (char.IsLetterOrDigit(current) || current is '_' or '.')
                        {
                            index++;
                            continue;
                        }

                        if (current == '-' &&
                            ((index + 1 == text.Length &&
                              string.Equals(text[start..index], "host", StringComparison.Ordinal)) ||
                             char.IsLetter(text[index + 1]) ||
                             text[index + 1] == '_' ||
                             (string.Equals(text[start..index], "host", StringComparison.Ordinal) &&
                              !char.IsLetterOrDigit(text[index + 1]))))
                        {
                            index++;
                            continue;
                        }

                        break;
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
                    while (index < text.Length)
                    {
                        char current = text[index];
                        if (char.IsLetterOrDigit(current) || current is '_' or '.')
                        {
                            index++;
                            continue;
                        }

                        if (current == '-' &&
                            ((index + 1 == text.Length &&
                              string.Equals(text[start..index], "host", StringComparison.Ordinal)) ||
                             char.IsLetter(text[index + 1]) ||
                             text[index + 1] == '_' ||
                             (string.Equals(text[start..index], "host", StringComparison.Ordinal) &&
                              !char.IsLetterOrDigit(text[index + 1]))))
                        {
                            index++;
                            continue;
                        }

                        if (current == '+' &&
                            string.Equals(text[start..index], "host", StringComparison.Ordinal) &&
                            (index + 1 == text.Length || !char.IsLetterOrDigit(text[index + 1])))
                        {
                            index++;
                            continue;
                        }

                        break;
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
