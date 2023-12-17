using Polo.Lexer;

namespace Polo.Runtime;

internal static class TypeInformation
{
    public static readonly HashSet<Type> NumericTypes = new HashSet<Type>
    {
        typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
        typeof(int), typeof(uint), typeof(long), typeof(ulong),
        typeof(decimal), typeof(double), typeof(float)
    };

    public static readonly HashSet<string> PrimitiveTypes = new HashSet<string>()
    {
        "u8",
        "i8",
        "u16",
        "i16",
        "u32",
        "i32",
        "u64",
        "i64",
        "u128",
        "i128",
        "f16",
        "f32",
        "f64",
        "f128",
        "__f256",
        "int",
        "uint",
        "float",
        "bool",
        "void",
        "char"
    };
    
    public static int GetSize(TokenType tokenType)
    {
        return tokenType switch
        {
            TokenType.U8 => 1,
            TokenType.I8 => 1,
            TokenType.U16 => 2,
            TokenType.I16 => 2,
            TokenType.U32 => 4,
            TokenType.I32 => 4,
            TokenType.U64 => 8,
            TokenType.I64 => 8,
            TokenType.U128 => 16,
            TokenType.I128 => 16,
            TokenType.F16 => 2,
            TokenType.F32 => 4,
            TokenType.F64 => 8,
            TokenType.F128 => 16,
            TokenType.F256 => 32,
            TokenType.Int => IntPtr.Size, // size_t
            TokenType.UInt => IntPtr.Size, // usize_t
            TokenType.Float => sizeof(float), // 32/64 bit depending on sys
            TokenType.Bool => 1,
            TokenType.Void => 0, // Size of void is typically considered 0
            TokenType.Char => 1, // uint8_t
            TokenType.Function => IntPtr.Size, // Assuming a function pointer size
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, "Invalid TokenType"),
        };
    }

    public static int GetSize(TypeCode typeCode)
    {
        return typeCode switch
        {
            TypeCode.Boolean => sizeof(bool),
            TypeCode.Byte => sizeof(byte),
            TypeCode.SByte => sizeof(sbyte),
            TypeCode.Char => sizeof(char),
            TypeCode.Int16 => sizeof(short),
            TypeCode.UInt16 => sizeof(ushort),
            TypeCode.Int32 => sizeof(int),
            TypeCode.UInt32 => sizeof(uint),
            TypeCode.Int64 => sizeof(long),
            TypeCode.UInt64 => sizeof(ulong),
            TypeCode.Single => sizeof(float),
            TypeCode.Double => sizeof(double),
            TypeCode.Decimal => sizeof(decimal),
            TypeCode.String => IntPtr.Size,
            TypeCode.Object => IntPtr.Size,
            _ => throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, "Invalid TypeCode"),
        };
    }

    public static bool CastImplicit(string typeName, in RuntimeType casted)
    {
        if (casted.TypeName == typeName)
        {
            return true;
        }
        
        switch (typeName)
        {
            case "":
                
                break;
        }

        return false;
    }

    public static string GetEquivalentName(Type managedType)
    {
        if (managedType == typeof(bool))
            return "bool";
        if (managedType == typeof(byte))
            return "u8";
        if (managedType == typeof(sbyte))
            return "i8";
        if (managedType == typeof(char))
            return "char";
        if (managedType == typeof(short))
            return "i16";
        if (managedType == typeof(ushort))
            return "u16";
        if (managedType == typeof(int))
            return "i32";
        if (managedType == typeof(uint))
            return "u32";
        if (managedType == typeof(long))
            return "i64";
        if (managedType == typeof(ulong))
            return "u64";
        if (managedType == typeof(float))
            return "f32";
        if (managedType == typeof(double))
            return "f64";
        if (managedType == typeof(string))
            return "u8*";
        if (managedType == typeof(object))
            return "void*";
        throw new ArgumentOutOfRangeException(nameof(managedType), managedType, "Invalid type");
    }

    public static int GetSize(Type managedType)
    {
        if (managedType == typeof(bool))
            return sizeof(bool);
        if (managedType == typeof(byte))
            return sizeof(byte);
        if (managedType == typeof(sbyte))
            return sizeof(sbyte);
        if (managedType == typeof(char))
            return sizeof(char);
        if (managedType == typeof(short))
            return sizeof(short);
        if (managedType == typeof(ushort))
            return sizeof(ushort);
        if (managedType == typeof(int))
            return sizeof(int);
        if (managedType == typeof(uint))
            return sizeof(uint);
        if (managedType == typeof(long))
            return sizeof(long);
        if (managedType == typeof(ulong))
            return sizeof(ulong);
        if (managedType == typeof(float))
            return sizeof(float);
        if (managedType == typeof(double))
            return sizeof(double);
        if (managedType == typeof(decimal))
            return sizeof(decimal);
        if (managedType == typeof(string) || managedType == typeof(object))
            return IntPtr.Size;
        throw new ArgumentOutOfRangeException(nameof(managedType), managedType, "Invalid type");
    }
}