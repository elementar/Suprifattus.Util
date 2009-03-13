using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace Suprifattus.Util.Data
{
	/// <summary>
	/// Cacheador de metadados.
	/// Atualmente funciona apenas para SQL Server.
	/// </summary>
	public class DbMetadataCache
	{
		IDbConnectionFactory connFactory;
		IDictionary tables = new HybridDictionary(true);

		/// <summary>
		/// Cria um novo cacheador de metadados para a conexão SQL especificada.
		/// </summary>
		/// <param name="connectionString">A conexão</param>
		[Obsolete("Forneça um IDbConnectionFactory")]
		public DbMetadataCache(string connectionString)
		{
			this.connFactory = new SqlConnectionFactory(connectionString);
		}

		/// <summary>
		/// Cria um novo cacheador de metadados, utilizando qualquer conexão
		/// fornecida.
		/// </summary>
		/// <param name="connFactory">O construtor de conexão</param>
		public DbMetadataCache(IDbConnectionFactory connFactory)
		{
			this.connFactory = connFactory;
		}

		/// <summary>
		/// Retorna a linha que contém os metadados sobre um campo.
		/// </summary>
		/// <param name="table">A tabela</param>
		/// <param name="field">O campo</param>
		/// <returns>A linha que contém os metadados sobre o campo especificado.</returns>
		protected DataRow GetFieldMetadata(string table, string field)
		{
			DataTable fields = (DataTable) tables[table];
			if (fields == null)
			{
				lock (this) 
				{
					using (IDbConnection conn = connFactory.GetConnection())
					{
						if (conn.State != ConnectionState.Open)
							conn.Open();
						IDbCommand cmd = conn.CreateCommand();
						cmd.CommandText = "SELECT * FROM " + table;
						using (IDataReader dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
							tables.Add(table, fields = dr.GetSchemaTable());
					}
				}
			}
			
			DataRow[] rr = fields.Select(String.Format("ColumnName = '{0}'", field));
			return (rr.Length > 0 ? rr[0] : null);
		}

		/// <summary>
		/// Retorna o tamanho máximo de um campo.
		/// </summary>
		/// <param name="table">A tabela</param>
		/// <param name="field">O campo</param>
		/// <returns>O tamanho máximo do campo especificado.</returns>
		public int MaxLength(string table, string field) 
		{
			DataRow dr = GetFieldMetadata(table, field);

			Type t = dr != null ? (Type) dr["DataType"] : null;
			if (t == typeof(String)) return (int) dr["ColumnSize"];
			if (t == typeof(DateTime)) return 10;
			if (t == typeof(int)) return 10;
			if (t == typeof(short)) return 5;
			if (t == typeof(byte)) return 3;
			return 20;
		}

		/// <summary>
		/// Para manter a compatibilidade com o construtor
		/// <see cref="DbMetadataCache(string)"/>.
		/// </summary>
		private class SqlConnectionFactory : IDbConnectionFactory
		{
			string connectionString;

			public SqlConnectionFactory(string connectionString)
			{
				this.connectionString = connectionString;
			}

			public IDbConnection GetConnection()
			{
				return new SqlConnection(connectionString);
			}
		}
	}
}
