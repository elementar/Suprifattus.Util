using System;

namespace Suprifattus.Util
{
	/// <summary>
	/// Formatador de objetos <see cref="KeyValue"/>.
	/// </summary>
	[CLSCompliant(false)]
	public class KeyValueFormatter : IFormatProvider, ICustomFormatter
	{
		/// <summary>
		/// A string de formatação.
		/// </summary>
		string formatString;
		
		/// <summary>
		/// Cria um novo formatador utilizando a string de formatação especificada.
		/// </summary>
		/// <param name="format">A string de formatação. Nela, {0} será substituído
		/// pelo Key, e {1} será substituído por Value.</param>
		public KeyValueFormatter(string format) 
		{
			this.formatString = format;
		}

		/// <summary>
		/// Cria um novo formatador utilizando a string de formatação padrão ({0}={1}).
		/// </summary>
		public KeyValueFormatter() 
			: this("{0}={1}") {}
		
		object IFormatProvider.GetFormat(Type formatType)
		{
			return (typeof(ICustomFormatter).Equals(formatType) ? this : null);
		}

		/// <summary>
		/// Realiza a formatação de um <see cref="KeyValue"/>
		/// </summary>
		/// <param name="format">Utilizado pelos chamadores de <see cref="ICustomFormatter"/>. 
		/// Não é considerado.</param>
		/// <param name="arg">O objeto <see cref="KeyValue"/> a ser formatado.</param>
		/// <param name="formatProvider">Nós próprios.</param>
		/// <returns>A representação string formatada do <see cref="KeyValue"/>.</returns>
		string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
		{
			if (arg == null) 
				throw new ArgumentNullException("arg");

			if (arg is KeyValue) 
				return FormatKeyValue(arg as KeyValue, format);
			
			if (arg is IFormattable)
				return ((IFormattable) arg).ToString(format, formatProvider);
			else 
				return arg.ToString();
		}

		/// <summary>
		/// Formata o <see cref="KeyValue"/>.
		/// </summary>
		/// <param name="nv">O <see cref="KeyValue"/></param>
		/// <param name="format">Formato. Ignorado.</param>
		/// <returns>A representação string formatada do <see cref="KeyValue"/>.</returns>
		public string FormatKeyValue(KeyValue nv, string format) 
		{
			return String.Format(formatString, nv.Key, nv.Value);
		}
	}
}