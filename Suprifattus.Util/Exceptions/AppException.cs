using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Exceções que representam falhas causadas pelo usuário
	/// devem herdar desta exceção.
	/// </summary>
	[Serializable]
	public class AppException : BaseException
	{
		public AppException()
		{
		}

		public AppException(string title, string message)
			: base(title, message) { }

		public AppException(string title, string message, Exception innerException)
			: base(title, message, innerException) { }

		#region Serialization Support
		protected AppException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
		#endregion
	}
}
