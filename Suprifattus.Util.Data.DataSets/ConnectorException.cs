using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Suprifattus.Util.Data
{
	#region enum ConnectorExceptionType
	/// <summary>
	/// Os tipos de exceções do Connector.
	/// </summary>
	public enum ConnectorExceptionType 
	{
		/// <summary>
		/// Outro tipo de exceção
		/// </summary>
		Other,

		/// <summary>
		/// Erro de violação de chave estrangeira.
		/// </summary>
		ForeignKeyViolation,

		/// <summary>
		/// Erro de violação de chave primária.
		/// </summary>
		PrimaryKeyViolation,

		/// <summary>
		/// Valores nulos não permitidos
		/// </summary>
		NullNotAllowed,

		/// <summary>
		/// Violação de constraint UNIQUE
		/// </summary>
		UniqueConstraintViolation,
	}
	#endregion

	/// <summary>
	/// Exceção lançada quando ocorre algum erro no conector.
	/// </summary>
	[Serializable]
	public class ConnectorException : Exception
	{
		private ConnectorExceptionType type;
		private string sqlQuery;

		#region Construtores
		/// <summary>
		/// Cria uma nova exceção do <see cref="IConnector"/>.
		/// </summary>
		/// <param name="type">O tipo de exceção, caso seja de um tipo específico</param>
		/// <param name="msg">A mensagem de erro</param>
		/// <param name="innerException">A exceção que é causa desta</param>
		public ConnectorException(ConnectorExceptionType type, string msg, Exception innerException)
			: base(FormatMessage(type, msg), innerException)
		{
			this.type = type;
		}

		/// <summary>
		/// Cria uma nova exceção do <see cref="IConnector"/>.
		/// </summary>
		/// <param name="type">O tipo de exceção, caso seja de um tipo específico</param>
		/// <param name="innerException">A exceção que é causa desta</param>
		public ConnectorException(ConnectorExceptionType type, Exception innerException)
			: this(type, innerException.Message, innerException) { }

		/// <summary>
		/// Cria uma nova exceção do <see cref="IConnector"/>.
		/// </summary>
		/// <param name="msg">A mensagem de erro</param>
		/// <param name="innerException">A exceção que é causa desta</param>
		public ConnectorException(string msg, Exception innerException)
			: this(ConnectorExceptionType.Other, msg, innerException) { }

		/// <summary>
		/// Cria uma nova exceção do <see cref="IConnector"/>.
		/// </summary>
		/// <param name="msg">A mensagem de erro</param>
		public ConnectorException(string msg)
			: this(ConnectorExceptionType.Other, msg, null) { }

		/// <summary>
		/// Cria uma nova exceção do <see cref="IConnector"/>.
		/// </summary>
		/// <param name="innerException">A exceção que é causa desta</param>
		public ConnectorException(Exception innerException)
			: this(innerException.Message, innerException) { }
		#endregion

		#region Suporte a Serialização
		/// <summary>
		/// Utilizado no suporte a serialização.
		/// </summary>
		protected ConnectorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			string p = GetType().FullName;
			this.type = (ConnectorExceptionType) info.GetInt32(p + ".type");
			this.sqlQuery = info.GetString(p + ".sql");
		}

		/// <summary>
		/// Utilizado no suporte a serialização.
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			string p = GetType().FullName;
			info.AddValue(p + ".type", (int) type);
			info.AddValue(p + ".sql", sqlQuery);
			base.GetObjectData(info, context);
		}
		#endregion

		#region Propriedades Públicas
		/// <summary>
		/// O tipo de exceção do <see cref="IConnector"/>.
		/// </summary>
		public ConnectorExceptionType ExceptionType 
		{
			get { return type; }
		}

		public string SqlQuery
		{
			get { return sqlQuery; }
		}
		#endregion

		internal void SetSqlQuery(string sqlQuery)
		{
			this.sqlQuery = sqlQuery;
		}
		
		/// <summary>
		/// Detalha uma exceção, encapsulando-a em uma nova exceção, contendo a exceção
		/// atual como InnerException.
		/// </summary>
		/// <param name="format">Formato da mensagem de detalhe</param>
		/// <param name="args">Argumentos da mensagem de detalhe</param>
		/// <returns>Uma nova <see cref="ConnectorException"/></returns>
		public ConnectorException Detail(string format, params object[] args)
		{
			return new ConnectorException(this.ExceptionType, String.Format(format, args), this);
		}
		
		private static string FormatMessage(ConnectorExceptionType t, string msg)
		{
			if (t == ConnectorExceptionType.Other)
				return msg;
			else
				return String.Format("[{0}] {1}", t, msg);
		}
	}
}
