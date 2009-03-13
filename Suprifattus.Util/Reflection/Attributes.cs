using System;
using System.ComponentModel;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// Classe para facilitar o uso de atributos.
	/// </summary>
	[Obsolete("Para consultar enumerações, use o EnumDescriptor")]
	public class Attributes
	{
		/// <summary>
		/// Retorna a descrição de um elemento de uma enumeração.
		/// </summary>
		/// <param name="value">O elemento da enumeração</param>
		/// <returns>A descrição do elemento, ou o valor de <c>ToString</c>, caso não haja descrição</returns>
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