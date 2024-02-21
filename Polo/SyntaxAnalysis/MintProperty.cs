namespace Polo.SyntaxAnalysis;

internal class MintProperty
{
    public MintType Type;
    public bool DefPublic;
    public string Name;

    public MintProperty(string name)
    {
        Name = name;
    }
}