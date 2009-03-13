using System;
using System.Collections;
using System.Reflection;

using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// Realiza consultas em objetos e listas de objetos.
	/// </summary>
	[CLSCompliant(true)]
#if GENERICS
	public static partial class ObjectQuery
#else
	public sealed class ObjectQuery
#endif
	{
#if !GENERICS
		private ObjectQuery()
		{
			throw new NotImplementedException();
		}
#endif

		/// <summary>
		/// Seleciona um grupo de objetos de uma coleção, 
		/// de acordo com a condição especificada.
		/// </summary>
		/// <param name="returnType">O tipo de objeto de retorno</param>
		/// <param name="source">A coleção de origem</param>
		/// <param name="condition">A condição (pode ser uma condição múltipla)</param>
		/// <returns>Um vetor com apenas os elementos que atendem à condição em <paramref name="condition"/></returns>
		public static Array Select(Type returnType, IEnumerable source, Condition condition)
		{
			IList result = new ArrayList();
			foreach (object o in source)
				if (condition.Satisfied(o))
					result.Add(o);
			return CollectionUtils.ToArray(returnType, result);
		}

		/// <summary>
		/// Seleciona um grupo de objetos de uma coleção, 
		/// de acordo com a condição especificada.
		/// </summary>
		/// <param name="source">A coleção de origem</param>
		/// <param name="condition">A condição (pode ser uma condição múltipla)</param>
		/// <returns>Um vetor com apenas os elementos que atendem à condição em <paramref name="condition"/></returns>
		public static Array Select(IEnumerable source, Condition condition)
		{
			return Select(typeof(object), source, condition);
		}

		/// <summary>
		/// Seleciona um grupo de objetos de uma coleção, 
		/// de acordo com a condição especificada.
		/// </summary>
		/// <param name="returnType">O tipo utilizado no vetor de retorno.</param>
		/// <param name="source">A coleção de origem</param>
		/// <param name="recurseProperty">A propriedade que será consultada ao realizar a recursividade. Geralmente é a propriedade Controls ou Items.</param>
		/// <param name="condition">A condição (pode ser uma condição múltipla)</param>
		/// <returns>Um vetor com apenas os elementos que atendem à condição em <paramref name="condition"/></returns>
		public static Array SelectRecursive(Type returnType, IEnumerable source, string recurseProperty, Condition condition)
		{
			IList result = new ArrayList();
			foreach (object o in source)
			{
				PropertyInfo p = o.GetType().GetProperty(recurseProperty);
				if (p != null)
					foreach (object r in SelectRecursive(returnType, (IEnumerable) p.GetValue(o, null), recurseProperty, condition))
						result.Add(r);
				if (condition.Satisfied(o))
					result.Add(o);
			}
			return CollectionUtils.ToArray(returnType, result);
		}

		/// <summary>
		/// Seleciona um grupo de objetos de uma coleção, 
		/// de acordo com a condição especificada.
		/// </summary>
		/// <param name="source">A coleção de origem</param>
		/// <param name="recurseProperty">A propriedade que será consultada ao realizar a recursividade. Geralmente é a propriedade Controls ou Items.</param>
		/// <param name="condition">A condição (pode ser uma condição múltipla)</param>
		/// <returns>Um vetor com apenas os elementos que atendem à condição em <paramref name="condition"/></returns>
		public static IList SelectRecursive(IEnumerable source, string recurseProperty, Condition condition)
		{
			ArrayList result = new ArrayList();
			foreach (object o in source)
			{
				PropertyInfo p = o.GetType().GetProperty(recurseProperty);
				if (p != null)
					result.AddRange(SelectRecursive((IEnumerable) p.GetValue(o, null), recurseProperty, condition));
				if (condition.Satisfied(o))
					result.Add(o);
			}
			return result;
		}
	}
}
