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
using System.Text;

namespace NexusVault.Shared.Util
{
    internal static class Text
    {
        public static string DecodeUTF16(byte[] data)
        {
            return Encoding.Unicode.GetString(data);
        }

        public static string ReadUTF16(BinaryReader reader, int chars)
        {
            return DecodeUTF16(reader.ReadBytes(chars * 2));
        }

        public static string ReadNullTerminatedUTF16(BinaryReader reader)
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                throw new InvalidDataException("No bytes available.");

            var byteBuffer = new byte[2]; // at least 2 byte per character
            var chrBuffer = new char[8];
            var strBuilder = new StringBuilder(128);
            var decoder = Encoding.Unicode.GetDecoder();

            int readBytes;
            int writtenChars;

            do
            {
                readBytes = reader.Read(byteBuffer, 0, byteBuffer.Length);
                if (byteBuffer[0] == 0 && byteBuffer[1] == 0)
                    break;
                writtenChars = decoder.GetChars(byteBuffer, 0, readBytes, chrBuffer, 0, false);
                strBuilder.Append(chrBuffer, 0, writtenChars);
            } while (readBytes != 0);

            writtenChars = decoder.GetChars(byteBuffer, 0, 0, chrBuffer, 0, true);
            strBuilder.Append(chrBuffer, 0, writtenChars);

            return strBuilder.ToString();
        }

        public static string DecodeUTF8(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static string ReadUTF8(BinaryReader reader, int chars)
        {
            return DecodeUTF16(reader.ReadBytes(chars));
        }

        public static string ReadNullTerminatedUTF8(BinaryReader reader)
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                throw new InvalidDataException("No bytes available.");

            var byteBuffer = new byte[1]; // at least 2 byte per character
            var chrBuffer = new char[8];
            var strBuilder = new StringBuilder(128);
            var decoder = Encoding.UTF8.GetDecoder();

            int readBytes;
            int writtenChars;

            do
            {
                readBytes = reader.Read(byteBuffer, 0, byteBuffer.Length);
                if (byteBuffer[0] == 0)
                    break;
                writtenChars = decoder.GetChars(byteBuffer, 0, readBytes, chrBuffer, 0, false);
                strBuilder.Append(chrBuffer, 0, writtenChars);
            } while (readBytes != 0);

            writtenChars = decoder.GetChars(byteBuffer, 0, 0, chrBuffer, 0, true);
            strBuilder.Append(chrBuffer, 0, writtenChars);

            return strBuilder.ToString();
        }

        public static int WriteUTF16(BinaryWriter writer, string value)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            writer.Write(bytes);
            return bytes.Length;
        }

        public static int WriteNullTerminatedUTF16(BinaryWriter writer, string value)
        {
            var written = WriteUTF16(writer, value);
            writer.Write((short)0);
            return written + 2;
        }

        public static int WriteUTF8(BinaryWriter writer, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(bytes);
            return bytes.Length;
        }

        public static int WriteNullTerminatedUTF8(BinaryWriter writer, string value)
        {
            var written = WriteUTF8(writer, value);
            writer.Write((byte)0);
            return written + 1;
        }
    }
}
