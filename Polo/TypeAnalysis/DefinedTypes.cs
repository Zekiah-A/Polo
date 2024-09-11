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
        
        // Compiler defined primitive types
        AddGlobal(new MintType("u0", 0));
        AddGlobal(new MintType("u8", 1));
        AddGlobal(new MintType("i8", 1));
        AddGlobal(new MintType("u16", 2));
        AddGlobal(new MintType("i16", 2));
        AddGlobal(new MintType("u32", 4));
        AddGlobal(new MintType("i32", 4));
        AddGlobal(new MintType("u64", 8));
        AddGlobal(new MintType("i64", 8));
        AddGlobal(new MintType("u128", 16));
        AddGlobal(new MintType("i128", 16));
        AddGlobal(new MintType("f16", 2));
        AddGlobal(new MintType("f32", 4));
        AddGlobal(new MintType("f64", 8));
    }

    public void AddGlobal(MintType type)
    {
        Types.Add(type.Name, type);
        GlobalTypes.Add(type);
    }
}