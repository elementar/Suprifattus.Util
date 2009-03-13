using System;
using System.Runtime.Serialization;
using System.Text;

namespace Suprifattus.Util.Exceptions
{
	/// <summary>
	/// Qualquer exceção causada deve herdar desta classe.
	/// </summary>
	[Serializable]
	public abstract class BaseException : Exception
	{
		private string title = null, additionalInfo = null;

		public BaseException()
		{
		}

		public BaseException(string title, string message)
			: base(message)
		{
			this.title = title;
		}

		public BaseException(string title, string message, Exception innerException)
			: base(message, innerException)
		{
			this.title = title;
		}

		#region Serialization Support
		protected BaseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.title = info.GetString("title");
			this.additionalInfo = info.GetString("additionalInfo");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("title", title);
			info.AddValue("additionalInfo", additionalInfo);
			base.GetObjectData(info, context);
		}
		#endregion

		public BaseException SetAdditionalInfo(string additionalInfo)
		{
			this.additionalInfo = additionalInfo;
			return this;
		}
		
		public BaseException SetAdditionalInfo(string additionalInfoFormat, params object[] additionalInfoArgs)
		{
			return SetAdditionalInfo(String.Format(additionalInfoFormat, additionalInfoArgs));
		}
		
		public string Title
		{
			get { return title; }
		}

		public string AdditionalInfo
		{
			get { return additionalInfo; }
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.GetType());
			sb.Append(": ");
			if (!Logic.StringEmpty(this.Title))
				sb.Append(this.Title).Append(": ");
			sb.Append(this.Message);
			if (!Logic.StringEmpty(this.AdditionalInfo))
				sb.Append(" (").Append(this.AdditionalInfo).Append(")");
			sb.Append(Environment.NewLine);
			sb.Append(this.StackTrace);
			return sb.ToString();
		}
	}
}
