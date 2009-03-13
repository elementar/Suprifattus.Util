using System;
using System.Text;
using System.Collections;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Uma origem para uma consulta SQL.
	/// </summary>
	[Serializable]
	public abstract class SqlSource 
	{
	}
	
	/// <summary>
	/// Um JOIN de duas origens no SQL.
	/// </summary>
	[Serializable]
	public class SqlJoin
	{
		/// <summary>
		/// O tipo de JOIN realizado: INNER ou OUTER.
		/// </summary>
		public enum Type 
		{
			/// <summary>
			/// OUTER JOIN
			/// </summary>
			Outer,
			/// <summary>
			/// INNER JOIN
			/// </summary>
			Inner,
		}

		string tableName;
		SqlCondition cond;
		SqlJoin.Type type;

		/// <summary>
		/// Cria um JOIN com a tabela e condições especificadas.
		/// </summary>
		/// <param name="type">O tipo de JOIN</param>
		/// <param name="tableName">A tabela</param>
		/// <param name="cond">A condição para o join</param>
		public SqlJoin(SqlJoin.Type type, string tableName, SqlCondition cond)
		{
			this.tableName = tableName;
			this.cond = cond;
			this.type = type;
		}

		/// <summary>
		/// Cria um INNER JOIN com a tabela e condições especificadas.
		/// </summary>
		/// <param name="tableName">A tabela</param>
		/// <param name="cond">A condição para o join</param>
		public SqlJoin(string tableName, SqlCondition cond) 
			: this(SqlJoin.Type.Inner, tableName, cond) { }

		/// <summary>
		/// Retorna a representação string do JOIN.
		/// </summary>
		/// <returns>A representação string do JOIN.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(type == SqlJoin.Type.Inner ? "INNER" : "LEFT OUTER");
			sb.AppendFormat(" JOIN ").Append(tableName);
			if (cond != null)
				sb.AppendFormat(" ON {0}", cond);

			return sb.ToString();
		}
	}
	
	/// <summary>
	/// Uma origem vinda de uma tabela SQL.
	/// </summary>
	[Serializable]
	public class SqlTableSource : SqlSource
	{
		string tableName;
		ArrayList joins;

		/// <summary>
		/// Cria uma nova origem de tabela SQL, para a tabela especificada.
		/// </summary>
		/// <param name="tableName">O nome da tabela</param>
		public SqlTableSource(string tableName)
		{
			this.tableName = tableName;
		}

		/// <summary>
		/// Realiza um JOIN com outra tabela.
		/// </summary>
		/// <param name="joinType">O tipo de JOIN</param>
		/// <param name="tableName">A tabela com a qual o JOIN será realizado</param>
		/// <param name="cond">A condição</param>
		public SqlTableSource Join(SqlJoin.Type joinType, string tableName, SqlCondition cond) 
		{
			if (joins == null)
				joins = new ArrayList();

			joins.Add(new SqlJoin(joinType, tableName, cond));

			return this;
		}

		/// <summary>
		/// Realiza um INNER JOIN com outra tabela.
		/// </summary>
		/// <param name="tableName">A tabela com a qual o JOIN será realizado</param>
		/// <param name="cond">A condição</param>
		public SqlTableSource Join(string tableName, SqlCondition cond) 
		{
			if (joins == null)
				joins = new ArrayList();

			joins.Add(new SqlJoin(tableName, cond));

			return this;
		}

		/// <summary>
		/// Retorna a representação string desta tabela para uso na cláusula FROM do SQL.
		/// </summary>
		/// <returns>A representação string desta tabela.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(tableName);
			if (joins != null)
				foreach (SqlJoin join in joins)
					sb.AppendFormat(" {0}", join);

			return sb.ToString();
		}

	}
}
