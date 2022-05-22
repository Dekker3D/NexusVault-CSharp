using System.IO;
using System.Runtime.InteropServices;

namespace NexusVault.Format.M3.Struct
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class M3DPointer
    {
        public ulong count;
        public ulong offsetKey;
        public ulong offsetValue;
    }
}
