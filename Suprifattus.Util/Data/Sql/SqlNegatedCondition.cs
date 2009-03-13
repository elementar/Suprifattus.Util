#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Condição SQL negada.
	/// </summary>
	[Serializable]
	public class SqlNegatedCondition : SqlCondition 
	{
		readonly SqlCondition cond;
		
		/// <summary>
		/// Cria uma nova condição que nega outra condição.
		/// </summary>
		/// <param name="cond">A condição que deve ser negada.</param>
		public SqlNegatedCondition(SqlCondition cond) 
		{
			this.cond = cond;
		}
		
		/// <summary>
		/// A condição que é negada.
		/// </summary>
		public SqlCondition InnerCondition
		{
			get { return cond; }
		}

		/// <summary>
		/// Renderiza o resultado da condição no <see cref="IndentedTextWriter"/> especificado.
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
		/// Retorna a representação string da condição.
		/// </summary>
		public override string ToString()
		{
			return String.Format("NOT ({0})", cond);
		}
	}

}
