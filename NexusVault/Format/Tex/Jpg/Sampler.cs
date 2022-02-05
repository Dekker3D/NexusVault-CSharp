using System;
using System.Collections.Generic;
using System.Text;

namespace NexusVault.tex
{
    static class Sampler
    {

        /**
	    *
	    * @param src
	    *            image array, can contain multiple images
	    * @param srcOffset
	    *            offset at which the image starts
	    * @param srcImageHeight
	    *            height of the image
	    * @param srcImageWidth
	    *            width of the image
	    * @param srcImageRowInterleave
	    *            in case of interleaved images the number of indices to skip to reach the next row of the image
	    * @param sampleScale
	    *            only supports quadratic upsampling. 2 means 2x2, 3 means 3x3, etc
	    * @param dst
	    *            target array, needs to be big enough to hold the upsampled image.
	    * @param dstOffset
	    *            offet at whicht the image should start
	    * @param dstImageRowInterleave
	    *            in case of interleaved images the number of indices to skip to reach the next row of the image
	    */
        public static void Upsample(int[] src, int srcOffset, int srcImageWidth, int srcImageHeight, int srcImageRowInterleave, int sampleScale, //
                int[] dst, int dstOffset, int dstImageRowInterleave)
        {

            // final int srcLength = srcImageHeight * srcImageWidth;
            int dstWidth = srcImageWidth * sampleScale;
            int dstHeight = srcImageWidth * sampleScale;
            // final int dstLength = dstWidth * dstHeight;
            int srcEndIdx = srcOffset + srcImageWidth + (srcImageHeight - 1) * (srcImageRowInterleave + srcImageWidth);
            int dstEndIdx = dstOffset + dstWidth + (dstHeight - 1) * (dstImageRowInterleave + dstWidth);

            if (src.Length < srcEndIdx)
            {
                throw new IndexOutOfRangeException(string.Format("src too small. Expected %d but was %d", srcEndIdx, src.Length));
            }
            if (dst.Length < dstEndIdx)
            {
                throw new IndexOutOfRangeException(string.Format("dst too small. Expected %d but was %d", dstEndIdx, dst.Length));
            }

            int srcWPos = srcImageWidth;
            int srcStartIdx = srcEndIdx - 1;
            int dstStartIdx = dstEndIdx - 1;
            int srcIdx = srcStartIdx;
            int dstIdx = dstStartIdx;
            for (; srcIdx >= srcOffset; --srcIdx)
            {
                int sampleValue = src[srcIdx];

                for (int i = 0; i < sampleScale; ++i)
                { // fill first row with values
                    dst[dstIdx--] = sampleValue;
                }
                for (int i = 1; i < sampleScale; ++i)
                { // copy row to the rows above
                    Array.Copy(dst, dstIdx + 1, dst, dstIdx + 1 - i * (dstWidth + dstImageRowInterleave), sampleScale);
                }

                if (--srcWPos == 0)
                { // move to next row
                    srcWPos = srcImageWidth;
                    srcStartIdx -= (srcImageWidth + srcImageRowInterleave);
                    srcIdx = srcStartIdx + 1;
                    dstStartIdx -= sampleScale * (dstWidth + dstImageRowInterleave);
                    dstIdx = dstStartIdx;
                }
            }
        }

    }
}
