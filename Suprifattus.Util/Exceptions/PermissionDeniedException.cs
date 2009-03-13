using System;
using System.Runtime.Serialization;

using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Violação de segurança explicitamente por ter permissão negada.
	/// </summary>
	[Serializable]
	public class PermissionDeniedException : SecurityException
	{
		const string DefaultTitle = "Usuário não possui permissão";
		
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