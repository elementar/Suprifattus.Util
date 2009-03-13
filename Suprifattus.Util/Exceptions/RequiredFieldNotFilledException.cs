using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Uma exce��o espec�fica e muito usual: campo obrigat�rio n�o preenchido.
	/// </summary>
	[Serializable]
	public class RequiredFieldNotFilledException : BusinessRuleViolationException
	{
		public RequiredFieldNotFilledException() { }

		public RequiredFieldNotFilledException(string title, string message)
			: base(title, message) { }

		public RequiredFieldNotFilledException(string title, string message, Exception innerException)
			: base(title, message, innerException) { }

		public RequiredFieldNotFilledException(string field)
			: base(GetTitle(), CreateMessage(field)) { }

		public RequiredFieldNotFilledException(string field, Exception innerException)
			: base(GetTitle(), CreateMessage(field), innerException) { }

		protected static string GetTitle()
		{
			return "Campo obrigat�rio n�o preenchido";
		}

		protected static string CreateMessage(string field)
		{
			return String.Format("O campo obrigat�rio '{0}' n�o foi preenchido.", field);
		}

		protected RequiredFieldNotFilledException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}