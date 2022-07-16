using NexusVault.Format.M3.Struct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NexusVault.Format.M3
{
    public class M3Model
    {
        public const uint M3signature = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'L';
        public const uint M3version = 100;

        M3StructHeader header;
        List<M3StructBone> bones;

        public M3Model(byte[] data)
        {
            read(new BinaryReader(new MemoryStream(data)));
        }

        public M3Model(Stream stream)
        {
            read(new BinaryReader(stream));
        }

        public M3Model(BinaryReader reader)
        {
            read(reader);
        }

        protected void read(BinaryReader reader)
        {
            readHeader(reader);
        }

        protected void readHeader(BinaryReader reader)
        {
            header = new M3StructHeader();
            header.signature = reader.ReadUInt32();
            if (header.signature != M3signature)
            {
                throw new InvalidDataException("M3 model signature does not match!");
            }
            header.version = reader.ReadUInt32();
            if (header.version != M3version)
            {
                throw new InvalidDataException("M3 model version does not match!");
            }

            reader.ReadUInt64(); // ignore gap

            header.unk_offset_010 = readArrayPointer(reader);
            header.unk_offset_020 = readDPointer(reader);
            header.unk_offset_038 = readDPointer(reader);
            header.unk_offset_050 = readDPointer(reader);
            header.unk_offset_068 = readDPointer(reader);
            header.unk_offset_080 = readArrayPointer(reader);
            header.unk_offset_090 = readDPointer(reader);
            header.unk_offset_0AB = readDPointer(reader);
            header.unk_offset_0C0 = readDPointer(reader);
            header.unk_offset_0D8 = readDPointer(reader);
            header.unk_offset_0F0 = readArrayPointer(reader);
            header.unk_offset_100 = readDPointer(reader);
            header.unk_offset_118 = readDPointer(reader);
            header.unk_offset_130 = readDPointer(reader);
            header.unk_offset_148 = readDPointer(reader);
            header.unk_offset_160 = readDPointer(reader);

            reader.ReadUInt64(); // ignore gap

            header.ptrBones = readArrayPointer(reader);
            header.unk_offset_190 = readArrayPointer(reader);
            header.unk_offset_1A0 = readArrayPointer(reader);
            header.ptrBoneMap = readArrayPointer(reader);
            header.ptrTextures = readArrayPointer(reader);
            header.unk_offset_1D0 = readArrayPointer(reader);
            header.unk_offset_1E0 = readArrayPointer(reader);
            header.ptrMaterials = readArrayPointer(reader);
            header.unk_offset_200 = readArrayPointer(reader);
            header.unk_offset_210 = readArrayPointer(reader);
            header.unk_offset_220 = readArrayPointer(reader);
            header.unk_offset_230 = readArrayPointer(reader);
            header.unk_offset_240 = readArrayPointer(reader);
            header.ptrGeometry = readArrayPointer(reader);
            header.unk_offset_260 = readArrayPointer(reader);
            header.unk_offset_270 = readArrayPointer(reader);
            header.unk_offset_280 = readArrayPointer(reader);
            header.unk_offset_290 = readDPointer(reader);
            header.unk_offset_2A8 = readArrayPointer(reader);
            header.unk_offset_2B8 = readArrayPointer(reader);
            header.unk_offset_2C8 = readArrayPointer(reader);
            header.unk_offset_2D8 = readArrayPointer(reader);
            header.unk_offset_2E8 = readArrayPointer(reader);
            header.unk_offset_2F8 = readArrayPointer(reader);
            header.unk_offset_308 = readArrayPointer(reader);
            header.unk_offset_318 = readArrayPointer(reader);
            header.unk_offset_328 = readArrayPointer(reader);
            header.unk_offset_338 = readArrayPointer(reader);

            reader.ReadUInt64(); // ignore gap

            header.unk_offset_350 = readDPointer(reader);
            reader.ReadUInt64();
            header.unk_offset_370 = readDPointer(reader);

            for(int i = 0; i < 33; ++i)
            {
                reader.ReadUInt64(); // 264 byte gap
            }

            header.unk_offset_490 = readArrayPointer(reader);
            header.unk_offset_4A0 = readArrayPointer(reader);

            for(int i = 0; i < 12; ++i)
            {
                reader.ReadUInt64(); // 96 byte gap
            }

            header.unk_offset_510 = readArrayPointer(reader);
            header.unk_offset_520 = readArrayPointer(reader);
            header.unk_offset_530 = readArrayPointer(reader);
            header.unk_offset_540 = readArrayPointer(reader);
            header.unk_offset_550 = readArrayPointer(reader);
            header.unk_offset_560 = readArrayPointer(reader);
            header.unk_offset_570 = readArrayPointer(reader);

            reader.ReadUInt64();

            header.unk_offset_588 = readArrayPointer(reader);
            header.unk_offset_598 = readArrayPointer(reader);
            header.unk_offset_5A8 = readArrayPointer(reader);

            reader.ReadUInt64();

            header.unk_offset_5C0 = readDPointer(reader);

            for(int i = 0; i < 11; ++i)
            {
                reader.ReadUInt64(); // 88 byte gap
            }

            return header;
        }

        protected void readBones(BinaryReader reader)
        {
            bones = new List<M3StructBone>();
            for (ulong i = 0; i < header.ptrBones.count; ++i) {
                M3StructBone bone = new M3StructBone();

                reader.ReadUInt32(); // gap
                bone.parentBone = reader.ReadInt16();
                reader.ReadUInt16();
                reader.ReadUInt32();
                reader.ReadUInt32(); // actual padding?
                bone.offset_010 = readDPointerList(reader, 4);
                bone.animation = readDPointerList(reader, 2);
                bone.offset_0A0 = readDPointerList(reader, 2);

                bone.matrix_A = new float[16];
                for (int j = 0; j < 16; ++j)
                {
                    bone.matrix_A[j] = reader.ReadSingle();
                }
                bone.matrix_B = new float[16];
                for (int j = 0; j < 16; ++j)
                {
                    bone.matrix_B[j] = reader.ReadSingle();
                }
                bone.matrix_B = new float[3];
                for (int j = 0; j < 3; ++j)
                {
                    bone.xyz[j] = reader.ReadSingle();
                }


                bones.Add(bone);
            }
        }
        
        protected M3StructArrayPointer readArrayPointer(BinaryReader reader)
        {
            M3StructArrayPointer pointer = new M3StructArrayPointer();
            pointer.count = reader.ReadUInt64();
            pointer.offset = reader.ReadUInt64();
            return pointer;
        }

        protected M3DPointer readDPointer(BinaryReader reader)
        {
            M3DPointer pointer = new M3DPointer();
            pointer.count = reader.ReadUInt64();
            pointer.offsetKey = reader.ReadUInt64();
            pointer.offsetValue = reader.ReadUInt64();
            return pointer;
        }

        protected M3DPointer[] readDPointerList(BinaryReader reader, uint amount)
        {
            M3DPointer[] pointers = new M3DPointer[amount];
            for(int i = 0; i < amount; ++i)
            {
                pointers[i] = readDPointer(reader);
            }
            return pointers;
        }
    }
}
