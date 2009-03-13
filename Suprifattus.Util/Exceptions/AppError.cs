using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Exceções que representam erros do sistema devem herdar desta exceção.
	/// </summary>
	[Serializable]
	[CLSCompliant(false)]
	public class AppError : BaseException
	{
		public AppError()
		{
		}

		public AppError(string title, string message)
			: base(title, message) { }

		public AppError(string title, string message, Exception innerException)
			: base(title, message, innerException) { }

		#region Serialization Support
		protected AppError(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
		#endregion
	}
}