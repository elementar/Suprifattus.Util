using System;
using System.Threading;
using System.Web;

namespace Suprifattus.Util.Web
{
	/// <summary>
	/// Bloqueia uma aplicação web até ser chamado o método <see cref="Dispose"/>.
	/// </summary>
	public class WebApplicationLock : IDisposable
	{
		private static readonly ManualResetEvent hardLock = new ManualResetEvent(true);
		private readonly HttpApplication application;

		public WebApplicationLock()
			: this(HttpContext.Current.ApplicationInstance)
		{
		}

		public WebApplicationLock(HttpApplication application)
		{
			this.application = application;

			Lock();
		}

		public HttpApplication Application
		{
			get { return this.application; }
		}

		public void Dispose()
		{
			UnLock();
		}

		private void Lock()
		{
			hardLock.Reset();
			application.Application.Lock();
		}

		private void UnLock()
		{
			hardLock.Set();
			application.Application.UnLock();
		}
	}
}