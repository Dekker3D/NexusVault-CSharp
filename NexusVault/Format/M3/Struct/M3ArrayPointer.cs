using System.IO;
using System.Runtime.InteropServices;

namespace NexusVault.Format.M3.Struct
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class M3ArrayPointer
    {
        public ulong count;
        public ulong offset;
    }
}
