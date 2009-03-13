using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Suprifattus.Util.Text.Formatters
{
	/// <summary>
	/// Formata números de CNPJ.
	/// </summary>
	public class CNPJFormatter : IFormatterPlugin
	{
		Regex rxCnpj = new Regex(@"^(\d?\d\d)\.?(\d\d\d)\.?(\d\d\d)/?(\d\d\d\d)-?(\d\d)$", RegexOptions.RightToLeft | RegexOptions.Compiled);

		/// <summary>
		/// A chave utilizada ao registrar o formatador.
		/// Para o <see cref="CNPJFormatter"/>, ela é '<c>cnpj</c>'.
		/// </summary>
		public string FormatKey
		{
			get { return "cnpj"; }
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

			string mask;
			if (formatString != null && formatString.EndsWith("-"))
				mask = "{0}{1}{2}{3}{4}";
			else
				mask = "{0}.{1}.{2}/{3}-{4}";
			
			string cnpj = Convert.ToString(arg, ci);
			Match m = rxCnpj.Match(cnpj);
			if (m != null && m.Success)
				return String.Format(ci, mask, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value, m.Groups[5].Value);
			else
				return cnpj;
		}
	}
}
