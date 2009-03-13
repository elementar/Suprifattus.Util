using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Exce��es que representam especificamente viola��es de regras de neg�cio programadas
	/// devem herdar dessa exce��o.
	/// </summary>
	[Serializable]
	public class BusinessRuleViolationException : AppException
	{
		public BusinessRuleViolationException()
		{
		}

		public BusinessRuleViolationException(string title, string message)
			: base(title, message)
		{
		}

		public BusinessRuleViolationException(string title, string message, Exception innerException)
			: base(title, message, innerException)
		{
		}

		#region Serialization support
		protected BusinessRuleViolationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion
	}
}
