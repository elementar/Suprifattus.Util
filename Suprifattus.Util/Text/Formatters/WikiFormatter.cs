using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Suprifattus.Util.Text.Formatters
{
	/// <summary>
	/// Formata texto estilo Wiki
	/// </summary>
	public class WikiFormatter : IFormatterPlugin
	{
		/// <summary>
		/// A chave utilizada ao registrar o formatador.
		/// Para o <see cref="WikiFormatter"/>, ela é '<c>wiki</c>'.
		/// </summary>
		public string FormatKey
		{
			get { return "wiki"; }
		}

		/// <summary>
		/// Realiza a formatação.
		/// </summary>
		/// <param name="formatString">A string de formatação</param>
		/// <param name="arg">O objeto a ser formatado</param>
		/// <returns>Uma string contendo o objeto formatado</returns>
		public string Format(string formatString, object arg)
		{
			CultureInfo ci = CultureInfo.CurrentCulture;

			string s = Convert.ToString(arg, ci);
			// \\ -> <br/>
			s = Regex.Replace(s, @"\\\\", "<br />");
			// '''*''' -> <strong>*</strong>
			s = Regex.Replace(s, @"'''([^']+)'''", "<strong>$1</strong>");
			// ''*'' -> <em>*</em>
			s = Regex.Replace(s, @"''([^']+)''", "<em>$1</em>");
			// [*] -> <a href='*'>*</a>
			s = Regex.Replace(s, @"\[([^\]|])+\]", "<a href='$1'>$1</a>");
			return s;
		}
	}
}