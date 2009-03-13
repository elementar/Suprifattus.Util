using System;
using System.ComponentModel;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// Classe para facilitar o uso de atributos.
	/// </summary>
	[Obsolete("Para consultar enumera��es, use o EnumDescriptor")]
	public class Attributes
	{
		/// <summary>
		/// Retorna a descri��o de um elemento de uma enumera��o.
		/// </summary>
		/// <param name="value">O elemento da enumera��o</param>
		/// <returns>A descri��o do elemento, ou o valor de <c>ToString</c>, caso n�o haja descri��o</returns>
		[Obsolete("Use o EnumDescriptor")]
		public static string GetDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attributes =
				(DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
		}
	}
}