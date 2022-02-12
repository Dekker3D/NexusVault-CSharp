using NexusVault.Format.Bin;
using NUnit.Framework;
using System.Collections.Generic;

namespace NexusVault.Test.Bin {
	class CreateWriteAndRead {

		[Test]
		public void EmptyToBinaryAndBack() {
			var original = new LanguageDictionary(new Locale(0, "en", "eng", "english"), new Dictionary<uint, string>());
			var binary = LanguageWriter.ToBinary(original);
			var recreated = LanguageReader.Read(binary);
			Assert.AreEqual(original, recreated);
		}

		[Test]
		public void NonEmptyToBinaryAndBack() {
			var original = new LanguageDictionary(new Locale(0, "en", "eng", "english"), new Dictionary<uint, string>());
			original.Entries.Add(1, "Chilly break of day");
			original.Entries.Add(2, "An old, gorgeous rabbit roars");
			original.Entries.Add(3, "enjoying the cow");
			var binary = LanguageWriter.ToBinary(original);
			var recreated = LanguageReader.Read(binary);
			Assert.AreEqual(original, recreated);
		}

	}
}