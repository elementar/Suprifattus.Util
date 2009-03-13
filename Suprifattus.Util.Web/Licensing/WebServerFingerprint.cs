using System;
using System.Reflection;
using System.Web;

using Suprifattus.Util.Encryption;
using Suprifattus.Util.Licensing;

namespace Suprifattus.Util.Web.Licensing
{
	public abstract class WebServerFingerprint
	{
		private string fingerprint;

		public WebServerFingerprint()
		{
			string key = AppDomain.CurrentDomain.BaseDirectory.ToLower();

			fingerprint = HashUtil.GetMD5Hash(key);
		}

		public bool IsDevelopmentEnvironment()
		{
			string machine = HttpContext.Current.Server.MachineName.ToLower();
			return (machine.Equals("carneiro") || machine.Equals("tigre"));
		}
		
		public bool IsValid() 
		{
#if DEBUG
			if (IsDevelopmentEnvironment())
				return true;
#endif

			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) 
			{
				LicenseKeyAttribute licenseKey = 
					(LicenseKeyAttribute) Attribute.GetCustomAttribute(asm, typeof(LicenseKeyAttribute));

				if (licenseKey != null)
					if (fingerprint.Equals(licenseKey.Key) && ValidateAssembly(asm))
						return true;
			}

			return false;
		}

		protected virtual bool ValidateAssembly(Assembly asm)
		{
			return true;
		}
	}
}
