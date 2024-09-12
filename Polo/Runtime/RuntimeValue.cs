using System.Runtime.InteropServices;

namespace Polo.Runtime;

public unsafe class RuntimeValue
{
    public int Size;
    public void* Value;
    public string TypeName;
    
    private RuntimeValue(int size, string typeName)
    {
        Size = size;
        TypeName = typeName;
    }
    
    public static RuntimeValue CreateFrom(object? managed, string typeName)
    {
        RuntimeValue runtimeValue;

        switch (typeName)
        {
            case "u0":
            {
                throw new NotImplementedException();
            }
            case "u8":
            {
                if (managed is byte u8Value)
                {
                    runtimeValue = new RuntimeValue(1, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(byte))
                    };
                    *(byte*)runtimeValue.Value = u8Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for u8");
                }
                break;
            }
            case "i8":
            {
                if (managed is sbyte i8Value)
                {
                    runtimeValue = new RuntimeValue(1, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(sbyte))
                    };
                    *(sbyte*)runtimeValue.Value = i8Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for i8");
                }
                break;
            }
            case "u16":
            {
                if (managed is ushort u16Value)
                {
                    runtimeValue = new RuntimeValue(2, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(ushort))
                    };
                    *(ushort*)runtimeValue.Value = u16Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for u16");
                }
                break;
            }
            case "i16":
            {
                if (managed is short i16Value)
                {
                    runtimeValue = new RuntimeValue(2, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(short))
                    };
                    *(short*)runtimeValue.Value = i16Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for i16");
                }
                break;
            }
            case "u32":
            {
                if (managed is uint u32Value)
                {
                    runtimeValue = new RuntimeValue(4, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(uint))
                    };
                    *(uint*)runtimeValue.Value = u32Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for u32");
                }
                break;
            }
            case "i32":
            {
                if (managed is int i32Value)
                {
                    runtimeValue = new RuntimeValue(4, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(int))
                    };
                    *(int*)runtimeValue.Value = i32Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for i32");
                }
                break;
            }
            case "u64":
            {
                if (managed is ulong u64Value)
                {
                    runtimeValue = new RuntimeValue(8, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(ulong))
                    };
                    *(ulong*)runtimeValue.Value = u64Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for u64");
                }
                break;
            }
            case "i64":
            {
                if (managed is long i64Value)
                {
                    runtimeValue = new RuntimeValue(8, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(long))
                    };
                    *(long*)runtimeValue.Value = i64Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for i64");
                }
                break;
            }
            case "f32":
            {
                if (managed is float f32Value)
                {
                    runtimeValue = new RuntimeValue(4, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(float))
                    };
                    *(float*)runtimeValue.Value = f32Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for f32");
                }
                break;
            }
            case "f64":
            {
                if (managed is double f64Value)
                {
                    runtimeValue = new RuntimeValue(8, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(double))
                    };
                    *(double*)runtimeValue.Value = f64Value;
                }
                else
                {
                    throw new ArgumentException("Invalid type for f64");
                }
                break;
            }
            case "float":
            {
                if (managed is float floatValue)
                {
                    runtimeValue = new RuntimeValue(4, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(float))
                    };
                    *(float*)runtimeValue.Value = floatValue;
                }
                else
                {
                    throw new ArgumentException("Invalid type for float");
                }
                break;
            }
            case "int":
            {
                if (managed is int intValue)
                {
                    runtimeValue = new RuntimeValue(4, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(int))
                    };
                    *(int*)runtimeValue.Value = intValue;
                }
                else
                {
                    throw new ArgumentException("Invalid type for int");
                }
                break;
            }
            case "uint":
            {
                if (managed is uint uintValue)
                {
                    runtimeValue = new RuntimeValue(4, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(uint))
                    };
                    *(uint*)runtimeValue.Value = uintValue;
                }
                else
                {
                    throw new ArgumentException("Invalid type for uint");
                }
                break;
            }
            case "void":
            {
                throw new NotImplementedException();
            }
            case "bool":
            {
                if (managed is bool boolValue)
                {
                    runtimeValue = new RuntimeValue(1, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(bool))
                    };
                    *(bool*)runtimeValue.Value = boolValue;
                }
                else
                {
                    throw new ArgumentException("Invalid type for char");
                }
                break;
            }
            case "char":
            {
                if (managed is char charValue)
                {
                    runtimeValue = new RuntimeValue(2, typeName)
                    {
                        Value = NativeMemory.Alloc(sizeof(char))
                    };
                    *(char*)runtimeValue.Value = charValue;
                }
                else
                {
                    throw new ArgumentException("Invalid type for char");
                }
                break;
            }
            default:
                throw new ArgumentException("Unsupported type name");
        }

        return runtimeValue;
    }
}