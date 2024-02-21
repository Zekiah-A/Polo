using Polo.Lexer;

namespace Polo.SyntaxAnalysis;

internal class MintType
{
    public string Name;
    public int SizeAligned;
    public int SizeUnaligned;
    public MintType? PointerFor;
    public List<MintType> Implements;
    // Type members
    public List<MintProperty> Properties;
    public List<MintFunction> Methods;
    // Special methods
    public Dictionary<TokenType, MintFunction> OperatorOverloads;
    public Dictionary<MintType, MintFunction> CastHandlers;

    public MintType(string name)
    {
        Name = name;
        Implements = new List<MintType>();
        Properties = new List<MintProperty>();
        Methods = new List<MintFunction>();
        OperatorOverloads = new Dictionary<TokenType, MintFunction>();
    }

    public int CalcSize()
    {
        return 0;
    }
}