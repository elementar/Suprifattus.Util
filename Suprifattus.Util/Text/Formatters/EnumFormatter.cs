using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Suprifattus.Util.Text.Formatters
{
	/// <summary>
	/// Formata enumerações
	/// </summary>
	public class EnumFormatter : IFormatterPlugin
	{
		private static readonly Dictionary<Type, Dictionary<Enum, string>> enumNamesCache = new Dictionary<Type, Dictionary<Enum, string>>();

		/// <summary>
		/// A chave utilizada ao registrar o formatador.
		/// Para o <see cref="EnumFormatter"/>, ela é '<c>enum</c>'.
		/// </summary>
		public string FormatKey
		{
			get { return "enum"; }
		}

		/// <summary>
		/// Realiza a formatação.
		/// </summary>
		/// <param name="formatString">A string de formatação</param>
		/// <param name="arg">O objeto a ser formatado</param>
		/// <returns>Uma string contendo o objeto formatado</returns>
		public string Format(string formatString, object arg)
		{
			if (arg == null)
				return null;

			Type t = arg.GetType();
			if (!t.IsEnum)
				return arg.ToString();

			var enValues = GetEnumNames(t);

			string val;
			if (enValues.TryGetValue((Enum) arg, out val))
				val = arg.ToString();

			return val;
		}

		private Dictionary<Enum, string> GetEnumNames(Type t)
		{
			Dictionary<Enum, string> names;
			if (!enumNamesCache.TryGetValue(t, out names))
				enumNamesCache.Add(t, names = BuildEnumValuesMap(t));

			return names;
		}

		private Dictionary<Enum, string> BuildEnumValuesMap(Type t)
		{
			var enValues = new Dictionary<Enum, string>();
			foreach (FieldInfo fi in t.GetFields(BindingFlags.Static | BindingFlags.Public))
				foreach (DescriptionAttribute d in fi.GetCustomAttributes(typeof(DescriptionAttribute), false))
					enValues[(Enum) fi.GetValue(null)] = d.Description;
			return enValues;
		}
	}
}