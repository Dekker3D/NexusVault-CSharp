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
using System.IO;
using System.Runtime.InteropServices;

namespace NexusVault.Format.Tex
{
    [StructLayout(LayoutKind.Sequential, Size = 0x70, Pack = 1)]
    public sealed class FileHeader
    {
        public const int Signature = 'G' << 16 | 'F' << 8 | 'X';
        public const int SizeOf = 0x70;

        public int signature; // " GFX"
        public int version;
        public int width;
        public int height;
        public int depth;
        public int sides;
        public int textureCount; // max 13
        public int format; // argb(0), argb(1), rgb(5), grayscale(6), dxt1(13), dxt3(14), dxt5(15), not used for jpg
        [MarshalAs(UnmanagedType.Bool)]
        public bool isJpg;
        public int jpgType; // 0, 1, 2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public JpgChannel[] jpgChannel;
        public int imageSizsCount; // max 13
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public int[] imageSizes;
        private int padding;

        public FileHeader() {
            jpgChannel = new JpgChannel[4];
            for (var i = 0; i < jpgChannel.Length; ++i)
                jpgChannel[i] = new JpgChannel();

            imageSizes = new int[13];
        }

        public FileHeader(BinaryReader reader) : this()
        {
            Read(reader);
        }

        public TextureType TextureType
        {
            get
            {
                if (isJpg)
                {
                    switch (jpgType)
                    {
                        case 0: return TextureType.Jpg1;
                        case 1: return TextureType.Jpg2;
                        case 2: return TextureType.Jpg3;
                    }
                }
                else
                {
                    switch (format){
                        case 0: return TextureType.ARGB1;
                        case 1: return TextureType.ARGB2;
                        case 5: return TextureType.RGB;
                        case 6: return TextureType.Grayscale;
                        case 13: return TextureType.Dxt1;
                        case 14: return TextureType.Dxt3;
                        case 15: return TextureType.Dxt5;
                    }
                }

                return TextureType.Unknown;
            }
            set
            {
                if (value == TextureType.Unknown)
                    throw new ArgumentException($"{value} is invalid.",nameof(TextureType));

                format = 0;
                isJpg = false;
                jpgType = 0;

                switch (value)
                {
                    case TextureType.Jpg1:
                        isJpg = true;
                        jpgType = 0;
                        break;
                    case TextureType.Jpg2:
                        isJpg = true;
                        jpgType = 1;
                        break;
                    case TextureType.Jpg3:
                        isJpg = true;
                        jpgType = 2;
                        break;
                    case TextureType.ARGB1:
                        format = 0;
                        break;
                    case TextureType.ARGB2:
                        format = 1;
                        break;
                    case TextureType.RGB:
                        format = 5;
                        break;
                    case TextureType.Grayscale:
                        format = 6;
                        break;
                    case TextureType.Dxt1:
                        format = 13;
                        break;
                    case TextureType.Dxt3:
                        format = 14;
                        break;
                    case TextureType.Dxt5:
                        format = 15;
                        break;
                }
            }
        }

        public void Read(BinaryReader reader)
        {
            signature = reader.ReadInt32();
            version = reader.ReadInt32();
            width = reader.ReadInt32();
            height = reader.ReadInt32();
            depth = reader.ReadInt32();
            sides = reader.ReadInt32();
            textureCount = reader.ReadInt32();
            format = reader.ReadInt32();
            isJpg = reader.ReadInt32() != 0;
            jpgType = reader.ReadInt32();
           
            for (var i = 0; i < jpgChannel.Length; ++i)
                jpgChannel[i].Read(reader);

            imageSizsCount = reader.ReadInt32();           
            for (var i = 0; i < imageSizes.Length; ++i)
                imageSizes[i] = reader.ReadInt32();

            padding = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(signature);
            writer.Write(version);
            writer.Write(width);
            writer.Write(height);
            writer.Write(depth);
            writer.Write(sides);
            writer.Write(textureCount);
            writer.Write(format);
            writer.Write((int)(isJpg ? 1 : 0));
            writer.Write(jpgType);

            for (var i = 0; i < jpgChannel.Length; ++i)
                jpgChannel[i].Write(writer);

            writer.Write(imageSizsCount);
            for (var i = 0; i < imageSizes.Length; ++i)
                 writer.Write(imageSizes[i]);

            writer.Write(padding);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 3, Pack = 1)]
    public sealed class JpgChannel
    {
        public const int SizeOf = 0x3;

        public byte quality;
        public bool hasColor;
        public byte color;

        public JpgChannel() { }

        public JpgChannel(BinaryReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            quality = reader.ReadByte();
            hasColor = reader.ReadByte() != 0;
            color = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(quality);
            writer.Write((byte)(hasColor ? 1 : 0));
            writer.Write(color);
        }
    }
}
