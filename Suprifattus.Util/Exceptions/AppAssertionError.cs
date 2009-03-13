using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Exce��es que representam erros de asser��o do sistema devem herdar desta exce��o.
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