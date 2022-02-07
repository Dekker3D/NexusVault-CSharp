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

namespace NexusVault.Format.Tex.Jpg
{
    public enum JpgType
    {
        Type1,
        Type2,
        Type3
    }

    public enum CompressionType
    {
        Chrominance,
        Luminance
    }

    public static class Constants
    {
        // uint8
        private readonly static byte[] HUFF_DC_LUMA_BITS = { 0, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
        private readonly static byte[] HUFF_DC_LUMA_VALS = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        private readonly static byte[] HUFF_AC_LUMA_BITS = { 0, 2, 1, 3, 3, 2, 4, 3, 5, 5, 4, 4, 0, 0, 1, 125 };
        private readonly static byte[] HUFF_AC_LUMA_VALS = { 0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07, 0x22, 0x71,
            0x14, 0x32, 0x81, 0x91, 0xa1, 0x08, 0x23, 0x42, 0xb1, 0xc1, 0x15, 0x52, 0xd1, 0xf0, 0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0a, 0x16, 0x17, 0x18,
            0x19, 0x1a, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x53,
            0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x83,
            0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9,
            0xaa, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6,
            0xd7, 0xd8, 0xd9, 0xda, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa };

        // uint8
        private readonly static byte[] HUFF_DC_CHROMA_BITS = { 0, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 };
        private readonly static byte[] HUFF_DC_CHROMA_VALS = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        private readonly static byte[] HUFF_AC_CHROMA_BITS = { 0, 2, 1, 2, 4, 4, 3, 4, 7, 5, 4, 4, 0, 1, 2, 119 };
        private readonly static byte[] HUFF_AC_CHROMA_VALS = { 0x00, 0x01, 0x02, 0x03, 0x11, 0x04, 0x05, 0x21, 0x31, 0x06, 0x12, 0x41, 0x51, 0x07, 0x61, 0x71, 0x13,
            0x22, 0x32, 0x81, 0x08, 0x14, 0x42, 0x91, 0xa1, 0xb1, 0xc1, 0x09, 0x23, 0x33, 0x52, 0xf0, 0x15, 0x62, 0x72, 0xd1, 0x0a, 0x16, 0x24, 0x34, 0xe1,
            0x25, 0xf1, 0x17, 0x18, 0x19, 0x1a, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49,
            0x4a, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79,
            0x7a, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6,
            0xa7, 0xa8, 0xa9, 0xaa, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2, 0xd3,
            0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9,
            0xfa };

        public readonly static HuffmanTable HUFFMAN_CHROMA_DC = new HuffmanTable(HUFF_DC_CHROMA_BITS, HUFF_DC_CHROMA_VALS);
        public readonly static HuffmanTable HUFFMAN_CHROMA_AC = new HuffmanTable(HUFF_AC_CHROMA_BITS, HUFF_AC_CHROMA_VALS);
        public readonly static HuffmanTable HUFFMAN_LUMA_DC = new HuffmanTable(HUFF_DC_LUMA_BITS, HUFF_DC_LUMA_VALS);
        public readonly static HuffmanTable HUFFMAN_LUMA_AC = new HuffmanTable(HUFF_AC_LUMA_BITS, HUFF_AC_LUMA_VALS);

        internal readonly static int[] QUANT_TABLE_LUMA = { 16, 11, 10, 16, 24, 40, 51, 61, 12, 12, 14, 19, 26, 58, 60, 55, 14, 13, 16, 24, 40, 57, 69, 56, 14, 17, 22,
            29, 51, 87, 80, 62, 18, 22, 37, 56, 68, 109, 103, 77, 24, 35, 55, 64, 81, 104, 113, 92, 49, 64, 78, 87, 103, 121, 120, 101, 72, 92, 95, 98, 112,
            100, 103, 99 };

        internal readonly static int[] QUANT_TABLE_CHROMA = { 17, 18, 24, 47, 99, 99, 99, 99, 18, 21, 26, 66, 99, 99, 99, 99, 24, 26, 56, 99, 99, 99, 99, 99, 47, 66, 99,
            99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99,
            99 };

        public const int BLOCK_WIDTH = 8;
        public const int BLOCK_HEIGHT = 8;
        public const int BLOCK_SIZE = BLOCK_WIDTH * BLOCK_HEIGHT;

        internal readonly static int[] ZIGZAG_SEQUENCE = { 0, 1, 5, 6, 14, 15, 27, 28, 2, 4, 7, 13, 16, 26, 29, 42, 3, 8, 12, 17, 25, 30, 41, 43, 9, 11, 18, 24, 31, 40,
            44, 53, 10, 19, 23, 32, 39, 45, 52, 54, 20, 22, 33, 38, 46, 51, 55, 60, 21, 34, 37, 47, 50, 56, 59, 61, 35, 36, 48, 49, 57, 58, 62, 63 };

        // this can be changed
        public const int ALPHA_IDX = 0;
        public const int RED_IDX = 1;
        public const int GREEN_IDX = 2;
        public const int BLUE_IDX = 3;
    }

    public static class QuantTable
    {
        internal static int[] GetQuantTable(CompressionType type)
        {
            switch (type)
            {
                case CompressionType.Chrominance:
                    return Constants.QUANT_TABLE_CHROMA;
                case CompressionType.Luminance:
                    return Constants.QUANT_TABLE_LUMA;
                default:
                    throw new ArgumentException(nameof(type));
            }
        }

        internal static float[] AdjustQuantTable(int[] quantTable, int qualityFactor)
        {
            float scale;
            if (qualityFactor <= 0)
                scale = 0.02f;
            else if (qualityFactor >= 100)
                scale = 1f;
            else
                scale = (200 - qualityFactor * 2) * 0.01f;

            var adjustedQuantTable = new float[quantTable.Length];
            if (scale >= 1f)
            {
                Array.Copy(quantTable, 0, adjustedQuantTable, 0, adjustedQuantTable.Length);
                return adjustedQuantTable;
            }

            for (int i = 0; i < adjustedQuantTable.Length; ++i)
                adjustedQuantTable[i] = Math.Max(1f, Math.Min(quantTable[i] * scale, 255f));
            return adjustedQuantTable;
        }

        public static float[] AdjustQuantTable(CompressionType type, int qualityFactor)
        {
            return AdjustQuantTable(GetQuantTable(type), qualityFactor);
        }

        public static float[][] AdjustQuantTables(JpgType type, int qualityFactor)
        {
            return AdjustQuantTables(type, qualityFactor, qualityFactor, qualityFactor, qualityFactor);
        }

        public static float[][] AdjustQuantTables(JpgType type, int qualityFactor1, int qualityFactor2, int qualityFactor3, int qualityFactor4)
        {
            switch (type)
            {
                case JpgType.Type1:
                case JpgType.Type3:
                    return new float[][] { //
						AdjustQuantTable(CompressionType.Luminance, qualityFactor1), //
						AdjustQuantTable(CompressionType.Chrominance, qualityFactor2), //
						AdjustQuantTable(CompressionType.Chrominance, qualityFactor3), //
						AdjustQuantTable(CompressionType.Luminance, qualityFactor4) //
				};
                case JpgType.Type2:
                    return new float[][] { //
						AdjustQuantTable(CompressionType.Luminance, qualityFactor1), //
						AdjustQuantTable(CompressionType.Luminance, qualityFactor2), //
						AdjustQuantTable(CompressionType.Luminance, qualityFactor3), //
						AdjustQuantTable(CompressionType.Luminance, qualityFactor4) //
				};
            }
            throw new ArgumentException($"Invalid jpg type: {type}.");
        }
    }
}