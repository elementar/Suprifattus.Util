using System;

namespace Suprifattus.Util
{
	/// <summary>
	/// Classe utilitária para parsing.
	/// </summary>
	public class ParseUtil
	{
		/// <summary>
		/// Realiza o parse de um número inteiro.
		/// </summary>
		/// <param name="obj">O objeto a ser convertido para o número inteiro</param>
		/// <param name="defaultValue">O valor padrão, caso a conversão não funcione</param>
		/// <returns>O número inteiro</returns>
		public static int ParseInt32(object obj, int defaultValue)
		{
			try
			{
				return Convert.ToInt32(obj);
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Realiza o parse de um número inteiro.
		/// </summary>
		/// <param name="obj">O objeto a ser convertido para o número inteiro</param>
		/// <param name="defaultValue">O valor padrão, caso a conversão não funcione</param>
		/// <returns>O número inteiro</returns>
		public static short ParseInt16(object obj, short defaultValue)
		{
			try
			{
				return Convert.ToInt16(obj);
			}
			catch
			{
				return defaultValue;
			}
		}
	}
}