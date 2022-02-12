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

using System.Collections.Generic;
using System.IO;
using NexusVault.Shared.Exception;
using NexusVault.Format.Bin.Struct;
using NexusVault.Shared.Extension;

namespace NexusVault.Format.Bin
{
    public static class LanguageReader
    {

        public static LanguageDictionary Read(byte[] data)
        {
            return Read(new BinaryReader(new MemoryStream(data)));
        }

        public static LanguageDictionary Read(BinaryReader reader)
        {
            var header = new FileHeader(reader);
 
            if (header.signature != FileHeader.Signature)            
                throw new SignatureMismatchException("bin", FileHeader.Signature, header.signature);            

            if (header.version != 4)            
                throw new VersionMismatchException("bin", 4, header.version);            

            var postHeaderPosition = reader.BaseStream.Position;

            reader.BaseStream.Position = postHeaderPosition + (long)header.tagNameOffset;
            var tagName = reader.ReadNullTerminatedUTF16();
            if (tagName.Length > header.tagNameLength - 1)
                throw new InvalidDataException();

            reader.BaseStream.Position = postHeaderPosition + (long)header.shortNameOffset;
            var shortName = reader.ReadNullTerminatedUTF16();
            if (shortName.Length > header.shortNameLength - 1)
                throw new InvalidDataException();

            reader.BaseStream.Position = postHeaderPosition + (long)header.longNameOffset;
            var longName = reader.ReadNullTerminatedUTF16();
            if (longName.Length > header.longNameLength - 1)
                throw new InvalidDataException();

            var locale = new Locale
            (
                (int)header.languageType,
                tagName,
                shortName,
                longName
            );

            
            reader.BaseStream.Position = postHeaderPosition + header.textOffset;
            var entries = new Entry[header.entryCount];
            for (var i = 0; i < entries.Length; ++i)            
                entries[i] = new Entry(reader);            

            var dictionary = new Dictionary<uint, string>(entries.Length);
            foreach (var entry in entries){
                reader.BaseStream.Position = postHeaderPosition + header.textOffset + entry.characterOffset * 2;
                dictionary.Add(entry.id, reader.ReadNullTerminatedUTF16()) ;
            }

            return new LanguageDictionary(locale, dictionary);
        }

    }
}
