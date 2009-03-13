using System;

namespace Suprifattus.Util.Text
{
	/// <summary>
	/// Interface que deve ser implementada pelos plugins de formatação,
	/// utilizados pelo <see cref="PluggableFormatProvider"/>.
	/// </summary>
	public interface IFormatterPlugin
	{
		/// <summary>
		/// A chave utilizada ao registrar o formatador.
		/// </summary>
		string FormatKey { get; }

		/// <summary>
		/// Realiza a formatação.
		/// </summary>
		/// <param name="formatString">A string de formatação</param>
		/// <param name="arg">O objeto a ser formatado</param>
		/// <returns>Uma string contendo o objeto formatado</returns>
		string Format(string formatString, object arg);
	}
}