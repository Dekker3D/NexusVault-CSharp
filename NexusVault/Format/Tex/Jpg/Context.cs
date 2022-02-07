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

namespace NexusVault.Format.Tex.Jpg
{

    internal sealed class Component
    {
        public int Index { get; }
        public CompressionType CompressionType { get; }
        public int[] Pixels { get; }
        public int DC { get; set; } = 0;

        internal Component(int index, CompressionType compressionType, int[] pixels)
        {
            Index = index;
            CompressionType = compressionType;
            Pixels = pixels;
        }
    }

    internal sealed class Context
    {
        public int[] Uncompressed { get; } = new int[Constants.BLOCK_SIZE];
        public int[] Block { get; } = new int[Constants.BLOCK_SIZE];

        public JpgType JpgType { get; set; }

        public int[] DefaultValues { get; set; }
        public float[][] QuantTables { get; set; }

        public int ComponentsWidth { get; }
        public int ComponentsHeight { get; }
        public Component[] Components { get; } = new Component[4];

        public BitSupply JpgInput { get; set; }
        public BitConsumer JpgOutput { get; set; }

        public int NumberOfBlocks { get; }
        public int BlocksPerRow { get; }

        public int ImageWidth { get; }
        public int ImageHeight { get; }
        public byte[] Image { get; }

        public Context(JpgType jpgType, int width, int height)
        {
            JpgType = jpgType;

            ImageWidth = width;
            ImageHeight = height;
            Image = new byte[ImageWidth * ImageHeight * 4];

            ComponentsWidth = ImageWidth + 7 & 0xFFFFF8; // multiple of 8
            ComponentsHeight = ImageHeight + 7 & 0xFFFFF8; // multiple of 8
            NumberOfBlocks = ComponentsWidth * ComponentsHeight / Constants.BLOCK_SIZE;
            BlocksPerRow = ComponentsWidth / Constants.BLOCK_WIDTH;
        }

        public void SetComponentTypes(CompressionType component0, CompressionType component1, CompressionType component2, CompressionType component3)
        {
            Components[0] = new Component(0, component0, new int[ComponentsWidth * ComponentsHeight]);
            Components[1] = new Component(1, component1, new int[ComponentsWidth * ComponentsHeight]);
            Components[2] = new Component(2, component2, new int[ComponentsWidth * ComponentsHeight]);
            Components[3] = new Component(3, component3, new int[ComponentsWidth * ComponentsHeight]);
        }

    }
}