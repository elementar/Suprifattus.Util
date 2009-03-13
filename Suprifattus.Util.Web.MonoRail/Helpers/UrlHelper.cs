using System;
using System.Collections;
using System.Text;

using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	public class UrlHelper : AbstractHelper
	{
		/// <summary>
		/// Monta uma URL para uma ação dentro do controller atual.
		/// A diferença desse método e do método <see cref="To(string)"/>
		/// é que esse método monta uma URL completa, enquanto o <see cref="To(string)"/>
		/// monta uma URL relativa ao documento atual, que pode falhar caso o
		/// <c>routing</c> do MonoRail esteja em uso.
		/// </summary>
		/// <param name="action">O nome da ação</param>
		/// <returns>A URL para a ação</returns>
		public string ToAction(string action)
		{
			Controller c = this.Controller;
			return To(c.AreaName, c.Name, action);
		}

		public string To(string action)
		{
			var sb = new StringBuilder();
			sb.Append(action).Append('.').Append(Ext);
			return sb.ToString();
		}

		public string To(string action, IDictionary parameters)
		{
			return To(action) + '?' + BuildQueryString(parameters);
		}

		public string To(string controller, string action)
		{
			return To(null, controller, action);
		}

		public string To(string controller, string action, IDictionary parameters)
		{
			return To(controller, action) + '?' + BuildQueryString(parameters);
		}

		public string To(string area, string controller, string action)
		{
			var sb = new StringBuilder();
			sb.Append(SiteRoot).Append('/');

			if (!String.IsNullOrEmpty(area))
				sb.Append(area).Append('/');
			if (!String.IsNullOrEmpty(controller))
				sb.Append(controller).Append('/');
			if (!String.IsNullOrEmpty(action))
				sb.Append(action).Append('.').Append(Ext);

			return sb.ToString();
		}

		public string To(string area, string controller, string action, IDictionary parameters)
		{
			return To(area, controller, action, parameters) + '?' + BuildQueryString(parameters);
		}

		private string SiteRoot
		{
			get { return Controller.Context.ApplicationPath; }
		}

		private string Ext
		{
			get { return Controller.Context.UrlInfo.Extension; }
		}
	}
}