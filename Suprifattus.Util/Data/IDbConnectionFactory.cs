using System;
using System.Data;

namespace Suprifattus.Util.Data
{
	/// <summary>
	/// Interface que objetos que prov�em conex�es devem implementar.
	/// </summary>
	public interface IDbConnectionFactory
	{
		/// <summary>
		/// Retorna a conex�o ADO.NET.
		/// </summary>
		/// <returns>A conex�o ADO.NET</returns>
		IDbConnection GetConnection();
	}
}