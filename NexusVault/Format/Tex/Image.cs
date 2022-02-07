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

namespace NexusVault.Format.Tex
{
    public static class ImageFormatExtension
    {
        public static int BytesPerPixel(this ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.Grayscale: return 1;
                case ImageFormat.RGB: return 3;
                case ImageFormat.ARGB: return 4;
                default: return 0;
            }
        }
    }

    public enum ImageFormat
    {
        Grayscale,
        RGB,
        ARGB
    }

    public sealed class Image
    {
        public ImageFormat Format { get; }
        public int Width { get; }
        public int Height { get; }
        public byte[] Data { get; }

        public Image(int width, int height, ImageFormat format, byte[] data)
        {
            if (width <= 0)
                throw new ArgumentException("'width' must be greater than zero");

            if (height <= 0)
                throw new ArgumentException("'height' must be greater than zero");

            if (data == null)
                throw new ArgumentException("'data' must not be null");

            var expectedBytes = width * height * format.BytesPerPixel();
            if (data.Length != expectedBytes)
                throw new ArgumentException($"Image data does not fit an image of {width}x{height} of type {format}. Expected number of bytes {expectedBytes}, actual number of bytes was {data.Length}.");

            Width = width;
            Height = height;
            Format = format;
            Data = data;
        }
    }
}
