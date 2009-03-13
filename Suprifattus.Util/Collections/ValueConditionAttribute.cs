using System;
using System.Diagnostics;

using Suprifattus.Util.Reflection;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Atributo que encapsula as Condi��es definidas em Suprifattus.Util.Reflection,
	/// de modo a defin�-las no escopo de uma classe e depois apenas testar valores
	/// novos que apare�am.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ValueConditionAttribute : Attribute
	{
		private readonly Condition cond;
		private readonly string condId;

		private ValueConditionAttribute(string condId, Condition cond)
		{
			this.condId = condId;
			this.cond = cond;
		}

		/// <summary>
		/// Cria uma nova condi��o para um tipo de valor.
		/// </summary>
		/// <param name="condId">O ID da condi��o</param>
		/// <param name="t">O tipo permitido</param>
		public ValueConditionAttribute(string condId, Type t)
			: this(condId, new TypeCondition(t))
		{
		}

		/// <summary>
		/// Cria uma nova condi��o para um tipo de valor.
		/// </summary>
		/// <param name="t">O tipo permitido</param>
		public ValueConditionAttribute(Type t)
			: this(null, t)
		{
		}

		/// <summary>
		/// Cria uma nova condi��o para um valor de uma propriedade
		/// </summary>
		/// <param name="condId">O ID da condi��o</param>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="propValue">O valor da propriedade</param>
		public ValueConditionAttribute(string condId, string propName, object propValue)
			: this(condId, new PropertyCondition(propName, propValue))
		{
		}

		/// <summary>
		/// Cria uma nova condi��o para um valor de uma propriedade
		/// </summary>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="propValue">O valor da propriedade</param>
		public ValueConditionAttribute(string propName, object propValue)
			: this(null, propName, propValue)
		{
		}

		/// <summary>
		/// A condi��o que deve ser satisfeita para que os valores sejam aceitos.
		/// </summary>
		public Condition Condition
		{
			get { return cond; }
		}

		/// <summary>
		/// O ID da condi��o
		/// </summary>
		public string ConditionID
		{
			get { return condId; }
		}

		/// <summary>
		/// Verifica se um valor satisfaz as condi��es especificadas nos atributos definidos 
		/// na classe chamadora.
		/// </summary>
		/// <param name="condId">O ID da condi��o</param>
		/// <param name="val">O valor</param>
		/// <returns>Verdadeiro se o item satisfaz � condi��o, falso caso contr�rio</returns>
		public static bool CheckValue(string condId, object val)
		{
			var stack = new StackTrace(1);
			Type clazz = stack.GetFrame(0).GetMethod().DeclaringType;

			object[] attrs = clazz.GetCustomAttributes(typeof(ValueConditionAttribute), true);

			foreach (ValueConditionAttribute attr in attrs)
				if ((condId == null || attr.ConditionID == null || attr.ConditionID == condId) && !attr.Condition.Satisfied(val))
					return false;

			return true;
		}

		/// <summary>
		/// Verifica se um valor satisfaz a todas as condi��es especificadas nas classes 
		/// que declaram este atributo.
		/// </summary>
		/// <param name="val">O valor</param>
		/// <returns>Verdadeiro se o item satisfaz � condi��o, falso caso contr�rio</returns>
		public static bool CheckValue(object val)
		{
			return CheckValue(null, val);
		}
	}
}