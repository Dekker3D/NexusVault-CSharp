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
using NexusVault.Format.Tex.Dxt;
using NexusVault.Format.Tex.Jpg;
using NexusVault.Format.Tex.Plain;
using NexusVault.Format.Tex.Util;

namespace NexusVault.Format.Tex
{
    public static class TextureWriter
    {
        public static byte[] ToBinary(TextureType target, Image image, int mipmapCount, int quality, int[] defaultColors)
        {
            var mipmaps = GenerateMipMaps(image, mipmapCount);
            return ToBinary(target, mipmaps, quality, defaultColors);
        }

        public static byte[] ToBinary(TextureType target, Image[] mipmaps, int quality, int[] defaultColors)
        {
            var stream = new MemoryStream(FileHeader.SizeOf);
            Write(target, mipmaps, quality, defaultColors, new BinaryWriter(stream));
            return stream.ToArray();
        }

        public static void Write(TextureType target, Image[] mipmaps, int quality, int[] defaultColors, BinaryWriter writer)
        {
            var header = new FileHeader
            {
                signature = FileHeader.Signature,
                version = 3,
                width = mipmaps[mipmaps.Length - 1].Width,
                height = mipmaps[mipmaps.Length - 1].Height,
                depth = 1,
                sides = 1,
                textureCount = mipmaps.Length,
                TextureType = target,
                imageSizsCount = mipmaps.Length
            };

            SortImagesSmallToLarge(mipmaps);

            writer.BaseStream.Position = FileHeader.SizeOf;
            for (var i = 0; i < mipmaps.Length; ++i)
            {
                var encodedImage = CompressImage(target, mipmaps[i], quality, defaultColors);
                header.imageSizes[i] = encodedImage.Length;
                writer.Write(encodedImage);
            }

            writer.BaseStream.Position = 0;
            header.Write(writer);
        }



        private static byte[] CompressImage(TextureType target, Image image, int quality, int[] defaultColors)
        {
            switch (target)
            {
                case TextureType.Dxt1:
                case TextureType.Dxt3:
                case TextureType.Dxt5:
                    return DXTImage.CompressSingleImage(target, image);
                case TextureType.Jpg1:
                case TextureType.Jpg2:
                case TextureType.Jpg3:
                    return JpgImage.CompressSingleImage(target, image, quality, defaultColors);
                case TextureType.ARGB1:
                case TextureType.ARGB2:
                case TextureType.RGB:
                case TextureType.Grayscale:
                    return PlainImage.GetBinary(target, image);
                default:
                    throw new ArgumentException($"Invalid texture type: {target}.");
            }
        }

        public static Image[] GenerateMipMaps(Image image, int mipmaps)
        {
            return TextureMipMapGenerator.Generate(image, mipmaps);
        }

        public static Image[] SortImagesSmallToLarge(Image[] mipmaps)
        {
            var copy = new Image[mipmaps.Length];
            Array.Copy(mipmaps, 0, copy, 0, copy.Length);
            Array.Sort(copy, (a, b) =>
            {
                int order = a.Height * a.Width - b.Height * b.Width;
                return order; // smallest to largest
            });
            return copy;
        }
    }
}