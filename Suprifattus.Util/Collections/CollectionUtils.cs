using System;
using System.Collections;
using System.Globalization;
using System.Text;

using Suprifattus.Util.DesignPatterns;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Delegate utilizado no método <see cref="CollectionUtils.Join(IEnumerable, string, FormatDelegate)"/>.
	/// </summary>
	public delegate string FormatDelegate(object obj);

	/// <summary>
	/// Classe com métodos utilitários para lidar com coleções.
	/// </summary>
	public partial class CollectionUtils
	{
		public static object NullSafeGet(IList list, int index)
		{
			return (list != null && index >= 0 && index < list.Count ? list[index] : null);
		}

		/// <summary>
		/// Descobre qual das coleções tem a maior quantidade de itens.
		/// </summary>
		/// <param name="col1">Uma coleção</param>
		/// <param name="col2">Outra coleção</param>
		/// <param name="max">Receberá a coleção máxima</param>
		/// <param name="min">Receberá a colexão mínima</param>
		/// <returns>O número de elementos da maior coleção</returns>
		public static int MaxMin(ICollection col1, ICollection col2, out ICollection max, out ICollection min)
		{
			max = (col1.Count > col2.Count ? col1 : col2);
			min = (ReferenceEquals(max, col1) ? col2 : col1);

			return max.Count;
		}

		/// <summary>
		/// Concatena uma série de <see cref="IEnumerable"/> em um só.
		/// Veja também: <seealso cref="ConcatEnumerator"/>
		/// </summary>
		/// <param name="enumerables">Os <see cref="IEnumerable"/></param>
		/// <returns>Um <see cref="IEnumerable"/> que passa por todos os <see cref="IEnumerable"/> especificados</returns>
		public static IEnumerable Concat(params IEnumerable[] enumerables)
		{
			return new ConcatEnumerator(enumerables);
		}

		#region Join
		public static string Join(IDictionary dict, string keyValueSeparator, string entrySeparator)
		{
			if (dict == null)
				return null;

			var sb = new StringBuilder();
			foreach (DictionaryEntry de in dict)
				sb.Append(de.Key).Append(keyValueSeparator).Append(de.Value).Append(entrySeparator);
			if (sb.Length > 0 && entrySeparator != null)
				sb.Length -= entrySeparator.Length;

			return sb.ToString();
		}

		/// <summary>
		/// Junta os elementos de uma coleção em uma string, com o delimitador especificado.
		/// </summary>
		/// <param name="en">A coleção</param>
		/// <param name="delimiter">O delimitador</param>
		/// <param name="formatDelegate">O formatador</param>
		/// <returns>Uma string com a representação string de todos os objetos, separados pelo delimitador especificado</returns>
		public static string Join(IEnumerable en, string delimiter, FormatDelegate formatDelegate)
		{
			var sb = new StringBuilder();
			foreach (object obj in en)
				sb.AppendFormat("{0}", formatDelegate(obj)).Append(delimiter);
			if (sb.Length >= delimiter.Length)
				sb.Length -= delimiter.Length;
			return sb.ToString();
		}

		/// <summary>
		/// Junta os elementos de uma coleção em uma string, com o delimitador especificado.
		/// </summary>
		/// <param name="en">A coleção</param>
		/// <param name="delimiter">O delimitador</param>
		/// <param name="formatProvider">O formatador</param>
		/// <returns>Uma string com a representação string de todos os objetos, separados pelo delimitador especificado</returns>
		public static string Join(IEnumerable en, string delimiter, IFormatProvider formatProvider)
		{
			var sb = new StringBuilder();
			foreach (object obj in en)
				sb.AppendFormat(formatProvider, "{0}", obj).Append(delimiter);
			if (sb.Length >= delimiter.Length)
				sb.Length -= delimiter.Length;
			return sb.ToString();
		}

		/// <summary>
		/// Junta os elementos de uma coleção em uma string, com o delimitador especificado.
		/// </summary>
		/// <param name="en">A coleção</param>
		/// <param name="delimiter">O delimitador</param>
		/// <returns>Uma string com a representação string de todos os objetos, separados pelo delimitador especificado</returns>
		public static string Join(IEnumerable en, string delimiter)
		{
			var sb = new StringBuilder();
			foreach (object obj in en)
				sb.Append(obj).Append(delimiter);
			if (sb.Length >= delimiter.Length)
				sb.Length -= delimiter.Length;
			return sb.ToString();
		}

		/// <summary>
		/// Junta os elementos de uma coleção em uma string, com o delimitador padrão da cultura atual,
		/// conforme <see cref="CultureInfo.CurrentCulture"/>.
		/// </summary>
		/// <param name="en">A coleção</param>
		/// <returns>Uma string com a representação string de todos os objetos, separados pelo delimitador padrão</returns>
		public static string Join(IEnumerable en)
		{
			return Join(en, CultureInfo.CurrentCulture.TextInfo.ListSeparator);
		}
		#endregion

		#region ToArray
		/// <summary>
		/// Converte uma coleção qualquer para um vetor.
		/// </summary>
		/// <param name="returnType">O tipo do vetor de retorno</param>
		/// <param name="col">A coleção de origem</param>
		/// <returns>Um array do tipo <paramref name="returnType"/></returns>
		public static Array ToArray(Type returnType, ICollection col)
		{
			Array arr = Array.CreateInstance(returnType, col.Count);
			col.CopyTo(arr, 0);
			return arr;
		}

		/// <summary>
		/// Converte um enumerável qualquer para um vetor.
		/// </summary>
		/// <param name="returnType">O tipo do vetor de retorno</param>
		/// <param name="en">O enumerável</param>
		/// <returns>Um array do tipo <paramref name="returnType"/></returns>
		public static Array ToArray(Type returnType, IEnumerable en)
		{
			if (en is ICollection)
				return ToArray(returnType, (ICollection) en);

			var al = new ArrayList();
			for (IEnumerator e = en.GetEnumerator(); e.MoveNext();)
				al.Add(e.Current);

			return ToArray(returnType, al);
		}
		#endregion

		#region FillDictionary
		/// <summary>
		/// Preenche o dicionário especificado com novas instâncias do
		/// tipo especificado. As chaves de cada entrada são fornecidas
		/// pelo enumerável.
		/// </summary>
		public static void FillDictionary(IDictionary dictionary, IEnumerable enumerable, Type objectToCreate)
		{
			foreach (object key in enumerable)
				dictionary.Add(key, Activator.CreateInstance(objectToCreate));
		}

		/// <summary>
		/// Preenche o dicionário especificado com novas instâncias, criadas
		/// pelo factory especificado. As chaves de cada entrada são fornecidas
		/// pelo enumerável.
		/// </summary>
		public static void FillDictionary(IDictionary dictionary, IEnumerable enumerable, IObjectFactory factory)
		{
			foreach (object key in enumerable)
				dictionary.Add(key, factory.Create());
		}

		/// <summary>
		/// Preenche o dicionário especificado com o objeto especificado. 
		/// As chaves de cada entrada são fornecidas pelo enumerável.
		/// </summary>
		public static void FillDictionary(IDictionary dictionary, IEnumerable enumerable, object value)
		{
			foreach (object key in enumerable)
				dictionary.Add(key, value);
		}
		#endregion
	}
}