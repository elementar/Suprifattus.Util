using System;
using System.IO;
using System.Text;

namespace Suprifattus.Util.Web.JavaScript
{
	/// <summary>
	/// Carrega scripts JavaScript.
	/// </summary>
	public class JavaScriptLoader
	{
		public const string ScriptBlockFormat = "<script type=\"text/javascript\">\n//<![CDATA[\n{0}\n//]]></script>";
		
		private static readonly System.Reflection.Assembly ThisAssembly = typeof(JavaScriptLoader).Assembly;
		private const string ResFmt = "Suprifattus.Util.Web.JavaScript.Sources.{0}.js";

		public static string Load(bool enclosed, params string[] scripts)
		{
			StringBuilder sb = new StringBuilder();

			foreach (string script in scripts)
			{
				Stream s = ThisAssembly.GetManifestResourceStream(String.Format(ResFmt, script));
				if (s != null) 
				{
					StreamReader sr = new StreamReader(s, Encoding.Default);

					sb.AppendFormat("/* script: {0}.js */" + Environment.NewLine, script);
					sb.Append(sr.ReadToEnd()).AppendFormat("{0}{0}", Environment.NewLine);
				}
			}

			return (enclosed ? String.Format(ScriptBlockFormat, sb.ToString()) : sb.ToString());
		}
	}
}
