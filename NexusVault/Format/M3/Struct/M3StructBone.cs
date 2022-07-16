using System.IO;
using System.Runtime.InteropServices;

namespace NexusVault.Format.M3.Struct
{
    [StructLayout(LayoutKind.Sequential, Size = 0x160, Pack = 1)]
    public sealed class M3StructBone
    {
        public uint gap_000;
        public short parentBone;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] gap_006;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] gap_008;
        public uint padding_00C;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public M3DPointer[] offset_010;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public M3DPointer[] animation;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public M3DPointer[] offset_0A0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] matrix_A;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] matrix_B;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] xyz;
        public uint padding_15C;
    }
}
