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

using NexusVault.Format.Tex.Dxt;
using NexusVault.Format.Tex.Jpg;
using NexusVault.Format.Tex.Plain;
using NexusVault.Shared.Exception;

namespace NexusVault.Format.Tex
{
    public sealed class Texture
    {
        public static Texture Read(byte[] data)
        {
            return new Texture(data);
        }

        private readonly FileHeader _header;
	    private readonly byte[] _data;

        public Texture(byte[] data)
        {
            _header = TextureReader.GetFileHeader(data);
            _data = data;
        }

        public int MipMapCount { get => _header.textureCount; }
        public int Width { get => _header.width; }
        public int Height { get => _header.height; }
        public int Version { get => _header.version; }
        public int Sides { get => _header.sides; }
        public int Depth { get => _header.depth; }

        public TextureType TextureType { get => _header.TextureType; }

        public ImageFormat ImageFormat
        {
            get
            {
                switch (TextureType)
                {
                    case TextureType.ARGB1:
                    case TextureType.ARGB2:
                    case TextureType.Dxt1:
                    case TextureType.Dxt3:
                    case TextureType.Dxt5:
                    case TextureType.Jpg1:
                    case TextureType.Jpg2:
                    case TextureType.Jpg3:
                        return ImageFormat.ARGB;
                    case TextureType.RGB:
                        return ImageFormat.RGB;
                    case TextureType.Grayscale:
                        return ImageFormat.Grayscale;
                    default:
                        throw new TextureException();
                }
            }
        }

        public Image GetMipMap(int index)
        {
            index = this._header.textureCount - index - 1;
            switch (TextureType)
            {
                case TextureType.Dxt1:
                case TextureType.Dxt3:
                case TextureType.Dxt5:
                    return DXTImageReader.Decompress(this._header, this._data, FileHeader.SizeOf, index);
                case TextureType.Jpg1:
                case TextureType.Jpg2:
                case TextureType.Jpg3:
                    return JPGImageReader.Decompress(this._header, this._data, FileHeader.SizeOf, index);
                case TextureType.ARGB1:
                case TextureType.ARGB2:
                case TextureType.RGB:
                case TextureType.Grayscale:
                    return PlainImageReader.GetImage(this._header, this._data, FileHeader.SizeOf, index);
                default:
                    throw new TextureException();
            }
        }
    }
}
