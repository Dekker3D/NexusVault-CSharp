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

namespace NexusVault.Shared.Extension
{
    internal static class BinaryReaderAndWriter
    {

        public static long Position(this BinaryReader reader)
        {
            return reader.BaseStream.Position;
        }

        public static long Position(this BinaryWriter writer)
        {
            return writer.BaseStream.Position;
        }

        public static void Position(this BinaryReader reader, long position)
        {
            reader.BaseStream.Position = position;
        }

        public static void Position(this BinaryWriter writer, long position)
        {
            writer.BaseStream.Position = position;
        }

        public static void AlignTo16Byte(this BinaryReader reader)
        {
            reader.BaseStream.Position = NexusVault.Shared.Util.ByteAlignment.AdvanceByToAlign16Byte(reader.BaseStream.Position);
        }

        public static void AlignTo8Byte(this BinaryReader reader)
        {
            reader.BaseStream.Position = NexusVault.Shared.Util.ByteAlignment.AdvanceByToAlign8Byte(reader.BaseStream.Position);
        }

        public static void AlignTo16Byte(this BinaryWriter writer)
        {
            writer.BaseStream.Position = NexusVault.Shared.Util.ByteAlignment.AdvanceByToAlign16Byte(writer.BaseStream.Position);
        }

        public static void AlignTo8Byte(this BinaryWriter writer)
        {
            writer.BaseStream.Position = NexusVault.Shared.Util.ByteAlignment.AdvanceByToAlign8Byte(writer.BaseStream.Position);
        }

        public static string ReadUTF16(this BinaryReader reader, int chars)
        {
            return NexusVault.Shared.Util.Text.ReadUTF16(reader, chars);
        }

        public static string ReadNullTerminatedUTF16(this BinaryReader reader)
        {
            return NexusVault.Shared.Util.Text.ReadNullTerminatedUTF16(reader);
        }

        public static string ReadUTF8(this BinaryReader reader, int chars)
        {
            return NexusVault.Shared.Util.Text.ReadUTF8(reader, chars);
        }

        public static string ReadNullTerminatedUTF8(this BinaryReader reader)
        {
            return NexusVault.Shared.Util.Text.ReadNullTerminatedUTF16(reader);
        }

        public static int WriteUTF16(this BinaryWriter writer, string value)
        {
            return NexusVault.Shared.Util.Text.WriteUTF16(writer, value);
        }

        public static int WriteNullTerminatedUTF16(this BinaryWriter writer, string value)
        {
            return NexusVault.Shared.Util.Text.WriteNullTerminatedUTF16(writer, value);
        }
    }

    
}
