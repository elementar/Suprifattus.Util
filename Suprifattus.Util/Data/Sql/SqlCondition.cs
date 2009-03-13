#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;
using System.Data;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// As opera��es poss�veis entre condi��es.
	/// </summary>
	public enum SqlLogicOperation
	{
		/// <summary>
		/// E
		/// </summary>
		And,
		/// <summary>
		/// Ou
		/// </summary>
		Or,
	}

	/// <summary>
	/// Representa uma condi��o SQL.
	/// </summary>
	[Serializable]
	public abstract class SqlCondition : ISqlRenderable
	{
		private static SqlCondition MergeConditions(SqlCondition cLeft, SqlCondition cRight, SqlLogicOperation op)
		{
			if (cLeft == null)
				return cRight;
			if (cRight == null)
				return cLeft;
	
#if SQL_OPTIMIZATION
			if (cLeft is SqlComplexCondition) 
			{
				SqlComplexCondition ccLeft = (SqlComplexCondition) cLeft;
				if (ccLeft.Operation == op)
					return ccLeft.AppendEnd(cRight);
			}
#endif
	
			return new SqlComplexCondition(op, cLeft, cRight);
		}

		/// <summary>
		/// Cria uma nova <see cref="SqlCondition"/> comparando as duas condi��es fornecidas,
		/// utilizando o operador <see cref="SqlLogicOperation.Or"/>.
		/// </summary>
		/// <param name="cLeft">Uma condi��o</param>
		/// <param name="cRight">Outra condi��o</param>
		/// <returns>Uma nova condi��o, comparando as condi��es <paramref name="cLeft"/>
		/// e <paramref name="cRight"/> utilizando o operator <see cref="SqlLogicOperation.Or"/></returns>
		public static SqlCondition operator |(SqlCondition cLeft, SqlCondition cRight)
		{
			return MergeConditions(cLeft, cRight, SqlLogicOperation.Or);
		}

		/// <summary>
		/// Cria uma nova <see cref="SqlCondition"/> comparando as duas condi��es fornecidas,
		/// utilizando o operador <see cref="SqlLogicOperation.And"/>.
		/// </summary>
		/// <param name="cLeft">Uma condi��o</param>
		/// <param name="cRight">Outra condi��o</param>
		/// <returns>Uma nova condi��o, comparando as condi��es <paramref name="cLeft"/>
		/// e <paramref name="cRight"/> utilizando o operator <see cref="SqlLogicOperation.And"/></returns>
		public static SqlCondition operator &(SqlCondition cLeft, SqlCondition cRight)
		{
			return MergeConditions(cLeft, cRight, SqlLogicOperation.And);
		}

		/// <summary>
		/// Cria uma nova <see cref="SqlCondition"/> negando a condi��o fornecida.
		/// </summary>
		/// <param name="c">A condi��o</param>
		/// <returns>Uma nova condi��o que nega a condi��o fornecida em <paramref name="c"/></returns>
		public static SqlCondition operator !(SqlCondition c)
		{
			if (c == null)
				return c;

#if SQL_OPTIMIZATION
			if (c is SqlNegatedCondition)
				return ((SqlNegatedCondition) c).InnerCondition;
#endif
			
			return new SqlNegatedCondition(c);
		}

		/// <summary>
		/// Tenta inferir o tipo de um objeto, baseado em seu valor.
		/// </summary>
		/// <param name="val">O valor</param>
		/// <returns>O prov�vel <see cref="DbType"/></returns>
		/// <exception cref="ArgumentNullException">Se o valor for nulo</exception>
		protected static DbType InferType(object val)
		{
			if (val == null)
				throw new ArgumentNullException("val");

			Type t = val.GetType();
			if (t.IsArray)
				t = t.GetElementType();
			
			return (DbType) Enum.Parse(typeof(DbType), t.Name);
		}

		/// <summary>
		/// Renderiza o resultado da condi��o no <see cref="IndentedTextWriter"/> especificado.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public abstract void Render(IndentedTextWriter tw);
	}
}
