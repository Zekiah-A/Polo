using Polo.Lexer;

namespace Polo.TypeAnalysis;

public class MintType
{
    public string Name { get; set; }
    public MintType? Typedefs { get; set; }
    public MintType? PointerFor { get; set; }
    public List<MintType> Implements { get; set; }
    // Type members
    public List<MintProperty> Properties { get; set; }
    public List<MintFunction> Methods { get; set; }
    // Special methods
    public Dictionary<TokenType, MintFunction> OperatorOverloads { get; set; }
    public Dictionary<MintType, MintFunction> CastHandlers { get; set; }
    // Used for primitives, such as u32, void, etc
    private readonly int constSize;

    public MintType(string name)
    {
        Name = name;
        Implements = new List<MintType>();
        Properties = new List<MintProperty>();
        Methods = new List<MintFunction>();
        OperatorOverloads = new Dictionary<TokenType, MintFunction>();
        CastHandlers = new Dictionary<MintType, MintFunction>();
        constSize = -1;
    }

    public MintType(string name, int constSize, MintType? typedefs = null, MintType? pointerFor = null) : this(name)
    {
        this.constSize = constSize;
        Typedefs = typedefs;
        PointerFor = pointerFor;
    }

    public int GetSizeAligned()
    {
        throw new NotImplementedException();
    }
    
    public int GetSizeUnaligned()
    {
        if (constSize != -1)
        {
            return constSize;
        }
        
        throw new NotImplementedException();
    }
}
