using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Suprifattus.Util.Text.Formatters
{
	/// <summary>
	/// Formata números de CPF.
	/// </summary>
	/// <example>
	///		Formatando um CPF:
	///		<code language="c#">
	///			Console.WriteLine(String.Format(PluggableFormatProvider, "{0:cpf}", "12345678901"))
	///		</code>
	///		
	///		O resultado obtido é:
	///		<code>
	///			123.456.789-01
	///		</code>
	/// </example>
	public class CPFFormatter : IFormatterPlugin
	{
		Regex rxCpf = new Regex(@"^(\d\d\d)\.?(\d\d\d)\.?(\d\d\d)-?(\d\d)$", RegexOptions.RightToLeft | RegexOptions.Compiled);

		/// <summary>
		/// A chave utilizada ao registrar o formatador.
		/// Para o <see cref="CPFFormatter"/>, ela é '<c>cpf</c>'.
		/// </summary>
		public string FormatKey
		{
			get { return "cpf"; }
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
				mask = "{0}{1}{2}{3}";
			else
				mask = "{0}.{1}.{2}-{3}";
			
			string cnpj = Convert.ToString(arg, ci);
			Match m = rxCpf.Match(cnpj);
			if (m != null && m.Success)
				return String.Format(ci, mask, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value);
			else
				return cnpj;
		}
	}
}
