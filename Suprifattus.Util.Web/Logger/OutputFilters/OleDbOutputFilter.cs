using System;
using System.Data;
using System.Data.OleDb;

namespace Suprifattus.Util.Web.Logger.OutputFilters
{
	public class OleDbOutputFilter : IOutputFilter
	{
		string connStr;

		public OleDbOutputFilter(string connectionString)
		{
			this.connStr = connectionString;
		}

		public void Write(LogEntry entry)
		{
			using (OleDbConnection conn = new OleDbConnection(connStr))
			{
				using (OleDbCommand cmd = conn.CreateCommand()) 
				{
					cmd.CommandText = @"
						INSERT INTO [Request]
						(Date, Method, URL, Referral, ASPNETSessionID, UserHost, UserAddress, UserAgent)
						VALUES
						(@Date, @Method, @URL, @Referral, @ASPNETSessionID, @UserHost, @UserAddress, @UserAgent)
					";

					cmd.Parameters.Add("@Date", entry.Date);
					cmd.Parameters.Add("@Method", entry.Method);
					cmd.Parameters.Add("@URL", Convert.ToString(entry.Url));
					cmd.Parameters.Add("@Referral", Convert.ToString(entry.Referral));
					cmd.Parameters.Add("@ASPNETSessionID", entry.ASPNETSessionID);
					cmd.Parameters.Add("@UserHost", entry.UserHost);
					cmd.Parameters.Add("@UserAddress", entry.UserAddress);
					cmd.Parameters.Add("@UserAgent", entry.UserAgent);

					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}

		public override string ToString()
		{
			return String.Format("[{0}] ConnectionString = {1}", GetType().FullName, connStr);
		}
	}
}
