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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NexusVault.Format.Tex.Jpg
{
    public sealed class HuffmanTable
    {
        private sealed class HuffmanKey : IComparable<HuffmanTable.HuffmanKey>
        {
            public int Bits { get; }
            public int Length { get; }

            public HuffmanKey(int bits, int length)
            {
                this.Bits = bits;
                this.Length = length;
            }

            public int CompareTo(HuffmanKey other)
            {
                if (Length != other.Length)
                    return Length - other.Length;
                else
                    return Bits - other.Bits;
            }

            public bool IsMatch(int bits, int length)
            {
                return Bits == bits && Length == length;
            }

            public bool IsMatch(HuffmanKey key)
            {
                return IsMatch(key.Bits, key.Length);
            }


            override public bool Equals(Object o)
            {
                if (o == this)
                {
                    return true;
                }
                if (o is HuffmanKey key)
                {
                    return IsMatch(key);
                }
                return false;
            }

            override public int GetHashCode()
            {
                const int prime = 31;
                int result = 1;
                result = prime * result + Bits;
                result = prime * result + Length;
                return result;
            }
        }

        public sealed class HuffmanValue
        {
            /** number of used bits in the encodedWord, starting with the lsb */
            public int EncodedWordBitLength { get; }
            public int EncodedWord { get; }

            /** Only the 16 least significant bits are used */
            public int DecodedWord { get; }

            public HuffmanValue(int encodedWord, int encodedWordBitLength, int decodedWord)
            {
                EncodedWordBitLength = encodedWordBitLength;
                EncodedWord = encodedWord;
                DecodedWord = decodedWord;
            }
        }

        private readonly IDictionary<HuffmanKey, HuffmanValue> decodeMapping = new SortedDictionary<HuffmanKey, HuffmanValue>();
        private readonly IDictionary<HuffmanKey, HuffmanValue> encodeMapping = new SortedDictionary<HuffmanKey, HuffmanValue>();
        private readonly byte[][] codes = new byte[16][];

        public HuffmanTable(byte[] numberOfValues, byte[] values)
        {
            // sort values in categories
            int pos = 0;
            for (int i = 0; i < numberOfValues.Length; ++i)
            {
                codes[i] = new byte[numberOfValues[i]];
                for (int j = 0; j < numberOfValues[i]; ++j)
                    codes[i][j] = values[pos++];
            }

            BuildHuffmanTree();
        }

        private void BuildHuffmanTree()
        {
            //Decoding
            int encodedWord = 0;
            for (int idx = 0; idx < codes.Length; ++idx)
            {
                int encodedLength = idx + 1;
                if (codes[idx].Length > 1 << idx + 1)
                    throw new ArgumentException($"Code error. Step {idx + 1} contains {codes[idx].Length} words. With a bit length of {encodedLength} only {1 << encodedLength} words are supported.");

                for (int nIdx = 0; nIdx < codes[idx].Length; ++nIdx)
                {
                    int decodedWord = codes[idx][nIdx];
                    var key = new HuffmanKey(encodedWord, encodedLength);
                    decodeMapping.Add(key, new HuffmanValue(encodedWord, encodedLength, decodedWord));
                    encodedWord += 1;
                }
                encodedWord <<= 1;
            }

            //Encoding
            foreach (var value in decodeMapping.Values)
                encodeMapping.Add(new HuffmanKey(value.DecodedWord, 32), value);
        }

        public bool TryDecode(int encodedWord, int bitLength, out HuffmanValue value)
        {
            return decodeMapping.TryGetValue(new HuffmanKey(encodedWord, bitLength), out value);
        }

        public bool TryEncode(int decodedWord, out HuffmanValue value)
        {
            return decodeMapping.TryGetValue(new HuffmanKey(decodedWord, 32), out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bits">length of the word. Given in the number of used bits</param>
        /// <returns>true when there is at least one decoding available</returns>
        public bool HasDecodingForWordOfLength(int bits)
        {
            if (bits < 0 || codes.Length < bits)
                return false;
            return codes[bits - 1].Length != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> minimal number of bits needed for decoding</returns>
        public int GetDecodeMinLength()
        {
            for (int i = 0; i < codes.Length; ++i)
                if (codes[i].Length != 0)
                    return i + 1;
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>maximal number of bits which can be used for decoding</returns>
        public int GetDecodeMaxLength()
        {
            for (int i = codes.Length - 1; 0 <= i; --i)
                if (codes[i].Length != 0)
                    return i + 1;
            return 0;
        }

        public string GetDecodingAsFormatedString()
        {
            var builder = new StringBuilder();
            builder.Append("Huffman Table: Total number of codes: ").Append(decodeMapping.Count).Append("\n");
            var strPadding = "%" + decodeMapping.Count + "s";

            var keys = new List<HuffmanKey>(decodeMapping.Keys);
            keys.Sort();

            foreach (var key in keys)
            {
                var value = decodeMapping[key];
                var binaryRepresentation = string.Format($"%{key.Length}s", Convert.ToString(key.Bits, 2));
                var padded = string.Format(strPadding, binaryRepresentation);
                var printable = string.Format("Key(%02d) %s -> Value: 0x%02X", key.Length, padded, value.DecodedWord);
                builder.Append(printable).Append("\n");
            }

            return builder.ToString();
        }
    }

    internal sealed class BitQueue
    {
        private ulong _bitQueue;

        public int Position { get; private set; }
        public int RemainingCapacity { get { return Huffman.BitsPerLong - Position; } }

        public int Pop(int requestedBits)
        {
            if (requestedBits < 0 || Huffman.BitsPerInt < requestedBits)
                throw new ArgumentException($"Can't pop less than 0 or more than {Huffman.BitsPerInt} bits.", nameof(requestedBits));
            if (Position < requestedBits)
                throw new ArgumentException($"Queue contains {Position} bits. Unable to pop {requestedBits} bits.");

            if (requestedBits == 0)
                return 0;

            int result = (int)(_bitQueue >> (Huffman.BitsPerLong - requestedBits));
            _bitQueue <<= requestedBits;
            Position -= requestedBits;
            return result;
        }

        public void Push(uint data, int lengthInBits)
        {
            Push((int)data, lengthInBits);
        }

        public void Push(int data, int lengthInBits)
        {
            if (lengthInBits < 0 || Huffman.BitsPerInt < lengthInBits)
                throw new ArgumentException($"Can't push less than 0 or more than {Huffman.BitsPerInt} bits.", nameof(lengthInBits));
            if (RemainingCapacity < lengthInBits)
                throw new IndexOutOfRangeException($"Queue can only store {RemainingCapacity} more bits. Unable to push {lengthInBits} bits.");

            if (lengthInBits == 0)
                return;

            uint mask = 0xFFFFFFFF >> (Huffman.BitsPerInt - lengthInBits);
            ulong alignedData = (ulong)(data & mask) << (RemainingCapacity - lengthInBits);
            _bitQueue |= alignedData;
            Position += lengthInBits;
        }

        public void Clear()
        {
            Position = 0;
            _bitQueue = 0;
        }

        override public string ToString()
        {
            var bin = Convert.ToString((long)_bitQueue, 2);
            while (bin.Length < 64)
                bin = "0" + bin;
            return $"[BitQueue: {bin.Substring(0, Position) + "|" + bin.Substring(Position)}]";
        }
    }

    internal sealed class BitSupply
    {
        private readonly BitQueue _queue = new BitQueue();
        private readonly byte[] _data;
        private int _index;

        public int RemainingBits
        {
            get => _queue.Position + (_data.Length - _index) * Huffman.BitsPerByte;
        }

        public BitSupply(byte[] data)
        {
            this._data = data;
        }

        public bool CanSupply(int requestedBits)
        {
            return requestedBits <= RemainingBits;
        }

        public int Supply(int requestedBits)
        {
            if (requestedBits > Huffman.BitsPerInt)
                throw new ArgumentException(nameof(requestedBits));

            if (_queue.Position < requestedBits)
            {
                var dif = requestedBits - _queue.Position;
                if (dif > _queue.RemainingCapacity)
                    throw new IndexOutOfRangeException();

                while (dif > 0)
                {
                    _queue.Push(_data[_index++] & 0xFF, Huffman.BitsPerByte);
                    dif -= Huffman.BitsPerByte;
                }
            }
            return _queue.Pop(requestedBits);
        }

        override public string ToString()
        {
            return $"[BitSupply: Remaining bits={RemainingBits} Queue: {_queue}]";
        }
    }

    internal sealed class BitConsumer
    {
        private readonly BitQueue _queue = new BitQueue();
        private readonly MemoryStream _output;

        public BitConsumer() : this(new MemoryStream()) { }

        public BitConsumer(MemoryStream output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public void Consume(int data, int numberOfBits)
        {
            _queue.Push(data, numberOfBits);
        }

        public long Size { get => _output.Position; }

        public void Flush()
        {
            FlushBytes();
            var pendingBits = _queue.Position;
            if(pendingBits > 0)
            {
                var data = _queue.Pop(pendingBits) << (((Huffman.BitsPerByte - pendingBits) | 0xFF) >> Huffman.BitsPerByte);
                _output.WriteByte((byte)data);
            }
        }

        private void FlushBytes()
        {
            while(_queue.Position > Huffman.BitsPerByte)            
                _output.WriteByte((byte)_queue.Pop(Huffman.BitsPerByte));            
        }

        public byte[] ToByteArray()
        {
            return _output.ToArray();
        }
    }

    internal static class Huffman
    {
        internal const int BitsPerByte = 8;
        internal const int BitsPerInt = sizeof(int) * BitsPerByte;
        internal const int BitsPerLong = sizeof(long) * BitsPerByte;

        private static void AssertNotOutOfBounds(string argumentName, int value, int lowerBound, int upperBound)
        {
            if (value < lowerBound || upperBound < value)
                throw new ArgumentException($"{value} is not within [{lowerBound}; {upperBound}]", argumentName);
        }

        public static void Decode(HuffmanTable dc, HuffmanTable ac, BitSupply supplier, int[] dst, int dstOffset, int dstLength)
        {
            _ = dc ?? throw new ArgumentNullException(nameof(dc));
            _ = ac ?? throw new ArgumentNullException(nameof(ac));
            _ = dst ?? throw new ArgumentNullException(nameof(dst));

            AssertNotOutOfBounds(nameof(dstOffset), dstOffset, 0, dst.Length);
            AssertNotOutOfBounds(nameof(dstLength), dstLength, 1, dst.Length);
            AssertNotOutOfBounds($"{nameof(dstOffset)}+{nameof(dstLength)}", dstOffset + dstLength, 0, dst.Length);

            int dcBits = Decode(dc, supplier);
            if (!supplier.CanSupply(dcBits))
            {
                return;
            }
            else
            {
                int dcDiff = supplier.Supply(dcBits);
                dst[dstOffset] = ConvertToSigned(dcDiff, dcBits);
            }

            for (int i = 1; i < dstLength;)
            {
                int acBits = Decode(ac, supplier);

                if (acBits == 0)
                { // End of block, zero out remaining elements
                    for (int j = i; j < dstLength; ++j)
                        dst[j + dstOffset] = 0;
                    break;
                }

                if (acBits == 0xF0)
                { // Zero out 16 elements
                    if (dstLength < i + 16)                    
                        throw new IndexOutOfRangeException($"Overflow : AC code 0xF0 detected. Unable to zero {16} elements at position {i} of {dstLength}.");

                    for (int l = i + 16; i < l; ++i)
                        dst[i + dstOffset] = 0;

                    continue;
                }

                int msbAC = acBits >> 4 & 0xF;
                if (dstLength < i + msbAC)                
                    throw new IndexOutOfRangeException($"Overflow : AC high bits set. Unable to zero {msbAC} elements at position {i} of {dstLength}");
                
                for (int l = i + msbAC; i < l; ++i)
                    dst[i + dstOffset] = 0;


                int lsbAC = acBits & 0xF;

                if (!supplier.CanSupply(lsbAC))
                    return;

                int acValue = supplier.Supply(lsbAC);
                dst[i++ + dstOffset] = ConvertToSigned(acValue, lsbAC);
            }

            return;
        }

        private static int Decode(HuffmanTable table, BitSupply supplier)
        {
            int maxLength = table.GetDecodeMaxLength();
            int minLength = table.GetDecodeMinLength();

            if (minLength == 0 || maxLength == 0)
                return 0;

            int word = 0;
            int wordLength = 0;
            int nBits = minLength;

            do
            {
                if (!table.HasDecodingForWordOfLength(nBits))
                {
                    nBits += 1;
                    continue;
                }

                int diff = nBits - wordLength;
                if (!supplier.CanSupply(diff))
                    return 0;

                word = word << diff | supplier.Supply(diff);
                wordLength += diff;

                if (table.TryDecode(word, wordLength, out var huffValue))
                    return huffValue.DecodedWord;

                nBits += 1;

            } while (nBits <= maxLength);

            if (nBits > maxLength)
                throw new ArgumentException($"Decoding not found : Available words are between {minLength} and {maxLength} bits. Word {Convert.ToString(word, 2)} with length {wordLength} has no match.");
            return 0;
        }

        public static void Encode(HuffmanTable dc, HuffmanTable ac, BitConsumer consumer, int[] src, int srcOffset, int srcLength)
        {
            _ = dc ?? throw new ArgumentNullException(nameof(dc));
            _ = ac ?? throw new ArgumentNullException(nameof(ac));
            _ = src ?? throw new ArgumentNullException(nameof(src));

            AssertNotOutOfBounds(nameof(srcOffset), srcOffset, 0, src.Length);
            AssertNotOutOfBounds(nameof(srcLength), srcLength, 1, src.Length);
            AssertNotOutOfBounds($"{nameof(srcOffset)}+{nameof(srcLength)}", srcOffset + srcLength, 0, src.Length);

            int dcValue = src[srcOffset];
            int dcBits = CalculateBitLength(dcValue);
            Encode(dc, consumer, dcBits);

            int dcDiff = ConvertToUnsigned(dcValue, dcBits);
            consumer.Consume(dcDiff, dcBits);

            for (int i = 1; i < srcLength;)
            {
                int acBits = 0x00;

                if (src[i + srcOffset] == 0)
                { // count zeros
                    int zeroCounter = 1;

                    for (int j = i + 1; j < srcLength; ++j)
                    {
                        if (src[j + srcOffset] == 0)                        
                            zeroCounter += 1;                        
                        else                        
                            break;                        
                    }

                    i += zeroCounter;

                    if (i == srcLength)
                    { // end of block
                        Encode(ac, consumer, 0x00);
                        break;
                    }

                    while (zeroCounter >= 16)
                    {
                        Encode(ac, consumer, 0xF0); // special code for 16 zeros
                        zeroCounter -= 16;
                    }

                    // msb contains the number of zeros before the next ac value
                    acBits |= zeroCounter << 4 & 0xF0;
                }

                int acValue = src[i++ + srcOffset];
                int acValueBits = CalculateBitLength(acValue);

                if (acValueBits > 0xF)
                {
                    throw new IndexOutOfRangeException($"Overflow : AC bit length {acValueBits} is greater than {16} bits, which is not supported by this encoder.");
                }

                // lsb contains the number of bits to read for the actual ac value
                acBits |= acValueBits & 0x0F;

                Encode(ac, consumer, acBits);

                int acValueConverted = ConvertToUnsigned(acValue, acValueBits);
                consumer.Consume(acValueConverted, acValueBits);
            }
        }

        private static void Encode(HuffmanTable table, BitConsumer consumer, int value)
        {
            if (table.TryEncode(value, out var huffValue))            
                throw new ArgumentException($"Encoding not found : Table contains no encoding for word {Convert.ToString(value, 2)}");            
            consumer.Consume(huffValue.EncodedWord, huffValue.EncodedWordBitLength);
        }

        private static int ConvertToSigned(int value, int numberOfBits)
        {
            if (value < (1 << (numberOfBits - 1)))
                return value + (-1 << numberOfBits) + 1;
            return value;
        }

        private static int ConvertToUnsigned(int value, int numberOfBits)
        {
            if (value < 0)
                return value - ((-1 << numberOfBits) + 1);
            return value;
        }

        private static int CalculateBitLength(int value)
        {
            if (value == 0)
                return 0;
            return BitsPerInt - CountLeadingZeros(Math.Abs(value));
        }

        /// <summary>
        /// Source: https://stackoverflow.com/questions/10439242/count-leading-zeroes-in-an-int32
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int CountLeadingZeros(int value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            //count the ones
            value -= value >> 1 & 0x55555555;
            value = (value >> 2 & 0x33333333) + (value & 0x33333333);
            value = (value >> 4) + value & 0x0f0f0f0f;
            value += value >> 8;
            value += value >> 16;
            return BitsPerInt - (value & 0x0000003f); //subtract # of 1s from 32
        }
    }
}
