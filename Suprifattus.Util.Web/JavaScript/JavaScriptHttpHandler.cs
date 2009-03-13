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
		/// Obtém o objeto de dependências do contexto desta requisição.
		/// </summary>
		private static object DependenciesObj
		{
			get { return HttpContext.Current.Items["jsdependencies"]; }
			set { HttpContext.Current.Items["jsdependencies"] = value; }
		}
		
		/// <summary>
		/// Lista de dependências desta requisição.
		/// </summary>
		[CLSCompliant(false)]
		public static ISet Dependencies 
		{
			get { return (ISet) (DependenciesObj is ISet ? DependenciesObj : (DependenciesObj = CreateDependencies())); }
		}

		/// <summary>
		/// Cria lista de dependências básicas.
		/// </summary>
		private static ISet CreateDependencies() 
		{
			return new ListSet("debug", "util", "compat", "innerxhtml", "css", "xml", "formsex");
		}
		
		/// <summary>
		/// Processa a requisição.
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
