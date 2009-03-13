using System;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Representa uma consulta SQL est�tica.
	/// </summary>
	[Serializable]
	public class SqlStaticQuery : ISqlQuery
	{
		string sql;
		
		/// <summary>
		/// Cria uma nova consulta SQL est�tica.
		/// </summary>
		/// <param name="sql">A consulta SQL</param>
		public SqlStaticQuery(string sql)
		{
			this.sql = sql;
		}

		#region Implementa��es expl�citas obrigat�rias
		SqlFieldCollection ISqlQuery.Fields
		{
			get { throw new NotImplementedException(); }
		}

		SqlOrder ISqlQuery.Order
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		#endregion

		/// <summary>
		/// Retorna a representa��o string da consulta.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return sql;
		}
	}
}
