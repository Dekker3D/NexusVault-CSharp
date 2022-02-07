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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NexusVault.Format.Tex.Util
{
    public static class ImageBitmapConverter
    {

        public static Bitmap Convert(Image image)
        {
            Bitmap bitmap;
            switch (image.Format)
            {
                case ImageFormat.ARGB:
                    bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                    break;
                case ImageFormat.RGB:
                    bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
                    break;
                case ImageFormat.Grayscale:
                    bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed);
                    var colorPalette = bitmap.Palette;
                    for (var i = 0; i < colorPalette.Entries.Length; i++)
                        colorPalette.Entries[i] = Color.FromArgb(i, i, i);
                    bitmap.Palette = colorPalette;
                    break;
                default:
                    throw new ArgumentException($"Unknown format: {image.Format}.");
            }

            var data = bitmap.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(image.Data, 0, data.Scan0, image.Data.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }

        public static Image Convert(Bitmap bitmap, ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.ARGB:
                    {
                        var bitmapBits = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                        var imageData = new byte[bitmap.Width * bitmap.Height * 4];
                        Marshal.Copy(bitmapBits.Scan0, imageData, 0, imageData.Length);
                        bitmap.UnlockBits(bitmapBits);
                        return new Image(bitmap.Width, bitmap.Height, ImageFormat.ARGB, imageData);
                    }
                case ImageFormat.RGB:
                    {
                        var bitmapBits = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                        var imageData = new byte[bitmap.Width * bitmap.Height * 3];
                        Marshal.Copy(bitmapBits.Scan0, imageData, 0, imageData.Length);
                        bitmap.UnlockBits(bitmapBits);
                        return new Image(bitmap.Width, bitmap.Height, ImageFormat.RGB, imageData);
                    }
                case ImageFormat.Grayscale:
                    {
                        var bitmapBits = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                        var imageData = new byte[bitmap.Width * bitmap.Height * 1];
                        Marshal.Copy(bitmapBits.Scan0, imageData, 0, imageData.Length);
                        bitmap.UnlockBits(bitmapBits);
                        return new Image(bitmap.Width, bitmap.Height, ImageFormat.RGB, imageData);
                    }
                default:
                    throw new ArgumentException($"Unknown format: {imageFormat}.");
            }
        }

        private static Bitmap ConvertPixelFormat(Bitmap bitmap, PixelFormat format)
        {
            var converted = new Bitmap(bitmap.Width, bitmap.Height, format);
            using(var graphics = Graphics.FromImage(converted))            
                graphics.DrawImage(bitmap, new Point(0, 0));            
            return converted;
        }

    }
}
