using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Suprifattus.Util.Data
{
	/// <summary>
	/// Ferramentas para trabalhar com DataSets
	/// </summary>
	public class DataSetTools
	{
		/// <summary>
		/// Constrói uma consulta SQL contendo os campos e o nome da tabela
		/// contidos na tabela especificada.
		/// </summary>
		/// <param name="dt">A tabela de origem</param>
		/// <returns>Uma consulta SQL baseada na tabela</returns>
		public static string MakeSelectSQL(DataTable dt) 
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("SELECT ");
			foreach (DataColumn col in dt.Columns)
				sb.AppendFormat("{0}, ", col.ColumnName);
			
			if (dt.Columns.Count > 0)
				sb.Length -= ", ".Length;
			else
				sb.Append("*");

			sb.AppendFormat(" FROM {0}", dt.TableName);

			Debug.WriteLine(sb.ToString());
			return sb.ToString();
		}

		/// <summary>
		/// Preenche as propriedades e os parâmetros de um comando do ADO.NET, baseado
		/// nos metadados contidos na DataTable, para construir um comando SQL DELETE.
		/// </summary>
		/// <param name="dt">A DataTable que contém os metadados</param>
		/// <param name="paramReplacement">Substituidor do nome do parâmetro (usualmente <c>?</c>)</param>
		/// <param name="cmd">O IDbCommand</param>
		/// <returns>O mesmo IDbCommand recebido</returns>
		public static IDbCommand FillDeleteCommand(DataTable dt, string paramReplacement, IDbCommand cmd) 
		{
			if (dt.PrimaryKey.Length == 0)
				throw new InvalidOperationException();

			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("DELETE FROM {0} WHERE ", dt.TableName);

			foreach (DataColumn col in dt.PrimaryKey) 
			{
				IDbDataParameter p = cmd.CreateParameter();
				p.SourceColumn = col.ColumnName;
				p.DbType = DbType.String;
				p.Size = 255;

				sb.AppendFormat("[{0}] = @{0} AND ", col.ColumnName);
				cmd.Parameters.Add(p);
			}

			sb.Length -= (" AND ").Length;

			Debug.WriteLine(sb.ToString());
			cmd.CommandText = sb.ToString();

			return ExpandParameters(cmd, paramReplacement);
		}

		/// <summary>
		/// Preenche as propriedades e os parâmetros de um comando do ADO.NET, baseado
		/// nos metadados contidos na DataTable, para construir um comando SQL INSERT.
		/// </summary>
		/// <param name="dt">A DataTable que contém os metadados</param>
		/// <param name="paramReplacement">Substituidor do nome do parâmetro (usualmente <c>?</c>)</param>
		/// <param name="cmd">O IDbCommand</param>
		/// <returns>O mesmo IDbCommand recebido</returns>
		public static IDbCommand FillInsertCommand(DataTable dt, string paramReplacement, IDbCommand cmd) 
		{
			if (dt.Columns.Count == 0)
				throw new InvalidOperationException();

			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("INSERT INTO [{0}] (", dt.TableName);

			foreach (DataColumn col in dt.Columns) 
			{
				if (!col.ReadOnly && !col.AutoIncrement)
					sb.AppendFormat("[{0}], ", col.ColumnName);
			}
			sb.Length -= (", ").Length;

			sb.Append(") VALUES (");

			foreach (DataColumn col in dt.Columns) 
				if (!col.ReadOnly && !col.AutoIncrement)
					sb.AppendFormat("@{0}, ", col.ColumnName);
			sb.Length -= ", ".Length;

			sb.Append(")");

			Debug.WriteLine(sb.ToString());

			cmd.CommandText = sb.ToString();
			
			return ExpandParameters(cmd, paramReplacement);
		}

		/// <summary>
		/// Expande os parâmetros.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="paramReplacement">Substituidor do nome do parâmetro (usualmente <c>?</c>)</param>
		/// <returns></returns>
		private static IDbCommand ExpandParameters(IDbCommand cmd, string paramReplacement) 
		{
			Regex rxParam = new Regex("@(\\w+)", RegexOptions.Compiled);

			foreach (Match m in rxParam.Matches(cmd.CommandText)) 
			{
				IDataParameter p = cmd.CreateParameter();
				p.SourceColumn = m.Groups[1].Value;
				cmd.Parameters.Add(p);
			}

			if (paramReplacement != null)
				cmd.CommandText = rxParam.Replace(cmd.CommandText, paramReplacement);

			return cmd;
		}
	}
}
