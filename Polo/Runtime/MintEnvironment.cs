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
    // VM environment memory & runtime info
    private byte* stack;
    private long stackSize = 4096;
    private byte* heap;
    private long heapSize = 2097152;
    private List<Dictionary<string, RuntimeValue>> stackIdentifiers;
    // VM environment virtual registers
    private byte* frameStart;
    private byte* previousFrameStart;
    private byte* stackPointer;
    
    public MintEnvironment()
    {
        stack = (byte*)NativeMemory.Alloc((UIntPtr)stackSize);
        frameStart = stack;
        stackPointer = stack;
        previousFrameStart = null;
        stackIdentifiers = new List<Dictionary<string, RuntimeValue>>();
        heap = (byte*)NativeMemory.Alloc((UIntPtr)heapSize);
    }

    /// <summary>
    /// Pushes a stack frame to the program's virtual stack
    /// </summary>
    public void PushFrame()
    {
        previousFrameStart = frameStart;
        frameStart = stackPointer;
        stackIdentifiers.Add(new Dictionary<string, RuntimeValue>());
    }

    /// <summary>
    /// Pops a stack frame from the program's virtual stack
    /// </summary>
    public void ExitFrame()
    {
        if (stackIdentifiers.Count == 0)
        {
            throw new RuntimeErrorException("No frames to exit.");
        }

        // Remove the current frame's variable dictionary
        stackIdentifiers.RemoveAt(stackIdentifiers.Count - 1);

        // Reset the stack pointer to the start of the current frame
        stackPointer = frameStart;

        // Restore the previous frame start
        frameStart = previousFrameStart;

        // Move the stack pointer back to the previous frame if one exists
        if (frameStart != null)
        {
            stackPointer = frameStart;
        }
    }

    /// <summary>
    /// Pushes a RuntimeValue (often marshalled from C# on the interpreter heap) to the program stack
    /// </summary>
    public void PushStack(RuntimeValue variable, string? identifier = null)
    {
        if (stackSize - ((long)stackPointer - (long)stack) < variable.Size)
        {
            throw new RuntimeErrorException("Could not push heap marshalled RuntimeType to stack. Stack overflow occurred");
        }
        if (identifier is not null)
        {
            stackIdentifiers.Last().Add(identifier, variable);
        }
        
        NativeMemory.Copy(variable.Value, stackPointer, (UIntPtr)variable.Size);
        stackPointer += variable.Size;
    }

    /// <summary>
    /// Will use stack lookup table to try and find the stack offset of the named variable.
    /// Returning it boxed in a runtime type if it can
    /// </summary>
    public RuntimeValue Get(string name)
    {
        for (var i = stackIdentifiers.Count - 1; i >= 0; i--)
        {
            var identPair = stackIdentifiers[i];
            if (identPair.TryGetValue(name, out var info))
            {
                return info;
            }
        }

        throw new RuntimeErrorException($"Undefined variable '{name}'");
    }

    public void Assign(string name, RuntimeValue value)
    {
        var variable = Get(name);
        // Absolutely devious hack / optimisation. Assignment with memcpy is significantly slower than
        // variable assignment, so by casting pointers to known CPU types based on size, we can achieve direct
        // assignment without the overhead of memcpy, improving performance for small, frequently accessed values.
        switch (value.Size)
        {
            case 0:
                break;
            case sizeof(byte): // 1 byte value
                *(byte*)variable.Value = *(byte*)value.Value;
                break;
            case sizeof(short): // 2 byte value
                *(short*)variable.Value = *(short*)value.Value;
                break;
            case sizeof(int): // 3 byte value
                *(int*)variable.Value = *(int*)value.Value;
                break;
            case sizeof(long): // 4 byte value
                *(long*)variable.Value = *(long*)value.Value;
                break;
            default:
                NativeMemory.Copy(value.Value, variable.Value, (UIntPtr)value.Size);
                break;
        }
    }
}
