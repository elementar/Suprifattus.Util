using System;
using System.IO;

using NUnit.Framework;

using Suprifattus.Util.IO;

namespace Suprifattus.Util.Tests
{
	[TestFixture]
	public class CircularStreamTests
	{
		[Test]
		public void SimpleTest()
		{
			const string msg = "olá!";

			var cb = new CircularStream();
			using (var w = new StreamWriter(cb))
				w.Write(msg);

			string v;
			using (var r = new StreamReader(cb))
				v = r.ReadToEnd();

			Assert.AreEqual(msg, v);
		}

		[Test]
		public void BufferFullTest()
		{
			const string msg = "olá, esta é uma mensagem que com certeza vai ultrapassar o buffer";

			var cb = new CircularStream(new CircularBuffer(10));
			using (var w = new StreamWriter(cb))
				w.Write(msg);

			string v;
			using (var r = new StreamReader(cb))
				v = r.ReadToEnd();

			Assert.AreEqual(msg, v);
		}
	}
}