using System;
using System.IO;

namespace Suprifattus.Util.Logging.Impl
{
	/// <summary>
	/// Um <see cref="ILogAppender"/> que grava os resultados em um <see cref="TextWriter"/>.
	/// </summary>
	public class TextWriterAppender : ILogAppender
	{
		TextWriter w;

		/// <summary>
		/// Cria um novo <see cref="TextWriterAppender"/>.
		/// </summary>
		/// <param name="w">O <see cref="TextWriter"/> onde gravar o resultado dos logs.</param>
		public TextWriterAppender(TextWriter w)
		{
			this.w = w;
		}

		/// <summary>
		/// Grava o resultado de um <see cref="LogEntry"/> no <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="logger">O logger</param>
		/// <param name="formatter">O formatador</param>
		/// <param name="entry">A entrada no log</param>
		public void Append(ILogger logger, ILogFormatter formatter, LogEntry entry)
		{
			w.WriteLine(formatter.Format(entry));
		}
	}
}