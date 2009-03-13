using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Web.Handlers
{
	/// <summary>
	/// Source code stolen from:
	/// http://www.aspnetresources.com/articles/HttpFilters.aspx
	/// </summary>
	public class XHTMLModule : HttpContextBound, IHttpModule
	{
		private readonly HashSet<string> filtrable = new HashSet<string>();

		public void Init(HttpApplication app)
		{
			app.ReleaseRequestState += InstallResponseFilter;

			filtrable.AddRange("text/html", "application/xhtml+xml", "application/vnd.mozilla.xul+xml");
		}

		/// <summary>
		/// Use this event to wire a page filter.
		/// </summary>
		private void InstallResponseFilter(object sender, EventArgs e)
		{
			if (Response.ContentType == "text/html" && Context.Handler is Page)
				WebUtil.EnableXHtml();

#if !GENERICS
			if (filtrable.Contains(Response.ContentType))
				Response.Filter = new ContentFilter(Response.Filter);
#endif
		}

		public void Dispose()
		{
		}
	}
}