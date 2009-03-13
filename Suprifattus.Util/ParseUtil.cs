using System;

namespace Suprifattus.Util
{
	/// <summary>
	/// Classe utilit�ria para parsing.
	/// </summary>
	public class ParseUtil
	{
		/// <summary>
		/// Realiza o parse de um n�mero inteiro.
		/// </summary>
		/// <param name="obj">O objeto a ser convertido para o n�mero inteiro</param>
		/// <param name="defaultValue">O valor padr�o, caso a convers�o n�o funcione</param>
		/// <returns>O n�mero inteiro</returns>
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
		/// Realiza o parse de um n�mero inteiro.
		/// </summary>
		/// <param name="obj">O objeto a ser convertido para o n�mero inteiro</param>
		/// <param name="defaultValue">O valor padr�o, caso a convers�o n�o funcione</param>
		/// <returns>O n�mero inteiro</returns>
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