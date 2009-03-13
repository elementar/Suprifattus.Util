using System;
using System.IO;

using Suprifattus.Util.IO;

namespace Suprifattus.Util.Text.Formatters
{
	/// <summary>
	/// Formata removendo caracteres acentuados.
	/// </summary>
	public class NoAccentsFormatter : IFormatterPlugin
	{
		/// <summary>
		/// A chave utilizada ao registrar o formatador.
		/// Para o <see cref="NoAccentsFormatter"/>, ela é '<c>noacc</c>'.
		/// </summary>
		public string FormatKey
		{
			get { return "noacc"; }
		}

		/// <summary>
		/// Realiza a formatação.
		/// </summary>
		/// <param name="formatString">A string de formatação</param>
		/// <param name="arg">O objeto a ser formatado</param>
		/// <returns>Uma string contendo o objeto formatado</returns>
		public string Format(string formatString, object arg)
		{
			using (StringWriter sw = new StringWriter())
			{
				using (NoAccentsTextWriter tw = new NoAccentsTextWriter(sw))
					tw.Write(arg);

				return sw.ToString();
			}
		}
	}
}