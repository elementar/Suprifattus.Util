using System;
using System.Data;

namespace Suprifattus.Util.Data
{
	/// <summary>
	/// Interface que objetos que provêem conexões devem implementar.
	/// </summary>
	public interface IDbConnectionFactory
	{
		/// <summary>
		/// Retorna a conexão ADO.NET.
		/// </summary>
		/// <returns>A conexão ADO.NET</returns>
		IDbConnection GetConnection();
	}
}