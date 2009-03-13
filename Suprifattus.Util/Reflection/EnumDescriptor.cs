using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// Descreve com detalhes uma enumeração.
	/// </summary>
	public class EnumDescriptor
	{
		Type enumType;

		Dictionary<Enum, EnumValueDescriptor> values 
			= new Dictionary<Enum, EnumValueDescriptor>();

		/// <summary>
		/// Cria um descritor para a enumeração especificada.
		/// </summary>
		/// <param name="enumType">O tipo da enumeração</param>
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
		/// Obtém o descritor de um valor específico da enumeração.
		/// </summary>
		/// <param name="value">O valor</param>
		/// <returns>O descritor</returns>
		public EnumValueDescriptor GetValueDescriptor(Enum value)
		{
			return values[value];
		}
		
		/// <summary>
		/// Obtém o descritor de um valor específico da enumeração.
		/// </summary>
		/// <param name="value">O valor</param>
		/// <returns>O descritor</returns>
		public EnumValueDescriptor this[Enum value]
		{
			get { return GetValueDescriptor(value); }
		}
		
		/// <summary>
		/// Obtém uma lista dos descritores de todos os valores desta enumeração.
		/// </summary>
		public ICollection<EnumValueDescriptor> Values
		{
			get { return values.Values; }
		}
	}

	/// <summary>
	/// Descreve um valor de uma enumeração.
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
		/// Obtém o valor ao qual esta descrição se refere.
		/// </summary>
		public Enum Value
		{
			get { return value; }
		}

		/// <summary>
		/// Obtém o valor do tipo básico da enumeração 
		/// descrita (geralmente <c>int</c>).
		/// </summary>
		public object BasicValue
		{
			get { return basicValue; }
		}

		/// <summary>
		/// Obtém o nome do item da enumeração.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Obtém a descrição textual do item da enumeração,
		/// descrito através do atributo <see cref="DescriptionAttribute"/>.
		/// </summary>
		public string Description
		{
			get { return description; }
		}
	}
}