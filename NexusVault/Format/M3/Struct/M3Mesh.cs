using System.IO;
using System.Runtime.InteropServices;

namespace NexusVault.Format.M3.Struct
{
    [StructLayout(LayoutKind.Sequential, Size = 0x70, Pack = 1)]
    public sealed class M3Mesh
    {
        public uint indexStart;
        public uint vertexStart;
        public uint indexCount;
        public uint vertexCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] gap_010;
        public byte materialSelector;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public byte[] gap_017;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] gap_040;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] gap_050;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] gap_060;
    }
}
