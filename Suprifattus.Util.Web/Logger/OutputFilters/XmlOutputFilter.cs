using System;
using System.IO;
using System.Text;

namespace Suprifattus.Util.Web.Logger.OutputFilters
{
	public class XmlOutputFilter : IOutputFilter
	{
		FileInfo fi;

		public XmlOutputFilter(FileInfo fi)
		{
			this.fi = fi;
		}

		public void Write(LogEntry entry)
		{
			StringBuilder line = new StringBuilder();

			line.Length = 0;
			line.AppendFormat("\t<webRequest method=\"{0}\" url=\"{1}\">\n", entry.Method, entry.Url);
			line.AppendFormat("\t\t<datetime value=\"{0}\"/>\n", DateTime.Now);
			line.AppendFormat("\t\t<session aspnet=\"{0}\"/>\n", entry.ASPNETSessionID);
			line.AppendFormat("\t\t<user host=\"{0}\" address=\"{1}\" agent=\"{2}\"/>\n", entry.UserHost, entry.UserAddress, entry.UserAgent);
			line.AppendFormat("\t\t<referral url=\"{0}\"/>\n", entry.Referral);
			line.AppendFormat("\t</webRequest>");
			
			using (TextWriter writer = new StreamWriter(fi.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite))) 
			{
				writer.WriteLine(line);
				writer.Flush();
			}
		}

		public override string ToString()
		{
			return String.Format("[{0}] File = {1}", GetType().FullName, fi.FullName);
		}
	}
}
