namespace Polo.SyntaxAnalysis;

public struct DefinedType
{
    public int SizeUnaligned;
    public int SizeAligned;
    public string Name;
    public List<Type> Containing;

    public DefinedType(string name)
    {
        Name = name;
        Containing = new List<Type>();
    }
}