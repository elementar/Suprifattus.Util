using System;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using System.Security.Principal;

namespace Suprifattus.Util.Web
{
	/// <summary>
	/// Classes que extenderem desta classe terão acesso aos itens de contexto
	/// HTTP, como Request, Response, Server, etc.
	/// </summary>
	[Serializable]
	public abstract class HttpContextBound
	{
		protected static HttpContext          Context     { get { return HttpContext.Current; } }
		protected static HttpResponse         Response    { get { return Context.Response; } }
		protected static HttpRequest          Request     { get { return Context.Request; } }
		protected static HttpServerUtility    Server      { get { return Context.Server; } }
		protected static HttpSessionState     Session     { get { return Context.Session; } }
		protected static HttpApplicationState Application { get { return Context.Application; } }
		protected static IPrincipal           User        { get { return Context.User; } }
		protected static Page                 Page        { get { return (Page) Context.Handler; } }
	}
}
