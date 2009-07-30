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
	/// Delegate utilizado no método <see cref="CollectionUtils.Join(IEnumerable,string,FormatDelegate)"/>.
	/// </summary>
	public delegate string FormatDelegate<T>(T obj);

	/// <summary>
	/// Classe com métodos utilitários para lidar com coleções.
	/// </summary>
	[CLSCompliant(false)]
	public static partial class CollectionUtils
	{
		#region NullSafeGet
		/// <summary>
		/// Obtém um item da lista, ou NULL se estiver fora dos limites da lista.
		/// </summary>
		[Obsolete("Usar LINQ: collection.ElementAtOrDefault(index)")]
		public static T NullSafeGet<T>(ICollection collection, int index)
		{
			if (collection.Count == 0 || index >= collection.Count)
				return default(T);

			return NullSafeGet<T>((IEnumerable) collection, index);
		}

		/// <summary>
		/// Obtém um item da lista, ou NULL se estiver fora dos limites da lista.
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
		/// Obtém um item da lista, ou NULL se estiver fora dos limites da lista.
		/// </summary>
		[Obsolete("Usar LINQ: list.ElementAtOrDefault(index)")]
		public static T NullSafeGet<T>(IList list, int index)
		{
			return (T) (list != null && index >= 0 && index < list.Count ? list[index] : null);
		}

		/// <summary>
		/// Obtém um item da lista, ou NULL se estiver fora dos limites da lista.
		/// </summary>
		[Obsolete("Usar LINQ: list.ElementAtOrDefault(index)")]
		public static T NullSafeGet<T>(IList<T> list, int index)
		{
			return list.ElementAtOrDefault(index);
		}
		#endregion

		#region MaxMin
		/// <summary>
		/// Descobre qual das coleções é a máxima e qual é a mínima.
		/// </summary>
		/// <typeparam name="C">O tipo da coleção</typeparam>
		/// <param name="col1">Uma coleção</param>
		/// <param name="col2">Outra coleção</param>
		/// <param name="max">Receberá a coleção máxima</param>
		/// <param name="min">Receberá a colexão mínima</param>
		/// <returns>O número de elementos da maior coleção</returns>
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
		/// Retorna uma seção de um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor</typeparam>
		/// <param name="array">O vetor</param>
		/// <param name="start">A posição onde começar a seção.</param>
		/// <param name="length">O tamanho da seção.</param>
		/// <returns>Um novo vetor, contendo os valores contidos na seção solicitada.</returns>
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
		/// Retorna uma seção de um vetor, a partir da posição solicitada.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor</typeparam>
		/// <param name="array">O vetor</param>
		/// <param name="start">A posição onde começar a seção.</param>
		/// <returns>Um novo vetor, contendo os valores contidos da posição solicitada até o final do vetor.</returns>
		[Obsolete("Usar LINQ: array.Skip(start)")]
		public static T[] SubArray<T>(T[] array, int start)
		{
			return SubArray(array, start, array.Length - start);
		}
		#endregion

		#region ToArray
		/// <summary>
		/// Converte um enumerável qualquer em um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <param name="en">O enumerável de origem</param>
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
		/// Converte uma coleção qualquer para um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <param name="col">A coleção de origem</param>
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
		/// Converte uma coleção qualquer para um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <param name="col">A coleção de origem</param>
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
		/// Converte uma coleção qualquer para um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <typeparam name="I">O tipo da coleção de origem</typeparam>
		/// <param name="col">A coleção de origem</param>
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
		/// Converte uma coleção para um vetor de um novo tipo, através de um conversor.
		/// </summary>
		/// <typeparam name="T">Tipo da coleção de entrada</typeparam>
		/// <typeparam name="I">Tipo do vetor de saída</typeparam>
		/// <param name="input">A coleção de entrada</param>
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
		/// Converte uma coleção qualquer para um vetor.
		/// </summary>
		/// <typeparam name="T">O tipo do vetor de retorno</typeparam>
		/// <param name="col">A coleção de origem</param>
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
		/// Junta os elementos de uma coleção em uma string, com o delimitador especificado.
		/// </summary>
		/// <param name="en">A coleção</param>
		/// <param name="delimiter">O delimitador</param>
		/// <param name="formatDelegate">O formatador</param>
		/// <returns>Uma string com a representação string de todos os objetos, separados pelo delimitador especificado</returns>
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
		/// Agrupa os itens de uma coleção.
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