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

namespace NexusVault.Shared.Exception
{
    public sealed class SignatureMismatchException : NexusVaultException
    {
        public int ExpectedSignature { get; }
        public int ActualSignature { get; }

        public SignatureMismatchException(string message, int expectedSignature, int actualSignature) : base($"{message} : Expected signature '{ToString(expectedSignature)}', but was '{ToString(actualSignature)}'.")
        {
            ExpectedSignature = expectedSignature;
            ActualSignature = actualSignature;
        }

        private static object ToString(int signature)
        {
            var a = (char)(signature >> 0x18 & 0xFF);
            var b = (char)(signature >> 0x10 & 0xFF);
            var c = (char)(signature >> 0x08 & 0xFF);
            var d = (char)(signature >> 0x00 & 0xFF);
            return $"{a}{b}{c}{d}";
        }
    }
}
