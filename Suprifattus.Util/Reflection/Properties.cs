using System;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
#if GENERICS
	/// <summary>
	/// Classe para facilitar o uso de propriedades.
	/// </summary>
	[CLSCompliant(false)]
	public static class Properties
	{
		/// <summary>
		/// Busca o valor de uma propriedade de um objeto.
		/// </summary>
		/// <typeparam name="T">O tipo do objeto</typeparam>
		/// <typeparam name="P">O tipo do valor da propriedade</typeparam>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="obj">O objeto</param>
		/// <returns>O valor da propriedade <paramref name="propName"/> do objeto <paramref name="obj"/></returns>
		public static P GetValue<T, P>(string propName, T obj)
		{
			PropertyInfo pi = typeof(T).GetProperty(propName, typeof(P));

			return (P) pi.GetValue(obj, null);
		}

		/// <summary>
		/// Busca o valor de uma propriedade de um objeto.
		/// </summary>
		/// <typeparam name="P">O tipo do valor da propriedade</typeparam>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="obj">O objeto</param>
		/// <returns>O valor da propriedade <paramref name="propName"/> do objeto <paramref name="obj"/></returns>
		public static P GetValue<P>(string propName, object obj)
		{
			PropertyInfo pi = obj.GetType().GetProperty(propName, typeof(P));

			return (P) pi.GetValue(obj, null);
		}

		/// <summary>
		/// Busca o valor de uma propriedade de diversos objetos.
		/// </summary>
		/// <typeparam name="T">O tipo do objeto</typeparam>
		/// <typeparam name="P">O tipo do valor da propriedade</typeparam>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="objs">Os objetos</param>
		/// <returns>Um vetor com o valor das propriedades <paramref name="propName"/> dos objetos <paramref name="objs"/></returns>
		public static P[] GetValues<T, P>(string propName, params T[] objs)
		{
			PropertyInfo pi = typeof(T).GetProperty(propName, typeof(P));

			P[] vals = new P[objs.Length];
			for (int i = 0; i < objs.Length; i++)
				vals[i] = (P) pi.GetValue(objs[i], null);

			return vals;
		}

		/// <summary>
		/// Atribui um valor a uma propriedade de diversos objetos.
		/// </summary>
		/// <typeparam name="T">O tipo do objeto</typeparam>
		/// <typeparam name="P">O tipo do valor da propriedade</typeparam>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="value">O valor a ser atribuído às propriedades <paramref name="propName"/> dos objetos <paramref name="objs"/></param>
		/// <param name="objs">Os objetos</param>
		public static void SetValues<T, P>(string propName, P value, params T[] objs)
		{
			PropertyInfo pi = typeof(T).GetProperty(propName, typeof(P));

			foreach (T obj in objs)
				pi.SetValue(obj, value, null);
		}
	}
#else
	/// <summary>
	/// Classe para facilitar o uso de propriedades.
	/// </summary>
	public sealed class Properties
	{
		private Properties() 
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Busca o valor de uma propriedade de um objeto.
		/// </summary>
		/// <typeparam name="P">O tipo do valor da propriedade</typeparam>
		/// <param name="propName">O nome da propriedade</param>
		/// <param name="obj">O objeto</param>
		/// <returns>O valor da propriedade <paramref name="propName"/> do objeto <paramref name="obj"/></returns>
		public static object GetValue(string propName, object obj)
		{
			PropertyInfo pi = obj.GetType().GetProperty(propName);

			return pi.GetValue(obj, null);
		}
	}
#endif
}
