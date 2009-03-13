using System;
using System.Data;
using System.Diagnostics;

using Advanced.Data.Provider;
using Advanced.Data.Provider.Providers;

using Suprifattus.Util.DesignPatterns;

namespace Suprifattus.Util.Data.Impl
{
	/// <summary>
	/// Comando para salvamento de alterações em um <see cref="ConnectorBase"/>.
	/// </summary>
	public abstract class CommitChangesCommand : ICommand, IDisposable
	{
		private bool disposeDBObjects = true;

		/// <summary>
		/// O Connector relacionado.
		/// </summary>
		protected ConnectorBase conn;
		/// <summary>
		/// A conexão ativa.
		/// </summary>
		protected AdpConnection dbConn;
		/// <summary>
		/// A transação ativa.
		/// </summary>
		protected AdpTransaction trans;
		/// <summary>
		/// O sumário.
		/// </summary>
		protected CommitChangesSummary sum = new CommitChangesSummary();

		/// <summary>
		/// Cria o comando de salvamento de alterações para o <see cref="ConnectorBase"/>
		/// especificado.
		/// </summary>
		/// <param name="conn">O <see cref="ConnectorBase"/> a ter seus dados salvos.</param>
		public CommitChangesCommand(ConnectorBase conn)
		{
			this.conn = conn;
		}

		/// <summary>
		/// Cria o comando de salvamento de alterações para o <see cref="ConnectorBase"/>
		/// especificado.
		/// </summary>
		/// <param name="conn">O <see cref="ConnectorBase"/> a ter seus dados salvos.</param>
		/// <param name="dbConn">A conexão a ser utilizada</param>
		/// <param name="trans">A transação ativa</param>
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
		/// <returns>O número de linhas inseridas</returns>
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

			// se não houver coluna de auto-incremento, realiza o update normal.
			if (autoIncCol == null)
				return da.Update(rows);

			// procura pelo comando de busca do auto-incremento
			if (dbConn.Provider is MsSqlProvider)
			{
				autoIncCmd = dbConn.CreateCommand();
				autoIncCmd.CommandText = "SELECT IDENT_CURRENT('" + dt.TableName + "')";
				autoIncCmd.Transaction = trans;
			}

			// se não encontra o comando, realiza o update normal
			if (autoIncCmd == null)
			{
				Debug.WriteLine("Provider not supported: " + dbConn.Provider);
				return da.Update(rows);
			}

			// se está tudo certo, pode começar:

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

					// busca o conteúdo do auto-incremento
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
				// devolve o estado de "read-only" à linha
				autoIncCol.ReadOnly = oldRO;
			}
		}

		/// <summary>
		/// Atribui a transação ativa ao <see cref="AdpDataAdapter"/> especificado.
		/// </summary>
		/// <param name="da">O <see cref="AdpDataAdapter"/> onde a transação deve ser atribuída</param>
		protected void SetTransaction(AdpDataAdapter da)
		{
			SetTransaction(da, trans);
		}

		/// <summary>
		/// Atribui a transação especificada ao <see cref="AdpDataAdapter"/> especificado.
		/// </summary>
		/// <param name="da">O <see cref="AdpDataAdapter"/> onde a transação deve ser atribuída</param>
		/// <param name="trans">A transação a ser atribuída.</param>
		protected void SetTransaction(AdpDataAdapter da, AdpTransaction trans)
		{
			AdpCommand cmd;
			if ((cmd = da.SelectCommand) != null) cmd.Transaction = trans;
			if ((cmd = da.UpdateCommand) != null) cmd.Transaction = trans;
			if ((cmd = da.DeleteCommand) != null) cmd.Transaction = trans;
			if ((cmd = da.InsertCommand) != null) cmd.Transaction = trans;
		}

		/// <summary>
		/// Finaliza os objetos necessários.
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
		/// Finaliza os objetos necessários.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#region CommitChangesSummary
		/// <summary>
		/// Informações sobre o salvamento doa dados.
		/// </summary>
		public struct CommitChangesSummary
		{
			private int added, deleted, modified;

			internal void Reset()
			{
				added = deleted = modified = 0;
			}

			/// <summary>
			/// O número total de registros atualizados.
			/// </summary>
			public int Total
			{
				get { return added + deleted + modified; }
			}

			/// <summary>
			/// O número de registros adicionados.
			/// </summary>
			public int Added
			{
				get { return added; }
				set { added = value; }
			}

			/// <summary>
			/// O número de registros excluídos.
			/// </summary>
			public int Deleted
			{
				get { return deleted; }
				set { deleted = value; }
			}

			/// <summary>
			/// O número de registros modificados.
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