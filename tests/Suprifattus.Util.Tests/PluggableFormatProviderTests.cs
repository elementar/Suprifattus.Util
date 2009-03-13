using System;
using System.IO;

using NUnit.Framework;

using Suprifattus.Util.Text;

namespace Suprifattus.Util.Tests
{
	[TestFixture]
	public class PluggableFormatProviderTests
	{
		private PluggableFormatProvider f;

		[SetUp]
		public void SetUp()
		{
			f = PluggableFormatProvider.Instance;
		}

		protected string Format(string formatString, object obj)
		{
			return String.Format(f, formatString, obj);
		}

		public enum Sample
		{
			[System.ComponentModel.Description("Can be described")] Descriptible,
			NonDescriptible,
		}

		[Test]
		public void EnumFormatterTests()
		{
			Assert.AreEqual("Read", Format("{0:enum}", FileAccess.Read));
			Assert.AreEqual("99", Format("{0:enum}", (FileAccess) 99));

			Assert.AreEqual("NonDescriptible", Format("{0:enum}", Sample.NonDescriptible));
			Assert.AreEqual("Can be described", Format("{0:enum}", Sample.Descriptible));
		}

		[Test]
		public void CNPJFormatterTests()
		{
			Assert.AreEqual("123.456.789/0001-23", Format("{0:cnpj}", "123456789000123"));
			Assert.AreEqual("123456789000123", Format("{0:cnpj-}", "123.456.789/0001-23"));
			Assert.AreEqual("12.345.678/0001-23", Format("{0:cnpj}", "12345678000123"));
			Assert.AreEqual("12345678000123", Format("{0:cnpj-}", "12.345.678/0001-23"));
		}
	}
}