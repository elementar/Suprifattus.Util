using System;

namespace Suprifattus.Util.Logging.Impl
{
	/// <summary>
	/// Um <see cref="ILogAppender"/> que escreve os resultados no <see cref="System.Console"/>.
	/// </summary>
	public class ConsoleAppender : ILogAppender
	{
		/// <summary>
		/// Escreve o resultado de um <see cref="LogEntry"/> no <see cref="System.Console"/>.
		/// </summary>
		/// <param name="logger">O logger</param>
		/// <param name="formatter">O formatador</param>
		/// <param name="entry">A entrada no log</param>
		public void Append(ILogger logger, ILogFormatter formatter, LogEntry entry)
		{
			Console.WriteLine(formatter.Format(entry));
		}
	}
}