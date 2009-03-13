using System;
using System.Configuration;
using System.Data;
using System.Text;

using Advanced.Data.Provider;

using Suprifattus.Util.DesignPatterns;

namespace Suprifattus.Util.Data
{
	/// <summary>
	/// Encapsula uma chamada a uma Stored Procedure no banco de dados.
	/// </summary>
	public class StoredProcedureCommand : ICommand, IDisposable
	{
		static readonly int DefaultCommandTimeout;
		AdpCommand cmd;

		static StoredProcedureCommand()
		{
			try
			{
				string spTimeout = ConfigurationSettings.AppSettings["Suprifattus.Util.Data.DbConnector.DefaultSPCommandTimeout"];
				DefaultCommandTimeout = Convert.ToInt32(spTimeout);
			}
			catch
			{
				DefaultCommandTimeout = 900;
			}
		}
		
		/// <summary>
		/// Cria um novo comando que encapsula uma chamada a uma Stored Procedure.
		/// </summary>
		/// <param name="conn">A conexão que será utilizada</param>
		/// <param name="procedureName">O nome da Stored Procedure</param>
		public StoredProcedureCommand(AdpConnection conn, string procedureName)
		{
			this.cmd = conn.CreateCommand();
			this.cmd.CommandType = CommandType.StoredProcedure;
			this.cmd.CommandText = procedureName;
			this.cmd.CommandTimeout = DefaultCommandTimeout;
		}

		/// <summary>
		/// Cria um novo comando que encapsula uma chamada a uma Stored Procedure,
		/// e roda em uma transação.
		/// </summary>
		/// <param name="trans">A transação que será utilizada</param>
		/// <param name="procedureName">O nome da Stored Procedure</param>
		public StoredProcedureCommand(AdpTransaction trans, string procedureName)
		{
			this.cmd = trans.Connection.CreateCommand();
			this.cmd.Transaction = trans;
			this.cmd.CommandType = CommandType.StoredProcedure;
			this.cmd.CommandText = procedureName;
			this.cmd.CommandTimeout = DefaultCommandTimeout;
		}

		/// <summary>
		/// A conexão sendo utilizada.
		/// </summary>
		public AdpConnection Connection
		{
			get { return cmd.Connection; }
		}

		public AdpParameterCollection Parameters
		{
			get { return cmd.Parameters; }
		}

		public int CommandTimeout
		{
			get { return cmd.CommandTimeout; }
			set { cmd.CommandTimeout = value; }
		}

		/// <summary>
		/// O nome da Stored Procedure
		/// </summary>
		public string ProcedureName
		{
			get { return cmd.CommandText; }
			set { cmd.CommandText = value; }
		}

		#region AddParameter
		public AdpParameter AddParameter(string name, ParameterDirection dir, DbType type, int size)
		{
			AdpParameter p = cmd.CreateParameter(name, type, size);
			p.Direction = dir;
			cmd.Parameters.Add(p);

			return p;
		}

		/// <summary>
		/// Adiciona um parâmetro.
		/// </summary>
		public AdpParameter AddParameter(string name, DbType type, ParameterDirection dir, object value)
		{
			AdpParameter p = cmd.CreateParameter(name, value);
			p.DbType = type;
			p.Direction = dir;
			cmd.Parameters.Add(p);
			return p;
		}
		
		/// <summary>
		/// Adiciona um parâmetro.
		/// </summary>
		public AdpParameter AddParameter(string name, object value)
		{
			AdpParameter p = cmd.CreateParameter(name, value);
			cmd.Parameters.Add(p);
			return p;
		}

		/// <summary>
		/// Adiciona um parâmetro se a condição for verdadeira.
		/// </summary>
		public AdpParameter AddParameterIf(bool condition, string name, object value)
		{
			return (condition ? AddParameter(name, value) : null);
		}
		#endregion

		#region Executes
		/// <summary>
		/// Executa a Stored Procedure.
		/// </summary>
		public void Execute()
		{
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw ConnectorExceptionFactory.FromDatabaseException(ex)
					.Detail("Erro ao executar stored procedure {0}", cmd.CommandText);
			}
		}

		/// <summary>
		/// Executa a Stored Procedure e retorna uma <see cref="DataTable"/>.
		/// </summary>
		public DataTable ExecuteDataTable()
		{
			try
			{
				return cmd.ExecuteDataTable();
			}
			catch (Exception ex)
			{
				throw ConnectorExceptionFactory.FromDatabaseException(ex)
					.Detail("Erro ao executar stored procedure {0}", cmd.CommandText);
			}
		}

		/// <summary>
		/// Executa a Stored Procedure e retorna uma <see cref="DataTable"/> com o nome especificado.
		/// </summary>
		/// <param name="table">O nome da <see cref="DataTable"/> a ser retornada</param>
		public DataTable ExecuteDataTable(string table)
		{
			try
			{
				return cmd.ExecuteDataTable(table);
			}
			catch (Exception ex)
			{
				throw ConnectorExceptionFactory.FromDatabaseException(ex)
					.Detail("Erro ao executar stored procedure {0}", cmd.CommandText);
			}
		}

		/// <summary>
		/// Executa a Stored Procedure e retorna um <see cref="AdpDataReader"/>.
		/// </summary>
		public AdpDataReader ExecuteReader()
		{
			try
			{
				return cmd.ExecuteReader();
			}
			catch (Exception ex)
			{
				throw ConnectorExceptionFactory.FromDatabaseException(ex)
					.Detail("Erro ao executar stored procedure {0}", cmd.CommandText);
			}
		}

		/// <summary>
		/// Executa a Stored Procedure e retorna o primeiro resultado obtido.
		/// </summary>
		public object ExecuteScalar()
		{
			try
			{
				return cmd.ExecuteScalar();
			}
			catch (Exception ex)
			{
				throw ConnectorExceptionFactory.FromDatabaseException(ex)
					.Detail("Erro ao executar stored procedure {0}", cmd.CommandText);
			}
		}
		#endregion

		#region Fills
		/// <summary>
		/// Preenche um <see cref="DataTable"/> com o resultado da Stored Procedure.
		/// </summary>
		public int Fill(DataTable dt)
		{
			try
			{
				using (AdpDataAdapter da = new AdpDataAdapter(cmd))
					return da.Fill(dt);
			}
			catch (Exception ex)
			{
				throw ConnectorExceptionFactory.FromDatabaseException(ex)
					.Detail("Erro ao executar stored procedure {0}", cmd.CommandText);
			}
		}
		#endregion

		public void Prepare()
		{
			cmd.Prepare();
		}
		
		/// <summary>
		/// Libera os recursos utilizados.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (cmd != null) cmd.Dispose();
			}
		}
		
		/// <summary>
		/// Libera os recursos utilizados
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public string ToDebugString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("EXEC {0}", ProcedureName);
			if (Parameters.Count > 0)
			{
				sb.Append(" (");
				sb.Append(Environment.NewLine);
				foreach (AdpParameter p in Parameters)
					sb.AppendFormat("\t{0} = {1}", p.ParameterName, (p.Value is DBNull ? "{null}" : "'" + p.Value + "'")).Append(Environment.NewLine);
				sb.Append(')');
			}
			
			return sb.ToString();
		}
	}
}
