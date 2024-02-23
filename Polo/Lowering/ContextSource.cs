namespace Polo.Lowering;

/// <summary>
/// Context is anything, such as previus lines that need to be retroactively appended to the CURRENT expression,
/// such as preceeding creation statements. "Source" MUST be an expression, that is, either an instruction or
/// identifier (Variable, Label, DataLocation, etc)
/// </summary>
public class ContextSource
{
    public string Source;
    public SourceType SourceType;
    public string? Context;

    public ContextSource(string source, SourceType sourceType, string? context = null)
    {
        Source = source;
        SourceType = sourceType;
        Context = context;
    }
}

public enum SourceType
{
    Variable, // Variable identifier, prefixed with %, treated as an asm identifier
    Label, //Label identifier, prefixed with @, treated as an asm expression
    DataLocation, // DataLlcation identifier, prefixed with $, treated as an asm literal for use in instructions (expression)
    Instruction, // Instruction, be it add, ceq, etc, treated as an asm expression
}