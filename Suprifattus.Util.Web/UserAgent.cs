using System;
using System.Text.RegularExpressions;

namespace Suprifattus.Util.Web
{
	/// <summary>
	/// Interpreta e cria um novo UserAgent.
	/// </summary>
	public class UserAgent
	{
		static readonly Regex rxUA = new Regex(@"^Mozilla/(?<mv>[\d.]+) \((?<comments>[^)]+)\) Gecko/(?<gr>\d+)( (?<v>.*?)/(?<vv>[\d.]+))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		bool valid = false;
		
		string mozillaVersion;
		string comments;
		long geckoRelease;
		string vendor;
		string vendorVersion;

		public UserAgent(string uaString)
		{
			Match m = rxUA.Match(uaString);
			if (!m.Success)
				return;

			try 
			{
				this.valid = true;
				this.mozillaVersion = m.Groups["mv"].Value;
				this.comments = m.Groups["comments"].Value;
				this.geckoRelease = Convert.ToInt64(m.Groups["gr"].Value);
				this.vendor = m.Groups["v"].Value;
				this.vendorVersion = m.Groups["vv"].Value;
			}
			catch
			{
				this.valid = false;
			}
		}

		public bool Valid
		{
			get { return valid; }
		}

		public string MozillaVersion
		{
			get { return mozillaVersion; }
		}

		public string Comments
		{
			get { return comments; }
		}

		public long GeckoRelease
		{
			get { return geckoRelease; }
		}

		public string Vendor
		{
			get { return vendor; }
		}

		public string VendorVersion
		{
			get { return vendorVersion; }
		}
	}
}
