using System;
using System.Web;

namespace Suprifattus.Util.Web.Handlers
{
	/// <summary>
	/// Source code stolen from:
	/// http://www.aspnetresources.com/articles/HttpFilters.aspx
	/// </summary>
	public class XHtmlCleanupModule : HttpContextBound, IHttpModule
	{
		public void Init(HttpApplication app)
		{
			app.ReleaseRequestState += new EventHandler(InstallResponseFilter);
		}

		/// <summary>
		/// Use this event to wire a page filter.
		/// </summary>
		private void InstallResponseFilter(object sender, EventArgs e)
		{
			if (Response.ContentType == "application/xhtml+xml")
				Response.Filter = new ContentFilter(Response.Filter, ContentFilter.RegexSet.Cleanup);
		}

		public void Dispose()
		{
		}
	}
}