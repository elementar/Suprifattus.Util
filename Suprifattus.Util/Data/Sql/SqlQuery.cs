using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Consulta SQL.
	/// </summary>
	[Serializable]
	public class SqlQuery : ISqlQuery
	{
		SqlFieldCollection fields = new SqlFieldCollection();
		SqlSource source;
		SqlCondition condition;
		SqlOrder order;
		bool distinct;
		string top;

		/// <summary>
		/// Cria uma nova consulta SQL.
		/// </summary>
		public SqlQuery() 
		{
		}
			
		/// <summary>
		/// Cria uma nova consulta SQL, a partir da origem especificada.
		/// </summary>
		/// <param name="fields">Os campos</param>
		/// <param name="source">A origem</param>
		/// <param name="condition">A condição</param>
		/// <param name="order">A ordem</param>
		public SqlQuery(string[] fields, SqlSource source, SqlCondition condition, SqlOrder order)
		{
			if (fields != null)
				this.fields.AddRange(fields);
			this.source = source;
			this.condition = condition;
			this.order = order;
		}

		/// <summary>
		/// Cria uma nova consulta SQL, a partir da origem especificada.
		/// </summary>
		/// <param name="source">A origem</param>
		/// <param name="condition">A condição</param>
		/// <param name="orderBy">A ordem</param>
		public SqlQuery(SqlSource source, SqlCondition condition, SqlOrder orderBy)
			: this(null, source, condition, orderBy)
		{
		}

		/// <summary>
		/// Cria uma nova consulta SQL, a partir da origem especificada.
		/// </summary>
		/// <param name="source">A origem</param>
		/// <param name="condition">A condição</param>
		public SqlQuery(SqlSource source, SqlCondition condition)
			: this(null, source, condition, null)
		{
		}

		/// <summary>
		/// Cria uma nova consulta SQL, a partir da origem especificada.
		/// </summary>
		/// <param name="source">A origem</param>
		public SqlQuery(SqlSource source)
			: this(null, source, null, null)
		{
		}

		/// <summary>
		/// A origem da consulta.
		/// </summary>
		public SqlSource Source 
		{
			get { return source; }
			set { source = value; }
		}

		/// <summary>
		/// Se a consulta deve retornar apenas registros distintos.
		/// </summary>
		public bool Distinct
		{
			get { return distinct; }
			set { distinct = value; }
		}

		/// <summary>
		/// Se a consulta deve retornar apenas os <c>X</c> primeiros registros.
		/// <c>X</c> pode ser um número inteiro ou um percentual.
		/// </summary>
		public string Top
		{
			get { return top; }
			set { top = value; }
		}

		/// <summary>
		/// A condição da consulta.
		/// </summary>
		public SqlCondition Condition
		{
			get { return condition; }
			set { condition = value; }
		}

		/// <summary>
		/// A ordem da consulta.
		/// </summary>
		public SqlOrder Order
		{
			get { return order; }
			set { order = value; }
		}

		/// <summary>
		/// Os campos da consulta.
		/// </summary>
		public SqlFieldCollection Fields
		{
			get { return fields; }
		}

		/// <summary>
		/// Adiciona um campo à consulta SQL.
		/// </summary>
		/// <param name="fieldName">O campo</param>
		[Obsolete]
		public void AddField(string fieldName) 
		{
			fields.Add(fieldName);
		}

		/// <summary>
		/// Adiciona vários campos à consulta SQL.
		/// </summary>
		/// <param name="fieldNames">Os campos</param>
		[Obsolete]
		public void AddFields(params string[] fieldNames) 
		{
			fields.AddRange(fieldNames);
		}
		
		/// <summary>
		/// Renderiza a consulta.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public void Render(IndentedTextWriter tw)
		{
			tw.Write("SELECT ");
			if (distinct)
				tw.Write("DISTINCT ");
			if (!Logic.StringEmpty(Top))
				tw.WriteLine("TOP {0}", Top);
			tw.WriteLine();
			tw.Indent++;

			int i = 0, l = fields.Count;
			foreach (SqlField field in fields)
				tw.WriteLine("{0}{1}", field, ++i >= l ? "" : ", ");
			
			if (l == 0)
				tw.WriteLine("*");

			tw.Indent--;

			if (source != null)
			{
				tw.WriteLine("FROM");
				tw.Indent++;
				tw.WriteLine(source);
				tw.Indent--;
			}

			if (condition != null) 
			{
				tw.WriteLine("WHERE");
				tw.Indent++;
				condition.Render(tw);
				tw.Indent--;
			}

			if (Order != null) 
			{
				tw.WriteLine("ORDER BY");
				tw.Indent++;
				Order.Render(tw);
				tw.Indent--;
			}
		}
		
		/// <summary>
		/// Retorna a representação string da consulta SQL.
		/// </summary>
		/// <returns>A representação string da consulta SQL.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			using (StringWriter sw = new StringWriter(sb))
			using (IndentedTextWriter tw = new IndentedTextWriter(sw)) 
			{
				Render(tw);
				tw.Flush();
			}
			
			return sb.ToString();
		}
	}
}
