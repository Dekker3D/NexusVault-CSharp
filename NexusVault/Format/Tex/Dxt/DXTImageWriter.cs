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
using Squish;
using System;

namespace NexusVault.Format.Tex.Dxt {
    public static class DXTImageWriter {

        public static byte[] CompressSingleImage(TextureType dxtType, Image image) {
            byte[] rgba;
            switch (image.Format) {
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

        public static byte[] CompressSingleImage(TextureType dxtType, byte[] rgba, int width, int height) {
            var compressionType = GetCompressionType(dxtType) | SquishFlags.kColourClusterFit;
            var dest = new byte[Squish.Squish.GetStorageRequirements(width, height, compressionType)];
            Squish.Squish.CompressImage(rgba, width, height, dest, compressionType);
            return dest;
        }

        private static SquishFlags GetCompressionType(TextureType type) {
            switch (type) {
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