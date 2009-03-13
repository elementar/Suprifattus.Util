#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Condi��o SQL negada.
	/// </summary>
	[Serializable]
	public class SqlNegatedCondition : SqlCondition 
	{
		readonly SqlCondition cond;
		
		/// <summary>
		/// Cria uma nova condi��o que nega outra condi��o.
		/// </summary>
		/// <param name="cond">A condi��o que deve ser negada.</param>
		public SqlNegatedCondition(SqlCondition cond) 
		{
			this.cond = cond;
		}
		
		/// <summary>
		/// A condi��o que � negada.
		/// </summary>
		public SqlCondition InnerCondition
		{
			get { return cond; }
		}

		/// <summary>
		/// Renderiza o resultado da condi��o no <see cref="IndentedTextWriter"/> especificado.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public override void Render(IndentedTextWriter tw)
		{
			tw.WriteLine("NOT (");
			tw.Indent++;

			cond.Render(tw);

			tw.Indent--;

			tw.WriteLine(")");
		}

		
		/// <summary>
		/// Retorna a representa��o string da condi��o.
		/// </summary>
		public override string ToString()
		{
			return String.Format("NOT ({0})", cond);
		}
	}

}
