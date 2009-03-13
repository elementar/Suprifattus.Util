using System;
using System.IO;

using NUnit.Framework;

using Suprifattus.Util.IO;

namespace Suprifattus.Util.Tests
{
	[TestFixture]
	public class StreamsTests
	{
		[Test]
		public void TestLimit()
		{
			var input = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 };
			var output = new byte[17];

			int n;
			using (var sIn = new MemoryStream(input))
			using (var sOut = new MemoryStream(output))
			{
				sIn.Seek(3, SeekOrigin.Current);
				n = Streams.SaveStream(3, sIn, sOut, 10);
			}

			var i = -1;
			Assert.AreEqual(10, n);
			Assert.AreEqual(input[3], output[++i]);
			Assert.AreEqual(input[4], output[++i]);
			Assert.AreEqual(input[5], output[++i]);
			Assert.AreEqual(input[6], output[++i]);
			Assert.AreEqual(input[7], output[++i]);
			Assert.AreEqual(input[8], output[++i]);
			Assert.AreEqual(input[9], output[++i]);
			Assert.AreEqual(input[10], output[++i]);
			Assert.AreEqual(input[11], output[++i]);
			Assert.AreEqual(input[12], output[++i]);
			Assert.AreEqual(0x00, output[++i]);
			Assert.AreEqual(0x00, output[++i]);
			Assert.AreEqual(0x00, output[++i]);
			Assert.AreEqual(0x00, output[++i]);
			Assert.AreEqual(0x00, output[++i]);
			Assert.AreEqual(0x00, output[++i]);
			Assert.AreEqual(0x00, output[++i]);
		}
	}
}