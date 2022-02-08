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
    internal static class FastDCT
    {
        private static readonly double[] coefficients = new double[64];
        private const int N = 8;

        private const double CO_SCALE = N + N;
        private const double DCT_SCALE = 1d / 4d;

        static FastDCT()
        {
            for (var k = 0; k < N; ++k)
                for (var n = 0; n < N; ++n)
                    coefficients[n + k * N] = Math.Cos(Math.PI * (2 * n + 1) * k / CO_SCALE);
        }

        public static void DiscreteCosineTransform(int[] data, int offset, int rowStride)
        {
            var buffer = new int[N * N];

            // TODO this can still be optimized

            for (var k1 = 0; k1 < N; ++k1)
            {
                for (var k2 = 0; k2 < N; ++k2)
                {
                    double sum = 0;
                    for (var y2 = 0; y2 < N; ++y2)                    
                        for (var x2 = 0; x2 < N; ++x2)                        
                            sum += data[y2 * rowStride + x2 + offset] * coefficients[y2 + k1 * N] * coefficients[x2 + k2 * N];                        
                    buffer[k1 * N + k2] = (int)Math.Round(DCT_SCALE * sum);
                }
            }

            for (var i = 0; i < N; ++i)
                Array.Copy(buffer, i * N, data, offset + i * rowStride, N);

        }
    }
}
