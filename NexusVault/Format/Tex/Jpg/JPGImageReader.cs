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

using NexusVault.Shared.Exception;
using NexusVault.Format.Tex.Jpg.Extension;
using System;
using NexusVault.Shared.Util;

namespace NexusVault.Format.Tex.Jpg {
    public static class JPGImageReader {

        public static Image Decompress(FileHeader header, byte[] jpgImages, int offset, int mipMap) {
            if (header.signature != FileHeader.Signature) 
                throw new SignatureMismatchException("tex", FileHeader.Signature, header.signature);            

            if (header.version != 3) 
                throw new VersionMismatchException("tex", 3, header.version);            

            var length = header.imageSizes[mipMap];
            var arrayOffset = offset;
            for (int i = 0; i < mipMap; ++i) 
                arrayOffset += header.imageSizes[i];            

            var width = header.width >> header.textureCount - mipMap - 1;
            var height = header.height >> header.textureCount - mipMap - 1;
            width = width <= 0 ? 1 : width;
            height = height <= 0 ? 1 : height;

            var buffer = new byte[length];
            Array.Copy(jpgImages, arrayOffset, buffer, 0, buffer.Length);
            return Decompress(header.GetJpgType(), header.GetDefaultColors(), header.GetQuantTables(), buffer, width, height);
        }

        private static Image Decompress(JpgType jpgType, int[] defaultColors, float[][] quantTables, byte[] jpg, int width, int height) {
            var decodedImage = DecompressToARGB(jpgType, defaultColors, quantTables, jpg, width, height);
            return new Image(width, height, ImageFormat.ARGB, decodedImage);
        }

        public static byte[] DecompressToARGB(JpgType type, int[] defaultColors, float[][] quantTables, byte[] jpg, int width, int height) {
            if (defaultColors.Length != 4) 
                throw new ArgumentException(nameof(defaultColors));
            
            if (quantTables.Length != 4) 
                throw new ArgumentException(nameof(quantTables));


            var context = new Context(type, width, height)
            {
                DefaultValues = defaultColors,
                QuantTables = quantTables,
                JpgInput = new BitSupply(jpg)
            };

            switch (type) {
                case JpgType.Type1:
                    context.SetComponentTypes(CompressionType.Luminance, CompressionType.Chrominance, CompressionType.Chrominance, CompressionType.Luminance);
                    break;
                case JpgType.Type2:
                    context.SetComponentTypes(CompressionType.Luminance, CompressionType.Luminance, CompressionType.Luminance, CompressionType.Luminance);
                    break;
                case JpgType.Type3:
                    context.SetComponentTypes(CompressionType.Luminance, CompressionType.Chrominance, CompressionType.Chrominance, CompressionType.Luminance);
                    break;
            }

            for (var c = 0; c < 4; ++c) 
                if (context.DefaultValues[c] > -1) 
                    context.Components[c].Pixels.FillWithValue(context.DefaultValues[c]);

            switch (context.JpgType) {
                case JpgType.Type1:
                    DecodeType1(context);
                    break;
                case JpgType.Type2:
                case JpgType.Type3:
                    DecodeType2Or3(context);
                    break;
            }

            return context.Image;
        }

        private static void DecodeType1(Context context) {
            var offset = 0;
            var column = 0;
            for (int i = 0; i < context.NumberOfBlocks / 4; ++i) {
                if (column == context.BlocksPerRow) {
                    offset += context.ComponentsWidth * (Constants.BLOCK_HEIGHT * 2 - 1);
                    column = 0;
                }

                var offset1 = offset;
                var offset2 = offset1 + Constants.BLOCK_WIDTH;
                var offset3 = offset + context.ComponentsWidth * Constants.BLOCK_HEIGHT;
                var offset4 = offset3 + Constants.BLOCK_WIDTH;

                if (context.DefaultValues[0] <= -1) {
                    var component = context.Components[0];
                    DecompressSingleBlockA(context, component, offset1);
                    DecompressSingleBlockA(context, component, offset2);
                    DecompressSingleBlockA(context, component, offset3);
                    DecompressSingleBlockA(context, component, offset4);
                }

                if (context.DefaultValues[1] <= -1) {
                    var component = context.Components[1];
                    DecompressSingleBlockB(context, component, offset1);
                }

                if (context.DefaultValues[2] <= -1) {
                    var component = context.Components[2];
                    DecompressSingleBlockB(context, component, offset1);
                }

                if (context.DefaultValues[3] <= -1) {
                    var component = context.Components[3];
                    DecompressSingleBlockA(context, component, offset1);
                    DecompressSingleBlockA(context, component, offset2);
                    DecompressSingleBlockA(context, component, offset3);
                    DecompressSingleBlockA(context, component, offset4);
                }

                Composite(context, offset1);
                Composite(context, offset2);
                Composite(context, offset3);
                Composite(context, offset4);

                offset += Constants.BLOCK_WIDTH * 2;
                column += 2;
            }
        }

        private static void DecodeType2Or3(Context context) {
            var offset = 0;
            var column = 0;
            for (var i = 0; i < context.NumberOfBlocks; ++i) {
                if (column == context.BlocksPerRow) { // move write to next line of blocks
                    offset += context.ComponentsWidth * (Constants.BLOCK_HEIGHT - 1);
                    column = 0;
                }

                for (var c = 0; c < 4; ++c) {
                    if (context.DefaultValues[c] > -1) 
                        continue;                    

                    var component = context.Components[c];
                    DecompressSingleBlockA(context, component, offset);
                }

                Composite(context, offset);

                offset += Constants.BLOCK_WIDTH;
                column += 1;
            }
        }

        private static void DecompressSingleBlockA(Context context, Component component, int offset) {
            DecodeToBlock(context, component);
            CopyBlockToPixelBuffer(context, component, offset);
            Dequantizate(context, component, offset);
            Idct(context, component, offset);
            ShiftAndClamp(context, component, offset);
        }

        private static void DecompressSingleBlockB(Context context, Component component, int offset) {
            DecodeToBlock(context, component);
            CopyBlockToPixelBuffer(context, component, offset);
            Dequantizate(context, component, offset);
            Idct(context, component, offset);
            ShiftAndClamp(context, component, offset);
            Upscale(context, component, offset);
        }

        private static void DecodeToBlock(Context context, Component component) {
            switch (component.CompressionType) {
                case CompressionType.Chrominance:
                    Huffman.Decode(Constants.HUFFMAN_CHROMA_DC, Constants.HUFFMAN_CHROMA_AC, context.JpgInput, context.Uncompressed, 0,
                            Constants.BLOCK_SIZE);
                    break;
                case CompressionType.Luminance:
                    Huffman.Decode(Constants.HUFFMAN_LUMA_DC, Constants.HUFFMAN_LUMA_AC, context.JpgInput, context.Uncompressed, 0, Constants.BLOCK_SIZE);
                    break;
            }

            // adjust dc
            context.Uncompressed[0] += component.DC;
            component.DC = context.Uncompressed[0];

            // reverse zigzag
            for (int n = 0; n < Constants.BLOCK_SIZE; ++n) 
                context.Block[n] = context.Uncompressed[Constants.ZIGZAG_SEQUENCE[n]];            
        }

        private static void CopyBlockToPixelBuffer(Context context, Component component, int dstOffset) {
            for (var y = 0; y < Constants.BLOCK_HEIGHT; ++y) {
                var dstStart = dstOffset + context.ComponentsWidth * y;
                Array.Copy(context.Block, Constants.BLOCK_WIDTH * y, component.Pixels, dstStart, Constants.BLOCK_WIDTH);
            }
        }

        private static void Dequantizate(Context context, Component component, int offset) {
            for (int y = 0, q = 0; y < Constants.BLOCK_HEIGHT; ++y) {
                for (var x = offset; x < offset + Constants.BLOCK_WIDTH; ++x) 
                    component.Pixels[x] = (int) (component.Pixels[x] * context.QuantTables[component.Index][q++]);                
                offset += context.ComponentsWidth;
            }
        }

        private static void Idct(Context context, Component component, int offset) {
            FastIntegerIDCT.InverseDiscreteCosineTransform(component.Pixels, offset, context.ComponentsWidth);
        }

        private static void ShiftAndClamp(Context context, Component component, int offset) {
            switch (component.CompressionType) {
                case  CompressionType.Chrominance:
                    ShiftAndClamp(component.Pixels, offset, context.ComponentsWidth, 0, -256, 255);
                    break;
                case CompressionType.Luminance:
                    ShiftAndClamp(component.Pixels, offset, context.ComponentsWidth, 128, 0, 255);
                    break;
            }
        }

        private static void ShiftAndClamp(int[] data, int offset, int rowStride, int shift, int min, int max) {
            for (var y = 0; y < Constants.BLOCK_HEIGHT; ++y, offset += rowStride) 
                for (var x = offset; x < offset + Constants.BLOCK_WIDTH; ++x) 
                    data[x] = Math.Max(min, Math.Min(max, data[x] + shift));               
        }

        private static void Upscale(Context context, Component component, int offset) {
            Sampler.Upsample(component.Pixels, offset, Constants.BLOCK_WIDTH, Constants.BLOCK_HEIGHT, context.ComponentsWidth, 2, component.Pixels, offset,
                    context.ComponentsWidth);
        }

        private static void Composite(Context context, int offset) {
            for (int y = 0; y < Constants.BLOCK_HEIGHT; ++y, offset += context.ComponentsWidth) {
                if (offset > context.Image.Length)
                    break;
                
                for (int x = offset, p = offset * 4; x < offset + Constants.BLOCK_WIDTH; ++x, p += 4) {
                    if (context.ImageWidth <= x % context.ComponentsWidth) 
                        break;
                    
                    switch (context.JpgType) {
                        case JpgType.Type2:
                            context.Image[p + 0] = (byte)MathHelper.Clamp(context.Components[0].Pixels[x], 0, 0xFF);
                            context.Image[p + 1] = (byte)MathHelper.Clamp(context.Components[1].Pixels[x], 0, 0xFF);
                            context.Image[p + 2] = (byte)MathHelper.Clamp(context.Components[2].Pixels[x], 0, 0xFF);
                            context.Image[p + 3] = (byte)MathHelper.Clamp(context.Components[3].Pixels[x], 0, 0xFF);
                            break;
                        case JpgType.Type1:
                        case JpgType.Type3:
                            int p1 = context.Components[0].Pixels[x];
                            int p2 = context.Components[1].Pixels[x];
                            int p3 = context.Components[2].Pixels[x];
                            int p4 = context.Components[3].Pixels[x];

                            int r1 = p1 - (p3 >> 1); // r1 = p1 - p3>>1
                            int r2 = MathHelper.Clamp(r1 + p3, 0, 0xFF); // r2 = p1 - p3>>1 + p3
                            int r3 = MathHelper.Clamp(r1 - (p2 >> 1), 0, 0xFF); // r3 = p1 - p3>>1 - p2>>1
                            int r4 = MathHelper.Clamp(r3 + p2, 0, 0xFF); // r4 = p1 - p3>>1 - p2>>1 + p2

                            context.Image[p + 0] = (byte)MathHelper.Clamp(p4, 0, 0xFF); // A = p4
                            context.Image[p + 1] = (byte)MathHelper.Clamp(r4, 0, 0xFF); // R = p1 - p3/2 - p2/2 + p2
                            context.Image[p + 2] = (byte)MathHelper.Clamp(r2, 0, 0xFF); // G = p1 - p3/2 + p3
                            context.Image[p + 3] = (byte)MathHelper.Clamp(r3, 0, 0xFF); // B = p1 - p3/2 - p2/2
                            break;
                    }
                }
            }
        }
    }
}