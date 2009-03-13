using System;
using System.Configuration;
using System.Xml;

namespace Suprifattus.Util.Web.Navigation
{
	[Serializable]
	public class SiteNavigationConfig : IConfigurationSectionHandler
	{
		public string ID { get; private set; }
		public string DefaultHome { get; private set; }
		public string Login { get; private set; }

		public object Create(object parent, object context, XmlNode el)
		{
			this.ID = ((XmlElement) el).GetAttribute("id");
			this.Login = el["login"].GetAttribute("url");
			this.DefaultHome = el["defaultHome"].GetAttribute("url");

			return this;
		}
	}
}