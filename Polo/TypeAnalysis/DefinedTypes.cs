namespace Polo.TypeAnalysis;

public class DefinedTypes
{
    // AST - All objects in the top level of these collections are global (project) scope
    public List<MintType> GlobalTypes { get; set; }
    public Dictionary<string, MintType> Types { get; set; }

    public DefinedTypes()
    {
        GlobalTypes = new List<MintType>();
        Types = new Dictionary<string, MintType>();
    }

    public void AddGlobal(MintType type)
    {
        Types.Add(type.Name, type);
        GlobalTypes.Add(type);
    }
}