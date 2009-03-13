using System;
using System.Collections;
using System.Globalization;
using System.Text;

using Suprifattus.Util.DesignPatterns;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Delegate utilizado no m�todo <see cref="CollectionUtils.Join(IEnumerable, string, FormatDelegate)"/>.
	/// </summary>
	public delegate string FormatDelegate(object obj);

	/// <summary>
	/// Classe com m�todos utilit�rios para lidar com cole��es.
	/// </summary>
	public partial class CollectionUtils
	{
		public static object NullSafeGet(IList list, int index)
		{
			return (list != null && index >= 0 && index < list.Count ? list[index] : null);
		}

		/// <summary>
		/// Descobre qual das cole��es tem a maior quantidade de itens.
		/// </summary>
		/// <param name="col1">Uma cole��o</param>
		/// <param name="col2">Outra cole��o</param>
		/// <param name="max">Receber� a cole��o m�xima</param>
		/// <param name="min">Receber� a colex�o m�nima</param>
		/// <returns>O n�mero de elementos da maior cole��o</returns>
		public static int MaxMin(ICollection col1, ICollection col2, out ICollection max, out ICollection min)
		{
			max = (col1.Count > col2.Count ? col1 : col2);
			min = (ReferenceEquals(max, col1) ? col2 : col1);

			return max.Count;
		}

		/// <summary>
		/// Concatena uma s�rie de <see cref="IEnumerable"/> em um s�.
		/// Veja tamb�m: <seealso cref="ConcatEnumerator"/>
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
		/// Junta os elementos de uma cole��o em uma string, com o delimitador especificado.
		/// </summary>
		/// <param name="en">A cole��o</param>
		/// <param name="delimiter">O delimitador</param>
		/// <param name="formatDelegate">O formatador</param>
		/// <returns>Uma string com a representa��o string de todos os objetos, separados pelo delimitador especificado</returns>
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
		/// Junta os elementos de uma cole��o em uma string, com o delimitador especificado.
		/// </summary>
		/// <param name="en">A cole��o</param>
		/// <param name="delimiter">O delimitador</param>
		/// <param name="formatProvider">O formatador</param>
		/// <returns>Uma string com a representa��o string de todos os objetos, separados pelo delimitador especificado</returns>
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
		/// Junta os elementos de uma cole��o em uma string, com o delimitador especificado.
		/// </summary>
		/// <param name="en">A cole��o</param>
		/// <param name="delimiter">O delimitador</param>
		/// <returns>Uma string com a representa��o string de todos os objetos, separados pelo delimitador especificado</returns>
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
		/// Junta os elementos de uma cole��o em uma string, com o delimitador padr�o da cultura atual,
		/// conforme <see cref="CultureInfo.CurrentCulture"/>.
		/// </summary>
		/// <param name="en">A cole��o</param>
		/// <returns>Uma string com a representa��o string de todos os objetos, separados pelo delimitador padr�o</returns>
		public static string Join(IEnumerable en)
		{
			return Join(en, CultureInfo.CurrentCulture.TextInfo.ListSeparator);
		}
		#endregion

		#region ToArray
		/// <summary>
		/// Converte uma cole��o qualquer para um vetor.
		/// </summary>
		/// <param name="returnType">O tipo do vetor de retorno</param>
		/// <param name="col">A cole��o de origem</param>
		/// <returns>Um array do tipo <paramref name="returnType"/></returns>
		public static Array ToArray(Type returnType, ICollection col)
		{
			Array arr = Array.CreateInstance(returnType, col.Count);
			col.CopyTo(arr, 0);
			return arr;
		}

		/// <summary>
		/// Converte um enumer�vel qualquer para um vetor.
		/// </summary>
		/// <param name="returnType">O tipo do vetor de retorno</param>
		/// <param name="en">O enumer�vel</param>
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
		/// Preenche o dicion�rio especificado com novas inst�ncias do
		/// tipo especificado. As chaves de cada entrada s�o fornecidas
		/// pelo enumer�vel.
		/// </summary>
		public static void FillDictionary(IDictionary dictionary, IEnumerable enumerable, Type objectToCreate)
		{
			foreach (object key in enumerable)
				dictionary.Add(key, Activator.CreateInstance(objectToCreate));
		}

		/// <summary>
		/// Preenche o dicion�rio especificado com novas inst�ncias, criadas
		/// pelo factory especificado. As chaves de cada entrada s�o fornecidas
		/// pelo enumer�vel.
		/// </summary>
		public static void FillDictionary(IDictionary dictionary, IEnumerable enumerable, IObjectFactory factory)
		{
			foreach (object key in enumerable)
				dictionary.Add(key, factory.Create());
		}

		/// <summary>
		/// Preenche o dicion�rio especificado com o objeto especificado. 
		/// As chaves de cada entrada s�o fornecidas pelo enumer�vel.
		/// </summary>
		public static void FillDictionary(IDictionary dictionary, IEnumerable enumerable, object value)
		{
			foreach (object key in enumerable)
				dictionary.Add(key, value);
		}
		#endregion
	}
}