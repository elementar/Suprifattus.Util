using System;
using System.Collections;

using Castle.MonoRail.Framework;

using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Web.MonoRail.Filters
{
	/// <summary>
	/// Filtro que adiciona ao <see cref="Controller.PropertyBag"/> um <see cref="IDictionary"/>
	/// contendo o conteúdo do <see cref="IRequest.QueryString"/>.
	/// </summary>
	[Obsolete("Utilizar PaginationHelper.CreatePageLinkWithCurrentQueryString()")]
	public class QueryStringFilter : IFilter
	{
		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			controller.PropertyBag["queryString"] = new NameValueCollectionDictionaryAdapter(context.Request.QueryString);
			return true;
		}
	}
}
