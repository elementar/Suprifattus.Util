using System;
using System.Text.RegularExpressions;

using Suprifattus.Util.Data.Resources;

namespace Suprifattus.Util.Data
{
	/// <summary>
	/// Static Factory para <see cref="ConnectorException"/>s.
	/// </summary>
	public sealed class ConnectorExceptionFactory
	{
		static Regex 
			rxDelete = new Regex("^DELETE statement conflicted with COLUMN REFERENCE constraint", RegexOptions.Compiled),
			rxInsert = new Regex("^INSERT statement conflicted with COLUMN FOREIGN KEY constraint '(?<c>[^']+)'. The conflict occurred in database '(?<d>[^']+)', table '(?<t>[^']+)', column '(?<col>[^']+)'.", RegexOptions.Compiled),
			rxPrimaryKey = new Regex("^Violation of PRIMARY KEY constraint '(?<c>[^']+)'. Cannot insert duplicate key in object '(?<t>[^']+)'", RegexOptions.Compiled),
			rxUnique = new Regex("^Cannot insert duplicate key row in object '(?<t>[^']+)' with unique index '(?<c>[^']+)'", RegexOptions.Compiled);
		
		private ConnectorExceptionFactory() { throw new InvalidOperationException(); }

		/// <summary>
		/// Encapsula uma exce��o em uma <see cref="ConnectorException"/>,
		/// salvando tamb�m a instru��o SQL que causou a exce��o.
		/// Se poss�vel, faz a interpreta��o da mensagem e preenche o campo
		/// <see cref="Type"/>.
		/// </summary>
		/// <param name="ex">A exce��o a ser encapsulada</param>
		/// <param name="sqlQuery">A consulta SQL que causou a exce��o.</param>
		/// <returns>A nova ConnectorException.</returns>
		public static ConnectorException FromDatabaseException(Exception ex, string sqlQuery)
		{
			ConnectorException e = FromDatabaseException(ex);
			e.SetSqlQuery(sqlQuery);

			return e;
		}
		
		/// <summary>
		/// Encapsula uma exce��o em uma <see cref="ConnectorException"/>.
		/// Se poss�vel, faz a interpreta��o da mensagem e preenche o campo
		/// <see cref="Type"/>.
		/// </summary>
		/// <param name="ex">A exce��o a ser encapsulada</param>
		/// <returns>A nova ConnectorException.</returns>
		public static ConnectorException FromDatabaseException(Exception ex)
		{
			string s = ex.Message;
			string msg = s;

			ConnectorExceptionType t = ConnectorExceptionType.Other;
			Match m;

			if ((m = rxDelete.Match(s)).Success) 
			{
				t = ConnectorExceptionType.ForeignKeyViolation;
				msg = Strings.FormatString("Data.Connector.Exception.FKViolation", "(null)");
			}
			else if ((m = rxInsert.Match(s)).Success)
			{
				t = ConnectorExceptionType.ForeignKeyViolation;
			}
			else if ((m = rxPrimaryKey.Match(s)).Success) 
			{
				t = ConnectorExceptionType.PrimaryKeyViolation;
				msg = Strings.FormatString("Data.Connector.Exception.PKViolation", m.Groups["t"]);
			}
			else if ((m = rxUnique.Match(s)).Success) 
			{
				t = ConnectorExceptionType.UniqueConstraintViolation;
				msg = Strings.FormatString("Data.Connector.Exception.UniqueViolation", m.Groups["t"]);
			}

			return new ConnectorException(t, msg, ex);
		}	
	}
}
