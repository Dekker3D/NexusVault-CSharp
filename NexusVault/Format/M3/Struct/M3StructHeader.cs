using System.IO;
using System.Runtime.InteropServices;

namespace NexusVault.Format.M3.Struct
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class M3StructHeader
    {
        public uint signature;
        public uint version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] gap_008;
        public M3StructArrayPointer unk_offset_010;
        public M3DPointer unk_offset_020;
        public M3DPointer unk_offset_038;
        public M3DPointer unk_offset_050;
        public M3DPointer unk_offset_068;
        public M3StructArrayPointer unk_offset_080;
        public M3DPointer unk_offset_090;
        public M3DPointer unk_offset_0AB;
        public M3DPointer unk_offset_0C0;
        public M3DPointer unk_offset_0D8;
        public M3StructArrayPointer unk_offset_0F0;
        public M3DPointer unk_offset_100;
        public M3DPointer unk_offset_118;
        public M3DPointer unk_offset_130;
        public M3DPointer unk_offset_148;
        public M3DPointer unk_offset_160;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] gap_178;
        public M3StructArrayPointer ptrBones;
        public M3StructArrayPointer unk_offset_190;
        public M3StructArrayPointer unk_offset_1A0;
        public M3StructArrayPointer ptrBoneMap;
        public M3StructArrayPointer ptrTextures;
        public M3StructArrayPointer unk_offset_1D0; // 2
        public M3StructArrayPointer unk_offset_1E0; // 152
        public M3StructArrayPointer ptrMaterials;
        public M3StructArrayPointer unk_offset_200; // 4
        public M3StructArrayPointer unk_offset_210; // 2
        public M3StructArrayPointer unk_offset_220; // 70
        public M3StructArrayPointer unk_offset_230; // 4
        public M3StructArrayPointer unk_offset_240; // 112
        public M3StructArrayPointer ptrGeometry;
        public M3StructArrayPointer unk_offset_260; // 4
        public M3StructArrayPointer unk_offset_270; // 2
        public M3StructArrayPointer unk_offset_280; // 8
        public M3DPointer unk_offset_290; // 4, 2
        public M3StructArrayPointer unk_offset_2A8; // 16
        public M3StructArrayPointer unk_offset_2B8; // 40
        public M3StructArrayPointer unk_offset_2C8; // 8
        public M3StructArrayPointer unk_offset_2D8; // 2
        public M3StructArrayPointer unk_offset_2E8; // 8
        public M3StructArrayPointer unk_offset_2F8; // 160
        public M3StructArrayPointer unk_offset_308; // 80
        public M3StructArrayPointer unk_offset_318; // 400
        public M3StructArrayPointer unk_offset_328; // 56
        public M3StructArrayPointer unk_offset_338; // 2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] gap_348;
        public M3DPointer unk_offset_350; // 4, 4
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] gap_368;
        public M3DPointer unk_offset_370; // 4, 4
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 264)]
        public byte[] gap_388;
        public M3StructArrayPointer unk_offset_490; // ?
        public M3StructArrayPointer unk_offset_4A0; // 4
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
        public byte[] gap_4B0;
        public M3StructArrayPointer unk_offset_510; // 16
        public M3StructArrayPointer unk_offset_520; // 4
        public M3StructArrayPointer unk_offset_530; // 4
        public M3StructArrayPointer unk_offset_540; // 104
        public M3StructArrayPointer unk_offset_550; // 2
        public M3StructArrayPointer unk_offset_560; // 160
        public M3StructArrayPointer unk_offset_570; // 32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] gap_580;
        public M3StructArrayPointer unk_offset_588; // 32
        public M3StructArrayPointer unk_offset_598; // 76
        public M3StructArrayPointer unk_offset_5A8; // 2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] gap_5B8;
        public M3DPointer unk_offset_5C0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 88)]
        public byte[] gap_5D8;
    }
}
