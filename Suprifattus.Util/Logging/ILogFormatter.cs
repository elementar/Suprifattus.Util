using System;

namespace Suprifattus.Util.Logging
{
	/// <summary>
	/// Formata mensagens de log.
	/// </summary>
	public interface ILogFormatter
	{
		/// <summary>
		/// Formata uma entrada de log.
		/// </summary>
		/// <param name="entry">A entrada</param>
		/// <returns>A entrada, formatada</returns>
		string Format(LogEntry entry);
	}
}
