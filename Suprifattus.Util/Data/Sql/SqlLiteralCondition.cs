#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Condi��o literal SQL.
	/// </summary>
	[Serializable]
	public class SqlLiteralCondition : SqlCondition 
	{
		private string text;

		/// <summary>
		/// Cria uma nova condi��o literal SQL.
		/// </summary>
		/// <param name="text">A condi��o</param>
		public SqlLiteralCondition(string text) 
		{
			this.text = text;
		}

		/// <summary>
		/// Cria uma nova condi��o literal SQL.
		/// </summary>
		/// <param name="format">O formato da condi��o</param>
		/// <param name="args">Os argumentos da condi��o.</param>
		public SqlLiteralCondition(string format, params object[] args)
			: this(String.Format(format, args)) { }
		
		/// <summary>
		/// Renderiza o resultado da condi��o no <see cref="IndentedTextWriter"/> especificado.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public override void Render(IndentedTextWriter tw)
		{
			tw.WriteLine(text);
		}
		
		/// <summary>
		/// Retorna a representa��o string da condi��o SQL.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return text;
		}
	}
	
}
