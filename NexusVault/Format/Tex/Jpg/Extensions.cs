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

namespace NexusVault.Format.Tex.Jpg.Extension
{
    internal static class FileHeaderExtension
    {
        public static JpgType GetJpgType(this FileHeader header) => header.TextureType.GetJpgType();

        public static JpgType GetJpgType(this TextureType type)
        {
            switch (type)
            {
                case TextureType.Jpg1:
                    return JpgType.Type1;
                case TextureType.Jpg2:
                    return JpgType.Type2;
                case TextureType.Jpg3:
                    return JpgType.Type3;
                default:
                    throw new ArgumentException("Invalid texture type: {type}");
            }
        }

        public static int[] GetDefaultColors(this FileHeader header)
        {
            return new int[] { GetDefaultColor(header, 0), GetDefaultColor(header, 1), GetDefaultColor(header, 2), GetDefaultColor(header, 3) };
        }

        private static int GetDefaultColor(FileHeader header, int idx)
        {
            return header.jpgChannel[idx].hasColor ? header.jpgChannel[idx].color & 0xFF : -1;
        }

        public static float[][] GetQuantTables(this FileHeader header)
        {
            return QuantTable.AdjustQuantTables(header.TextureType.GetJpgType(), header.jpgChannel[0].quality, header.jpgChannel[1].quality, header.jpgChannel[2].quality,
                    header.jpgChannel[3].quality);
        }
    }

    internal static class ArrayExtension
    {
        public static void FillWithValue<T>(this T[] arr, T fillValue)
        {
            arr.FillWithValue(fillValue, 0, arr.Length);
        }

        public static void FillWithValue<T>(this T[] arr, T fillValue, int offset, int length)
        {
            var startIdx = offset;
            var endIdx = startIdx + length;
            arr[startIdx] = fillValue;
            for (int dstPos = startIdx + 1, copyLength = 1; dstPos < endIdx; dstPos += copyLength, copyLength += copyLength)
                Array.Copy(arr, startIdx, arr, dstPos, dstPos + copyLength > endIdx ? endIdx - dstPos : copyLength);
        }
    }


}
