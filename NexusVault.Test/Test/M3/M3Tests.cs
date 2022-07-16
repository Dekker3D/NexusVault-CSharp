using NexusVault.Format.Bin;
using NexusVault.Format.M3;
using NUnit.Framework;
using System.Collections.Generic;

namespace NexusVault.Test.M3 {
	class M3Tests {

		[Test]
		public void ReadWriteAndCompare() {
			M3ModelReader.Read("..\\..\\..\\..\\Aurin_M.m3");
		}
	}
}