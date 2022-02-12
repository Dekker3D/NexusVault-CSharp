using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NexusVault.Test.Test.Util
{
    class Text
    {
        [Test]
        public void ReadNullTerminatedUTF16_1()
        {
            var expectedOutput = "AB YZ 19 \uD800\udc05 \u00e4";
            var binaryReader = new BinaryReader(new MemoryStream(Encoding.Unicode.GetBytes(expectedOutput + char.MinValue)));
            var output = NexusVault.Shared.Util.Text.ReadNullTerminatedUTF16(binaryReader);
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void ReadNullTerminatedUTF8_1()
        {
            var expectedOutput = "AB YZ 19 \uD800\udc05 \u00e4";
            var binaryReader = new BinaryReader(new MemoryStream(Encoding.UTF8.GetBytes(expectedOutput + char.MinValue)));
            var output = NexusVault.Shared.Util.Text.ReadNullTerminatedUTF8(binaryReader);
            Assert.AreEqual(expectedOutput, output);
        }

    }
}
