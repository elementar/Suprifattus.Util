using System;
using System.Text.RegularExpressions;

using Castle.MonoRail.Framework.Helpers;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	public class RegexHelper : AbstractHelper
	{
		public Regex Create(string pattern)
		{
			return new Regex(pattern);
		}

		public string Replace(string pattern, string replacement, string input)
		{
			return Regex.Replace(input, pattern, replacement);
		}

		public Match Match(string pattern, string input)
		{
			return Regex.Match(input, pattern);
		}
	}
}