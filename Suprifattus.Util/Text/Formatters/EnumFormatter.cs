using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Suprifattus.Util.Text.Formatters
{
	/// <summary>
	/// Formata enumerações
	/// </summary>
	public class EnumFormatter : IFormatterPlugin
	{
		static Hashtable enumNamesCache = new Hashtable();

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
				return null;
			
			Hashtable enValues = (Hashtable) enumNamesCache[t];

			if (enValues == null)
				enValues = BuildEnumValuesMap(t);

			string val = (string) enValues[arg];
			if (val == null)
				val = arg.ToString();

			return val;
		}

		private Hashtable BuildEnumValuesMap(Type t)
		{
			Hashtable enValues = new Hashtable();
			foreach (FieldInfo fi in t.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				foreach (DescriptionAttribute d in fi.GetCustomAttributes(typeof(DescriptionAttribute), false))
					enValues.Add(fi.GetValue(null), d.Description);
			}
			return enValues;
		}
	}
}