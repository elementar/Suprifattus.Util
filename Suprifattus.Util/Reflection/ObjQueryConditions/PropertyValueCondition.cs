using System;
using System.Diagnostics;
#if GENERICS
using System.Collections.Generic;
#endif
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
#if GENERICS
	/// <summary>
	/// Condição por valor de propriedade: verifica se o valor da propriedade
	/// especificada é igual ao valor da propriedade.
	/// </summary>
	[CLSCompliant(false)]
	public class PropertyCondition : PropertyCondition<object>
	{
		/// <summary>
		/// Cria uma nova condição por valor de propriedade.
		/// </summary>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="propValue">O valor da propriedade</param>
		public PropertyCondition(string propName, object propValue) : base(propName, propValue) { }
	}

	/// <summary>
	/// Condição por valor de propriedade: verifica se o valor da propriedade
	/// especificada é igual ao valor da propriedade.
	/// </summary>
	/// <typeparam name="P">O tipo da propriedade</typeparam>
	[CLSCompliant(false)]
	public class PropertyCondition<P> : Condition
	{
		P propValue;
		string propName;

		/// <summary>
		/// Cria uma nova condição por valor de propriedade.
		/// </summary>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="propValue">O valor da propriedade</param>
		public PropertyCondition(string propName, P propValue)
		{
			this.propName = propName;
			this.propValue = propValue;
		}

		/// <summary>
		/// Verifica se esta condição é satisfeita para o objeto <paramref name="obj"/>
		/// </summary>
		/// <param name="obj">O objeto a ser verificado</param>
		/// <returns>Verdadeiro se a condição é safisfeita, falso caso contrário</returns>
		public override bool Satisfied(object obj)
		{
			P val = Properties.GetValue<P>(propName, obj);
			return (val == null ? propValue == null : val.Equals(propValue));
		}
	}
}
#else
	/// <summary>
	/// Condição por valor de propriedade: verifica se o valor da propriedade
	/// especificada é igual ao valor da propriedade.
	/// </summary>
	public class PropertyCondition : Condition
	{
		object propValue;
		string propName;

		/// <summary>
		/// Cria uma nova condição por valor de propriedade.
		/// </summary>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="propValue">O valor da propriedade</param>
		public PropertyCondition(string propName, object propValue)
		{
			this.propName = propName;
			this.propValue = propValue;
		}

		/// <summary>
		/// Verifica se esta condição é satisfeita para o objeto <paramref name="obj"/>
		/// </summary>
		/// <param name="obj">O objeto a ser verificado</param>
		/// <returns>Verdadeiro se a condição é safisfeita, falso caso contrário</returns>
		public override bool Satisfied(object obj)
		{
			object val = Properties.GetValue(propName, obj);
			return (val == null ? propValue == null : val.Equals(propValue));
		}
	}
}
#endif
