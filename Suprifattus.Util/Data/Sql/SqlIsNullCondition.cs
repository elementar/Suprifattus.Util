#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Condi��o "campo � nulo".
	/// </summary>
	[Serializable]
	public class SqlIsNullCondition : SqlCondition
	{
		string fieldName;
		bool isNull;

		/// <summary>
		/// Cria uma nova condi��o "campo � nulo".
		/// </summary>
		/// <param name="fieldName">Nome do campo</param>
		/// <param name="isNull">Verdadeiro para verificar se � nulo, falso caso contr�rio</param>
		public SqlIsNullCondition(string fieldName, bool isNull)
		{
			this.fieldName = fieldName;
			this.isNull = isNull;
		}

		/// <summary>
		/// Renderiza o resultado da condi��o no <see cref="IndentedTextWriter"/> especificado.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public override void Render(IndentedTextWriter tw)
		{
			tw.WriteLine("{0} {1} NULL", fieldName, isNull ? "IS" : "IS NOT");
		}
		
		/// <summary>
		/// Retorna a representa��o string da condi��o.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} {1} NULL", fieldName, isNull ? "IS" : "IS NOT");
		}
	}
	
}
