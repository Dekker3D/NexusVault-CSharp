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

using NexusVault.Format.Tex.Util;
using NexusVault.Shared.Exception;
using System;

namespace NexusVault.Format.Tex.Plain {
    public static class PlainImageReader {
        public static Image GetImage(FileHeader header, byte[] images, int offset, int mipMap) {
            if (header.signature != FileHeader.Signature) {
                throw new SignatureMismatchException("tex", FileHeader.Signature, header.signature);
            }

            if (header.version != 3) {
                throw new VersionMismatchException("tex", 3, header.version);
            }

            var textureType = header.TextureType;

            int bytesPerPixel;
            switch (textureType) {
                case TextureType.ARGB1:
                case TextureType.ARGB2:
                    bytesPerPixel = 4;
                    break;
                case TextureType.Grayscale:
                    bytesPerPixel = 1;
                    break;
                case TextureType.RGB:
                    bytesPerPixel = 2;
                    break;
                default:
                    throw new ArgumentException($"Invalid texture type {textureType}.");
            }

            int width = 0, height = 0, length = 0, arrayOffset = offset;
            for (int i = 0; i <= mipMap; ++i) {
                width = (int)(header.width / Math.Pow(2, header.textureCount - 1 - i));
                height = (int)(header.height / Math.Pow(2, header.textureCount - 1 - i));
                width = width <= 0 ? 1 : width;
                height = height <= 0 ? 1 : height;
                arrayOffset += length;
                length = width * height * bytesPerPixel;
            }

            var buffer = new byte[length];
            Array.Copy(images, arrayOffset, buffer, 0, buffer.Length);

            switch (textureType) {
                case TextureType.ARGB1:
                case TextureType.ARGB2:
                    ColorModelConverter.InplaceConvertBGRAToARGB(buffer);
                    return new Image(width, height, ImageFormat.ARGB, buffer);
                case TextureType.RGB:
                    return new Image(width, height, ImageFormat.RGB, ColorModelConverter.UnpackBGR565ToRGB(buffer));
                case TextureType.Grayscale:
                    return new Image(width, height, ImageFormat.Grayscale, buffer);
                default:
                    throw new ArgumentException($"Invalid texture type {textureType}.");
            }
        }
    }
}