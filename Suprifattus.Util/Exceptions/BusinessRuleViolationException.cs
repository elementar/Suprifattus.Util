using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Exceções que representam especificamente violações de regras de negócio programadas
	/// devem herdar dessa exceção.
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
