using System;
using System.Text.RegularExpressions;

namespace Suprifattus.Util.Text
{
	/// <summary>
	/// Classe utilitária para lidar com expressões em singular e plural.
	/// </summary>
	public sealed class PluralForm
	{
		static Regex rxTags = new Regex(@"\[ (?<singular>[^]:]*) (:(?<plural>[^]:]*) (:(?<zero>[^]:]*))? )? \]", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

		private PluralForm() { throw new InvalidOperationException(); }
		
		/// <summary>
		/// Formata um texto.
		/// </summary>
		/// <param name="n">O número que deve ser verificado se é singular ou plural.</param>
		/// <param name="s">O texto a ser formatado</param>
		/// <returns>O texto, formatado de acordo com o número</returns>
		public static string Format(long n, string s)
		{
			return rxTags.Replace(s, new MatchEvaluator(new Replacer(n).Replace));
		}

		/// <summary>
		/// Formata um texto.
		/// </summary>
		/// <param name="n">O número que deve ser verificado se é singular ou plural.</param>
		/// <param name="format">A string de formatação do texto</param>
		/// <param name="args">Os argumentos de formatação do texto</param>
		/// <returns>O texto, formatado de acordo com o número</returns>
		public static string Format(long n, string format, params object[] args)
		{
			return Format(n, String.Format(format, args));
		}

		/// <summary>
		/// Formata um texto.
		/// </summary>
		/// <param name="n">O número que deve ser verificado se é singular ou plural.</param>
		///	<param name="provider">O <see cref="IFormatProvider"/> a ser utilizado na formatação.</param>
		/// <param name="format">A string de formatação do texto</param>
		/// <param name="args">Os argumentos de formatação do texto</param>
		/// <returns>O texto, formatado de acordo com o número</returns>
		public static string Format(long n, IFormatProvider provider, string format, params object[] args)
		{
			return Format(n, String.Format(provider, format, args));
		}

		private class Replacer
		{
			int p;
			public Replacer(long n)
			{
				if (n == 1) p = 1;
				if (n >= 2) p = 2;
				if (n == 0) p = 3;
			}

			public string Replace(Match m) 
			{
				int c = p;
				if (!m.Groups[c].Success)
					c = 1;
				return m.Groups[c].Value;
			}
		}
	}
}
