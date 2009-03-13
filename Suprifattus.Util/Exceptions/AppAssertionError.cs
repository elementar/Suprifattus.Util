using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Exceções que representam erros de asserção do sistema devem herdar desta exceção.
	/// </summary>
	[Serializable]
	[CLSCompliant(false)]
	public class AppAssertionError : AppError
	{
		public AppAssertionError()
		{
		}

		public AppAssertionError(string title, string message)
			: base(title, message) { }

		public AppAssertionError(string title, string message, Exception innerException)
			: base(title, message, innerException) { }

		#region Serialization Support
		protected AppAssertionError(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
		#endregion
	}
}