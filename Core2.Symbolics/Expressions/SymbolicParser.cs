namespace Core2.Symbolics.Expressions;

public static partial class SymbolicParser
{
    public static SymbolicTerm Parse(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var parser = new Parser(text);
        return parser.Parse();
    }

    private sealed partial class Parser
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
            name is "share" or "route" or "junction" or "has" or "require" or "prefer" or "constraints";

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
