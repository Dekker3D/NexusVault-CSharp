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
using Squish;
using System;

namespace NexusVault.Format.Tex.Dxt
{
    public static class DXTImage {

        #region read

        public static Image Decompress(FileHeader header, byte[] dxtImages, int offset, int mipMap) {
            if (header.signature != FileHeader.Signature) {
                throw new SignatureMismatchException("tex", FileHeader.Signature, header.signature);
            }

            if (header.version != 3) {
                throw new VersionMismatchException("tex", 3, header.version);
            }

            var textureType = header.TextureType;
            var compressionType = GetCompressionType(textureType);

            int width = 0, height = 0, length = 0, arrayOffset = offset;
            for (int i = 0; i <= mipMap; ++i) {
                width = (int)(header.width / Math.Pow(2, header.textureCount - 1 - i));
                height = (int)(header.height / Math.Pow(2, header.textureCount - 1 - i));
                width = width <= 0 ? 1 : width;
                height = height <= 0 ? 1 : height;
                arrayOffset += length;
                length = (width + 3) / 4 * ((height + 3) / 4) * (compressionType.HasFlag(SquishFlags.kDxt1) ? 8 : 16);
            }

            var buffer = new byte[length];
            Array.Copy(dxtImages, arrayOffset, buffer, 0, buffer.Length);
            return DecompressSingleImage(textureType, buffer, width, height);
        }

        public static Image DecompressSingleImage(TextureType type, byte[] dxtData, int width, int height) {
            var decompressedImage = DecompressToRGBA(type, dxtData, width, height);
            ColorModelConverter.InplaceConvertRGBAToARGB(decompressedImage);
            return new Image(width, height, ImageFormat.ARGB, decompressedImage);
        }

        public static byte[] DecompressToRGBA(TextureType dxtType, byte[] dxt, int width, int height) {
            var rgba = new byte[width * height * 4];
            Squish.Squish.DecompressImage(rgba, width, height, dxt, GetCompressionType(dxtType));
            return rgba;
        }

        #endregion

        #region write

        public static byte[] CompressSingleImage(TextureType dxtType, Image image)
        {
            byte[] rgba;
            switch (image.Format)
            {
                case ImageFormat.ARGB:
                    rgba = ColorModelConverter.ConvertARGBToRGBA(image.Data);
                    break;
                case ImageFormat.RGB:
                    rgba = ColorModelConverter.ConvertRGBToRGBA(image.Data);
                    break;
                case ImageFormat.Grayscale:
                    rgba = ColorModelConverter.ConvertGrayscaleToRGBA(image.Data);
                    break;
                default:
                    throw new ArgumentException($"Invalid image format: {image.Format}.");
            }
            return CompressSingleImage(dxtType, rgba, image.Width, image.Height);
        }

        public static byte[] CompressSingleImage(TextureType dxtType, byte[] rgba, int width, int height)
        {
            var compressionType = GetCompressionType(dxtType) | SquishFlags.kColourClusterFit;
            var dest = new byte[Squish.Squish.GetStorageRequirements(width, height, compressionType)];
            Squish.Squish.CompressImage(rgba, width, height, dest, compressionType);
            return dest;
        }

        #endregion

        private static SquishFlags GetCompressionType(TextureType type)
        {
            switch (type)
            {
                case TextureType.Dxt1:
                    return SquishFlags.kDxt1;
                case TextureType.Dxt3:
                    return SquishFlags.kDxt3;
                case TextureType.Dxt5:
                    return SquishFlags.kDxt5;
                default:
                    throw new ArgumentException($"Invalid texture type: {type}.", nameof(type));
            }
        }
    }
}