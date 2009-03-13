using System;

using NUnit.Framework;

using Suprifattus.Util.Text;

namespace Suprifattus.Util.Tests
{
	[TestFixture]
	public class PluralFormTests
	{
		[Test]
		public void Test3()
		{
			var format = "[S� uma:Mais de uma:Nenhuma] vez";
			Assert.AreEqual("Nenhuma vez", PluralForm.Format(0, format));
			Assert.AreEqual("S� uma vez", PluralForm.Format(1, format));
			Assert.AreEqual("Mais de uma vez", PluralForm.Format(5, format));
		}

		[Test]
		public void Test2()
		{
			var format = "[S� uma:Mais de uma] vez";
			Assert.AreEqual("S� uma vez", PluralForm.Format(0, format));
			Assert.AreEqual("S� uma vez", PluralForm.Format(1, format));
			Assert.AreEqual("Mais de uma vez", PluralForm.Format(5, format));
		}

		[Test]
		public void Test1()
		{
			var format = "[S� uma] vez";
			Assert.AreEqual("S� uma vez", PluralForm.Format(0, format));
			Assert.AreEqual("S� uma vez", PluralForm.Format(1, format));
			Assert.AreEqual("S� uma vez", PluralForm.Format(5, format));
		}
	}
}