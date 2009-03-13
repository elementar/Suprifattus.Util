using System;

namespace Suprifattus.Util.Web.Logger
{
	public struct LogEntry
	{
		public DateTime Date;
		public Uri Url, Referral;
		public string Method, ASPNETSessionID, UserHost, UserAddress, UserAgent;

		public LogEntry(DateTime date, string method, Uri url, Uri referral, string aspnetSessionID, string userHost, string userAddress, string userAgent)
		{
			this.Date = date;
			this.Method = method;
			this.Url = url;
			this.Referral = referral;
			this.ASPNETSessionID = aspnetSessionID;
			this.UserHost = userHost;
			this.UserAddress = userAddress;
			this.UserAgent = userAgent;
		}
		
		public void Clear()
		{
			Date = DateTime.Now;
			Method = ASPNETSessionID = UserHost = UserAddress = UserAgent = "";
			Url = Referral = null;
		}
	}
}
