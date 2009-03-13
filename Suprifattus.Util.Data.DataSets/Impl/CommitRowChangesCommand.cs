using System;
using System.Collections;
using System.Data;

using Advanced.Data.Provider;

namespace Suprifattus.Util.Data.Impl
{
	public class CommitRowChangesCommand : CommitChangesCommand
	{
		DataRow[] rows;
			
		public CommitRowChangesCommand(ConnectorBase conn)
			: base(conn)
		{
		}

		public CommitRowChangesCommand(ConnectorBase conn, AdpConnection dbConn, AdpTransaction trans)
			: base(conn, dbConn, trans)
		{
		}

		public DataRow[] Rows
		{
			get { return rows; }
			set { rows = value; }
		}

		protected override void CommitChanges()
		{
			try
			{
				IEnumerator en = rows.GetEnumerator();
				bool hasNext = en.MoveNext();
				while (hasNext)
				{
					DataRow row = (DataRow) en.Current;

					DataTable dt = row.Table;
					using (new PhysicalTableAdapter(conn, dt))
					using (AdpDataAdapter da = new AdpDataAdapter())
					{
						if (!conn.BuildCommands(da, dbConn, dt))
							new AdpCommandBuilder(da, dbConn, dt);
					
						if (trans != null) 
							SetTransaction(da);
						
						do 
						{
							if (row.RowState == DataRowState.Added)
								CommitInserts(dt, da, row); // HACK: otimizar para enviar todas a serem inseridas ao mesmo tempo
							else
								da.Update(row);

							// repete enquanto houverem mais linhas da mesma tabela
							row = (hasNext = en.MoveNext()) ? (DataRow) en.Current : null;
						} while (hasNext && Object.ReferenceEquals(row.Table, dt));
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is ConnectorException)
					throw;
				else
					throw ConnectorExceptionFactory.FromDatabaseException(ex);
			}
		}
	}
}
