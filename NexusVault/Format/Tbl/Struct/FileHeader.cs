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

namespace NexusVault.Format.Tbl.Struct
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class FileHeader
    {
        public int signature; // "DTBL"
        public int version;

        public long nameLength; // number of UTF-16 encoded characters, aligned to 16 byte and null terminated
        public long unk_010;
        public long recordSize;   // size of a single record in bytes
        public long fieldCount;
        public long fieldOffset;
        public long recordCount;
        public long totalRecordsSize; // Size of all records
        public long recordsOffset;
        public long lookupCount;
        public long lookupOffset; // id to index lookup
        private long padding_058;

        public FileHeader() { }

        public FileHeader(BinaryReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            signature = reader.ReadInt32();
            version = reader.ReadInt32();
            nameLength = reader.ReadInt64();
            unk_010 = reader.ReadInt64();
            recordSize = reader.ReadInt64();
            fieldCount = reader.ReadInt64();
            fieldOffset = reader.ReadInt64();
            recordCount = reader.ReadInt64();
            totalRecordsSize = reader.ReadInt64();
            recordsOffset = reader.ReadInt64();
            lookupCount = reader.ReadInt64();
            lookupOffset = reader.ReadInt64();
            padding_058 = reader.ReadInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(signature);
            writer.Write(version);
            writer.Write(nameLength);
            writer.Write(unk_010);
            writer.Write(recordSize);
            writer.Write(fieldCount);
            writer.Write(fieldOffset);
            writer.Write(recordCount);
            writer.Write(totalRecordsSize);
            writer.Write(recordsOffset);
            writer.Write(lookupCount);
            writer.Write(lookupOffset);
            writer.Write(padding_058);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class Column
    {
        public int nameLength;  // number of UTF-16 encoded characters, aligned to 16 byte and null terminated
        public int unk_04;
        public long nameOffset;
        public int dataType;
        public int unk_14; // seems to be 24 for dataType int and float and 104 for string ?

        public Column() { }

        public Column(BinaryReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            nameLength = reader.ReadInt32();
            unk_04 = reader.ReadInt32();
            nameOffset = reader.ReadInt64();
            dataType = reader.ReadInt32();
            unk_14 = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(nameLength);
            writer.Write(unk_04);
            writer.Write(nameOffset);
            writer.Write(dataType);
            writer.Write(unk_14);
        }
    }
}
