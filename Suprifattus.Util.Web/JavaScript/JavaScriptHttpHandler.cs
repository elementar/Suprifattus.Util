using System;
using System.Web;
using System.Text;

namespace Suprifattus.Util.Web.JavaScript
{
	using Collections;

	/// <summary>
	/// Um <see cref="IHttpHandler"/> para incluir javascripts.
	/// </summary>
	public class JavaScriptHttpHandler : IHttpHandler
	{
		bool IHttpHandler.IsReusable { get { return true; } }

		/// <summary>
		/// Obt�m o objeto de depend�ncias do contexto desta requisi��o.
		/// </summary>
		private static object DependenciesObj
		{
			get { return HttpContext.Current.Items["jsdependencies"]; }
			set { HttpContext.Current.Items["jsdependencies"] = value; }
		}
		
		/// <summary>
		/// Lista de depend�ncias desta requisi��o.
		/// </summary>
		[CLSCompliant(false)]
		public static ISet Dependencies 
		{
			get { return (ISet) (DependenciesObj is ISet ? DependenciesObj : (DependenciesObj = CreateDependencies())); }
		}

		/// <summary>
		/// Cria lista de depend�ncias b�sicas.
		/// </summary>
		private static ISet CreateDependencies() 
		{
			return new ListSet("debug", "util", "compat", "innerxhtml", "css", "xml", "formsex");
		}
		
		/// <summary>
		/// Processa a requisi��o.
		/// </summary>
		public void ProcessRequest(HttpContext ctx)
		{
			ctx.Response.ContentType = "text/javascript";
			ctx.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");

			string scripts = ctx.Server.UrlDecode(ctx.Request.QueryString.ToString());
			ctx.Response.Write("/* requested scripts: " + scripts + " */\n\n");
			ctx.Response.Write( JavaScriptLoader.Load(false, scripts.Split(',')) );
		}
	}
}
