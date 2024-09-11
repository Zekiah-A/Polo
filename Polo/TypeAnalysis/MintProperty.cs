namespace Polo.TypeAnalysis;

public class MintProperty
{
    public MintType Type { get; set; }
    public PropertyVisibility Visibility { get; set; }
    public string Name { get; set; }

    public MintProperty(string name)
    {
        Name = name;
    }
}