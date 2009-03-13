using System;
using System.Collections;

using NUnit.Framework;

namespace Suprifattus.Util.Tests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void TestNullify()
		{
			// Nullify(String)
			Assert.IsNull(Logic.Nullify(""));
			Assert.IsNull(Logic.Nullify(null));
			Assert.IsNotNull(Logic.Nullify("-"));
			Assert.IsNotNull(Logic.Nullify(" "));

			// NullifyIfEqual(String, String)
			Assert.IsNull(Logic.NullifyIfEqual("", ""));
			Assert.IsNull(Logic.NullifyIfEqual("-", "-"));
			Assert.IsNotNull(Logic.NullifyIfEqual("", (string) null));
			Assert.IsNotNull(Logic.NullifyIfEqual("-", ""));
			Assert.IsNotNull(Logic.NullifyIfEqual("-", "/"));
			Assert.IsNotNull(Logic.NullifyIfEqual("", "*"));
			Assert.IsNotNull(Logic.NullifyIfEqual("a", "A"));

			// NullifyIfEqual(String, String[])
			Assert.IsNull(Logic.NullifyIfEqual("", "a", "b", ""));
			Assert.IsNull(Logic.NullifyIfEqual("-", "/", "-", "\\"));
			Assert.IsNull(Logic.NullifyIfEqual("-", "-", "--", "---"));
			Assert.IsNotNull(Logic.NullifyIfEqual("-", "/", "--", "*"));
			Assert.IsNotNull(Logic.NullifyIfEqual("a", "A", "B", "C"));

			// NullifyIfEqual(IComparer, String, String[])
			var comp = CaseInsensitiveComparer.DefaultInvariant;
			Assert.IsNull(Logic.NullifyIfEqual(comp, "a", "A", "B", "C"));
			Assert.IsNull(Logic.NullifyIfEqual(comp, "ñ", "Ñ", "N", "A"));
			Assert.IsNotNull(Logic.NullifyIfEqual(comp, "ñ", "N", "X"));
		}

		[Test]
		public void TestStringEmpty()
		{
			Assert.IsTrue(Logic.StringEmpty(null));
			Assert.IsTrue(Logic.StringEmpty(""));
			Assert.IsTrue(Logic.StringEmpty(String.Empty));

			Assert.IsFalse(Logic.StringEmpty("sample"));
			Assert.IsFalse(Logic.StringEmpty("null"));
			Assert.IsFalse(Logic.StringEmpty(" "));
		}

		[Test]
		public void TestAllEmpty()
		{
			Assert.IsTrue(Logic.AllEmpty(null, "", "", "", null, ""));
			Assert.IsFalse(Logic.AllEmpty(null, null, "NOTEMPTY", ""));
		}

		[Test]
		public void TestAnyEmpty()
		{
			Assert.IsTrue(Logic.AnyEmpty(null, "", "", "", null, ""));
			Assert.IsTrue(Logic.AnyEmpty(null, null, "NOTEMPTY", ""));
			Assert.IsFalse(Logic.AnyEmpty("this", "is", "a", "sample"));
		}

		[Test]
		public void TestRepresentsTrue()
		{
			Assert.IsTrue(Logic.RepresentsTrue("true"));
			Assert.IsTrue(Logic.RepresentsTrue("yes"));
			Assert.IsTrue(Logic.RepresentsTrue("Sim"));
			Assert.IsTrue(Logic.RepresentsTrue("S"));
			Assert.IsTrue(Logic.RepresentsTrue("s"));
			Assert.IsTrue(Logic.RepresentsTrue("TrUe"));
			Assert.IsTrue(Logic.RepresentsTrue("t"));
			Assert.IsTrue(Logic.RepresentsTrue("1"));
			Assert.IsTrue(Logic.RepresentsTrue("on"));

			Assert.IsFalse(Logic.RepresentsTrue("0"));
			Assert.IsFalse(Logic.RepresentsTrue("false"));
			Assert.IsFalse(Logic.RepresentsTrue("off"));
		}

		[Test]
		public void TestIsNumeric()
		{
			Assert.IsTrue(Logic.IsNumeric("10"));
			Assert.IsTrue(Logic.IsNumeric("0"));
			Assert.IsTrue(Logic.IsNumeric("999"));
			Assert.IsTrue(Logic.IsNumeric("+1"));
			Assert.IsTrue(Logic.IsNumeric("-8"));
			Assert.IsTrue(Logic.IsNumeric("78715187"));

			Assert.IsFalse(Logic.IsNumeric(null));
			Assert.IsFalse(Logic.IsNumeric(""));
			Assert.IsFalse(Logic.IsNumeric(" 45"));
			Assert.IsFalse(Logic.IsNumeric("12 "));
			Assert.IsFalse(Logic.IsNumeric("1e2"));
			Assert.IsFalse(Logic.IsNumeric("XYZ"));
			Assert.IsFalse(Logic.IsNumeric("O"));
			Assert.IsFalse(Logic.IsNumeric("O1"));
			Assert.IsFalse(Logic.IsNumeric("1O"));
		}
	}
}