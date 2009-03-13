using System;
using System.Data;
using System.Diagnostics;

using Advanced.Data.Provider;
using Advanced.Data.Provider.Providers;

using Suprifattus.Util.DesignPatterns;

namespace Suprifattus.Util.Data.Impl
{
	/// <summary>
	/// Comando para salvamento de altera��es em um <see cref="ConnectorBase"/>.
	/// </summary>
	public abstract class CommitChangesCommand : ICommand, IDisposable
	{
		private bool disposeDBObjects = true;

		/// <summary>
		/// O Connector relacionado.
		/// </summary>
		protected ConnectorBase conn;
		/// <summary>
		/// A conex�o ativa.
		/// </summary>
		protected AdpConnection dbConn;
		/// <summary>
		/// A transa��o ativa.
		/// </summary>
		protected AdpTransaction trans;
		/// <summary>
		/// O sum�rio.
		/// </summary>
		protected CommitChangesSummary sum = new CommitChangesSummary();

		/// <summary>
		/// Cria o comando de salvamento de altera��es para o <see cref="ConnectorBase"/>
		/// especificado.
		/// </summary>
		/// <param name="conn">O <see cref="ConnectorBase"/> a ter seus dados salvos.</param>
		public CommitChangesCommand(ConnectorBase conn)
		{
			this.conn = conn;
		}

		/// <summary>
		/// Cria o comando de salvamento de altera��es para o <see cref="ConnectorBase"/>
		/// especificado.
		/// </summary>
		/// <param name="conn">O <see cref="ConnectorBase"/> a ter seus dados salvos.</param>
		/// <param name="dbConn">A conex�o a ser utilizada</param>
		/// <param name="trans">A transa��o ativa</param>
		public CommitChangesCommand(ConnectorBase conn, AdpConnection dbConn, AdpTransaction trans)
			: this(conn)
		{
			this.disposeDBObjects = false;

			this.dbConn = dbConn;
			this.trans = trans;
		}

		/// <summary>
		/// Finaliza o objeto e libera os recursos utilizados.
		/// </summary>
		~CommitChangesCommand()
		{
			Dispose(false);
		}

		/// <summary>
		/// Retorna o resultado do processo de salvamento.
		/// <seealso cref="CommitChangesSummary"/>
		/// </summary>
		public CommitChangesSummary Summary
		{
			get { return sum; }
		}

		/// <summary>
		/// Deve ser sobrescrito pelas subclasses.
		/// </summary>
		protected abstract void CommitChanges();

		/// <summary>
		/// Executa o salvamento propriamente dito.
		/// </summary>
		public void Execute()
		{
			sum.Reset();

			try
			{
				if (dbConn != null) 
					CommitChanges();
				else
				{
					using (dbConn = conn.CreateConnection())
					{
						dbConn.Open();
						using (trans = dbConn.BeginTransaction(IsolationLevel.Serializable))
							CommitChanges();
					}
				}
			}
			catch (ConnectorException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw ConnectorExceptionFactory.FromDatabaseException(ex);
			}
		}

		/// <summary>
		/// Insere as linhas na base de dados.
		/// </summary>
		/// <param name="dt">A tabela com as linhas a serem inseridas</param>
		/// <param name="da">O <see cref="AdpDataAdapter"/> a ser utilizado</param>
		/// <param name="rows">As linhas a serem inseridas</param>
		/// <returns>O n�mero de linhas inseridas</returns>
		protected int CommitInserts(DataTable dt, AdpDataAdapter da, params DataRow[] rows)
		{
			int c = 0;

			AdpCommand autoIncCmd = null;
			DataColumn autoIncCol = null;

			// procura pela coluna de auto-incremento
			foreach (DataColumn col in dt.PrimaryKey)
			{
				if (col.AutoIncrement)
				{
					autoIncCol = col;
					break;
				}
			}

			// se n�o houver coluna de auto-incremento, realiza o update normal.
			if (autoIncCol == null)
				return da.Update(rows);

			// procura pelo comando de busca do auto-incremento
			if (dbConn.Provider is MsSqlProvider)
			{
				autoIncCmd = dbConn.CreateCommand();
				autoIncCmd.CommandText = "SELECT IDENT_CURRENT('" + dt.TableName + "')";
				autoIncCmd.Transaction = trans;
			}

			// se n�o encontra o comando, realiza o update normal
			if (autoIncCmd == null)
			{
				Debug.WriteLine("Provider not supported: " + dbConn.Provider);
				return da.Update(rows);
			}

			// se est� tudo certo, pode come�ar:

			// prepara o comando de auto-incremento
			autoIncCmd.Prepare();

			bool oldRO = autoIncCol.ReadOnly;
			try
			{
				autoIncCol.ReadOnly = false;

				foreach (DataRow row in rows)
				{
					// atualiza linha a linha
					c += da.Update(row);

					// busca o conte�do do auto-incremento
					object id = autoIncCmd.ExecuteScalar();
					if (!(id is DBNull))
					{
						// se houver resultado, atualiza a linha do DataSet
						row[autoIncCol] = id;
						row.AcceptChanges();
					}
				}
				
				// retorna a quantidade de linhas atualizadas
				return c;
			}
			finally
			{
				// devolve o estado de "read-only" � linha
				autoIncCol.ReadOnly = oldRO;
			}
		}

		/// <summary>
		/// Atribui a transa��o ativa ao <see cref="AdpDataAdapter"/> especificado.
		/// </summary>
		/// <param name="da">O <see cref="AdpDataAdapter"/> onde a transa��o deve ser atribu�da</param>
		protected void SetTransaction(AdpDataAdapter da)
		{
			SetTransaction(da, trans);
		}

		/// <summary>
		/// Atribui a transa��o especificada ao <see cref="AdpDataAdapter"/> especificado.
		/// </summary>
		/// <param name="da">O <see cref="AdpDataAdapter"/> onde a transa��o deve ser atribu�da</param>
		/// <param name="trans">A transa��o a ser atribu�da.</param>
		protected void SetTransaction(AdpDataAdapter da, AdpTransaction trans)
		{
			AdpCommand cmd;
			if ((cmd = da.SelectCommand) != null) cmd.Transaction = trans;
			if ((cmd = da.UpdateCommand) != null) cmd.Transaction = trans;
			if ((cmd = da.DeleteCommand) != null) cmd.Transaction = trans;
			if ((cmd = da.InsertCommand) != null) cmd.Transaction = trans;
		}

		/// <summary>
		/// Finaliza os objetos necess�rios.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (disposeDBObjects) 
				{
					if (trans != null)
					{
						trans.Dispose();
						trans = null;
					}
					if (dbConn != null)
					{
						dbConn.Dispose();
						dbConn = null;
					}
				}
			}
		}
		
		/// <summary>
		/// Finaliza os objetos necess�rios.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#region CommitChangesSummary
		/// <summary>
		/// Informa��es sobre o salvamento doa dados.
		/// </summary>
		public struct CommitChangesSummary
		{
			private int added, deleted, modified;

			internal void Reset()
			{
				added = deleted = modified = 0;
			}

			/// <summary>
			/// O n�mero total de registros atualizados.
			/// </summary>
			public int Total
			{
				get { return added + deleted + modified; }
			}

			/// <summary>
			/// O n�mero de registros adicionados.
			/// </summary>
			public int Added
			{
				get { return added; }
				set { added = value; }
			}

			/// <summary>
			/// O n�mero de registros exclu�dos.
			/// </summary>
			public int Deleted
			{
				get { return deleted; }
				set { deleted = value; }
			}

			/// <summary>
			/// O n�mero de registros modificados.
			/// </summary>
			public int Modified
			{
				get { return modified; }
				set { modified = value; }
			}
		}
		#endregion
	}
}