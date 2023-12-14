using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.VisualBasic.FileIO;
using Polo.Exceptions;
using Polo.Lexer;

namespace Polo.Runtime;

// Mint environment "virtual machine". Will act as a store for variables and datam
internal unsafe class MintEnvironment
{
    private readonly MintEnvironment? enclosing;

    // VM environment memory
    private byte* memory;
    private long memorySize = 1024;

    // For now interpreter holds a map of variable name : memory address
    private readonly Dictionary<string, long> variableMap;
    private List<(long RelativeAddr, int Size)> memoryBlocks;
    private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly;

    public MintEnvironment(MintEnvironment? enclosing = null)
    {
        this.enclosing = enclosing;
        memory = (byte*) NativeMemory.Alloc((UIntPtr) memorySize);
        variableMap = new Dictionary<string, long>();
        memoryBlocks = new List<(long, int)>();
    }

    public long Malloc(int @object)
    {
        // Try to allocate between existing memory blocks, else add new trailing block
        if (memoryBlocks.Count > 1)
        {
            for (var i = 0; i < memoryBlocks.Count - 1; i++)
            {
                if (memoryBlocks[i + 1].Size - memoryBlocks[i].Size > 4)
                {
                    var betweenAddr = memoryBlocks[i].RelativeAddr + 
                    memoryBlocks.Add((0L, 4));
                    return ;
                }
            }
        }
        else
        {
            memoryBlocks.Add((0L, 4));
            return 0;
        }
        
        var lastItem = memoryBlocks.Last();
        var alignedSize = 1;
        while (alignedSize > lastItem.Size)
        {
            alignedSize *= 2;
        }
        var nextAddr = lastItem.RelativeAddr + alignedSize;
        memoryBlocks.Add((nexdtAddr))
    }
    
    // Variables can not have empty names.
    public void Define(Token token, object? value)
    {
        long address = 0;
        if (value is int intValue)
        {
            address = Malloc(intValue);
        }
        
        variableMap.Add(token.Value!.ToString()!, address);
    }

    public void Free(Token token, long addr)
    {
        variableMap.Remove(token.Value?.ToString()!);
    }

    public object? Get(Token name)
    {
        if (name.Value is not null)
        {
            var value = name.Value.ToString();

            if (value is not null)
            {
                if (values.ContainsKey(value))
                    return values[value];

                if (enclosing != null)
                    return enclosing.Get(name);

                foreach (var fieldInfos in Assembly.GetExecutingAssembly().GetTypes().Select(type => type.GetFields(BindingFlags)))
                {
                    foreach (var field in fieldInfos)
                    {
                        if (!field.Name.Equals(value)) continue;
                        return field.GetValue(field);
                    }
                }
                    
                throw new RuntimeErrorException($"Undefined variable '{name}'");
            }
        }

        throw new RuntimeErrorException($"Undefined variable '{name}'");
    }

    public void Assign(Token token, object value)
    {
        if (token.Value is not null)
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
            
            
        }

        throw new RuntimeErrorException($"Undefined variable '{token}'");
    }
}
