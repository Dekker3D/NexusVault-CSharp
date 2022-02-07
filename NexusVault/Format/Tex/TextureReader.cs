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

namespace NexusVault.Format.Tex {
    public static class TextureReader {

        public static FileHeader GetFileHeader(byte[] data) {
            return ReadFileHeader(new BinaryReader(new MemoryStream(data)));
        }

        public static FileHeader ReadFileHeader(BinaryReader reader) {
            return new FileHeader(reader);
        }

        public static Texture Read(byte[] data) {
            return new Texture(data);
        }

        public static Image ReadFirstImage(byte[] data) {
            var texture = Read(data);
            return texture.getMipMap(0);
        }
    }
}
