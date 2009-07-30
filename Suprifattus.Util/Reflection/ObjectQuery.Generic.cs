using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Suprifattus.Util.Collections;

using System.Linq;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// Realiza consultas em objetos e listas de objetos.
	/// </summary>
	partial class ObjectQuery
	{
		/// <summary>
		/// Seleciona um grupo de objetos de uma coleção, 
		/// de acordo com a condição especificada.
		/// </summary>
		/// <typeparam name="T">O tipo de objeto de retorno</typeparam>
		/// <param name="source">A coleção de origem</param>
		/// <param name="condition">A condição (pode ser uma condição múltipla)</param>
		/// <returns>Um vetor com apenas os elementos que atendem à condição em <paramref name="condition"/></returns>
		public static T[] Select<T>(ICollection source, Condition condition)
		{
			var result = new Collection<T>();
			foreach (object o in source)
				if (condition.Satisfied(o))
					result.Add((T) o);
			return result.ToArray();
		}
	}
}