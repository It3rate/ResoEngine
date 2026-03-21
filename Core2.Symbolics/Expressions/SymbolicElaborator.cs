namespace Core2.Symbolics.Expressions;

public static class SymbolicElaborator
{
    public static SymbolicElaborationResult Elaborate(SymbolicTerm term, SymbolicEnvironment? environment = null)
    {
        ArgumentNullException.ThrowIfNull(term);

        var current = environment ?? SymbolicEnvironment.Empty;
        return term switch
        {
            ProgramTerm program => SymbolicElaborationProgramFlow.ElaborateProgram(program, current, ElaborateTerm),
            _ => new SymbolicElaborationResult(current, ElaborateTerm(term, current)),
        };
    }

    private static SymbolicTerm ElaborateTerm(SymbolicTerm term, SymbolicEnvironment environment) =>
        SymbolicTermElaboration.ElaborateTerm(term, environment, ElaborateTerm);
}
