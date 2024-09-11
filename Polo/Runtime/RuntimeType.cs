using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Polo.Runtime;

public unsafe struct RuntimeType
{
    public int Size;
    public void* Value;
    public string TypeName;
    
    private RuntimeType(int size)
    {
        Size = size;
    }

    public static RuntimeType CreateFrom(object? managed)
    {
        var rtType = new RuntimeType(0);

        /*if (managed == null)
        {
            rtType.Value = (void*)0;
            rtType.TypeName = "null";
            return rtType;
        }
        var managedType = managed.GetType();

        var handle = GCHandle.Alloc(managed, GCHandleType.Pinned);
        try
        {
            var size = TypeInformation.GetSize(managedType);
            rtType.Value = NativeMemory.Alloc((UIntPtr) size);
            rtType.Size = size;
            rtType.TypeName = TypeInformation.GetEquivalentName(managedType);
            
            var address = handle.AddrOfPinnedObject();
            NativeMemory.Copy((void*) address, rtType.Value, (UIntPtr)size);
        }
        finally
        {
            handle.Free();
        }*/

        return rtType;
    }
}