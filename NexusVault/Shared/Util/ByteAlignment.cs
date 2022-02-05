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

using System.IO;

namespace NexusVault.Shared.Util
{
    public static class ByteAlignment
    {
        public static long AlignTo16Byte(long position)
        {
            return position + 15 & 0xFFFFFFFFFFFFF0L;
        }

        public static long AdvanceByToAlign16Byte(long position)
        {
            return AlignTo16Byte(position) - position;
        }

        public static int AlignTo16Byte(int position)
        {
            return position + 15 & 0xFFFFF0;
        }

        public static int AdvanceByToAlign16Byte(int position)
        {
            return AlignTo16Byte(position) - position;
        }

        public static long AlignTo8Byte(long position)
        {
            return position + 7 & 0xFFFFFFFFFFFFF8L;
        }

        public static long AdvanceByToAlign8Byte(long position)
        {
            return AlignTo8Byte(position) - position;
        }

        public static int AlignTo8Byte(int position)
        {
            return position + 7 & 0xFFFFF8;
        }

        public static int AdvanceByToAlign8Byte(int position)
        {
            return AlignTo8Byte(position) - position;
        }

        public static void AlignTo16Byte(BinaryReader reader)
        {
            reader.BaseStream.Position = AdvanceByToAlign16Byte(reader.BaseStream.Position);
        }

        public static void AlignTo8Byte(BinaryReader reader)
        {
            reader.BaseStream.Position = AdvanceByToAlign8Byte(reader.BaseStream.Position);
        }

        public static void AlignTo16Byte(BinaryWriter writer)
        {
            writer.BaseStream.Position = AdvanceByToAlign16Byte(writer.BaseStream.Position);
        }

        public static void AlignTo8Byte(BinaryWriter writer)
        {
            writer.BaseStream.Position = AdvanceByToAlign8Byte(writer.BaseStream.Position);
        }
    }
}
