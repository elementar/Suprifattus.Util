using System;
using System.Runtime.Serialization;

using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Viola��o de seguran�a explicitamente por ter permiss�o negada.
	/// </summary>
	[Serializable]
	public class PermissionDeniedException : SecurityException
	{
		const string DefaultTitle = "Usu�rio n�o possui permiss�o";
		
		public PermissionDeniedException(string message)
			: base(DefaultTitle, message)
		{
		}

		public PermissionDeniedException(string title, string message)
			: base(title, message)
		{
		}

		#region Serialization support
		protected PermissionDeniedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion
	}
}