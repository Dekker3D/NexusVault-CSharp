/*******************************************************************************
 * Copyright (C) 2018-2022 MarbleBag
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>
 *
 * SPDX-License-Identifier: AGPL-3.0-or-later
 *******************************************************************************/

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NexusVault.Format.Bin.Struct
{

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class FileHeader
    {
        public const int Signature = 'L' << 24 | 'T' << 16 | 'E' << 8 | 'X';
        public const int SizeOf = 0x60;

        public Int32 signature; // "LTEX"
        public Int32 version;

        /// <summary>
        /// Values are hardcoded
        /// - 1 english
        /// - 2 german
        /// - 3 french
        /// - 4 korean
        /// </summary>
        public Int64 languageType;
        public Int64 tagNameLength;
        public Int64 tagNameOffset;
        public Int64 shortNameLength;
        public Int64 shortNameOffset;
        public Int64 longNameLength;
        public Int64 longNameOffset;
        public Int64 entryCount;
        public Int64 entryOffset;
        public Int64 textLength;
        public Int64 textOffset;

        public FileHeader() { }

        public FileHeader(BinaryReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            signature = reader.ReadInt32();
            version = reader.ReadInt32();
            languageType = reader.ReadInt64();
            tagNameLength = reader.ReadInt64();
            tagNameOffset = reader.ReadInt64();
            shortNameLength = reader.ReadInt64();
            shortNameOffset = reader.ReadInt64();
            longNameLength = reader.ReadInt64();
            longNameOffset = reader.ReadInt64();
            entryCount = reader.ReadInt64();
            entryOffset = reader.ReadInt64();
            textLength = reader.ReadInt64();
            textOffset = reader.ReadInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(signature);
            writer.Write(version);
            writer.Write(languageType);
            writer.Write(tagNameLength);
            writer.Write(tagNameOffset);
            writer.Write(shortNameLength);
            writer.Write(shortNameOffset);
            writer.Write(longNameLength);
            writer.Write(longNameOffset);
            writer.Write(entryCount);
            writer.Write(entryOffset);
            writer.Write(textLength);
            writer.Write(textOffset);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 4)]
    public class Entry
    {
        public const int SizeOf = 0x8;

        public UInt32 id;
        public UInt32 characterOffset;

        public Entry() { }

        public Entry(BinaryReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            id = reader.ReadUInt32();
            characterOffset = reader.ReadUInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(characterOffset);
        }
    }

}
