#define SQL_OPTIMIZATION

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;

using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Condição de comparação de campos SQL.
	/// </summary>
	[Serializable]
	public class SqlFieldCondition : SqlCondition
	{
		static readonly DbType[] NumericTypes 
			= { DbType.Boolean, DbType.Byte,   DbType.Currency, DbType.Decimal, DbType.Double,
					DbType.Int16,   DbType.Int32,  DbType.Int64,    DbType.SByte,   DbType.Single, 
					DbType.UInt16,  DbType.UInt32, DbType.UInt64,   DbType.VarNumeric };

		string fieldName;
		string op = "=";
		DbType fieldType;
		object fieldValue;

		/// <summary>
		/// Cria uma nova condição de comparação de campos em SQL.
		/// </summary>
		/// <param name="fieldName">Nome do campo</param>
		/// <param name="sqlOperation">A operação de comparação (=, &lt;&gt;, &lt;=, etc)</param>
		/// <param name="fieldValue">Valor do campo</param>
		public SqlFieldCondition(string fieldName, string sqlOperation, object fieldValue)
			: this(fieldName, InferType(fieldValue), sqlOperation, fieldValue)
		{
		}

		/// <summary>
		/// Cria uma nova condição de comparação de campos em SQL.
		/// </summary>
		/// <param name="fieldName">Nome do campo</param>
		/// <param name="fieldValue">Valor do campo</param>
		public SqlFieldCondition(string fieldName, object fieldValue)
			: this(fieldName, InferType(fieldValue), "=", fieldValue)
		{
		}

		/// <summary>
		/// Cria uma nova condição de comparação de campos em SQL.
		/// </summary>
		/// <param name="fieldName">Nome do campo</param>
		/// <param name="fieldType">Tipo do campo</param>
		/// <param name="fieldValue">Valor do campo</param>
		public SqlFieldCondition(string fieldName, DbType fieldType, object fieldValue) 
			: this(fieldName, fieldType, "=", fieldValue)
		{
		}

		/// <summary>
		/// Cria uma nova condição de comparação de campos em SQL.
		/// </summary>
		/// <param name="fieldName">Nome do campo</param>
		/// <param name="fieldType">Tipo do campo</param>
		/// <param name="sqlOperation">A operação de comparação (=, &lt;&gt;, &lt;=, etc)</param>
		/// <param name="fieldValue">Valor do campo</param>
		public SqlFieldCondition(string fieldName, DbType fieldType, string sqlOperation, object fieldValue) 
		{
			this.fieldName = fieldName;
			this.fieldType = fieldType;
			this.op = sqlOperation;
			this.fieldValue = fieldValue;

			CheckType();
		}

		private void CheckType()
		{
			string typeName = "System." + fieldType.ToString();
			if (typeName.EndsWith("Date"))
				typeName += "Time";

			Type t = Type.GetType(typeName);

			try 
			{
				if (fieldValue.GetType().IsArray)
				{
					Array oldArray = (Array) fieldValue;
					if (oldArray.GetType().GetElementType() != t)
					{
						Array newArray = Array.CreateInstance(t, oldArray.Length);
						for (int i=0; i < newArray.Length; i++)
							newArray.SetValue(Convert.ChangeType(oldArray.GetValue(i), t), i);
					}
				}
				else
					fieldValue = Convert.ChangeType(fieldValue, t);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(String.Format("The supplied value is not of type '{0}'", fieldType), ex);
			}
		}

		/// <summary>
		/// Delimitador de literais, utilizado na representação string.
		/// É baseado no tipo do campo, especificado em <see cref="fieldValue"/>.
		/// </summary>
		public string LiteralDelimiter
		{
			get 
			{
				if (Array.IndexOf(NumericTypes, fieldType) != -1)
					return "";

				return "'";
			}
		}
		
		protected string QuoteValue(object val)
		{
			string s;
			if (val is bool)
				s = ((bool) val) ? "1" : "0";
			else
			{
				string fmt = "{0}";
				if (fieldType == DbType.Date || fieldType == DbType.DateTime)
					fmt = "{0:s}";
				s = String.Format(CultureInfo.InvariantCulture, fmt, val);
				s = s.Replace("'", "''");
			}
			
			return String.Format("{1}{0}{1}", s, LiteralDelimiter);
		}
		
		/// <summary>
		/// Renderiza o resultado da condição no <see cref="IndentedTextWriter"/> especificado.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public override void Render(IndentedTextWriter tw)
		{
			tw.WriteLine(ToString());
		}
		
		/// <summary>
		/// Retorna a representação string da condição SQL.
		/// </summary>
		/// <returns>A representação string da condição SQL.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} {1} ", fieldName, op);
			if (fieldValue is Array)
				sb.AppendFormat("({0})", CollectionUtils.Join((IEnumerable) fieldValue, ", ", new FormatDelegate(QuoteValue)));
			else
				sb.Append(QuoteValue(fieldValue));
			
			return sb.ToString();
		}
	}
}
