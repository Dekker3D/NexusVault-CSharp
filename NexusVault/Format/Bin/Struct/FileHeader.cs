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

using System.IO;
using System.Runtime.InteropServices;

namespace NexusVault.Format.Bin.Struct
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public sealed class FileHeader
    {
        public const int Signature = 'L' << 24 | 'T' << 16 | 'E' << 8 | 'X';
        public const int SizeOf = 0x60;

        public int signature; // "LTEX"
        public int version;

        /// <summary>
        /// Values are hardcoded
        /// - 1 english
        /// - 2 german
        /// - 3 french
        /// - 4 korean
        /// </summary>
        public long languageType;
        public long tagNameLength;
        public long tagNameOffset;
        public long shortNameLength;
        public long shortNameOffset;
        public long longNameLength;
        public long longNameOffset;
        public long entryCount;
        public long entryOffset;
        public long textLength;
        public long textOffset;

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

}
