using System;
using System.Collections;

using Suprifattus.Util.Text.Formatters;

namespace Suprifattus.Util.Text
{
	/// <summary>
	/// Provedor de formatação que permite que sejam registrados formatadores personalizados,
	/// abstraindo as complexidades de <see cref="ICustomFormatter"/> e <see cref="IFormatProvider"/>.
	/// </summary>
	public sealed class PluggableFormatProvider : ICustomFormatter, IFormatProvider
	{
		/// <summary>
		/// A única instância do formatador.
		/// </summary>
		public static readonly PluggableFormatProvider Instance = new PluggableFormatProvider();

		private static readonly Hashtable plugins = new Hashtable();

		static PluggableFormatProvider()
		{
			RegisterFormatPlugin(new CNPJFormatter());
			RegisterFormatPlugin(new CPFFormatter());
			RegisterFormatPlugin(new EnumFormatter());
			RegisterFormatPlugin(new WikiFormatter());
			RegisterFormatPlugin(new NoAccentsFormatter());
		}

		/// <summary>
		/// Registra um novo plugin de formatação, se ele ainda não estiver registrado.
		/// </summary>
		/// <param name="plugin">O plugin</param>
		public static bool TryRegisterFormatPlugin(IFormatterPlugin plugin)
		{
			if (plugins.ContainsKey(plugin.FormatKey))
				return false;

			RegisterFormatPlugin(plugin);
			return true;
		}

		/// <summary>
		/// Registra um novo plugin de formatação.
		/// </summary>
		/// <param name="plugin">O plugin</param>
		/// <exception cref="ArgumentException">Se o plugin já estiver registrado.</exception>
		public static void RegisterFormatPlugin(IFormatterPlugin plugin)
		{
			if (plugin == null)
				throw new ArgumentNullException("plugin");

			plugins.Add(plugin.FormatKey, plugin);
		}

		/// <summary>
		/// Registra um novo plugin de formatação, com uma chave de formatação diferente.
		/// </summary>
		/// <param name="formatKey">A chave de formatação</param>
		/// <param name="plugin">O plugin</param>
		/// <exception cref="ArgumentException">Se o plugin já estiver registrado.</exception>
		public static void RegisterFormatPlugin(string formatKey, IFormatterPlugin plugin)
		{
			plugins.Add(formatKey, plugin);
		}

		object IFormatProvider.GetFormat(Type formatType)
		{
			if (typeof(ICustomFormatter).Equals(formatType))
				return this;

			return null;
		}

		/// <summary>
		/// Realiza a formatação, conforme <see cref="ICustomFormatter"/>.
		/// </summary>
		/// <param name="formatString">A string de formatação</param>
		/// <param name="arg">O objeto a ser formatado</param>
		/// <param name="formatProvider">O provedor de formatação atual</param>
		/// <returns>A string que é o resultado da formatação.</returns>
		public string Format(string formatString, object arg, IFormatProvider formatProvider)
		{
			if (arg == null)
				return null;

			if (formatString != null)
			{
				// tenta encontar o nome completo do plugin na hashtable
				var plugIn = (IFormatterPlugin) plugins[formatString];
				if (plugIn != null)
					return plugIn.Format(formatString, arg);

				// tenta encontrar um que tenha o início igual
				foreach (DictionaryEntry entry in plugins)
					if (formatString.StartsWith(entry.Key.ToString()))
						return ((IFormatterPlugin) entry.Value).Format(formatString, arg);
			}

			// não encontrou nenhum customizado, usa os padrões
			if (arg is IFormattable)
				return ((IFormattable) arg).ToString(formatString, formatProvider);
			
			return arg.ToString();
		}
	}
}