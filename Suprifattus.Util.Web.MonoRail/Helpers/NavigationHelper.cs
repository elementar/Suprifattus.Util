using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Castle.MonoRail.Framework.Helpers;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	public class NavigationHelper : AbstractHelper
	{
		bool hide = false;
		string titulo;
		List<NavigationItem> nav = new List<NavigationItem>();

		public string Titulo
		{
			get { return titulo; }
			set { titulo = value; }
		}

		public void Hide()
		{
			this.hide = true;
		}

		public NavigationHelper Add(string area, string controller, string action, string title)
		{
			EnsureHome();
			nav.Add(new NavigationItem(String.Format("{0}/{1}/{2}/{3}.{4}", Controller.Context.ApplicationPath, area, controller, action, Controller.Context.UrlInfo.Extension), title));
			return this;
		}

		public NavigationHelper Add(string controller, string action, string title)
		{
			EnsureHome();
			nav.Add(new NavigationItem(String.Format("{0}/{1}/{2}.{3}", Controller.Context.ApplicationPath, controller, action, Controller.Context.UrlInfo.Extension), title));
			return this;
		}

		public NavigationHelper Add(string url, string title)
		{
			EnsureHome();
			nav.Add(new NavigationItem(url, title));
			return this;
		}

		public string Render()
		{
			if (hide)
				return String.Empty;

			EnsureHome();
			EnsureCurrent();

			StringBuilder sb = new StringBuilder();

			using (TextWriter w = new StringWriter(sb))
			{
				XmlWriter xw = new XmlTextWriter(w);

				xw.WriteStartElement("div");
				xw.WriteAttributeString("class", "history-nav");

				bool first = true;

				foreach (NavigationItem i in nav)
				{
					if (!first)
						xw.WriteString(" > ");
					else
						first = false;

					xw.WriteStartElement("a");

					if (i.Url != null)
						xw.WriteAttributeString("href", i.Url);
					else
						xw.WriteAttributeString("class", "no-link");

					xw.WriteAttributeString("title", i.Title);
					xw.WriteString(i.Title);
					xw.WriteEndElement();
				}

				xw.WriteEndElement();
			}

			return sb.ToString();
		}

		public override string ToString()
		{
			return String.Empty;
		}

		#region EnsureHome e EnsureCurrent
		private void EnsureHome()
		{
			if (nav.Count == 0)
			{
				nav.Add(null);
				//Add(Controller.Context.ApplicationPath + "/", "Início");
				Add("app", "inicial", "inicial", "Início");
				nav.RemoveAt(0);
			}
		}

		private void EnsureCurrent()
		{
			NavigationItem i = this.nav[nav.Count - 1];
			if (i.Title != Titulo)
				Add(null, Titulo);
		}
		#endregion

		#region class NavigationItem
		public class NavigationItem
		{
			private string url, title;

			public NavigationItem(string url, string title)
			{
				this.url = url;
				this.title = title;
			}

			public string Url
			{
				get { return url; }
				set { url = value; }
			}

			public string Title
			{
				get { return title; }
				set { title = value; }
			}
		}
		#endregion
	}

}
