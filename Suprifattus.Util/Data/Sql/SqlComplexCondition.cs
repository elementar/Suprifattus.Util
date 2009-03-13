#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Text;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Condição complexa SQL.
	/// </summary>
	[Serializable]
	public class SqlComplexCondition : SqlCondition 
	{
		readonly SqlLogicOperation op;
		ArrayList conds = new ArrayList(2);
		
		/// <summary>
		/// Cria uma nova condição complexa SQL.
		/// </summary>
		/// <param name="op">O operador (pode ser E ou OU)</param>
		/// <param name="cond1">A condição da esquerda</param>
		/// <param name="cond2">A condição da direita</param>
		public SqlComplexCondition(SqlLogicOperation op, SqlCondition cond1, SqlCondition cond2) 
		{
			this.op = op;
			conds.Add(cond1);
			conds.Add(cond2);
		}
		
		/// <summary>
		/// A operação lógica que esta condição representa.
		/// Pode ser <see cref="SqlLogicOperation.And"/> ou
		/// <see cref="SqlLogicOperation.Or"/>.
		/// </summary>
		public SqlLogicOperation Operation
		{
			get { return op; }
		}
		
		/// <summary>
		/// Adiciona uma condição ao início desta.
		/// </summary>
		/// <param name="cond">A condição a ser adicionada</param>
		/// <returns>Esta mesma condição complexa</returns>
		public SqlComplexCondition AppendStart(SqlCondition cond)
		{
			conds.Insert(0, cond);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição ao fim desta.
		/// </summary>
		/// <param name="cond">A condição a ser adicionada</param>
		/// <returns>Esta mesma condição complexa</returns>
		public SqlComplexCondition AppendEnd(SqlCondition cond)
		{
			conds.Add(cond);
			return this;
		}
		
		/// <summary>
		/// Renderiza o resultado da condição no <see cref="IndentedTextWriter"/> especificado.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public override void Render(IndentedTextWriter tw)
		{
			string ops = (op == SqlLogicOperation.And ? "AND" : "OR");
			int i = 0;

			tw.WriteLine("(");
			tw.Indent++;

			foreach (SqlCondition cond in conds)
			{
				if (i++ > 0)
					tw.Write("{0} ", ops);
				cond.Render(tw);
			}

			tw.Indent--;
			tw.WriteLine(")");
		}
		
		/// <summary>
		/// Retorna a representação string da condição SQL.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			string ops = (op == SqlLogicOperation.And ? "AND" : "OR");
			int i = 0;
			foreach (SqlCondition cond in conds)
				sb.AppendFormat("{0} {1}", (i++ > 0 ? ops : ""), cond);

			return sb.ToString();
		}
	}
	
}
