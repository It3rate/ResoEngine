namespace Core2.Symbolics.Expressions;

public sealed record SymbolicElaborationResult(SymbolicEnvironment Environment, SymbolicTerm? Output);
