#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Condição literal SQL.
	/// </summary>
	[Serializable]
	public class SqlLiteralCondition : SqlCondition 
	{
		private string text;

		/// <summary>
		/// Cria uma nova condição literal SQL.
		/// </summary>
		/// <param name="text">A condição</param>
		public SqlLiteralCondition(string text) 
		{
			this.text = text;
		}

		/// <summary>
		/// Cria uma nova condição literal SQL.
		/// </summary>
		/// <param name="format">O formato da condição</param>
		/// <param name="args">Os argumentos da condição.</param>
		public SqlLiteralCondition(string format, params object[] args)
			: this(String.Format(format, args)) { }
		
		/// <summary>
		/// Renderiza o resultado da condição no <see cref="IndentedTextWriter"/> especificado.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public override void Render(IndentedTextWriter tw)
		{
			tw.WriteLine(text);
		}
		
		/// <summary>
		/// Retorna a representação string da condição SQL.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return text;
		}
	}
	
}
