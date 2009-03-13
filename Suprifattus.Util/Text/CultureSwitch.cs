using System;
using System.Globalization;
using System.Threading;

namespace Suprifattus.Util.Text
{
	public class CultureSwitch : IDisposable
	{
		private CultureInfo oldCulture;

		public CultureSwitch(CultureInfo ci)
		{
			this.oldCulture = GetCulture();
			SetCulture(ci);
		}

		protected CultureInfo GetCulture()
		{
			return Thread.CurrentThread.CurrentCulture;
		}

		protected void SetCulture(CultureInfo ci)
		{
			Thread.CurrentThread.CurrentCulture = ci;
		}

		public void Dispose()
		{
			if (this.oldCulture == null)
				return;

			SetCulture(this.oldCulture);
			this.oldCulture = null;
		}
	}
}