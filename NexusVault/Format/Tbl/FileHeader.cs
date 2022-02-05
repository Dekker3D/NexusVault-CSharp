using System.Runtime.InteropServices;

namespace NexusVault.tbl
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileHeader
    {
        public uint signature; // "DTBL"
        public uint version;

        public ulong nameLength; // number of UTF-16 encoded characters, aligned to 16 byte and null terminated
        public ulong unk_010;
        public ulong recordSize;   // size of a single record in bytes
        public ulong fieldCount;
        public ulong fieldOffset;
        public ulong recordCount;
        public ulong totalRecordsSize; // Size of all records
        public ulong recordsOffset;
        public ulong lookupCount;
        public ulong lookupOffset; // id to index lookup
        private ulong padding_058;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TblColumn
    {
        public uint nameLength;  // number of UTF-16 encoded characters, aligned to 16 byte and null terminated
        public uint unk_04;
        public ulong nameOffset;
        public uint dataType;
        public uint unk_14; // seems to be 24 for dataType int and float and 104 for string ?
    }
}
