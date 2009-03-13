using System;
using System.Diagnostics;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// Condição por tipo: verifica se o tipo do objeto é o tipo fornecido.
	/// </summary>
	public class TypeCondition : Condition
	{
		Type t;

		/// <summary>
		/// Cria uma nova condição por tipo.
		/// </summary>
		/// <param name="t"></param>
		public TypeCondition(Type t)
		{
			this.t = t;
		}

		/// <summary>
		/// Verifica se esta condição é satisfeita para o objeto <paramref name="obj"/>
		/// </summary>
		/// <param name="obj">O objeto a ser verificado</param>
		/// <returns>Verdadeiro se a condição é safisfeita, falso caso contrário</returns>
		public override bool Satisfied(object obj)
		{
			return t.IsInstanceOfType(obj);
		}
	}
}
