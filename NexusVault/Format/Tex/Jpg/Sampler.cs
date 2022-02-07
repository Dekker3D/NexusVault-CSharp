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
    internal static class Sampler
    {
        public static void Upsample(int[] src, int srcOffset, int srcImageWidth, int srcImageHeight, int srcImageRowStride, int sampleScale, //
            int[] dst, int dstOffset, int dstImageRowStride)
        {

            if (srcImageRowStride < srcImageWidth)
                throw new ArgumentException();

            // final int srcLength = srcImageHeight * srcImageWidth;
            var dstImageWidth = srcImageWidth * sampleScale;
            var dstImageHeight = srcImageHeight * sampleScale;

            if (dstImageRowStride < dstImageWidth)
                throw new ArgumentException();


            // final int dstLength = dstWidth * dstHeight;
            var srcEndIdx = srcOffset + (srcImageHeight - 1) * srcImageRowStride + srcImageWidth;
            var dstEndIdx = dstOffset + (dstImageHeight - 1) * dstImageRowStride + dstImageWidth;

            if (src.Length < srcEndIdx)
                throw new ArgumentException(string.Format("src too small. Expected %d but was %d", srcEndIdx, src.Length));

            if (dst.Length < dstEndIdx)
                throw new ArgumentException(string.Format("dst too small. Expected %d but was %d", dstEndIdx, dst.Length));

            var srcStartIdx = srcEndIdx;
            var dstStartIdx = dstEndIdx;

            for (var y = 0; y < srcImageHeight; ++y)
            {
                var srcIdx = srcStartIdx;
                var dstIdx = dstStartIdx;

                for (var x = 0; x < srcImageWidth; ++x)
                {
                    var sampleValue = src[--srcIdx];
                    for (var i = 0; i < sampleScale; ++i) // fill first row with values
                        dst[--dstIdx] = sampleValue;
                }

                for (var i = 1; i < sampleScale; ++i)
                    Array.Copy(dst, dstIdx, dst, dstIdx - i * dstImageRowStride, dstImageWidth);

                srcStartIdx -= srcImageRowStride;
                dstStartIdx -= sampleScale * dstImageRowStride;
            }
        }

        public static void Downsample(int[] src, int srcOffset, int srcImageWidth, int srcImageHeight, int srcImageRowStride, int sampleScale, //
                int[] dst, int dstOffset, int dstImageRowStride)
        {

            if (srcImageRowStride < srcImageWidth)
                throw new ArgumentException();

            // final int srcLength = srcImageHeight * srcImageWidth;
            var dstImageWidth = srcImageWidth / sampleScale;
            var dstImageHeight = srcImageHeight / sampleScale;

            if (dstImageRowStride < dstImageWidth)
                throw new ArgumentException();

            // final int dstLength = dstWidth * dstHeight;
            var srcEndIdx = srcOffset + (srcImageHeight - 1) * srcImageRowStride + srcImageWidth;
            var dstEndIdx = dstOffset + (dstImageHeight - 1) * dstImageRowStride + dstImageWidth;

            if (src.Length < srcEndIdx)
                throw new ArgumentException(string.Format("src too small. Expected %d but was %d", srcEndIdx, src.Length));

            if (dst.Length < dstEndIdx)
                throw new ArgumentException(string.Format("dst too small. Expected %d but was %d", dstEndIdx, dst.Length));


            double downsampleFactor = 1.0d / (sampleScale * sampleScale);
            var srcStartIdx = srcOffset;
            var dstStartIdx = dstOffset;

            for (var y = 0; y < dstImageHeight; ++y)
            {
                var srcIdx = srcStartIdx;
                var dstIdx = dstStartIdx;

                for (var x = 0; x < dstImageWidth; ++x)
                {
                    long sum = 0;
                    for (var i = 0; i < sampleScale; ++i)
                        for (var j = 0; j < sampleScale; ++j)
                            sum += src[srcIdx + j + i * srcImageRowStride];

                    srcIdx += sampleScale;
                    dst[dstIdx++] = (int)(sum * downsampleFactor);
                }

                dstStartIdx += dstImageRowStride;
                srcStartIdx += sampleScale * srcImageRowStride;
            }
        }
    }
}
