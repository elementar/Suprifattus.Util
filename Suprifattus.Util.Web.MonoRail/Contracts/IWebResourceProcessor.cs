using System;
using System.Collections;

using Castle.MonoRail.Framework;

namespace Suprifattus.Util.Web.MonoRail.Contracts
{
	public interface IWebResourceProcessor
	{
		string ProcessCss(string css, IRailsEngineContext ctx, IDictionary parameters);
		string ProcessJs(string js, IRailsEngineContext ctx, IDictionary parameters);
	}
}