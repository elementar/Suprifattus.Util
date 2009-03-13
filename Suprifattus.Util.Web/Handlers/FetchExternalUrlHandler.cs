using System;
using System.IO;
using System.Net;
using System.Web;

namespace Suprifattus.Util.Web.Handlers
{
	public class FetchExternalUrlHandler : IHttpHandler
	{
		public bool IsReusable { get { return true; } }

		public void ProcessRequest(HttpContext context)
		{
			Uri uri = new Uri(context.Request.QueryString["url"]);
			try
			{
				WebRequest req = WebRequest.Create(uri);

				Stream sout = context.Response.OutputStream;
				using (WebResponse resp = req.GetResponse())
				{
					context.Response.ContentType = resp.ContentType;
					using (Stream sin = resp.GetResponseStream())
					{
						int b;
						while ((b = sin.ReadByte()) != -1)
							sout.WriteByte((byte) b);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error fetching URL: " + uri, ex);
			}
		}
	}
}
