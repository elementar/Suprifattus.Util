using System;

namespace Suprifattus.Util.Logging
{
	/// <summary>
	/// Os níveis de log.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Debug
		/// </summary>
		Debug = 1,
		/// <summary>
		/// Informação
		/// </summary>
		Info = 2,
		/// <summary>
		/// Aviso
		/// </summary>
		Warn = 3,
		/// <summary>
		/// Erro
		/// </summary>
		Error = 4,
		/// <summary>
		/// Fatal
		/// </summary>
		Fatal = 5,
	}
}