using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Polo.Lowering;

public struct ILLiteral
{
    public int Size;
    public object? Value;
    public string TypeName;

    public ILLiteral(int size, string typeName)
    {
        Size = size;
        TypeName = typeName;
    }

    public static ILLiteral? CreateFrom(object? literalValue)
    {
        var type = new ILLiteral();
        if (literalValue is int intValue)
        {
            type.Size = 4;
            type.Value = intValue;
            type.TypeName = "f64";
        }
        else if (literalValue is float floatValue)
        {
            type.Size = 4;
            type.Value = floatValue;
            type.TypeName = "f32";
        }
        else if (literalValue is double doubleValue)
        {
            type.Size = 8;
            type.Value = doubleValue;
            type.TypeName = "f64";
        }
        else if (literalValue is string stringValue)
        {
            type.TypeName = "u8*";
            type.Value = stringValue;
            type.Size = 8;
        }
        else
        {
            return null;
        }

        return type;
    }
}
