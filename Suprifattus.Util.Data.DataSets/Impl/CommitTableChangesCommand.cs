using System;
using System.Data;

using Advanced.Data.Provider;

namespace Suprifattus.Util.Data.Impl
{
	/// <summary>
	/// Summary description for CommitTablesChangesCommand.
	/// </summary>
	public class CommitTableChangesCommand : CommitChangesCommand
	{
		DataTable[] tables;

		public CommitTableChangesCommand(ConnectorBase conn)
			: base(conn)
		{
		}

		public CommitTableChangesCommand(ConnectorBase conn, AdpConnection dbConn, AdpTransaction trans)
			: base(conn, dbConn, trans)
		{
		}

		public DataTable[] Tables
		{
			get { return tables; }
			set { tables = value; }
		}

		protected override void CommitChanges()
		{
			foreach (DataTable dt in tables)
				sum.Added += CommitTable(dt, DataViewRowState.Added);
	
			Array.Reverse(tables);
			foreach (DataTable dt in tables)
				sum.Modified += CommitTable(dt, DataViewRowState.ModifiedCurrent);
			Array.Reverse(tables);
	
			foreach (DataTable dt in tables)
				sum.Deleted += CommitTable(dt, DataViewRowState.Deleted);
	
			trans.Commit();
			trans = null;
		}

		/// <summary>
		/// Salva no banco de dados as alterações em uma tabela.
		/// </summary>
		/// <param name="dt">A tabela</param>
		/// <param name="state">
		/// O estado que estará sendo processado. Pode ser 
		/// <see cref="DataViewRowState.Added"/>, 
		/// <see cref="DataViewRowState.ModifiedCurrent"/> ou 
		/// <see cref="DataViewRowState.Deleted"/>
		/// </param>
		/// <returns>O número de registros atualizados</returns>
		protected int CommitTable(DataTable dt, DataViewRowState state)
		{
			int c = 0;

			if (dt != null && dt.Rows.Count > 0)
			{
				try
				{
					using (new PhysicalTableAdapter(conn, dt))
					using (AdpDataAdapter da = new AdpDataAdapter())
					{
						DataRow[] rows = dt.Select(null, null, state);

						if (rows.Length > 0)
						{
							if (!conn.BuildCommands(da, dbConn, dt))
								new AdpCommandBuilder(da, dbConn, dt);

							if (trans != null)
								SetTransaction(da);

							if (state == DataViewRowState.Added)
								c += CommitInserts(dt, da, rows);
							else
								c += da.Update(rows);
						}
					}
				}
				catch (Exception ex)
				{
					throw ConnectorExceptionFactory.FromDatabaseException(ex).Detail("Error while updating data table named {0}", dt.TableName);
				}
			}
			return c;
		}
	}
}
