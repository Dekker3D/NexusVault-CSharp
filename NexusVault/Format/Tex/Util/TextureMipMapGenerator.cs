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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NexusVault.Format.Tex.Util
{
    public static class TextureMipMapGenerator
    {
        /// <summary>
        /// Generates a mipmap for a given image. Images are stored in sequence in an array. The first image is the original image.
        /// </summary>
        /// <param name="src">The original image</param>
        /// <param name="numberOfMipMaps">Number of mipmaps, needs to be at least 1</param>
        /// <returns>A Mipmap</returns>
        public static Image[] Generate(Image src, int numberOfMipMaps)
        {
            if (numberOfMipMaps == 0)            
                throw new ArgumentException("Must be greater than 0", nameof(numberOfMipMaps));
            

            if (numberOfMipMaps == 1)            
                return new Image[] { src };
            

            if (numberOfMipMaps < 0)
            {
                var value = Math.Min(src.Width, src.Height);
                numberOfMipMaps = (int)Math.Ceiling(Math.Log(value) / Math.Log(2));
            }

            var tmp = new Bitmap[numberOfMipMaps];
            tmp[0] = ImageBitmapConverter.Convert(src);

            for (var i = 1; i < tmp.Length; ++i)
            {
                var newWidth = Math.Max(1, tmp[i - 1].Width >> 1);
                var newHeight = Math.Max(1, tmp[i - 1].Height >> 1);
                tmp[i] = ResizeImage(tmp[i - 1], newWidth, newHeight);
            }

            var images = new Image[tmp.Length];
            for (var i = 0; i < images.Length; ++i)
                images[i] = ImageBitmapConverter.Convert(tmp[i], src.Format);

            return images;
        }


        private static Bitmap ResizeImage(System.Drawing.Image src, int width, int height)
        {
            var dst = new Bitmap(width, height);
            dst.SetResolution(src.HorizontalResolution, src.VerticalResolution);

            using (var graphics = Graphics.FromImage(dst))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(src, new Rectangle(0, 0, width, height), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return dst;
        }
    }
}