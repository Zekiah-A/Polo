using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Polo.Exceptions;
using Polo.Lexer;

namespace Polo.Runtime;

/// <summary>
/// Mint environment "virtual machine". An environment is spawned for each thread of the process, responsible for
/// containing memory management methods for that thread, as well as the stack for the given thread. The heap is
/// shared across all threads of the mint 'process' hence just using the interpreter's shared heap
/// </summary>
internal unsafe class MintEnvironment
{
    // VM environment memory
    private byte* stack;
    private long stackSize = 4096;
    private byte* frameStart;
    private byte* stackPointer;
    private List<StackIdentifier> stackIdentifiers;
    
    public MintEnvironment()
    {
        stack = (byte*) NativeMemory.Alloc((UIntPtr) stackSize);
        frameStart = stack;
        stackPointer = stack;
        stackIdentifiers = new List<StackIdentifier>();
    }
    
    /// <summary>
    /// Pushes a RuntimeType that was marshalled from a C# type from the heap to the stack program stack
    /// when it is needed, for example, as a literal
    /// </summary>
    public void PushStack(RuntimeType variable, string? identifier = null)
    {
        if (stackSize - ((long)stackPointer - (long)stack) < variable.Size)
        {
            throw new RuntimeErrorException("Could not push heap marshalled RuntimeType to stack. Stack overflow occurred");
        }
        if (identifier is not null)
        {
            stackIdentifiers.Add(new StackIdentifier(identifier, variable));
        }
        
        NativeMemory.Copy(variable.Value, stackPointer, (UIntPtr)variable.Size);
        stackPointer += variable.Size;
    }

    public void PopStack(long size)
    {
        var newSp = stackPointer - size;
        if (newSp < frameStart)
        {
            throw new RuntimeErrorException("Impossible operation. Trying to pop beyond stack frame");
        }
        for (var i = stackIdentifiers.Count - 1; i >= 0; i--)
        {
            var identPair = stackIdentifiers[i];
            if ((long) identPair.TypeInfo.Value > (long) newSp)
            {
                stackIdentifiers.RemoveAt(i);
            }
            else
            {
                break;
            }
        }

        stackPointer = newSp;
    }

    public void ExitFrame()
    {
        stackPointer = frameStart;
    }
    
    public void Malloc(long length)
    {
        NativeMemory.Alloc((UIntPtr) length);
    }

    public void Free(long addr)
    {
        NativeMemory.Free((void*) addr);
    }

    /// <summary>
    /// Will use stack lookup table to try and find the stack offset of the named variable.
    /// Returning it boxed in a runtime type if it can
    /// </summary>
    public RuntimeType Get(string name)
    {
        for (var i = stackIdentifiers.Count - 1; i >= 0; i--)
        {
            var identPair = stackIdentifiers[i];
            if (identPair.Identifier == name)
            {
                return stackIdentifiers[i].TypeInfo;
            }
        }

        throw new RuntimeErrorException($"Undefined variable '{name}'");
    }

    public void Assign(string name, object value)
    {
        /*if (token.Value is not null)
        {
            var name = token.Value.ToString();

            if (name is not null)
            {
                if (values.ContainsKey(name))
                {
                    values[name] = value;
                    return;
                }

                if (enclosing != null)
                {
                    enclosing.Assign(token, value);
                    return;
                }
                
                foreach (var fieldInfos in Assembly.GetExecutingAssembly().GetTypes().Select(type => type.GetFields(BindingFlags)))
                {
                    foreach (var field in fieldInfos)
                    {
                        try
                        {
                            if (!field.Name.Equals(name)) continue;
                            var convertedValue = Convert.ChangeType(value, field.GetValue(field)?.GetType() ?? throw new InvalidOperationException());
                            field.SetValue(field, convertedValue);
                            return;
                        }
                        catch (Exception e)
                        {
                            throw new RuntimeErrorException($"Cast error, could could not cast value to type of '{name}'.\n{e}'");
                        }
                    }
                }
            }
        }*/

        throw new RuntimeErrorException($"Undefined variable '{name}'");
    }
}
