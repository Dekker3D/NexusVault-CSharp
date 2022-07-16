using System.IO;
using System.Runtime.InteropServices;

namespace NexusVault.Format.M3.Struct
{
    [StructLayout(LayoutKind.Sequential, Size = 0xC8, Pack = 1)]
    public sealed class M3StructGeometry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] gap_008;
        public M3StructArrayPointer offset_008;
        public uint vertexCount;
        public ushort vertexSize;
        public ushort vertexFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public byte[] vertexFieldTypes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public byte[] vertexFieldPos;
        public ushort gap_036;
        public M3StructArrayPointer ptrVertexData;
        public M3StructArrayPointer offset_048;
        public M3StructArrayPointer offset_050;
        public uint indexCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] unk_offset_06C;
        public ushort padding;
        public M3StructArrayPointer indexData;
        public M3StructArrayPointer ptrMeshes;
        public uint vertexCount2;
        public uint gap_093;
        public M3StructArrayPointer meshVertexRange;
        public M3StructArrayPointer offset_0A8;
        public M3StructArrayPointer offset_0B8;
    }
}
