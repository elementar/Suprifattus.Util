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
		/// Retorna o nome físico de uma tabela controlada por este Connector.
		/// </summary>
		/// <param name="dataTableName">O nome da tabela</param>
		/// <returns>O nome físico da tabela</returns>
		string GetPhysicalTableName(string dataTableName);

		/// <summary>
		/// Atualiza as alterações em todas as tabelas do banco de dados.
		/// </summary>
		/// <returns>O número de linhas modificadas</returns>
		int CommitChanges();

		/// <summary>
		/// Atualiza as alterações nas tabelas no banco de dados.
		/// </summary>
		/// <param name="tables">As tabelas que devem ser atualizadas</param>
		/// <returns>O número de linhas modificadas</returns>
		int CommitChanges(params string[] tables);

		/// <summary>
		/// Preenche a tabela de dados especificada, com os critérios especificados.
		/// </summary>
		/// <param name="dataTable">O nome da tabela no DataSet que deverá ser preenchida.</param>
		/// <param name="query">A consulta de origem dos dados</param>
		int FillData(string dataTable, ISqlQuery query);

		/// <summary>
		/// Preenche a tabela de dados especificada, com os critérios especificados.
		/// </summary>
		/// <param name="source">O nome da origem. Formato: <c>dataTable:tabela_ou_view</c></param>
		/// <param name="cond">O critério de seleção dos registros.</param>
		int FillData(string source, SqlCondition cond);

		/// <summary>
		/// Preenche apenas as tabelas de dados especificadas.
		/// </summary>
		/// <param name="tables">O nome das tabelas que deverão ser preenchidas.</param>
		int FillData(params string[] tables);
	}
}
