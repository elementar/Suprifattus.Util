using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

using Advanced.Data.Provider;

using Suprifattus.Util.Data.Impl;
using Suprifattus.Util.Data.Sql;

namespace Suprifattus.Util.Data
{
	/// <summary>
	/// Provides a base class for Database Connectors.
	/// </summary>
	[Serializable]
	public abstract class ConnectorBase : Component, IConnector, IDisposable, ISerializable
	{
		/// <summary>
		/// The dataset managed by this Connector.
		/// </summary>
		protected DataSet ds;
		/// <summary>
		/// The default view exposed by this connector.
		/// </summary>
		protected DataView view;

		#region Connection Related
		/// <summary>
		/// Creates a new Connection object.
		/// </summary>
		/// <returns>The new Connection object.</returns>
		[CLSCompliant(false)]
		protected internal virtual AdpConnection CreateConnection() 
		{
			return CreateConnection(false);
		}

		/// <summary>
		/// Creates a new Connection object.
		/// </summary>
		/// <param name="open">If true, the connection will be open automatically</param>
		/// <returns>The new Connection object.</returns>
		[CLSCompliant(false)]
		protected internal virtual AdpConnection CreateConnection(bool open) 
		{
			string cs = null;
			try 
			{
				cs = ConnectionString;
				cs = cs.Replace("${user}", DatabaseUser);
				cs = cs.Replace("${password}", DatabasePassword);
				// para compatibilidade
				cs = cs.Replace("{0}", DatabaseUser);
				cs = cs.Replace("{1}", DatabasePassword);
				
				AdpConnection conn = new AdpConnection(cs);
				if (open)
					conn.Open();

				return conn;
			}
			catch (Exception ex)
			{
				throw new ConnectorException(String.Format("Error while creating connection.{0}ConnectionString = {1}{0}User = {2}{0}Password=*******", Environment.NewLine, ConnectionString, DatabaseUser), ex);
			}
		}

		/// <summary>
		/// Gets the connection string used for new connections.
		/// </summary>
		protected virtual string ConnectionString 
		{
			get { return ConfigurationSettings.AppSettings["Suprifattus.Util.Data.DbConnector.ConnectionString"]; }
		}

		/// <summary>
		/// Gets the database user used for new connections.
		/// </summary>
		protected virtual string DatabaseUser 
		{ 
			get { return ConfigurationSettings.AppSettings["Suprifattus.Util.Data.DbConnector.DefaultUser"]; } 
		}

		/// <summary>
		/// Gets the database password used for new connections.
		/// </summary>
		protected virtual string DatabasePassword 
		{ 
			get { return ConfigurationSettings.AppSettings["Suprifattus.Util.Data.DbConnector.DefaultPassword"]; } 
		}
		#endregion

		/// <summary>
		/// Initializes the new Connector, using the provided DataSet.
		/// </summary>
		/// <param name="ds"></param>
		protected ConnectorBase(DataSet ds) 
		{
			this.ds = ds;
		}

		#region Utility
		/// <summary>
		/// Define todas as colunas do tipo string que aceitam nulos e que estão nulas
		/// para <see cref="DBNull"/>.
		/// </summary>
		/// <param name="row">A linha em questão</param>
		public void SetEmptyStringsNull(DataRow row)
		{
			foreach (DataColumn col in row.Table.Columns)
				if (col.AllowDBNull && col.DataType == typeof(string))
					if (!(row[col] is DBNull) && Logic.StringEmpty(row[col]))
						row[col] = DBNull.Value;
		}

		private void HandleException(Exception ex)
		{
			if (ex is ConnectorException)
				throw ex;
			else
				throw ConnectorExceptionFactory.FromDatabaseException(ex);
		}

		protected virtual void Trace(string msg)
		{
			System.Diagnostics.Trace.WriteLine(msg);
		}

		protected void Trace(string format, params object[] args)
		{
			Trace(String.Format(format, args));
		}
		#endregion
		
		#region Physical Table Name Support
		/// <summary>
		/// Adiciona um prefixo ao nome físico de tabelas.
		/// </summary>
		/// <param name="prefix">O prefixo</param>
		/// <param name="tables">As tabelas</param>
		protected void AddPhysicalTablePrefix(string prefix, params DataTable[] tables)
		{
			foreach (DataTable dt in tables)
				SetPhysicalTableName(dt, prefix + dt.TableName);
		}

		/// <summary>
		/// Define um nome físico para uma tabela.
		/// </summary>
		/// <param name="table">A tabela</param>
		/// <param name="physicalName">O nome físico</param>
		protected void SetPhysicalTableName(DataTable table, string physicalName)
		{
			table.ExtendedProperties["physical_table_name"] = physicalName;
		}

		/// <summary>
		/// Retorna o nome físico de uma tabela.
		/// </summary>
		/// <param name="table">A tabela</param>
		/// <returns>O nome físico da tabela</returns>
		protected string GetPhysicalTableName(DataTable table)
		{
			object p = table.ExtendedProperties["physical_table_name"];
			return (p == null ? table.TableName : Convert.ToString(p));
		}

		/// <summary>
		/// Retorna o nome físico de uma tabela.
		/// </summary>
		/// <param name="dataTableName">A tabela</param>
		/// <returns>O nome físico da tabela, ou o próprio nome fornecido, caso a tabela não exista.</returns>
		public string GetPhysicalTableName(string dataTableName)
		{
			DataTable dt = ds.Tables[dataTableName];
			if (dt != null)
				return GetPhysicalTableName(dt);
			else
				return dataTableName;
		}
		#endregion

		#region Stored Procedure Support
		/// <summary>
		/// Adiciona nomes de <c>stored procedures</c> às <see cref="DataTable"/>s especificadas,
		/// que serão utilizados no <see cref="CommitChanges"/>, ao invés de gerar SQL padrão.
		/// </summary>
		/// <param name="prefixo">O prefixo das <c>stored procedures</c>.</param>
		/// <param name="ops">As operações para as quais deve ser adicionado o nome de <c>stored procedure</c></param>
		/// <param name="tables">As tabelas onde as <c>stored procedures</c> serão atribuídas</param>
		/// <remarks>
		/// Os nomes de <c>stored procedure</c> adicionados terão o seguinte formato:
		/// 
		/// <list type="bullet">
		///		<item>
		///			<strong>Insert: </strong>
		///			<paramref name="prefixo"/> INS_ <c>nome da tabela</c>
		///		</item>
		///		<item>
		///			<strong>Update: </strong>
		///			<paramref name="prefixo"/> UPD_ <c>nome da tabela</c>
		///		</item>
		///		<item>
		///			<strong>Delete: </strong>
		///			<paramref name="prefixo"/> DEL_ <c>nome da tabela</c>
		///		</item>
		/// </list>
		/// 
		/// As <c>stored procedures</c> devem ter um formato específico:
		/// <list type="definition">
		///		<item>
		///			<term>INSERT</term>
		///			<description>
		///				Deve ter um parâmetro para cada campo da tabela, com exceção de valores 
		///				<see cref="DataColumn.AutoIncrement"/>, e um parâmetro 
		///				<see cref="ParameterDirection.Output"/> para o retorno dos valores
		///				<see cref="DataColumn.AutoIncrement"/>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>DELETE</term>
		///			<description>
		///				Deve ter um parâmetro para cada campo que compõe a chave primária da tabela.
		///			</description>
		///		</item>
		///		<item>
		///			<term>UPDATE</term>
		///			<description>
		///				Dele ter um parâmetro para cada campo, seja chave primária ou não.
		///			</description>
		///		</item>
		/// </list>
		/// </remarks>
		protected void AddStoredProcedureForCommiting(string prefixo, AdpCommandOperation ops, params DataTable[] tables)
		{
			foreach (DataTable dt in tables)
			{
				string fmt = prefixo + "{0}_" + dt.TableName;
				if ((ops & AdpCommandOperation.Insert) != 0)
					SetStoredProcedureForCommiting(dt, AdpCommandOperation.Insert, String.Format(fmt, "INS"));
				if ((ops & AdpCommandOperation.Update) != 0)
					SetStoredProcedureForCommiting(dt, AdpCommandOperation.Update, String.Format(fmt, "UPD"));
				if ((ops & AdpCommandOperation.Delete) != 0)
					SetStoredProcedureForCommiting(dt, AdpCommandOperation.Delete, String.Format(fmt, "DEL"));
			}
		}

		/// <summary>
		/// Atribui o nome de <c>stored procedures</c> à <see cref="DataTable"/> especificada.
		/// A <c>stored procedure</c> será utilizada no <see cref="CommitChanges"/>, ao invés 
		/// da geração de SQL padrão.
		/// </summary>
		/// <param name="dt">A <see cref="DataTable"/></param>
		/// <param name="op">A operação, conforme <see cref="AdpCommandOperation"/></param>
		/// <param name="procName">O nome da <c>stored procedure</c></param>
		/// <remarks>
		/// As <c>stored procedures</c> devem ter um formato específico:
		/// <list type="definition">
		///		<item>
		///			<term>INSERT</term>
		///			<description>
		///				Deve ter um parâmetro para cada campo da tabela, com exceção de valores 
		///				<see cref="DataColumn.AutoIncrement"/>, e um parâmetro 
		///				<see cref="ParameterDirection.Output"/> para o retorno dos valores
		///				<see cref="DataColumn.AutoIncrement"/>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>DELETE</term>
		///			<description>
		///				Deve ter um parâmetro para cada campo que compõe a chave primária da tabela.
		///			</description>
		///		</item>
		///		<item>
		///			<term>UPDATE</term>
		///			<description>
		///				Dele ter um parâmetro para cada campo, seja chave primária ou não.
		///			</description>
		///		</item>
		/// </list>
		/// </remarks>
		protected void SetStoredProcedureForCommiting(DataTable dt, AdpCommandOperation op, string procName)
		{
			dt.ExtendedProperties["storedproc_"+op] = procName;
		}

		/// <summary>
		/// Retorna o nome da <c>stored procedure</c> atribuída à <see cref="DataTable"/>
		/// especificada em <paramref name="dt"/>.
		/// </summary>
		/// <param name="dt">A <see cref="DataTable"/></param>
		/// <param name="op">A operação, conforme <see cref="AdpCommandOperation"/></param>
		/// <returns>O nome da <c>stored procedure</c>, ou null.</returns>
		protected string GetStoredProcedureForCommiting(DataTable dt, AdpCommandOperation op)
		{
			return dt.ExtendedProperties["storedproc_"+op] as string;
		}
		#endregion

		#region View Management
		/// <summary>
		/// Gets the currently selected view of the DataSet.
		/// </summary>
		public DataView CurrentView 
		{
			get { return view; }
		}
		
		/// <summary>
		/// Sets the current DataSet view.
		/// </summary>
		/// <param name="table">The table name to show</param>
		/// <param name="filter">The filter expression</param>
		/// <param name="sort">The sort order expression</param>
		public void SetView(string table, string filter, string sort)
		{
			view = new DataView(ds.Tables[table], filter, sort, DataViewRowState.CurrentRows);
		}

		/// <summary>
		/// Sets the current DataSet view.
		/// </summary>
		/// <param name="table">The table name to show</param>
		public void SetView(string table)
		{
			SetView(table, null, null);
		}
		#endregion
		
		#region Delete
		/// <summary>
		/// Excluir itens da tabela.
		/// </summary>
		/// <param name="table">A tabela</param>
		/// <param name="cods">Os códigos dos itens a serem excluídos</param>
		/// <param name="mustFillFirst">Se verdadeiro, será realizado um <see cref="FillData"/> antes da exclusão</param>
		/// <param name="mustCommitLast">Se verdadeiro, será realizado um <see cref="CommitChanges"/> na tabela após a exclusão</param>
		/// <returns>O número de registros excluídos</returns>
		public int Delete(string table, Array cods, bool mustFillFirst, bool mustCommitLast)
		{
			DataTable dt = ds.Tables[table];
			if (dt == null)
				throw new ArgumentException(String.Format("Datatable {0} not found.", table));
			if (dt.PrimaryKey.Length != 1)
				throw new ArgumentException("Primary key must have exactly one element.");

			if (mustFillFirst)
				FillData(table);
			
			Type pkType = dt.PrimaryKey[0].DataType;
			
			int c = 0;
			foreach (object cod in cods) 
			{
				DataRow row = dt.Rows.Find(Convert.ChangeType(cod, pkType));
				if (row != null) 
				{
					row.Delete();
					c++;
				}
			}

			if (mustCommitLast)
				CommitChanges(table);
			
			return c;
		}

		/// <summary>
		/// Excluir itens da tabela.
		/// </summary>
		/// <param name="table">A tabela</param>
		/// <param name="cods">Os códigos dos itens a serem excluídos</param>
		/// <returns>O número de registros excluídos</returns>
		public int Delete(string table, Array cods)
		{
			return Delete(table, cods, false, false);
		}
		#endregion
		
		#region Commit Changes
		/// <summary>
		/// Atualiza as alterações em todas as tabelas do banco de dados.
		/// </summary>
		/// <returns>O número de linhas modificadas</returns>
		public virtual int CommitChanges() 
		{
			string[] tables = new string[ds.Tables.Count];
			for (int i=0; i < tables.Length; i++)
				tables[i] = ds.Tables[i].TableName;
			return CommitChanges(tables);
		}

		/// <summary>
		/// Atualiza as alterações nas tabelas no banco de dados.
		/// </summary>
		/// <param name="tableNames">Os nomes das tabelas que devem ser atualizadas</param>
		/// <returns>O número de linhas modificadas</returns>
		public int CommitChanges(params string[] tableNames) 
		{
			DataTable[] dataTables = new DataTable[tableNames.Length];
			for (int i=0; i < dataTables.Length; i++)
				dataTables[i] = ds.Tables[tableNames[i]];

			return CommitChanges(dataTables);
		}

		/// <summary>
		/// Atualiza as alterações nas tabelas no banco de dados.
		/// </summary>
		/// <param name="tables">As tabelas que devem ser atualizadas</param>
		/// <returns>O número de linhas modificadas</returns>
		protected int CommitChanges(params DataTable[] tables)
		{
			using (CommitTableChangesCommand cmd = new CommitTableChangesCommand(this)) 
			{
				cmd.Tables = tables;
				cmd.Execute();
				return cmd.Summary.Total;
			}
		}
		
		/// <summary>
		/// Atualiza as informações das linhas especificadas no banco de dados.
		/// </summary>
		/// <param name="rows">As linhas a atualizar</param>
		private void CommitChanges(AdpConnection conn, AdpTransaction trans, params DataRow[] rows)
		{
			using (CommitRowChangesCommand cmd = new CommitRowChangesCommand(this, conn, trans)) 
			{
				cmd.Rows = rows;
				cmd.Execute();
			}
		}
		
		/// <summary>
		/// Atualiza as informações das linhas especificadas no banco de dados.
		/// </summary>
		/// <param name="rows">As linhas a atualizar</param>
		protected void CommitChanges(AdpTransaction trans, params DataRow[] rows)
		{
			CommitChanges(trans.Connection, trans, rows);
		}
			
		/// <summary>
		/// Atualiza as informações das linhas especificadas no banco de dados.
		/// </summary>
		/// <param name="rows">As linhas a atualizar</param>
		protected void CommitChanges(params DataRow[] rows)
		{
			try 
			{
				using (AdpConnection conn = CreateConnection())
					CommitChanges(conn, null, rows);
			}
			catch (Exception ex)
			{
				HandleException(ex);
			}		
		}

		/// <summary>
		/// Tenta definir os comandos para atualização da <see cref="DataTable"/> especificada.
		/// Caso não seja possível definir os comandos, deve-se retornar <c>false</c>.
		/// </summary>
		/// <param name="da">O <see cref="AdpDataAdapter"/></param>
		/// <param name="conn">A <see cref="AdpConnection"/></param>
		/// <param name="dataTable">O <see cref="DataTable"/></param>
		/// <returns><c>true</c> se foi possível definir os comandos, <c>false</c> se eles devem ser definidos automaticamente pelo <see cref="AdpCommandBuilder"/>.</returns>
		/// <remarks>
		/// A implementação padrão tenta criar comandos com as <c>stored procedures</c> definidas
		/// pelos métodos <see cref="SetStoredProcedureForCommiting"/> ou 
		/// <see cref="AddStoredProcedureForCommiting"/>.
		/// </remarks>
		[CLSCompliant(false)]
		protected internal virtual bool BuildCommands(AdpDataAdapter da, AdpConnection conn, DataTable dataTable)
		{
			string 
				procINS = GetStoredProcedureForCommiting(dataTable, AdpCommandOperation.Insert),
				procUPD = GetStoredProcedureForCommiting(dataTable, AdpCommandOperation.Update),
				procDEL = GetStoredProcedureForCommiting(dataTable, AdpCommandOperation.Delete);
			
			if (procINS != null || procUPD != null || procDEL != null)
			{
				new AdpCommandBuilder(da, conn, dataTable);

				if (procINS != null)
				{
					new AdpCommandBuilder(da, conn, dataTable, AdpCommandOperation.Insert, true);
					da.InsertCommand.CommandType = CommandType.StoredProcedure;
					da.InsertCommand.CommandText = procINS;
				}

				if (procUPD != null)
				{
					new AdpCommandBuilder(da, conn, dataTable, AdpCommandOperation.Update, true);
					da.UpdateCommand.CommandType = CommandType.StoredProcedure;
					da.UpdateCommand.CommandText = procUPD;
				}
				
				if (procDEL != null)
				{
					new AdpCommandBuilder(da, conn, dataTable, AdpCommandOperation.Delete, true);
					da.DeleteCommand.CommandType = CommandType.StoredProcedure;
					da.DeleteCommand.CommandText = procDEL;
				}
			
				return true;
			}

			return false;
		}
		#endregion

		#region Fill Data
		/// <summary>
		/// Preenche a tabela de dados especificada, com os critérios especificados.
		/// </summary>
		/// <param name="dataTable">O nome da tabela no DataSet que deverá ser preenchida.</param>
		/// <param name="query">A consulta de origem dos dados</param>
		public int FillData(string dataTable, ISqlQuery query)
		{
			string sql = null;
			try 
			{
#if DEBUG
				Debug.WriteLine("FillData");
				Debug.IndentLevel++;
				Debug.WriteLine("Data Table: " + dataTable);
				Debug.WriteLine("SQL: " + Regex.Replace(query.ToString().Replace(Environment.NewLine, " "), "\\s+", " "));
				Debug.IndentLevel--;
#endif
				sql = query.ToString();
				DataTable dt = ds.Tables[dataTable];
				if (dt != null)
					sql = Regex.Replace(sql, "FROM\\s+" + dataTable, "FROM " + GetPhysicalTableName(dt), RegexOptions.IgnoreCase);
			
				using (AdpConnection conn = CreateConnection()) 
				using (AdpDataAdapter da = new AdpDataAdapter())
				{
					da.SelectCommand = new AdpCommand(sql, conn);

					return da.Fill(ds, dataTable);
				}
			}
			catch (Exception ex)
			{
				if (sql == null)
					throw new ConnectorException("Erro ao realizar o preenchimento dos dados", ex);
				else
					throw ConnectorExceptionFactory.FromDatabaseException(ex, sql);
			}
		}
		
		/// <summary>
		/// Preenche a tabela de dados especificada, com os critérios especificados.
		/// </summary>
		/// <param name="source">O nome da origem. Formato: <c>dataTable:tabela_ou_view</c></param>
		/// <param name="cond">O critério de seleção dos registros.</param>
		public int FillData(string source, SqlCondition cond)
		{
			return FillData(source, cond, null);
		}

		/// <summary>
		/// Preenche a tabela de dados especificada, com os critérios especificados.
		/// </summary>
		/// <param name="source">O nome da origem. Formato: <c>dataTable:tabela_ou_view</c></param>
		/// <param name="orderBy">A ordem</param>
		public int FillData(string source, SqlOrder orderBy)
		{
			return FillData(source, null, orderBy);
		}

		/// <summary>
		/// Preenche a tabela de dados especificada, com os critérios especificados.
		/// </summary>
		/// <param name="source">O nome da origem. Formato: <c>dataTable:tabela_ou_view</c></param>
		/// <param name="cond">O critério de seleção dos registros.</param>
		/// <param name="orderBy">A ordem</param>
		public int FillData(string source, SqlCondition cond, SqlOrder orderBy)
		{
			FillInfo fi = new FillInfo(source);
			return FillData(fi.DataTable, new SqlQuery(new SqlTableSource(fi.Source), cond, orderBy));
		}
		
		/// <summary>
		/// Preenche apenas as tabelas de dados especificadas.
		/// </summary>
		/// <param name="sources">Os nomes das origens. Formato de cada uma: <c>dataTable:tabela_ou_view</c></param>
		public int FillData(params string[] sources) 
		{
			try 
			{
				int c = 0;
				using (AdpConnection conn = CreateConnection()) 
				{
					foreach (string rawInfo in sources) 
					{
						FillInfo fi = new FillInfo(rawInfo);
						using (AdpDataAdapter da = new AdpDataAdapter())
						{
							if (fi.Source == fi.DataTable && ds.Tables.Contains(fi.DataTable)) 
							{
								DataTable dt = ds.Tables[fi.DataTable];
								lock (dt) 
								{
									string old = dt.TableName;
									dt.TableName = GetPhysicalTableName(dt);
									new AdpCommandBuilder(da, conn, dt, AdpCommandOperation.Select);
									c += da.Fill(dt);
									dt.TableName = old;
								}
							}
							else
							{
								da.SelectCommand = new AdpCommand("SELECT * FROM " + fi.Source, conn);
								if (!ds.Tables.Contains(fi.DataTable))
									Debug.WriteLine(String.Format("WARN: DataTable {0} doesn't exist.", fi.DataTable));
								c += da.Fill(ds, fi.DataTable);
							}
						}
					}
				}

				return c;
			}
			catch (Exception ex)
			{
				throw new ConnectorException("Erro ao realizar o preenchimento dos dados", ex);
			}
		}

		private struct FillInfo
		{
			public readonly string DataTable;
			public readonly string Source;

			public FillInfo(string rawInfo)
			{
				string[] info = rawInfo.Split(new char[] {':'}, 2);
				DataTable = info[0];
				Source = (info.Length > 1 ? info[1] : DataTable);
			}
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Libera os recursos utilizados por este Connector.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		#endregion

		#region ISerializable
		/// <summary>
		/// Construtor utilizado ao deserializar o objeto.
		/// </summary>
		/// <param name="info">Os dados da deserialização</param>
		/// <param name="context">O contexto da deserialização</param>
		protected ConnectorBase(SerializationInfo info, StreamingContext context)
		{
			Type datasetType = (Type) info.GetValue("dataset.Type", typeof(Type));
			
			this.ds = (DataSet) info.GetValue("dataset", datasetType);

			if (info.GetBoolean("view"))
			{
				this.view = new DataView();
				this.view.Table = ds.Tables[info.GetString("view.TableName")];
				this.view.Sort = info.GetString("view.Sort");
				this.view.RowStateFilter = (DataViewRowState) info.GetValue("view.RowStateFilter", typeof(DataViewRowState));
				this.view.ApplyDefaultSort = info.GetBoolean("view.ApplyDefaultSort");
				this.view.AllowDelete = info.GetBoolean("view.AllowDelete");
				this.view.AllowNew = info.GetBoolean("view.AllowNew");
				this.view.AllowEdit = info.GetBoolean("view.AllowEdit");
			}
		}
		
		/// <summary>
		/// Preenche o objeto <see cref="SerializationInfo"/> com o estado do objeto atual,
		/// para serialização.
		/// </summary>
		/// <param name="info">Os dados da serialização</param>
		/// <param name="context">O contexto da serialização</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("dataset.Type", ds.GetType());
			info.AddValue("dataset", ds);
			info.AddValue("view", view != null);
			if (view != null) 
			{
				info.AddValue("view.Filter", view.RowFilter);
				info.AddValue("view.Sort", view.Sort);
				info.AddValue("view.TableName", view.Table.TableName);
				info.AddValue("view.RowStateFilter", view.RowStateFilter, typeof(DataViewRowState));
				info.AddValue("view.ApplyDefaultSort", view.ApplyDefaultSort);
				info.AddValue("view.AllowDelete", view.AllowDelete);
				info.AddValue("view.AllowNew", view.AllowNew);
				info.AddValue("view.AllowEdit", view.AllowEdit);
			}
		}
		#endregion
	}
}
