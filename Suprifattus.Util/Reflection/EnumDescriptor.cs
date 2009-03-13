using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// Descreve com detalhes uma enumera��o.
	/// </summary>
	public class EnumDescriptor
	{
		Type enumType;

		Dictionary<Enum, EnumValueDescriptor> values 
			= new Dictionary<Enum, EnumValueDescriptor>();

		/// <summary>
		/// Cria um descritor para a enumera��o especificada.
		/// </summary>
		/// <param name="enumType">O tipo da enumera��o</param>
		public EnumDescriptor(Type enumType)
		{
			this.enumType = enumType;

			BuildDescriptor();
		}
		
		private void BuildDescriptor()
		{
			foreach (FieldInfo fi in enumType.GetFields())
			{
				Enum v = (Enum) fi.GetValue(null);
				values.Add(v, new EnumValueDescriptor(enumType, fi, v));
			}
		}
		
		/// <summary>
		/// Obt�m o descritor de um valor espec�fico da enumera��o.
		/// </summary>
		/// <param name="value">O valor</param>
		/// <returns>O descritor</returns>
		public EnumValueDescriptor GetValueDescriptor(Enum value)
		{
			return values[value];
		}
		
		/// <summary>
		/// Obt�m o descritor de um valor espec�fico da enumera��o.
		/// </summary>
		/// <param name="value">O valor</param>
		/// <returns>O descritor</returns>
		public EnumValueDescriptor this[Enum value]
		{
			get { return GetValueDescriptor(value); }
		}
		
		/// <summary>
		/// Obt�m uma lista dos descritores de todos os valores desta enumera��o.
		/// </summary>
		public ICollection<EnumValueDescriptor> Values
		{
			get { return values.Values; }
		}
	}

	/// <summary>
	/// Descreve um valor de uma enumera��o.
	/// </summary>
	public class EnumValueDescriptor
	{
		Enum value;
		object basicValue;
		string name;
		string description;
		
		internal EnumValueDescriptor(Type enumType, FieldInfo fi, Enum v)
		{
			this.value = v;
			this.basicValue = Convert.ChangeType(v, Enum.GetUnderlyingType(enumType));
			this.name = fi.Name;

			foreach (DescriptionAttribute desc in enumType.GetCustomAttributes(typeof(DescriptionAttribute), false))
				description = desc.Description;
		}

		/// <summary>
		/// Obt�m o valor ao qual esta descri��o se refere.
		/// </summary>
		public Enum Value
		{
			get { return value; }
		}

		/// <summary>
		/// Obt�m o valor do tipo b�sico da enumera��o 
		/// descrita (geralmente <c>int</c>).
		/// </summary>
		public object BasicValue
		{
			get { return basicValue; }
		}

		/// <summary>
		/// Obt�m o nome do item da enumera��o.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Obt�m a descri��o textual do item da enumera��o,
		/// descrito atrav�s do atributo <see cref="DescriptionAttribute"/>.
		/// </summary>
		public string Description
		{
			get { return description; }
		}
	}
}