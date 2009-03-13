using System;
using System.Globalization;
using System.Threading;

namespace Suprifattus.Util.Text
{
	public class CultureSwitch : IDisposable
	{
		CultureInfo oldCulture;
		
		public CultureSwitch(CultureInfo ci)
		{
			this.oldCulture = GetCulture();
			SetCulture(ci);
		}

		protected virtual CultureInfo GetCulture()
		{
			return Thread.CurrentThread.CurrentCulture;
		}

		protected virtual void SetCulture(CultureInfo ci)
		{
			Thread.CurrentThread.CurrentCulture = ci;
		}

		public void Dispose()
		{
			if (this.oldCulture != null)
			{
				SetCulture(this.oldCulture);
				this.oldCulture = null;
			}
		}
	}
}
