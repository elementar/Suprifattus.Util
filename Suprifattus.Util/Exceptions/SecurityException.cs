using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Exceções de violação de segurança, como acesso negado.
	/// </summary>
	[Serializable]
	public class SecurityException : BusinessRuleViolationException
	{
		const string DefaultTitle = "Acesso Negado";
		
		public SecurityException(string message)
			: base(DefaultTitle, message) { }

		public SecurityException(string title, string message)
			: base(title, message)
		{
		}

		#region Serialization support
		protected SecurityException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion
	}
}
