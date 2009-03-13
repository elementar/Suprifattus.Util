#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Text;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Condi��o complexa SQL.
	/// </summary>
	[Serializable]
	public class SqlComplexCondition : SqlCondition 
	{
		readonly SqlLogicOperation op;
		ArrayList conds = new ArrayList(2);
		
		/// <summary>
		/// Cria uma nova condi��o complexa SQL.
		/// </summary>
		/// <param name="op">O operador (pode ser E ou OU)</param>
		/// <param name="cond1">A condi��o da esquerda</param>
		/// <param name="cond2">A condi��o da direita</param>
		public SqlComplexCondition(SqlLogicOperation op, SqlCondition cond1, SqlCondition cond2) 
		{
			this.op = op;
			conds.Add(cond1);
			conds.Add(cond2);
		}
		
		/// <summary>
		/// A opera��o l�gica que esta condi��o representa.
		/// Pode ser <see cref="SqlLogicOperation.And"/> ou
		/// <see cref="SqlLogicOperation.Or"/>.
		/// </summary>
		public SqlLogicOperation Operation
		{
			get { return op; }
		}
		
		/// <summary>
		/// Adiciona uma condi��o ao in�cio desta.
		/// </summary>
		/// <param name="cond">A condi��o a ser adicionada</param>
		/// <returns>Esta mesma condi��o complexa</returns>
		public SqlComplexCondition AppendStart(SqlCondition cond)
		{
			conds.Insert(0, cond);
			return this;
		}

		/// <summary>
		/// Adiciona uma condi��o ao fim desta.
		/// </summary>
		/// <param name="cond">A condi��o a ser adicionada</param>
		/// <returns>Esta mesma condi��o complexa</returns>
		public SqlComplexCondition AppendEnd(SqlCondition cond)
		{
			conds.Add(cond);
			return this;
		}
		
		/// <summary>
		/// Renderiza o resultado da condi��o no <see cref="IndentedTextWriter"/> especificado.
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
		/// Retorna a representa��o string da condi��o SQL.
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
