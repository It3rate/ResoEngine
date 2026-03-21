namespace Core2.Symbolics.Expressions;

public static partial class SymbolicParser
{
    private sealed partial class Parser
    {
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
    }
}
