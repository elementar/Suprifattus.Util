using System;

namespace Suprifattus.Util.Logging
{
	/// <summary>
	/// Uma entrada no log.
	/// </summary>
	public class LogEntry
	{
		string fmt;
		object[] args;

		string msg;

		/// <summary>
		/// Cria uma nova entrada no log, com a mensagem especificada.
		/// </summary>
		/// <param name="messageFormat">O formato da mensagem</param>
		/// <param name="messageArguments">Os argumentos da mensagem</param>
		public LogEntry(string messageFormat, object[] messageArguments)
		{
			this.fmt = messageFormat;
			this.args = messageArguments;
		}

		/// <summary>
		/// Cria uma nova entrada no log, com a mensagem especificada.
		/// </summary>
		/// <param name="msg">A mensagem</param>
		public LogEntry(string msg)
		{
			this.msg = msg;
		}

		/// <summary>
		/// Retorna a representação string da entrada no log.
		/// </summary>
		public override string ToString()
		{
			return (msg != null ? msg : String.Format(fmt, args));
		}

		/// <summary>
		/// Retorna a representação string da entrada no log.
		/// </summary>
		/// <param name="formatProvider">O <see cref="IFormatProvider"/> a ser utilizado</param>
		public string ToString(IFormatProvider formatProvider)
		{
			return (msg != null ? msg : String.Format(formatProvider, fmt, args));
		}
	}
}
