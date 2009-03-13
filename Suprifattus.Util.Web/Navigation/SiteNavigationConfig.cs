using System;
using System.Configuration;
using System.Xml;

namespace Suprifattus.Util.Web.Navigation
{
	[Serializable]
	public class SiteNavigationConfig : IConfigurationSectionHandler
	{
		string id;
		string defaultHome, login;

		public string ID
		{
			get { return id; }
		}

		public string DefaultHome
		{
			get { return defaultHome; }
		}

		public string Login
		{
			get { return login; }
		}

		public object Create(object parent, object context, XmlNode el)
		{
			this.id = (el as XmlElement).GetAttribute("id");
			this.login = el["login"].GetAttribute("url");
			this.defaultHome = el["defaultHome"].GetAttribute("url");
			
			return this;
		}
	}
}