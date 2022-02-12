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

using NexusVault.Shared.Util;
using System.Collections.Generic;
using System.IO;
using NexusVault.Format.Bin.Struct;
using NexusVault.Shared.Extension;

namespace NexusVault.Format.Bin
{
    public static class LanguageWriter
    {
        public static byte[] ToBinary(LanguageDictionary dictionary)
        {
            var stream = new MemoryStream();
            Write(dictionary, new BinaryWriter(stream));
            return stream.ToArray();
        }

        public static void Write(LanguageDictionary dictionary, BinaryWriter writer)
        {
            writer.BaseStream.Position = FileHeader.SizeOf;
            var postHeaderPosition = writer.BaseStream.Position;

            var header = new FileHeader
            {
                signature = FileHeader.Signature,
                version = 4,
                languageType = dictionary.Locale.Type
            };

            header.tagNameOffset = writer.BaseStream.Position - postHeaderPosition;
            header.tagNameLength = dictionary.Locale.TagName.Length + 1;
            writer.WriteNullTerminatedUTF16(dictionary.Locale.TagName);
            writer.AlignTo16Byte();

            header.shortNameOffset = writer.BaseStream.Position - postHeaderPosition;
            header.shortNameLength = dictionary.Locale.ShortName.Length + 1;
            writer.WriteNullTerminatedUTF16(dictionary.Locale.ShortName);            
            writer.AlignTo16Byte();

            header.longNameOffset = writer.BaseStream.Position - postHeaderPosition;
            header.longNameLength = dictionary.Locale.LongName.Length + 1;
            writer.WriteNullTerminatedUTF16(dictionary.Locale.LongName);           
            writer.AlignTo16Byte();

            header.entryOffset = writer.BaseStream.Position - postHeaderPosition;
            header.entryCount = dictionary.Entries.Count;
            header.textOffset = ByteAlignment.AlignTo16Byte(header.entryOffset + header.entryCount * Entry.SizeOf);

            var cache = new Dictionary<string, int>();
            int characterOffset = 0;
            foreach(var entry in dictionary.Entries)
            {
                writer.Write(entry.Key);
                if(cache.TryGetValue(entry.Value, out var offset))
                {
                    writer.Write(offset);
                }
                else
                {
                    writer.Write(characterOffset);
                    cache.Add(entry.Value, characterOffset);

                    var position = writer.BaseStream.Position;
                    writer.BaseStream.Position = postHeaderPosition + header.textOffset + characterOffset * 2;
                    characterOffset += entry.Value.Length + 1;
                    writer.WriteNullTerminatedUTF16(entry.Value);
                    writer.BaseStream.Position = position;
                }
            }
            header.textLength = characterOffset;

            var padding = ByteAlignment.AlignTo16Byte(writer.BaseStream.Position) - writer.BaseStream.Position;
            for (var i = 0; i < padding; ++i)
                writer.Write((byte)0);

            writer.BaseStream.Position = 0;
            header.Write(writer);
        }

    }
}
