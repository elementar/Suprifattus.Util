using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

using Castle.ActiveRecord;
using Castle.Windsor;

namespace Suprifattus.Util.Web.MonoRail
{
	public static class Utils
	{
		private static readonly Regex rxSimbolos = new Regex(@"\W", RegexOptions.Compiled);

		public static bool HasFlags<T>(T value, params T[] flags)
			where T: struct, IConvertible
		{
			var ci = CultureInfo.InvariantCulture;

			var longValue = value.ToInt64(ci);
			foreach (T flag in flags)
				if ((longValue & flag.ToInt64(ci)) == 0)
					return false;

			return true;
		}

		#region NullIf
		public static T? NullIf<T>(T? value, T nullValue)
			where T: struct, IEquatable<T>
		{
			return (value.HasValue && !nullValue.Equals(value.Value) ? value : null);
		}
		#endregion

		#region TryParse
		public static T? TryParse<T>(string value)
			where T: struct
		{
			return TryParse<T>(value, CultureInfo.CurrentCulture);
		}

		public static T? TryParse<T>(string value, IFormatProvider formatProvider)
			where T: struct
		{
			if (String.IsNullOrEmpty(value))
				return null;

			try
			{
				if (typeof(T).IsEnum)
					return (T?) Enum.Parse(typeof(T), value, true);

				return (T?) Convert.ChangeType(value, typeof(T), formatProvider);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static DateTime? TryParseDate(string value)
		{
			DateTime d;
			if (DateTime.TryParse(value, out d))
				return d;
			return null;
		}

		public static DateTime? TryParseDate(string value, string format)
		{
			DateTime d;
			if (DateTime.TryParseExact(value, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out d))
				return d;
			return null;
		}

		public static DateTime? TryParseDate(string value, IFormatProvider formatProvider, string format)
		{
			DateTime d;
			if (DateTime.TryParseExact(value, format, formatProvider, DateTimeStyles.None, out d))
				return d;
			return null;
		}
		#endregion

		#region GetAttribute
		public static T GetAttribute<T>(ICustomAttributeProvider attrProvider, bool inherit, bool throwIfMultiple)
			where T: Attribute
		{
			if (attrProvider == null)
				throw new ArgumentNullException("attrProvider", "Objeto a obter atributo não pode ser nulo");

			object[] attrs = attrProvider.GetCustomAttributes(typeof(T), inherit);
			if (attrs.Length == 0)
				return null;
			if (throwIfMultiple && attrs.Length > 1)
				throw new Exception("Mais de um atributo encontrado");
			return (T) attrs[0];
		}
		#endregion

		#region CopiaStream
		public static void CopiaStream(Stream ins, Stream outs)
		{
			var buffer = new byte[10240];
			int l;
			while ((l = ins.Read(buffer, 0, buffer.Length)) > 0)
				outs.Write(buffer, 0, l);
		}
		#endregion

		#region AtualizaRelacionamento
		/// <summary>
		/// Atualiza um relacionamento "N para N". Realiza a exclusão
		/// através do método <see cref="ICollection{T}.Remove"/>.
		/// </summary>
		/// <typeparam name="I">O tipo da tabela intermediária</typeparam>
		/// <typeparam name="F">O tipo do dado da outra ponta do relacionamento</typeparam>
		/// <param name="itens">A coleção que armazena os registros <typeparamref name="I"/>, da tabela intermediária</param>
		/// <param name="selecionados">Uma lista de elementos <typeparamref name="F"/>, da outra ponta do relacionamento</param>
		/// <param name="creator">Um delegate especificando a criação de novos registros na tabela intermediária</param>
		/// <param name="getter">Um delegate especificando a obtenção de um registro do tipo <typeparamref name="I"/> (tabela da outra ponta do relacionamento) a partir de um registro <typeparamref name="F"/> (tabela intermediária)</param>
		public static void AtualizaRelacionamento<I, F>(IList<I> itens, IEnumerable<F> selecionados,
		                                                Converter<F, I> creator, Converter<I, F> getter)
			where I: class, IRecord
			where F: class, IRecord
		{
			AtualizaRelacionamento(itens, selecionados, creator, getter, item => itens.Remove(item));
		}

		/// <summary>
		/// Atualiza um relacionamento "N para N".
		/// </summary>
		/// <typeparam name="I">O tipo da tabela intermediária</typeparam>
		/// <typeparam name="F">O tipo do dado da outra ponta do relacionamento</typeparam>
		/// <param name="itens">A coleção que armazena os registros <typeparamref name="I"/>, da tabela intermediária</param>
		/// <param name="selecionados">Uma lista de elementos <typeparamref name="F"/>, da outra ponta do relacionamento</param>
		/// <param name="creator">Um delegate especificando a criação de novos registros na tabela intermediária</param>
		/// <param name="getter">Um delegate especificando a obtenção de um registro do tipo <typeparamref name="I"/> (tabela da outra ponta do relacionamento) a partir de um registro <typeparamref name="F"/> (tabela intermediária)</param>
		/// <param name="delete">Um delegate especificando a exclusão de um registro da lista de itens</param>
		public static void AtualizaRelacionamento<I, F>(IList<I> itens, IEnumerable<F> selecionados,
		                                                Converter<F, I> creator, Converter<I, F> getter,
		                                                Action<I> delete)
			where I: class, IRecord
			where F: class, IRecord
		{
			if (itens == null)
				throw new ArgumentNullException("itens");
			if (creator == null)
				throw new ArgumentNullException("creator");
			if (getter == null)
				throw new ArgumentNullException("getter");
			if (selecionados == null)
				selecionados = new F[0];

			// preenche lista com todas as possibilidades de selecionados
			var todos = new List<F>(ActiveRecordMediator<F>.FindAll());

			// preenche mapa com os registros de relacionamento atuais
			var atuais = new Dictionary<F, I>();
			foreach (I item in itens)
				atuais.Add(getter(item), item);

			// cria novos registros selecionados novos, e previne exclusão de já existentes
			foreach (F selecionado in selecionados)
			{
				I item;
				if (!atuais.TryGetValue(selecionado, out item))
				{
					item = creator(selecionado);
					itens.Add(item);
					atuais.Add(selecionado, item);
				}
				todos.Remove(selecionado);
			}

			// exclui os relacionamentos já existentes
			foreach (F selecionado in todos)
			{
				I item;
				if (atuais.TryGetValue(selecionado, out item))
					delete(item);
			}
		}

		/// <summary>
		/// Atualiza um relacionamento N-para-N simples, sem tabela intermediária.
		/// </summary>
		public static void AtualizaRelacionamento<T>(IList<T> itens, IEnumerable<T> selecionados)
		{
			itens.Clear();
			foreach (T item in selecionados)
				itens.Add(item);
		}
		#endregion

		#region GetWindsor
		public static IWindsorContainer GetWindsor()
		{
			return ((IContainerAccessor) HttpContext.Current.ApplicationInstance).Container;
		}
		#endregion

		#region FieldComparer
		public class FieldComparer<R, F> : IComparer<R>
		{
			private readonly Converter<R, F> getter;

			public FieldComparer(Converter<R, F> getter)
			{
				this.getter = getter;
			}

			public int Compare(R x, R y)
			{
				return Comparer<F>.Default.Compare(getter(x), getter(y));
			}
		}
		#endregion

		#region NullIfEmpty
		/// <summary>
		/// Retorna <c>null</c> caso a string passada seja nula ou vazia.
		/// Caso contrário, retorna a mesma string recebida como parâmetro.
		/// </summary>
		/// <param name="s">A string</param>
		public static string NullIfEmpty(string s)
		{
			return String.IsNullOrEmpty(s) ? null : s;
		}

		/// <summary>
		/// Retorna <c>null</c> caso o objeto passado seja seu valor padrão.
		/// Caso contrário, retorna o mesmo objeto recebido como parâmetro.
		/// </summary>
		/// <param name="obj">O objeto</param>
		public static T? NullIfEmpty<T>(T obj)
			where T: struct
		{
			return !default(T).Equals(obj) ? obj : (T?) null;
		}

		/// <summary>
		/// Retorna <c>null</c> caso o objeto passado seja seu valor padrão.
		/// Caso contrário, retorna o mesmo objeto recebido como parâmetro.
		/// </summary>
		/// <param name="obj">O objeto</param>
		public static T? NullIfEmpty<T>(T? obj)
			where T: struct
		{
			return obj.HasValue && !default(T).Equals(obj.Value) ? obj : null;
		}
		#endregion

		#region CombinaData
		public static DateTime? CombinaData(DateTime? data, DateTime? hora)
		{
			if (data.HasValue && hora.HasValue)
				data = data.Value.AddHours(hora.Value.Hour).AddMinutes(hora.Value.Minute);
			return data;
		}
		#endregion

		#region GetOrCreate
		public static T GetOrCreate<T>(IDictionary items, string key)
			where T: new()
		{
			return (T) (items[key] ?? (items[key] = new T()));
		}

		public static T GetOrCreate<T, K>(IDictionary<K, T> items, K key)
			where T: new()
		{
			T value;
			if (!items.TryGetValue(key, out value))
				items.Add(key, value = new T());
			return value;
		}
		#endregion

		/// <summary>
		/// Remove todos os símbolos e espaços de uma string.
		/// Útil para armazenar campos com máscara.
		/// </summary>
		/// <param name="s">A string</param>
		public static string RemoveSimbolos(string s)
		{
			return String.IsNullOrEmpty(s) ? s : rxSimbolos.Replace(s, String.Empty);
		}

		public static void Sort<T, C>(T[] array, Converter<T, C> getter)
		{
			Array.Sort(array, (v1, v2) => Comparer<C>.Default.Compare(getter(v1), getter(v2)));
		}
	}
}