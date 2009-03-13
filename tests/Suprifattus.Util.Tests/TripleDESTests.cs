using System;

using NUnit.Framework;

using Suprifattus.Util.Encryption;

namespace Suprifattus.Util.Tests
{
	[TestFixture]
	public class TripleDESTests
	{
		private const string EncryptionKey = "1!#3^a";

		[Test]
		public void EncriptAndDecryptSimpleText()
		{
			var text = "alo mundo";
			string encrypted, decrypted;

			using (var s3des = new SimpleTripleDES(EncryptionKey))
				encrypted = s3des.Encrypt(text);

			using (var s3des = new SimpleTripleDES(EncryptionKey))
				decrypted = s3des.Decrypt(encrypted);

			Console.WriteLine("original : {0}", text);
			Console.WriteLine("encrypted: {0}", encrypted);
			Console.WriteLine("decrypted: {0}", decrypted);

			Assert.AreEqual(text, decrypted);
		}

		[Test]
		public void EncriptAndDecryptComplexTest()
		{
			var text = "<alô, mundo CÃO!\ntudo certo?>";
			string encrypted, decrypted;

			using (var s3des = new SimpleTripleDES(EncryptionKey))
				encrypted = s3des.Encrypt(text);

			using (var s3des = new SimpleTripleDES(EncryptionKey))
				decrypted = s3des.Decrypt(encrypted);

			Assert.AreEqual(text, decrypted);
		}

		[Test]
		public void EncriptAndDecryptBytes()
		{
			byte[] input = { 0x00, 0x12, 0x64, 0x12, 0x00, 0x19, 0xff, 0x55 };
			byte[] encrypted, decrypted;

			using (var s3des = new SimpleTripleDES(EncryptionKey))
				encrypted = s3des.Encrypt(input);

			using (var s3des = new SimpleTripleDES(EncryptionKey))
				decrypted = s3des.Decrypt(encrypted);

			Assert.AreEqual(input, decrypted);
		}
	}
}