using System;
using System.Text.RegularExpressions;

namespace Suprifattus.Util.Text
{
	/// <summary>
	/// Classe utilit�ria para lidar com express�es em singular e plural.
	/// </summary>
	public sealed class PluralForm
	{
		static Regex rxTags = new Regex(@"\[ (?<singular>[^]:]*) (:(?<plural>[^]:]*) (:(?<zero>[^]:]*))? )? \]", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

		private PluralForm() { throw new InvalidOperationException(); }
		
		/// <summary>
		/// Formata um texto.
		/// </summary>
		/// <param name="n">O n�mero que deve ser verificado se � singular ou plural.</param>
		/// <param name="s">O texto a ser formatado</param>
		/// <returns>O texto, formatado de acordo com o n�mero</returns>
		public static string Format(long n, string s)
		{
			return rxTags.Replace(s, new MatchEvaluator(new Replacer(n).Replace));
		}

		/// <summary>
		/// Formata um texto.
		/// </summary>
		/// <param name="n">O n�mero que deve ser verificado se � singular ou plural.</param>
		/// <param name="format">A string de formata��o do texto</param>
		/// <param name="args">Os argumentos de formata��o do texto</param>
		/// <returns>O texto, formatado de acordo com o n�mero</returns>
		public static string Format(long n, string format, params object[] args)
		{
			return Format(n, String.Format(format, args));
		}

		/// <summary>
		/// Formata um texto.
		/// </summary>
		/// <param name="n">O n�mero que deve ser verificado se � singular ou plural.</param>
		///	<param name="provider">O <see cref="IFormatProvider"/> a ser utilizado na formata��o.</param>
		/// <param name="format">A string de formata��o do texto</param>
		/// <param name="args">Os argumentos de formata��o do texto</param>
		/// <returns>O texto, formatado de acordo com o n�mero</returns>
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
