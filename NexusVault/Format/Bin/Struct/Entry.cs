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

    [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 4)]
    public sealed class Entry
    {
        public const int SizeOf = 0x8;

        public uint id;
        public uint characterOffset;

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
