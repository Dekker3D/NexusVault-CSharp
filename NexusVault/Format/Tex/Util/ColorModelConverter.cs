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

namespace NexusVault.Format.Tex.Util
{
    public static class ColorModelConverter
    {
        public static byte[] PackRGBToBGR565(byte[] src)
        {
            var dst = new byte[src.Length / 3 * 2];
            for (int s = 0, d = 0; d < dst.Length; s += 3, d += 2)
            {
                var r = src[s + 0]; // R
                var g = src[s + 1]; // G
                var b = src[s + 2]; // B
                int data = (r & 0xF8) << 8 | (g & 0xFC) << 3 | (b & 0xF8) << 0;
                dst[d + 0] = (byte)(data >> 0 & 0xFF);
                dst[d + 1] = (byte)(data >> 8 & 0xFF);               
            }
            return dst;
        }

        public static byte[] PackARGBToBGR565(byte[] src)
        {
            var dst = new byte[src.Length / 2];
            for (int s = 0, d = 0; d < dst.Length; s += 4, d += 2)
            {
                var r = src[s + 1]; // R
                var g = src[s + 2]; // G
                var b = src[s + 3]; // B
                int data = (r & 0xF8) << 8 | (g & 0xFC) << 3 | (b & 0xF8) << 0;
                dst[d + 0] = (byte)(data >> 0 & 0xFF);
                dst[d + 1] = (byte)(data >> 8 & 0xFF);               
            }
            return dst;
        }

        public static byte[] PackGrayscaleToBGR565(byte[] src)
        {
            var dst = new byte[src.Length * 2];
            for (int s = 0, d = 0; d < dst.Length; s += 1, d += 2)
            {
                var shade = src[s + 0]; // R
                var data = (shade & 0xF8) << 8 | (shade & 0xFC) << 3 | (shade & 0xF8) << 0;
                dst[d + 0] = (byte)(data >> 0 & 0xFF);
                dst[d + 1] = (byte)(data >> 8 & 0xFF);               
            }
            return dst;
        }

        public static byte[] UnpackBGR565ToRGB(byte[] src)
        {
            var dst = new byte[src.Length / 2 * 3];
            for (int d = 0, s = 0; d < dst.Length; d += 3, s += 2)
            {
                dst[d++] = (byte)(src[s + 1] & 0x1F); // r
                dst[d++] = (byte)(src[s + 0] << 3 | ((src[s + 1] >> 5) & 0x7)); // g
                dst[d++] = (byte)((src[s + 0] >> 3) & 0x1F); // b
            }
            return dst;
        }

        public static byte[] ConvertRGBToBGR(byte[] src)
        {
            var dst = new byte[src.Length];
            for (int i = 0; i < dst.Length; i += 3)
            {
                dst[i + 2] = src[i + 0];
                dst[i + 1] = src[i + 1];
                dst[i + 0] = src[i + 2];
            }
            return dst;
        }

        public static byte[] ConvertBGRToRGB(byte[] src)
        {
            var dst = new byte[src.Length];
            for (int i = 0; i < dst.Length; i += 3)
            {
                dst[i + 2] = src[i + 0];
                dst[i + 1] = src[i + 1];
                dst[i + 0] = src[i + 2];
            }
            return dst;
        }

        public static byte[] ConvertBGRToARGB(byte[] src)
        {
            var dst = new byte[src.Length / 3 * 4];
            for (int s = 0, d = 0; d < dst.Length; s += 3, d += 4)
            {
                dst[d + 0] = (byte)0xFF;
                dst[d + 3] = src[s + 0];
                dst[d + 2] = src[s + 1];
                dst[d + 1] = src[s + 2];
            }
            return dst;
        }

        public static byte[] ConvertRGBToBGRA(byte[] src)
        {
            var dst = new byte[src.Length / 3 * 4];
            for (int s = 0, d = 0; d < dst.Length; s += 3, d += 4)
            {
                dst[d + 3] = (byte)0xFF;
                dst[d + 2] = src[s + 0];
                dst[d + 1] = src[s + 1];
                dst[d + 0] = src[s + 2];
            }
            return dst;
        }

        public static byte[] ConvertARGBToABGR(byte[] src)
        {
            var dst = new byte[src.Length];
            for (int i = 0; i < dst.Length; i += 4)
            {
                dst[i + 0] = src[i + 0];
                dst[i + 3] = src[i + 1];
                dst[i + 2] = src[i + 2];
                dst[i + 1] = src[i + 3];
            }
            return dst;
        }

        public static byte[] ConvertARGBToBGRA(byte[] src)
        {
            var dst = new byte[src.Length];
            for (int i = 0; i < dst.Length; i += 4)
            {
                dst[i + 3] = src[i + 0];
                dst[i + 2] = src[i + 1];
                dst[i + 1] = src[i + 2];
                dst[i + 0] = src[i + 3];
            }
            return dst;
        }

        public static byte[] ConvertABGRToARGB(byte[] src)
        {
            var dst = new byte[src.Length];
            for (int i = 0; i < dst.Length; i += 4)
            {
                dst[i + 0] = src[i + 0];
                dst[i + 3] = src[i + 1];
                dst[i + 2] = src[i + 2];
                dst[i + 1] = src[i + 3];
            }
            return dst;
        }

        public static void InplaceConvertARGBToRGBA(byte[] arr)
        {
            for (int i = 0; i < arr.Length; i += 4)
            {
                var a = arr[i + 0];
                var r = arr[i + 1];
                var g = arr[i + 2];
                var b = arr[i + 3];
                arr[i + 0] = r;
                arr[i + 1] = g;
                arr[i + 2] = b;
                arr[i + 3] = a;
            }
        }

        public static void InplaceConvertRGBAToARGB(byte[] arr)
        {
            for (int i = 0; i < arr.Length; i += 4)
            {
                var r = arr[i + 0];
                var g = arr[i + 1];
                var b = arr[i + 2];
                var a = arr[i + 3];
                arr[i + 0] = a;
                arr[i + 1] = r;
                arr[i + 2] = g;
                arr[i + 3] = b;
            }
        }

        public static void InplaceConvertBGRAToARGB(byte[] arr)
        {
            for (int i = 0; i < arr.Length; i += 4)
            {
                var b = arr[i + 0];
                var g = arr[i + 1];
                var r = arr[i + 2];
                var a = arr[i + 3];
                arr[i + 0] = a;
                arr[i + 1] = r;
                arr[i + 2] = g;
                arr[i + 3] = b;
            }
        }

        public static byte[] ConvertARGBToRGBA(byte[] src)
        {
            var dst = new byte[src.Length];
            for (int i = 0; i < dst.Length; i += 4)
            {
                dst[i + 3] = src[i + 0];
                dst[i + 0] = src[i + 1];
                dst[i + 1] = src[i + 2];
                dst[i + 2] = src[i + 3];
            }
            return dst;
        }

        public static byte[] ConvertGrayscaleToRGBA(byte[] src)
        {
            var dst = new byte[src.Length * 4];
            for (int s = 0, d = 0; d < dst.Length; s += 1, d += 4)
            {
                dst[d + 0] = src[s + 0];
                dst[d + 1] = src[s + 0];
                dst[d + 2] = src[s + 0];
                dst[d + 3] = (byte)0xFF;
            }
            return dst;
        }

        public static byte[] ConvertARGBToGrayscale(byte[] src)
        {
            var dst = new byte[src.Length / 4];
            for (int s = 0, d = 0; d < dst.Length; s += 4, d += 1)
            {
                var r = src[s + 1];
                var g = src[s + 2];
                var b = src[s + 3];
                var luminosity = Math.Min(255, Math.Max(0, Math.Round(0.21f * r + 0.72f * g + 0.07f * b)));
                dst[d] = (byte)luminosity;
            }
            return dst;
        }

        public static byte[] ConvertABGRToGrayscale(byte[] src)
        {
            var dst = new byte[src.Length / 4];
            for (int s = 0, d = 0; d < dst.Length; s += 4, d += 1)
            {
                var b = src[s + 1];
                var g = src[s + 2];
                var r = src[s + 3];
                var luminosity = Math.Min(255, Math.Max(0, Math.Round(0.21f * r + 0.72f * g + 0.07f * b)));
                dst[d] = (byte)luminosity;
            }
            return dst;
        }

        public static byte[] ConvertGrayscaleToARGB(byte[] src)
        {
            var dst = new byte[src.Length * 4];
            for (int s = 0, d = 0; d < dst.Length; s += 1, d += 4)
            {
                dst[d + 0] = (byte)0xFF;
                dst[d + 1] = src[s + 0];
                dst[d + 2] = src[s + 0];
                dst[d + 3] = src[s + 0];
            }
            return dst;
        }

        public static byte[] ConvertGrayscaleToBGRA(byte[] src)
        {
            var dst = new byte[src.Length * 4];
            for (int s = 0, d = 0; d < dst.Length; s += 1, d += 4)
            {
                dst[d + 0] = src[s + 0];
                dst[d + 1] = src[s + 0];
                dst[d + 2] = src[s + 0];
                dst[d + 3] = (byte)0xFF;
            }
            return dst;
        }

        public static byte[] ConvertARGBToRGB(byte[] src)
        {
            var dst = new byte[src.Length / 4 * 3];
            for (int s = 0, d = 0; d < dst.Length; s += 4, d += 3)
            {
                dst[d + 0] = src[s + 1];
                dst[d + 1] = src[s + 2];
                dst[d + 2] = src[s + 3];
            }
            return dst;
        }

        public static byte[] ConvertABGRToRGB(byte[] src)
        {
            var dst = new byte[src.Length / 4 * 3];
            for (int s = 0, d = 0; d < dst.Length; s += 4, d += 3)
            {
                dst[d + 2] = src[s + 1];
                dst[d + 1] = src[s + 2];
                dst[d + 0] = src[s + 3];
            }
            return dst;
        }

        public static byte[] ConvertRGBToARGB(byte[] src)
        {
            var dst = new byte[src.Length / 3 * 4];
            for (int s = 0, d = 0; d < dst.Length; s += 3, d += 4)
            {
                dst[d + 0] = (byte)0xFF;
                dst[d + 1] = src[s + 0];
                dst[d + 2] = src[s + 1];
                dst[d + 3] = src[s + 2];
            }
            return dst;
        }

        public static byte[] ConvertRGBToRGBA(byte[] src)
        {
            var dst = new byte[src.Length / 3 * 4];
            for (int s = 0, d = 0; d < dst.Length; s += 3, d += 4)
            {
                dst[d + 0] = src[s + 0];
                dst[d + 1] = src[s + 1];
                dst[d + 2] = src[s + 2];
                dst[d + 3] = (byte)0xFF;
            }
            return dst;
        }

        public static byte[] ConvertGrayscaleToRGB(byte[] src)
        {
            var dst = new byte[src.Length * 3];
            for (int s = 0, d = 0; d < dst.Length; s += 1, d += 3)
            {
                dst[d + 0] = src[s + 0];
                dst[d + 1] = src[s + 0];
                dst[d + 2] = src[s + 0];
            }
            return dst;
        }

        public static byte[] ConvertRGBToGrayscale(byte[] src)
        {
            var dst = new byte[src.Length / 3];
            for (int s = 0, d = 0; d < dst.Length; s += 3, d += 1)
            {
                var r = src[s + 0];
                var g = src[s + 1];
                var b = src[s + 2];
                var luminosity = Math.Min(255, Math.Max(0, Math.Round(0.21f * r + 0.72f * g + 0.07f * b)));
                dst[d] = (byte)luminosity;
            }
            return dst;
        }

        public static byte[] ConvertBGRToGrayscale(byte[] src)
        {
            var dst = new byte[src.Length / 3];
            for (int s = 0, d = 0; d < dst.Length; s += 3, d += 1)
            {
                var b = src[s + 0];
                var g = src[s + 1];
                var r = src[s + 2];
                var luminosity = Math.Min(255, Math.Max(0, Math.Round(0.21f * r + 0.72f * g + 0.07f * b)));
                dst[d] = (byte)luminosity;
            }
            return dst;
        }
    }
}
