using System;
using System.Text.RegularExpressions;

namespace Suprifattus.Util.Xml
{
	/// <summary>
	/// Performs XML encoding.
	/// </summary>
	public sealed class XmlEncoder
	{
		private XmlEncoder() { }
		
		private static Regex rxXmlEscapable = new Regex(@"[&""<>]");

		/// <summary>
		/// Encodes an object's string representation into a XML string, 
		/// replacing characters like <c>&amp; &lt; &gt; &quot;</c> 
		/// by its XML equivalents, <c>&amp;amp; &amp;lt; &amp;gt; &amp;quot;</c>.
		/// </summary>
		/// <param name="obj">The object to be converted to string and then encoded</param>
		/// <returns>The encoded string</returns>
		/// <remarks>
		/// The object is converted to string using <see cref="object"/>
		/// method.
		/// </remarks>
		public static string Encode(object obj)
		{
			if (obj == null)
				return null;

			return Encode(Convert.ToString(obj));
		}

		/// <summary>
		/// Encodes a string into a XML string, replacing characters like
		/// <c>&amp; &lt; &gt; &quot;</c> by its XML equivalents,
		/// <c>&amp;amp; &amp;lt; &amp;gt; &amp;quot;</c>.
		/// </summary>
		/// <param name="s">The string to be encoded</param>
		/// <returns>The encoded string</returns>
		public static string Encode(string s)
		{
			if (s == null)
				return null;

			return rxXmlEscapable.Replace(s, new MatchEvaluator(EncodeReplacer));
		}

		private static string EncodeReplacer(Match m)
		{
			switch (m.Value)
			{
				case "&": return "&amp;";
				case "\"": return "&quot;";
				case "<": return "&lt;";
				case ">": return "&gt;";

				default:
					return m.Value;
			}
		}
	}
}
