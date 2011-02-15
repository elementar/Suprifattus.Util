using System;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Qualquer exceção causada deve herdar desta classe.
	/// </summary>
	[Serializable]
	public abstract class BaseException : Exception
	{
		protected BaseException()
		{
		}

		protected BaseException(string title, string message)
			: base(message)
		{
			this.Title = title;
		}

		protected BaseException(string title, string message, Exception innerException)
			: base(message, innerException)
		{
			this.Title = title;
		}

		#region Serialization Support
		protected BaseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Title = info.GetString("title");
			this.AdditionalInfo = info.GetString("additionalInfo");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("title", Title);
			info.AddValue("additionalInfo", AdditionalInfo);
			base.GetObjectData(info, context);
		}
		#endregion

		public BaseException SetAdditionalInfo(string additionalInfo)
		{
			this.AdditionalInfo = additionalInfo;
			return this;
		}

		public BaseException SetAdditionalInfo(string additionalInfoFormat, params object[] additionalInfoArgs)
		{
			return SetAdditionalInfo(String.Format(additionalInfoFormat, additionalInfoArgs));
		}

		public string Title { get; private set; }

		public string AdditionalInfo { get; private set; }

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(this.GetType()).Append(": ");
			if (!String.IsNullOrEmpty(this.Title))
				sb.Append(this.Title).Append(": ");
			sb.Append(this.Message);
			if (!String.IsNullOrEmpty(this.AdditionalInfo))
				sb.Append(" (").Append(this.AdditionalInfo).Append(")");
			sb.Append(Environment.NewLine);
			sb.Append(this.StackTrace);
			return sb.ToString();
		}
	}
}