#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Condição "campo é nulo".
	/// </summary>
	[Serializable]
	public class SqlIsNullCondition : SqlCondition
	{
		string fieldName;
		bool isNull;

		/// <summary>
		/// Cria uma nova condição "campo é nulo".
		/// </summary>
		/// <param name="fieldName">Nome do campo</param>
		/// <param name="isNull">Verdadeiro para verificar se é nulo, falso caso contrário</param>
		public SqlIsNullCondition(string fieldName, bool isNull)
		{
			this.fieldName = fieldName;
			this.isNull = isNull;
		}

		/// <summary>
		/// Renderiza o resultado da condição no <see cref="IndentedTextWriter"/> especificado.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public override void Render(IndentedTextWriter tw)
		{
			tw.WriteLine("{0} {1} NULL", fieldName, isNull ? "IS" : "IS NOT");
		}
		
		/// <summary>
		/// Retorna a representação string da condição.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} {1} NULL", fieldName, isNull ? "IS" : "IS NOT");
		}
	}
	
}
