using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace Suprifattus.Util.Web.Navigation
{
	[Serializable]
	public class SiteNavigationController : HttpContextBound
	{
		private readonly Stack path = new Stack();
		private readonly SiteNavigationConfig cfg;
		private string home;
		private string nextPopFallback;

		#region Constructors
		public SiteNavigationController(string configuration)
		{
			var h = (SiteNavigationSectionHandler) ConfigurationManager.GetSection("suprifattus.sitenavigation");

			cfg = h.GetConfiguration(configuration);
			if (cfg == null)
			{
				throw new ConfigurationErrorsException("Navigation configuration not found: " + configuration);
			}
		}

		public SiteNavigationController()
			: this("default")
		{
		}
		#endregion

		public string Home
		{
			get { return home ?? cfg.DefaultHome; }
			set { home = value; }
		}

		#region GoHome overloads
		public void GoHome()
		{
			GoHome(NavigationMethod.RedirectJS);
		}

		public void GoHome(string message)
		{
			GoHome(NavigationMethod.RedirectJS, message);
		}

		public void GoHome(NavigationMethod method)
		{
			GoHome(method, null);
		}

		public void GoHome(NavigationMethod method, string message)
		{
			Navigate(method, home ?? cfg.DefaultHome, message);
		}
		#endregion

		#region GoLogin overloads
		public void GoLogin()
		{
			GoLogin(NavigationMethod.RedirectJS);
		}

		public void GoLogin(string message)
		{
			GoLogin(NavigationMethod.RedirectJS, message);
		}

		public void GoLogin(NavigationMethod method)
		{
			GoLogin(method, null);
		}

		public void GoLogin(NavigationMethod method, string message)
		{
			Navigate(method, cfg.Login, message);
		}
		#endregion

		#region PushAndGo overloads
		public void PushAndGo(string page)
		{
			PushAndGo(NavigationMethod.RedirectJS, page);
		}

		public void PushAndGo(NavigationMethod method, string page)
		{
			if (page == null)
				throw new ArgumentNullException("page");

			if (path.Count == 0 || !Request.Url.Equals(path.Peek()))
				path.Push(Request.Url);

			Navigate(method, page);
		}
		#endregion

		#region Pop overloads
		public void Pop()
		{
			Pop(NavigationMethod.RedirectJS);
		}

		public void Pop(NavigationMethod method)
		{
			Pop(method, 1);
		}

		public void Pop(NavigationMethod method, int count)
		{
			PerformPop(method, count, null);
		}
		#endregion

		public void PopWithMessage(string message, params object[] args)
		{
			PerformPop(NavigationMethod.RedirectJS, 1, String.Format(CultureInfo.CurrentCulture, message, args));
		}

		public void SetNextPopFallback(string nextPopFallback)
		{
			this.nextPopFallback = nextPopFallback;
		}

		private void PerformPop(NavigationMethod method, int count, string message)
		{
			try
			{
				if (count <= 0)
					throw new ArgumentException("Must be greater than zero", "count");

				string url = null;
				while (count-- >= 0 && path.Count > 0)
					url = Convert.ToString(path.Pop());

				if (Logic.StringEmpty(url))
				{
					if (!Logic.StringEmpty(nextPopFallback))
						Navigate(method, nextPopFallback, message);
					else
						GoHome(method, message);
				}
				else
					Navigate(method, url, message);
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException)
					throw;
				else
					throw new NavigationException();
			}
			finally
			{
				nextPopFallback = null;
			}
		}

		#region Navigate
		public void Navigate(NavigationMethod method, string page)
		{
			Navigate(method, page, null);
		}

		public void Navigate(NavigationMethod method, string page, string message)
		{
			switch (method)
			{
				case NavigationMethod.Redirect:
					Response.Redirect(page);
					break;
				case NavigationMethod.RedirectJS:
					WebUtil.JavascriptRedirect(page, message);
					break;
				case NavigationMethod.Transfer:
					Server.Transfer(page);
					break;
			}
		}
		#endregion
	}
}