#if GENERICS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Delegate utilizado no m�todo <see cref="CollectionUtils.Join(IEnumerable,string,FormatDelegate)"/>.
	/// </summary>
	public delegate string FormatDelegate<T>(T obj);

	/// <summary>
	/// Classe com m�todos utilit�rios para lidar com cole��es.
	/// </summary>
	[CLSCompliant(false)]
	public static partial class CollectionUtils
	{
		#region NullSafeGet
		/// <summary>
		/// Obt�m um item da lista, ou NULL se estiver fora dos limites da lista.
		/// </summary>
		[Obsolete("Usar LINQ: collection.ElementAtOrDefault(index)")]
		public static T NullSafeGet<T>(ICollection collection, int index)
		{
			if (collection.Count == 0 || index >= collection.Count)
				return default(T);

			return NullSafeGet<T>((IEnumerable) collection, index);
		}

		/// <summary>
		/// Obt�m um item da lista, ou NULL se estiver fora dos limites da lista.
		/// </summary>
		[Obsolete("Usar LINQ: list.ElementAtOrDefault(index)")]
		public static T NullSafeGet<T>(IEnumerable list, int index)
		{
			foreach (T obj in list)
				if (index-- == 0)
					return obj;
			return default(T);
		}

		/// <summary>
		/// Obt�m um item da lista, ou NULL se estiver fora dos limites da lista.
		/// </summary>
		[Obsolete("Usar LINQ: list.ElementAtOrDefault(index)")]
		public static T NullSafeGet<T>(IList list, int index)
		{
			return (T) (list != null && index >= 0 && index < list.Count ? list[index] : null);
		}

		/// <summary>
		/// Obt�m um item da lista, ou NULL se estiver fora dos limites da lista.
		/// </summary>
		[Obsolete("Usar LINQ: list.ElementAtOrDefault(index)")]
		public static T NullSafeGet<T>(IList<T> list, int index)
		{
			return list.ElementAtOrDefault(index);
		}
		#endregion

		#region MaxMin
		/// <summary>
		/// Descobre qual das cole��es � a m�xima e qual � a m�nima.
		/// </summary>
		/// <typeparam name="C">O tipo da cole��o</typeparam>
		/// <param name="col1">Uma cole��o</param>
		/// <param name="col2">Outra cole��o</param>
		/// <param name="max">Receber� a cole��o m�xima</param>
		/// <param name="min">Receber� a colex�o m�nima</param>
		/// <returns>O n�mero de elementos da maior cole��o</returns>
		public static int MaxMin<C>(C col1, C col2, out C max, out C min)
			where C: ICollection
		{
			max = (col1.Count > col2.Count ? col1 : col2);
			min = (ReferenceEquals(max, col1) ? col2 : col1);

			return max.Count;
		}
		#endregion

		#region SubArray
		/// <summary>
		/// Retorna uma se��o de um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor</typeparam>
		/// <param name="array">O vetor</param>
		/// <param name="start">A posi��o onde come�ar a se��o.</param>
		/// <param name="length">O tamanho da se��o.</param>
		/// <returns>Um novo vetor, contendo os valores contidos na se��o solicitada.</returns>
		[Obsolete("Usar LINQ: array.Skip(start).Take(length)")]
		public static T[] SubArray<T>(T[] array, int start, int length)
		{
			if (start + length > array.Length)
				throw new IndexOutOfRangeException();

			var newArray = new T[length];
			Array.Copy(array, start, newArray, 0, length);

			return newArray;
		}

		/// <summary>
		/// Retorna uma se��o de um vetor, a partir da posi��o solicitada.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor</typeparam>
		/// <param name="array">O vetor</param>
		/// <param name="start">A posi��o onde come�ar a se��o.</param>
		/// <returns>Um novo vetor, contendo os valores contidos da posi��o solicitada at� o final do vetor.</returns>
		[Obsolete("Usar LINQ: array.Skip(start)")]
		public static T[] SubArray<T>(T[] array, int start)
		{
			return SubArray(array, start, array.Length - start);
		}
		#endregion

		#region ToArray
		/// <summary>
		/// Converte um enumer�vel qualquer em um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <param name="en">O enumer�vel de origem</param>
		/// <returns>Um array do tipo <typeparamref name="T"/></returns>
		[Obsolete("Usar LINQ: en.ToArray()")]
		public static T[] ToArray<T>(IEnumerable<T> en)
		{
			return ToArray(en.GetEnumerator());
		}

		/// <summary>
		/// Converte um enumerador qualquer em um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <param name="en">O enumerador de origem</param>
		/// <returns>Um array do tipo <typeparamref name="T"/></returns>
		[Obsolete("Usar LINQ: en.ToArray()")]
		public static T[] ToArray<T>(IEnumerator<T> en)
		{
			var col = new Collection<T>();
			while (en.MoveNext())
				col.Add(en.Current);
			return ToArray<T>((ICollection) col);
		}

		/// <summary>
		/// Converte uma cole��o qualquer para um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <param name="col">A cole��o de origem</param>
		/// <returns>Um array do tipo <typeparamref name="T"/></returns>
		[Obsolete("Usar LINQ: col.Cast<T>.ToArray()")]
		public static T[] ToArray<T>(ICollection col)
		{
			if (col is List<T>)
				return ((List<T>) col).ToArray();

			var arr = new T[col.Count];
			col.CopyTo(arr, 0);
			return arr;
		}

		/// <summary>
		/// Converte uma cole��o qualquer para um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <param name="col">A cole��o de origem</param>
		/// <returns>Um array do tipo <typeparamref name="T"/></returns>
		[Obsolete("Usar LINQ: col.ToArray()")]
		public static T[] ToArray<T>(ICollection<T> col)
		{
			if (col is T[])
				return (T[]) col;
			if (col is List<T>)
				return ((List<T>) col).ToArray();

			var arr = new T[col.Count];
			col.CopyTo(arr, 0);
			return arr;
		}

		/// <summary>
		/// Converte uma cole��o qualquer para um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <typeparam name="I">O tipo da cole��o de origem</typeparam>
		/// <param name="col">A cole��o de origem</param>
		/// <returns>Um array do tipo <typeparamref name="T"/></returns>
		[Obsolete("Usar LINQ: col.Cast<T>.ToArray()")]
		public static T[] ToArray<T, I>(ICollection<I> col)
			where I: T
		{
			var arr = new T[col.Count];
			var i = 0;
			foreach (I item in col)
				arr[i++] = item;
			return arr;
		}

		/// <summary>
		/// Converte uma cole��o para um vetor de um novo tipo, atrav�s de um conversor.
		/// </summary>
		/// <typeparam name="T">Tipo da cole��o de entrada</typeparam>
		/// <typeparam name="I">Tipo do vetor de sa�da</typeparam>
		/// <param name="input">A cole��o de entrada</param>
		/// <param name="converter">O conversor de <typeparamref name="I"/> para <typeparamref name="T"/></param>
		[Obsolete("Usar LINQ: input.Select(i => ...).ToArray()")]
		public static T[] ToArray<T, I>(IEnumerable<I> input, Converter<I, T> converter)
		{
			var result = new List<T>();
			foreach (I item in input)
				result.Add(converter(item));
			return result.ToArray();
		}
		#endregion

		#region ConvertAll
		/// <summary>
		/// Converte uma cole��o qualquer para um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <param name="col">A cole��o de origem</param>
		/// <returns>Um array do tipo <typeparamref name="T"/></returns>
		[Obsolete("Usar LINQ: col.Cast<object>().Select(item => (T) Convert.ChangeType(item, typeof(T))).ToArray()")]
		public static T[] ConvertAll<T>(ICollection col)
			where T: IConvertible
		{
			var arr = new T[col.Count];
			var i = 0;
			foreach (var item in col)
				arr[i++] = (T) Convert.ChangeType(item, typeof(T));
			return arr;
		}
		#endregion

		#region Join
		/// <summary>
		/// Junta os elementos de uma cole��o em uma string, com o delimitador especificado.
		/// </summary>
		/// <param name="en">A cole��o</param>
		/// <param name="delimiter">O delimitador</param>
		/// <param name="formatDelegate">O formatador</param>
		/// <returns>Uma string com a representa��o string de todos os objetos, separados pelo delimitador especificado</returns>
		[Obsolete("Usar interface 3.5: en.StringJoin(delimiter, s => ...)")]
		public static string Join<T>(IEnumerable<T> en, string delimiter, FormatDelegate<T> formatDelegate)
		{
			String.Join(delimiter, en.Select(s => formatDelegate(s)).ToArray());
			var sb = new StringBuilder();
			foreach (var obj in en)
				sb.AppendFormat("{0}", formatDelegate(obj)).Append(delimiter);
			if (sb.Length >= delimiter.Length)
				sb.Length -= delimiter.Length;
			return sb.ToString();
		}

		public static string StringJoin<T>(this IEnumerable<T> en, string delimiter, Func<T, string> converter)
		{
			return String.Join(delimiter, en.Select(converter).ToArray());
		}
		#endregion

		#region Filter
		/// <summary>
		/// Filtra um <see cref="IEnumerable{T}"/>
		/// </summary>
		[Obsolete("Usar LINQ: items.Where(i => ...)")]
		public static IEnumerable<T> Filter<T>(IEnumerable<T> items, Predicate<T> filterCriteria)
		{
			foreach (T item in items)
				if (filterCriteria(item))
					yield return item;
		}
		#endregion

		#region Group
		/// <summary>
		/// Agrupa os itens de uma cole��o.
		/// </summary>
		public static Dictionary<G, List<V>> Group<V, G>(this IEnumerable<V> itens, Func<V, G> getter)
		{
			var dict = new Dictionary<G, List<V>>();

			foreach (V val in itens)
			{
				var g = getter(val);
				List<V> list;
				if (!dict.TryGetValue(g, out list))
					dict.Add(g, list = new List<V>());
				list.Add(val);
			}

			return dict;
		}
		#endregion

		#region AddRange
		public static void AddRange<T>(this ICollection<T> col, IEnumerable<T> items)
		{
			foreach (var item in items)
				col.Add(item);
		}

		public static void AddRange<T>(this ICollection<T> col, params T[] items)
		{
			AddRange(col, (IEnumerable<T>) items);
		}
		#endregion

		public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, V def)
		{
			V val;
			return dict.TryGetValue(key, out val) ? val : def;
		}
	}
}

#endif