using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Exce��es que representam falhas causadas pelo usu�rio
	/// devem herdar desta exce��o.
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
