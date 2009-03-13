using System;

namespace Suprifattus.Util.Logging
{
	/// <summary>
	/// Grava mensagens de log em uma saída qualquer.
	/// </summary>
	public interface ILogAppender
	{
		/// <summary>
		/// Grava uma mensagem de log.
		/// </summary>
		/// <param name="logger">O logger</param>
		/// <param name="entry">A entrada do log</param>
		/// <param name="formatter">O formatador</param>
		void Append(ILogger logger, ILogFormatter formatter, LogEntry entry);
	}
}