using System;
using System.Collections.Generic;
using System.Text;

namespace NexusVault.tex
{
    public class HuffmanTable
    {
        private class HuffmanKey : IComparable<HuffmanTable.HuffmanKey>
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

        public class HuffmanValue
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
        private readonly byte[][] codeTable;

        public HuffmanTable(byte[] numberOfValues, byte[] values)
        {
            // sort values in categories
            codeTable = new byte[16][];
            int pos = 0;
            for (int i = 0; i < numberOfValues.Length; ++i)
            {
                codeTable[i] = new byte[numberOfValues[i]];
                for (int j = 0; j < numberOfValues[i]; ++j)
                    codeTable[i][j] = values[pos++];
            }

            buildHuffmanTree();
        }

        private void buildHuffmanTree()
        {
            //Decoding
            int encodedWord = 0;
            for (int idx = 0; idx < codeTable.Length; ++idx)
            {
                int encodedLength = idx + 1;
                if (codeTable[idx].Length > 1 << idx + 1)
                    throw new ArgumentException($"Code error. Step {idx + 1} contains {codeTable[idx].Length} words. With a bit length of {encodedLength} only {1 << encodedLength} words are supported.");

                for (int nIdx = 0; nIdx < codeTable[idx].Length; ++nIdx)
                {
                    int decodedWord = codeTable[idx][nIdx];
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


        /**
         * @param nBits
         *            length of the word. Given in the number of used bits
         * @return true when there is at least one decoding available
         */
        public bool HasDecodingForWordOfLength(int bits)
        {
            if (bits < 0 || codeTable.Length < bits)
                return false;
            return codeTable[bits - 1].Length != 0;
        }

        /**
         * @return minimal number of bits needed for decoding
         */
        public int GetDecodeMinLength()
        {
            for (int i = 0; i < codeTable.Length; ++i)
                if (codeTable[i].Length != 0)
                    return i + 1;
            return 0;
        }

        /**
         * @return maximal number of bits which can be used for decoding
         */
        public int GetDecodeMaxLength()
        {
            for (int i = codeTable.Length - 1; 0 <= i; --i)
                if (codeTable[i].Length != 0)
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

    public class HuffmanDecoder
    {
        public static void Decode(HuffmanTable dc, HuffmanTable ac, BitSupply supplier, int[] block, int offset, int blockLength)
        {
            _ = dc ?? throw new ArgumentNullException(nameof(dc));
            _ = ac ?? throw new ArgumentNullException(nameof(ac));
            _ = block ?? throw new ArgumentNullException(nameof(block));

            AssertNotOutOfBounds(nameof(offset), offset, 0, block.Length);
            AssertNotOutOfBounds(nameof(blockLength), blockLength, 1, block.Length);
            AssertNotOutOfBounds($"{nameof(offset)}+{nameof(blockLength)}", offset + blockLength, 0, block.Length);

            int dcBits = Decode(dc, supplier);
            if (!supplier.CanSupply(dcBits))
            {
                return;
            }
            else
            {
                int dcDiff = supplier.Supply(dcBits);
                block[offset] = ConvertToSigned(dcDiff, dcBits);
            }

            for (int i = 1; i < blockLength;)
            {
                int acBits = Decode(ac, supplier);

                if (acBits == 0)
                { // End of block, zero out remaining elements
                    for (int j = i; j < blockLength; ++j)
                        block[j + offset] = 0;
                    break;
                }

                if (acBits == 0xF0)
                { // Zero out 16 elements
                    if (blockLength < i + 16)
                    {
                        throw new IndexOutOfRangeException(
                                string.Format("Overflow : AC code 0xF0 detected. Unable to zero %d elements at position %d of %d", 16, i, blockLength));
                    }

                    for (int l = i + 16; i < l; ++i)
                        block[i + offset] = 0;

                    continue;
                }

                int msbAC = acBits >> 4 & 0xF;
                if (blockLength < i + msbAC)
                {
                    throw new IndexOutOfRangeException(
                            string.Format("Overflow : AC high bits set. Unable to zero %d elements at position %d of %d", msbAC, i, blockLength));
                }

                for (int l = i + msbAC; i < l; ++i)
                    block[i + offset] = 0;


                int lsbAC = acBits & 0xF;

                if (!supplier.CanSupply(lsbAC))
                    return;

                int acValue = supplier.Supply(lsbAC);
                block[i++ + offset] = ConvertToSigned(acValue, lsbAC);
            }

            return;
        }

        private static int ConvertToSigned(int data, int bits)
        {
            if (data < 1 << bits - 1)
                return data + (-1 << bits) + 1;
            return data;
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

        private static void AssertNotOutOfBounds(String argumentName, int value, int lowerBound, int upperBound)
        {
            if (value < lowerBound || upperBound < value)
                throw new ArgumentException($"{value} is not within [{lowerBound}; {upperBound}]", argumentName);
        }
    }

    class BitQueue
    {
        private ulong bitQueue;
        public int Position { get; private set; }
        public int RemainingCapacity { get { return 64 - Position; } }


        public int Pop(int requestedBits)
        {
            if (requestedBits < 0 || 32 < requestedBits)
                throw new IndexOutOfRangeException("Can't pop less than 0 or more than 32 bits.");
            if (Position < requestedBits)
                throw new IndexOutOfRangeException($"Queue contains {Position} bits. Unable to pop {requestedBits} bits.");

            if (requestedBits == 0)
                return 0;

            int result = (int)(bitQueue >> (64 - requestedBits));
            bitQueue <<= requestedBits;
            Position -= requestedBits;
            return result;
        }

        public void Push(uint data, int lengthInBits)
        {
            Push((int)data, lengthInBits);
        }

        public void Push(int data, int lengthInBits)
        {
            if (lengthInBits < 0 || 32 < lengthInBits)
                throw new IndexOutOfRangeException("Can't push less than 0 or more than 32 bits.");
            if (RemainingCapacity < lengthInBits)
                throw new IndexOutOfRangeException($"Queue can only store {RemainingCapacity} more bits. Unable to push {lengthInBits} bits.");

            if (lengthInBits == 0)
                return;

            uint mask = 0xFFFFFFFF >> (32 - lengthInBits);
            ulong alignedData = (ulong)(data & mask) << (RemainingCapacity - lengthInBits);
            bitQueue |= alignedData;
            Position += lengthInBits;
        }

        public void Clear()
        {
            Position = 0;
            bitQueue = 0;
        }

        override public string ToString()
        {
            string bin = Convert.ToString((long)bitQueue, 2);
            while (bin.Length < 64)
                bin = "0" + bin;
            return $"[BitQueue: {bin.Substring(0, Position) + "|" + bin.Substring(Position)}]";
        }
    }

    public class BitSupply
    {
        private readonly BitQueue queue = new BitQueue();
        private readonly byte[] data;
        private int index;

        public int RemainingBits
        {
            get
            {
                return queue.Position + (data.Length - index) * 8 /*bits per byte*/ ;
            }
        }

        public BitSupply(byte[] data)
        {
            this.data = data;
        }

        public bool CanSupply(int requestedBits)
        {
            return requestedBits <= RemainingBits;
        }

        public int Supply(int requestedBits)
        {
            if (requestedBits > 32)
                throw new IndexOutOfRangeException();

            if (queue.Position < requestedBits)
            {
                var dif = requestedBits - queue.Position;
                if (dif > queue.RemainingCapacity)
                    throw new IndexOutOfRangeException();

                while (dif > 0)
                {
                    queue.Push(data[index++] & 0xFF, 8);
                    dif -= 8;
                }
            }
            return queue.Pop(requestedBits);
        }

        override public string ToString()
        {
            return $"[BitSupply: Remaining bits={RemainingBits} Queue: {queue}]";
        }
    }
}
