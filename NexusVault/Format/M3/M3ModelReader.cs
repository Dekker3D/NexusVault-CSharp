using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NexusVault.Format.M3
{
    public static class M3ModelReader
    {
        public static M3Model Read(byte[] data)
        {
            return new M3Model(data);
        }

        public static M3Model Read(Stream stream)
        {
            return new M3Model(stream);
        }

        public static M3Model Read(BinaryReader reader)
        {
            return new M3Model(reader);
        }

        public static M3Model Read(string path)
        {
            return new M3Model(new FileStream(path, FileMode.Open));
        }
    }
}
