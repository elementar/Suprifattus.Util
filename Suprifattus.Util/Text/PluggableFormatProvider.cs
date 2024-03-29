using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Suprifattus.Util.Text.Formatters;

namespace Suprifattus.Util.Text
{
	/// <summary>
	/// Provedor de formata��o que permite que sejam registrados formatadores personalizados,
	/// abstraindo as complexidades de <see cref="ICustomFormatter"/> e <see cref="IFormatProvider"/>.
	/// </summary>
	public sealed class PluggableFormatProvider : ICustomFormatter, IFormatProvider
	{
		/// <summary>
		/// A �nica inst�ncia do formatador.
		/// </summary>
		public static readonly PluggableFormatProvider Instance = new PluggableFormatProvider();

		private static readonly Dictionary<string, IFormatterPlugin> plugins = new Dictionary<string, IFormatterPlugin>();

		static PluggableFormatProvider()
		{
			RegisterFormatPlugin(new CNPJFormatter());
			RegisterFormatPlugin(new CPFFormatter());
			RegisterFormatPlugin(new EnumFormatter());
			RegisterFormatPlugin(new WikiFormatter());
			RegisterFormatPlugin(new NoAccentsFormatter());
		}

		/// <summary>
		/// Registra um novo plugin de formata��o, se ele ainda n�o estiver registrado.
		/// </summary>
		/// <param name="plugin">O plugin</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static bool TryRegisterFormatPlugin(IFormatterPlugin plugin)
		{
			if (plugins.ContainsKey(plugin.FormatKey))
				return false;

			RegisterFormatPlugin(plugin);
			return true;
		}

		/// <summary>
		/// Registra um novo plugin de formata��o.
		/// </summary>
		/// <param name="plugin">O plugin</param>
		/// <exception cref="ArgumentException">Se o plugin j� estiver registrado.</exception>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void RegisterFormatPlugin(IFormatterPlugin plugin)
		{
			if (plugin == null)
				throw new ArgumentNullException("plugin");

			plugins.Add(plugin.FormatKey, plugin);
		}

		/// <summary>
		/// Registra um novo plugin de formata��o, com uma chave de formata��o diferente.
		/// </summary>
		/// <param name="formatKey">A chave de formata��o</param>
		/// <param name="plugin">O plugin</param>
		/// <exception cref="ArgumentException">Se o plugin j� estiver registrado.</exception>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void RegisterFormatPlugin(string formatKey, IFormatterPlugin plugin)
		{
			plugins.Add(formatKey, plugin);
		}

		object IFormatProvider.GetFormat(Type formatType)
		{
			return typeof(ICustomFormatter).Equals(formatType) ? this : null;
		}

		/// <summary>
		/// Realiza a formata��o, conforme <see cref="ICustomFormatter"/>.
		/// </summary>
		/// <param name="formatString">A string de formata��o</param>
		/// <param name="arg">O objeto a ser formatado</param>
		/// <param name="formatProvider">O provedor de formata��o atual</param>
		/// <returns>A string que � o resultado da formata��o.</returns>
		public string Format(string formatString, object arg, IFormatProvider formatProvider)
		{
			if (arg == null)
				return null;

			if (formatString != null)
			{
				// tenta encontar o nome completo do plugin na hashtable
				IFormatterPlugin plugIn;
				if (plugins.TryGetValue(formatString, out plugIn))
					return plugIn.Format(formatString, arg);

				// tenta encontrar um que tenha o in�cio igual
				foreach (var entry in plugins)
					if (formatString.StartsWith(entry.Key))
						return entry.Value.Format(formatString, arg);
			}

			// n�o encontrou nenhum customizado, usa os padr�es
			if (arg is IFormattable)
				return ((IFormattable) arg).ToString(formatString, formatProvider);

			return arg.ToString();
		}
	}
}