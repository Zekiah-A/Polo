namespace Polo.TypeAnalysis;

public class MintArgument
{
    private string Name { get; set; }
    private MintType Type { get; set; }
    
    public MintArgument(string name, MintType type)
    {
        Name = name;
        Type = type;
    }
}