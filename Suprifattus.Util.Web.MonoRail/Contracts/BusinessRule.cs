using System;
using System.Web;

using Castle.Core;
using Castle.MonoRail.Framework;

namespace Suprifattus.Util.Web.MonoRail.Contracts
{
	/// <summary>
	/// Representa uma regra de neg�cio.
	/// </summary>
	[Transient]
	public abstract class BusinessRule : IDisposable
	{
		~BusinessRule()
		{
			Dispose(false);
		}
		
		[Obsolete("Use RailsContext")]
		public static IRailsEngineContext MonoRailContext
		{
			get { return MonoRailHttpHandler.CurrentContext; }
		}
		
		public static IRailsEngineContext RailsContext
		{
			get { return MonoRailHttpHandler.CurrentContext; }
		}
		
		public static HttpContext AspNetContext
		{
			get { return RailsContext == null ? HttpContext.Current : RailsContext.UnderlyingContext; }
		}

		public string UserIP
		{
			get
			{
				if (AspNetContext != null)
					return AspNetContext.Request.UserHostAddress;
				else
					return "0.0.0.0";
			}
		}

		public string UserHost
		{
			get
			{
				if (AspNetContext != null)
					return AspNetContext.Request.UserHostName;
				else
					return "0.0.0.0";
			}
		}
		
		[Obsolete("Use a propriedade RailsContext diretamente")]
		protected static IRailsEngineContext GetMonoRailContext()
		{
			return RailsContext;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				GC.SuppressFinalize(this);
		}
		
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
