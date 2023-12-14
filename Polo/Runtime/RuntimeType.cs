using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Polo.Runtime;

public unsafe struct RuntimeType
{
    public int Size;
    public void* Value;
    
    public RuntimeType(int size)
    {
        Size = size;
    }

    public static RuntimeType CreateFrom(object? managed)
    {
        var rtType = new RuntimeType(0);

        if (managed == null)
        {
            rtType.Value = (void*)0;
            return rtType;
        }

        var handle = GCHandle.Alloc(managed, GCHandleType.Pinned);
        try
        {
            var size = TypeInformation.GetSize(managed.GetType());
            rtType.Value = NativeMemory.Alloc((UIntPtr) size);
            var address = handle.AddrOfPinnedObject();
            NativeMemory.Copy((void*) address, rtType.Value, (UIntPtr)size);
        }
        finally
        {
            handle.Free();
        }

        return rtType;
    }
}