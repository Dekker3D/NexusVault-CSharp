using System;

namespace NexusVault.tex
{
    static class JpgDecoderPrototype
    {
        // uint8
        static readonly byte[] HUFF_DC_LUMA_BITS = { 0, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
        static readonly byte[] HUFF_DC_LUMA_VALS = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        static readonly byte[] HUFF_AC_LUMA_BITS = { 0, 2, 1, 3, 3, 2, 4, 3, 5, 5, 4, 4, 0, 0, 1, 125 };
        static readonly byte[] HUFF_AC_LUMA_VALS = { 0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07, 0x22, 0x71,
            0x14, 0x32, 0x81, 0x91, 0xa1, 0x08, 0x23, 0x42, 0xb1, 0xc1, 0x15, 0x52, 0xd1, 0xf0, 0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0a, 0x16, 0x17, 0x18,
            0x19, 0x1a, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x53,
            0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x83,
            0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9,
            0xaa, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6,
            0xd7, 0xd8, 0xd9, 0xda, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa };

        // uint8
        static readonly byte[] HUFF_DC_CHROMA_BITS = { 0, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 };
        static readonly byte[] HUFF_DC_CHROMA_VALS = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        static readonly byte[] HUFF_AC_CHROMA_BITS = { 0, 2, 1, 2, 4, 4, 3, 4, 7, 5, 4, 4, 0, 1, 2, 119 };
        static readonly byte[] HUFF_AC_CHROMA_VALS = { 0x00, 0x01, 0x02, 0x03, 0x11, 0x04, 0x05, 0x21, 0x31, 0x06, 0x12, 0x41, 0x51, 0x07, 0x61, 0x71, 0x13,
            0x22, 0x32, 0x81, 0x08, 0x14, 0x42, 0x91, 0xa1, 0xb1, 0xc1, 0x09, 0x23, 0x33, 0x52, 0xf0, 0x15, 0x62, 0x72, 0xd1, 0x0a, 0x16, 0x24, 0x34, 0xe1,
            0x25, 0xf1, 0x17, 0x18, 0x19, 0x1a, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49,
            0x4a, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79,
            0x7a, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6,
            0xa7, 0xa8, 0xa9, 0xaa, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2, 0xd3,
            0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9,
            0xfa };

        public static readonly int[] QUANT_TABLE_LUM = { 16, 11, 10, 16, 24, 40, 51, 61, 12, 12, 14, 19, 26, 58, 60, 55, 14, 13, 16, 24, 40, 57, 69, 56, 14, 17, 22,
            29, 51, 87, 80, 62, 18, 22, 37, 56, 68, 109, 103, 77, 24, 35, 55, 64, 81, 104, 113, 92, 49, 64, 78, 87, 103, 121, 120, 101, 72, 92, 95, 98, 112,
            100, 103, 99 };

        public static readonly int[] QUANT_TABLE_CHROMA = { 17, 18, 24, 47, 99, 99, 99, 99, 18, 21, 26, 66, 99, 99, 99, 99, 24, 26, 56, 99, 99, 99, 99, 99, 47, 66,
            99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99,
            99 };

        private static readonly int[] ZigZagSequence = { 0, 1, 5, 6, 14, 15, 27, 28, 2, 4, 7, 13, 16, 26, 29, 42, 3, 8, 12, 17, 25, 30, 41, 43, 9, 11, 18, 24, 31, 40,
            44, 53, 10, 19, 23, 32, 39, 45, 52, 54, 20, 22, 33, 38, 46, 51, 55, 60, 21, 34, 37, 47, 50, 56, 59, 61, 35, 36, 48, 49, 57, 58, 62, 63 };


        public static float[] AdjustQuantTable(int[] quantTable, int qualityFactor)
        {
            float scale = (200 - qualityFactor * 2) * 0.01f;
            float[] adjustedQuantTable = new float[quantTable.Length];
            for (int i = 0; i < adjustedQuantTable.Length; ++i)
                adjustedQuantTable[i] = Math.Max(1f, Math.Min(quantTable[i] * scale, 255f));
            return adjustedQuantTable;
        }

        private const int BlockWidth = 8;
        private const int BlockHeight = 8;
        private const int BlockSize = BlockWidth * BlockHeight;

        private static readonly HuffmanTable Chroma_DC_Huffman = new HuffmanTable(HUFF_DC_CHROMA_BITS, HUFF_DC_CHROMA_VALS);
        private static readonly HuffmanTable Chroma_AC_Huffman = new HuffmanTable(HUFF_AC_CHROMA_BITS, HUFF_AC_CHROMA_VALS);
        private static readonly HuffmanTable Luma_DC_Huffman = new HuffmanTable(HUFF_DC_LUMA_BITS, HUFF_DC_LUMA_VALS);
        private static readonly HuffmanTable Luma_AC_Huffman = new HuffmanTable(HUFF_AC_LUMA_BITS, HUFF_AC_LUMA_VALS);

        private const int ALPHA = 0;
        private const int RED = 1;
        private const int GREEN = 2;
        private const int BLUE = 3;

        class Context
        {
            public class Component
            {
                internal readonly CompressionType type;
                internal readonly int reads;
                internal readonly int[] pixels;
                internal readonly bool upsampleRequired = false;
                internal int dc = 0;
                internal int pixelWriteIdx = 0;

                internal Component(CompressionType type, int reads = 1, bool upsample = false)
                {
                    this.type = type;
                    this.reads = reads;
                    this.upsampleRequired = upsample;
                    this.pixels = new int[reads * (upsample ? 4 : 1) * BlockSize];
                }
            }

            internal delegate void PixelComposition();

            internal JpgType type;

            internal byte[] image;
            internal int widthInPixel; // pixel
            internal int heightInPixel; // pixel

            internal readonly int[] decoding = new int[BlockSize];
            internal readonly int[] block = new int[BlockSize];

            internal int[] defaultValues;

            internal float[][] quantTables;
            internal Component[] components;

            internal PixelComposition pixelComposition;

            internal BitSupply jpg;
            
            internal int imageStride;
            internal int elementsToDecode;
            internal int decodeElement;
        }

        public static byte[] Decode(JpgType type, int[] defaultValues, float[][] quantTables, byte[] jpg, int imageWidth, int imageHeight)
        {
            if (defaultValues.Length != 4)
                throw new ArgumentException(nameof(defaultValues));
            if (quantTables.Length != 4)
                throw new ArgumentException(nameof(quantTables));

            var context = new Context
            {
                type = type,
                widthInPixel = imageWidth,
                heightInPixel = imageHeight,
                quantTables = quantTables,
                jpg = new BitSupply(jpg),
                image = new byte[imageWidth * imageHeight * 4],
                imageStride = imageWidth * 4
            };

            var numberOfBlocks = ((imageWidth + BlockWidth - 1) / BlockWidth) * ((imageHeight + BlockHeight - 1) / BlockHeight);

            switch (type)
            {
                case JpgType.Type1:
                    context.components = new Context.Component[]
                    {
                        new Context.Component(type: CompressionType.Luminance, reads: 4),
                        new Context.Component(type: CompressionType.Chrominance, reads: 1, upsample: true),
                        new Context.Component(type: CompressionType.Chrominance, reads: 1, upsample: true),
                        new Context.Component(type: CompressionType.Luminance, reads: 4),
                    };

                    context.elementsToDecode = numberOfBlocks / 4; //each decoding results in 4 blocks
                    context.pixelComposition = Pixel_YCbCrY_Transformation; // colorspace transformation

                    break;
                case JpgType.Type2:
                    context.components = new Context.Component[]
                    {
                        new Context.Component(type: CompressionType.Luminance, reads: 1),
                        new Context.Component(type: CompressionType.Luminance, reads: 1),
                        new Context.Component(type: CompressionType.Luminance, reads: 1),
                        new Context.Component(type: CompressionType.Luminance, reads: 1),
                    };

                    context.elementsToDecode = numberOfBlocks; //each decoding results in 1 blocks
                    context.pixelComposition = Pixel_No_Transformation; // no colorspace transformation required

                    break;
                case JpgType.Type3:
                    context.components = new Context.Component[]
                    {
                        new Context.Component(type: CompressionType.Luminance, reads: 1),
                        new Context.Component(type: CompressionType.Chrominance, reads: 1),
                        new Context.Component(type: CompressionType.Chrominance, reads: 1),
                        new Context.Component(type: CompressionType.Luminance, reads: 1),
                    };

                    context.elementsToDecode = numberOfBlocks; //each decoding results in 4 blocks
                    context.pixelComposition = Pixel_YCbCrY_Transformation; // colorspace transformation

                    break;
            }

            Decode(context);

            return context.image;
        }

        private static void Decode(Context context)
        {
            for (var element = 0; element < context.elementsToDecode; ++element)
            {
                context.decodeElement = element;
                DecodeNextElement(context);
            }
        }

        private static void DecodeNextElement(Context context)
        {
            for (int c = 0; c < context.components.Length; ++c)
            {
                var component = context.components[c];
                if (HasDefaultValue(context.defaultValues[c]))
                {
                    component.pixels.FillWithValue(context.defaultValues[c] & 0xFF);
                }
                else
                {
                    for (int b = 0; b < component.reads; ++b)
                    {
                        switch (component.type)
                        {
                            case CompressionType.Chrominance:
                                HuffmanDecoder.Decode(dc: Chroma_DC_Huffman, ac: Chroma_AC_Huffman, context.jpg, block: context.decoding, 0, BlockSize);
                                break;
                            case CompressionType.Luminance:
                                HuffmanDecoder.Decode(dc: Luma_DC_Huffman, ac: Luma_AC_Huffman, context.jpg, block: context.decoding, 0, BlockSize);
                                break;
                        }

                        // adjust dc
                        context.decoding[0] += component.dc;
                        component.dc = context.decoding[0];

                        // reverse zigzag
                        for (int p = b * BlockSize, z = 0; z < BlockSize; ++p, ++z)
                            component.pixels[p] = context.decoding[ZigZagSequence[z]];

                        // dequantization
                        for (int p = b * BlockSize, z = 0; z < BlockSize; ++p, ++z)
                            component.pixels[p] = (int)Math.Round(component.pixels[p] * context.quantTables[c][z]);

                        // inverse discrete cosine transform
                        FastIntegerIDCT.InverseDiscreteCosineTransform(component.pixels, b * BlockSize);


                        switch (component.type)
                        {
                            case CompressionType.Chrominance:
                                ShiftAndClamp(component.pixels, b * BlockSize, 0, -256, 255);
                                break;
                            case CompressionType.Luminance:
                                ShiftAndClamp(component.pixels, b * BlockSize, 128, 0, 255);
                                break;
                        }
                    }

                    // upsample
                    if (component.upsampleRequired)
                    {
                        //inplace, starts at the back
                        Sampler.Upsample(component.pixels, 0x24, 4, 4, 4, 2, component.pixels, BlockSize * 3, 0);
                        Sampler.Upsample(component.pixels, 0x20, 4, 4, 4, 2, component.pixels, BlockSize * 2, 0);
                        Sampler.Upsample(component.pixels, 0x04, 4, 4, 4, 2, component.pixels, BlockSize * 1, 0);
                        Sampler.Upsample(component.pixels, 0x00, 4, 4, 4, 2, component.pixels, BlockSize * 0, 0);
                    }
                }
            }

            //TODO write to image

        }

        private static void Pixel_YCbCrY_Transformation(int[]src, byte[]dst, int dstIdx)
        {
            int p1 = src[0];
            int p2 = src[1];
            int p3 = src[2];
            int p4 = src[3];

            int r1 = p1 - (p3 >> 1); // r1 = p1 - p3>>1
            int r2 = r1 + p3; // r2 = p1 - p3>>1 + p3
            int r3 = r1 - (p2 >> 1); // r3 = p1 - p3>>1 - p2>>1
            int r4 = r3 + p2; // r4 = p1 - p3>>1 - p2>>1 + p2

            dst[dstIdx + ALPHA] = ClampByte(p4, 0, 0xFF); // A = p4
            dst[dstIdx + RED] = ClampByte(r4, 0, 0xFF); // R = p1 - p3/2 - p2/2 + p2
            dst[dstIdx + GREEN] = ClampByte(r2, 0, 0xFF); // G = p1 - p3/2 + p3
            dst[dstIdx + BLUE] = ClampByte(r3, 0, 0xFF); // B = p1 - p3/2 - p2/2
        }

        private static void Pixel_No_Transformation(int[] src, byte[] dst, int dstIdx)
        {
            dst[dstIdx + ALPHA] = ClampByte(src[0], 0, 0xFF);
            dst[dstIdx + RED] = ClampByte(src[1], 0, 0xFF);
            dst[dstIdx + GREEN] = ClampByte(src[2], 0, 0xFF);
            dst[dstIdx + BLUE] = ClampByte(src[3], 0, 0xFF);
        }

        private static void ShiftAndClamp(int[] data, int offset, int shift, int min, int max)
        {
            for (int i = offset; i < offset + BlockSize; ++i)
                data[i] = Math.Max(min, Math.Min(max, data[i] + shift));
        }

        private static bool HasDefaultValue(int value)
        {
            return 0 <= value && value <= 0xFF;
        }

        private static byte ClampByte(int value, int min, int max)
        {
            return (byte)Math.Max(Math.Min(value, max), min);
        }

        private static void FillWithValue<T>(this T[] arr, T fillValue)
        {
            arr.FillWithValue(fillValue, 0, arr.Length);
        }

        private static void FillWithValue<T>(this T[] arr, T fillValue, int offset, int length)
        {
            var startIdx = offset;
            var endIdx = startIdx + length;
            arr[startIdx] = fillValue;
            for (int dstPos = startIdx + 1, copyLength = 1; dstPos < endIdx; dstPos += copyLength, copyLength += copyLength)
                Array.Copy(arr, startIdx, arr, dstPos, dstPos + copyLength > endIdx ? endIdx - dstPos : copyLength);
        }
    }
}
