using System;
using System.Runtime.Serialization;

using Castle.ActiveRecord;

using Suprifattus.Util.Collections;
using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Web.MonoRail
{
	[Serializable]
	public class AppValidationException : AppException
	{
		/*public AppValidationException(string title, string message, ValidationException ex, ActiveRecordValidationBase ar)
			: base(title, message, ex)
		{
			SetAdditionalInfo(CollectionUtils.Join(ar.ValidationErrorMessages, "\n"));
		}

		public AppValidationException(ValidationException ex, ActiveRecordValidationBase ar)
			: this("Falha na validação", "Ocorreram erros durante a validação", ex, ar)
		{
		}*/

		public AppValidationException(string title, string message)
			: base(title, message) { }

		public AppValidationException(string title, string message, Exception innerException)
			: base(title, message, innerException) { }

		protected AppValidationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}

	[Serializable]
	public class DuplicatedRecordException : AppValidationException
	{
		public DuplicatedRecordException(string message)
			: base(GetTitle(), message) { }

		public DuplicatedRecordException(string message, Exception innerException)
			: base(GetTitle(), message, innerException) { }

		protected DuplicatedRecordException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }

		protected static string GetTitle()
		{
			return "Registro Duplicado";
		}
	}

	[Serializable]
	public class CouldNotDeleteException : AppValidationException
	{
		public CouldNotDeleteException(string message)
			: base(GetTitle(), message) { }

		public CouldNotDeleteException(string message, Exception innerException)
			: base(GetTitle(), message, innerException) { }

		protected CouldNotDeleteException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }

		protected static string GetTitle()
		{
			return "Exclusão não permitida";
		}
	}
}