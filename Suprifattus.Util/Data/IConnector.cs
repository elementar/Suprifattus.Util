using System;
using System.Data;

using Suprifattus.Util.Data.Sql;

namespace Suprifattus.Util.Data
{
	/// <summary>
	/// Conector.
	/// </summary>
	public interface IConnector
	{
		/// <summary>
		/// Gets the currently selected view of the DataSet.
		/// </summary>
		DataView CurrentView { get; }

		/// <summary>
		/// Sets the current DataSet view.
		/// </summary>
		/// <param name="table">The table name to show</param>
		/// <param name="filter">The filter expression</param>
		/// <param name="sort">The sort order expression</param>
		void SetView(string table, string filter, string sort);

		/// <summary>
		/// Retorna o nome f�sico de uma tabela controlada por este Connector.
		/// </summary>
		/// <param name="dataTableName">O nome da tabela</param>
		/// <returns>O nome f�sico da tabela</returns>
		string GetPhysicalTableName(string dataTableName);

		/// <summary>
		/// Atualiza as altera��es em todas as tabelas do banco de dados.
		/// </summary>
		/// <returns>O n�mero de linhas modificadas</returns>
		int CommitChanges();

		/// <summary>
		/// Atualiza as altera��es nas tabelas no banco de dados.
		/// </summary>
		/// <param name="tables">As tabelas que devem ser atualizadas</param>
		/// <returns>O n�mero de linhas modificadas</returns>
		int CommitChanges(params string[] tables);

		/// <summary>
		/// Preenche a tabela de dados especificada, com os crit�rios especificados.
		/// </summary>
		/// <param name="dataTable">O nome da tabela no DataSet que dever� ser preenchida.</param>
		/// <param name="query">A consulta de origem dos dados</param>
		int FillData(string dataTable, ISqlQuery query);

		/// <summary>
		/// Preenche a tabela de dados especificada, com os crit�rios especificados.
		/// </summary>
		/// <param name="source">O nome da origem. Formato: <c>dataTable:tabela_ou_view</c></param>
		/// <param name="cond">O crit�rio de sele��o dos registros.</param>
		int FillData(string source, SqlCondition cond);

		/// <summary>
		/// Preenche apenas as tabelas de dados especificadas.
		/// </summary>
		/// <param name="tables">O nome das tabelas que dever�o ser preenchidas.</param>
		int FillData(params string[] tables);
	}
}
