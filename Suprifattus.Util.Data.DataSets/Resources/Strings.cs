using System;
using System.Globalization;
using System.Resources;

namespace Suprifattus.Util.Data.Resources
{
	internal class Strings
	{
		private static readonly ResourceManager instance = new ResourceManager(typeof(Strings));

		private Strings() { throw new InvalidOperationException(); }

		public static string FormatString(string name, params object[] args)
		{
			return String.Format(GetString(name), args);
		}
		
		public static string FormatString(IFormatProvider formatProvider, string name, params object[] args)
		{
			return String.Format(formatProvider, GetString(name), args);
		}
		
		public static string FormatString(CultureInfo culture, string name, params object[] args)
		{
			return String.Format(GetString(name, culture), args);
		}

		public static string FormatString(CultureInfo culture, IFormatProvider formatProvider, string name, params object[] args)
		{
			return String.Format(formatProvider, GetString(name, culture), args);
		}
		
		public static string GetString(string name)
		{
			return instance.GetString(name);
		}

		public static string GetString(string name, CultureInfo culture)
		{
			return instance.GetString(name, culture);
		}

		public static object GetObject(string name)
		{
			return instance.GetObject(name);
		}

		public static object GetObject(string name, CultureInfo culture)
		{
			return instance.GetObject(name, culture);
		}
	}
}
