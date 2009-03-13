using System;
using System.Collections;
using System.Text.RegularExpressions;

using Suprifattus.Util.Reflection;
using Suprifattus.Util.Text;

namespace Suprifattus.Util
{
	/// <summary>
	/// Classe com métodos auxiliares para lidar com operações lógicas.
	/// </summary>
	public static class Logic
	{
		private static readonly Regex rxTrue = new Regex("^(-?1|y(es)?|t(rue)?|s(im)?|on)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex rxValidNumber = new Regex(@"^[-+]?\d+$", RegexOptions.Compiled);

		/// <summary>
		/// Realiza a formatação utilizando 
		/// <see cref="string.Format(IFormatProvider,string,object[])"/>,
		/// mas apenas se nenhum dos argumentos em <paramref name="args"/>
		/// for <c>null</c>. Se algum dos argumentos for <c>null</c>, 
		/// retorna <c>null</c>.
		/// </summary>
		/// <param name="format">A string de formatação</param>
		/// <param name="args">Os argumentos de formatação</param>
		/// <returns>A string formatada, ou <c>null</c> caso algum dos argumentos seja <c>null</c></returns>
		public static string FormatIfNoNulls(string format, params object[] args)
		{
			if (AnyNullOrEmpty(args))
				return null;

			return String.Format(PluggableFormatProvider.Instance, format, args);
		}

		#region Nullify, NullifyIfEqual
		/// <summary>
		/// Retorna <c>null</c> se a string for vazia,
		/// caso contrário, retorna a própria string.
		/// </summary>
		/// <param name="s">A string</param>
		/// <returns><c>null</c> se a string for vazia, 
		/// <paramref name="s"/> caso contrário</returns>
		public static string Nullify(string s)
		{
			return string.IsNullOrEmpty(s) ? null : s;
		}

		/// <summary>
		/// Retorna <c>null</c> se as duas strings forem idênticas,
		/// caso contrário, retorna a primeira string.
		/// </summary>
		/// <param name="s1">A primeira string</param>
		/// <param name="s2">A segunda string</param>
		/// <returns><c>null</c> se <paramref name="s1"/> for igual 
		/// a <paramref name="s2"/>, caso contrário <paramref name="s1"/>
		/// </returns>
		public static string NullifyIfEqual(string s1, string s2)
		{
			return s1 == null || s1.Equals(s2) ? null : s1;
		}

		/// <summary>
		/// Retorna <c>null</c> se a primeira string for idêntica
		/// a alguma das especificadas em seguida,
		/// caso contrário, retorna a primeira string.
		/// </summary>
		/// <param name="s">A primeira string</param>
		/// <param name="other">As outras strings</param>
		/// <returns><c>null</c> se <paramref name="s"/> for igual 
		/// a alguma das strings em <paramref name="other"/>, 
		/// caso contrário <paramref name="s"/>.
		/// </returns>
		public static string NullifyIfEqual(string s, params string[] other)
		{
			if (s == null)
				return null;

			foreach (string o in other)
				if (s.Equals(o)) return null;

			return s;
		}

		/// <summary>
		/// Retorna <c>null</c> se a primeira string for idêntica
		/// a alguma das especificadas em seguida,
		/// caso contrário, retorna a primeira string. A comparação
		/// é realizada utilizando o comparador especificado em 
		/// <paramref name="comp" />.
		/// </summary>
		/// <param name="comp">O comparador de strings</param>
		/// <param name="s">A primeira string</param>
		/// <param name="other">As outras strings</param>
		/// <returns><c>null</c> se <paramref name="s"/> for igual 
		/// a alguma das strings em <paramref name="other"/>, 
		/// caso contrário <paramref name="s"/>.
		/// </returns>
		public static string NullifyIfEqual(IComparer comp, string s, params string[] other)
		{
			if (s == null)
				return null;

			foreach (string o in other)
				if (comp.Compare(s, o) == 0) return null;

			return s;
		}
		#endregion

		#region Coalesce
		/// <summary>
		/// Retorna a primeira string não-nula da lista.
		/// </summary>
		public static string Coalesce(params string[] strings)
		{
			return (string) Coalesce((object[]) strings);
		}

		/// <summary>
		/// Retorna o primeiro objeto não-nulo da lista.
		/// </summary>
		public static object Coalesce(params object[] objs)
		{
			foreach (object obj in objs)
				if (obj != null)
					return obj;
			return null;
		}
		#endregion

		#region Swap
		/// <summary>
		/// Substitui um número pelo outro.
		/// </summary>
		public static void Swap(ref long n1, ref long n2)
		{
			long aux = n1;
			n1 = n2;
			n2 = aux;
		}

		/// <summary>
		/// Substitui um número pelo outro.
		/// </summary>
		public static void Swap(ref int n1, ref int n2)
		{
			int aux = n1;
			n1 = n2;
			n2 = aux;
		}

		/// <summary>
		/// Substitui um número pelo outro.
		/// </summary>
		public static void Swap(ref float n1, ref float n2)
		{
			float aux = n1;
			n1 = n2;
			n2 = aux;
		}
		#endregion

		#region StringEmpty
		/// <summary>
		/// Verifica se uma string está vazia.
		/// Uma string é vazia quando é nula ou tem largura zero.
		/// </summary>
		/// <param name="s">A string</param>
		/// <returns>Verdadeiro se for fazia, falso caso tenha conteúdo.</returns>
		public static bool StringEmpty(string s)
		{
			return String.IsNullOrEmpty(s);
		}

		/// <summary>
		/// Verifica se todas as strings estão vazias.
		/// Uma string é vazia quando é nula ou tem largura zero.
		/// </summary>
		/// <param name="strings">A string</param>
		/// <returns>Verdadeiro se todas as strings forem vazias, falso caso pelo menos uma tenha conteúdo.</returns>
		public static bool AllEmpty(params string[] strings)
		{
			if (strings == null || strings.Length == 0)
				return true;

			foreach (string s in strings)
				if (!StringEmpty(s))
					return false;

			return true;
		}

		/// <summary>
		/// Verifica se alguma das strings está vazia.
		/// Uma string é vazia quando é nula ou tem largura zero.
		/// </summary>
		/// <param name="strings">A string</param>
		/// <returns>Verdadeiro se uma das strings for vazia, falso caso todas tenham conteúdo.</returns>
		public static bool AnyEmpty(params string[] strings)
		{
			if (strings == null || strings.Length == 0)
				return false;

			foreach (string s in strings)
				if (StringEmpty(s))
					return true;

			return false;
		}

		/// <summary>
		/// Verifica se uma string está vazia.
		/// Uma string é vazia quando é nula ou tem largura zero.
		/// </summary>
		/// <param name="obj">O objeto que será convertido para string com <see cref="Convert.ToString(object)"/></param>
		/// <returns>Verdadeiro se for fazia, falso caso tenha conteúdo.</returns>
		public static bool StringEmpty(object obj)
		{
			return StringEmpty(Convert.ToString(obj));
		}
		#endregion

		/// <summary>
		/// Verifica se há algum item nulo na lista.
		/// </summary>
		/// <param name="args">A lista</param>
		/// <returns>
		/// <c>true</c> se houver algum item <c>null</c>, 
		/// <c>false</c> caso contrário
		/// </returns>
		public static bool AnyNull(params object[] args)
		{
			foreach (object o in args)
				if (o == null)
					return true;
			return false;
		}

		/// <summary>
		/// Verifica se há algum item nulo ou string vazia na lista.
		/// </summary>
		/// <param name="args">A lista</param>
		/// <returns>
		/// <c>true</c> se houver algum item <c>null</c> ou <see cref="String.Empty"/>, 
		/// <c>false</c> caso contrário
		/// </returns>
		public static bool AnyNullOrEmpty(params object[] args)
		{
			foreach (object o in args)
				if (o == null || (o is string && ((string) o).Length == 0))
					return true;
			return false;
		}

		#region AllTrue, AllFalse
		/// <summary>
		/// Testa se todos os booleanos da lista são verdadeiros.
		/// </summary>
		/// <param name="tests">Os testes a serem realizados.</param>
		/// <returns><c>true</c> se todos forem verdadeiros, <c>false</c> caso haja pelo menos um item falso</returns>
		public static bool AllTrue(params bool[] tests)
		{
			foreach (bool test in tests)
				if (!test)
					return false;
			return true;
		}

		/// <summary>
		/// Testa se todos os booleanos da lista são falsos.
		/// </summary>
		/// <param name="tests">Os testes a serem realizados.</param>
		/// <returns><c>true</c> se todos forem falsos, <c>false</c> caso haja pelo menos um item verdadeiro</returns>
		public static bool AllFalse(params bool[] tests)
		{
			foreach (bool test in tests)
				if (test)
					return false;
			return true;
		}
		#endregion

#if GENERICS
		/// <summary>
		/// Verifica se todos os objetos passados por parâmetro são iguais.
		/// </summary>
		/// <typeparam name="T">O tipo dos objetos utilizados na comparação</typeparam>
		/// <param name="obj">O objeto</param>
		/// <param name="items">Outros objetos do mesmo tipo de <paramref name="obj"/></param>
		/// <returns>Verdadeiro se todos os parâmetros são iguais, falso caso contrário</returns>
		[CLSCompliant(false)]
		public static bool AllEqual<T>(T obj, params T[] items)
		{
			bool compareAgainstNull = Equals(obj, default(T));

			foreach (T item in items)
			{
				if (compareAgainstNull && !Equals(item, default(T)))
					return false;
				if (Equals(item, default(T)) || !item.Equals(obj))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Verifica se todos os objetos fornecidos (nos parâmetros <paramref name="obj"/> e
		/// <paramref name="items"/>) tem o mesmo valor na propriedade <paramref name="propName"/>,
		/// do tipo <typeparamref name="P"/>.
		/// </summary>
		/// <typeparam name="T">O tipo dos objetos envolvidos</typeparam>
		/// <typeparam name="P">O tipo da propriedade</typeparam>
		/// <param name="obj">Um objeto</param>
		/// <param name="propName">O nome da propriedade dos objetos</param>
		/// <param name="items">Os outros objetos</param>
		/// <returns>Verdadeiro se os valores das propriedades de todos os objetos forem iguais, falso caso contrário</returns>
		[CLSCompliant(false)]
		public static bool AllEqual<T, P>(T obj, string propName, params T[] items)
		{
			P val = Properties.GetValue<T, P>(propName, obj);
			return AllEqual(val, propName, items);
		}

		/// <summary>
		/// Verifica se todos os objetos fornecidos (no parâmetro <paramref name="items"/>) 
		/// tem valor na propriedade <paramref name="propName"/>, do tipo <typeparamref name="P"/>,
		/// igual a <paramref name="val"/>.
		/// </summary>
		/// <typeparam name="T">O tipo dos objetos envolvidos</typeparam>
		/// <typeparam name="P">O tipo da propriedade</typeparam>
		/// <param name="val">O valor</param>
		/// <param name="propName">O nome da propriedade dos objetos</param>
		/// <param name="items">Os outros objetos</param>
		/// <returns>Verdadeiro se os valores das propriedades de todos os objetos forem iguais, falso caso contrário</returns>
		[CLSCompliant(false)]
		public static bool AllEqual<T, P>(P val, string propName, params T[] items)
		{
			P[] vals = Properties.GetValues<T, P>(propName, items);
			return AllEqual(val, vals);
		}
#endif

		/// <summary>
		/// Verifica se uma string representa um valor numérico.
		/// </summary>
		/// <param name="s">A string</param>
		/// <returns>
		/// <c>true</c> se a string contém um valor numérico, <c>false</c> caso contrário.
		/// </returns>
		public static bool IsNumeric(string s)
		{
			return !String.IsNullOrEmpty(s) && rxValidNumber.IsMatch(s);
		}

		#region RepresentsTrue
		/// <summary>
		/// Verifica se a string representa o valor "verdadeiro".
		/// Valores possíveis são:
		/// 1, -1, y, yes, t, true, s, sim, on
		/// </summary>
		public static bool RepresentsTrue(string s)
		{
			return s != null && rxTrue.IsMatch(s);
		}

		/// <summary>
		/// Verifica se a string representa o valor "verdadeiro".
		/// Valores possíveis são:
		/// 1, -1, y, yes, t, true, s, sim, on
		/// </summary>
		public static bool RepresentsTrue(object obj)
		{
			if (NullableHelper.IsNull(obj))
				return false;

			obj = NullableHelper.GetValue(obj);
			if (obj is bool)
				return (bool) obj;
			if (obj.GetType().IsPrimitive)
				obj = Convert.ToString(obj);
			if (obj is string)
				return !"".Equals(obj) && RepresentsTrue((string) obj);

			// qualquer outro objeto não-null representa verdadeiro
			return !NullableHelper.IsNull(obj);
		}
		#endregion
	}
}