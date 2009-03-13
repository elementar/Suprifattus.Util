using System;
using System.Collections;
using System.Configuration;
using System.Xml;

namespace Suprifattus.Util.Web.Navigation
{
	public class SiteNavigationSectionHandler : IConfigurationSectionHandler
	{
		Hashtable configurations = new Hashtable();

		public object Create(object parent, object configContext, XmlNode section)
		{
			foreach (XmlElement el in section.SelectNodes("configuration"))
			{
				SiteNavigationConfig cfg = (SiteNavigationConfig) new SiteNavigationConfig().Create(this, configContext, el);
				configurations.Add(cfg.ID, cfg);
			}
			
			return this;
		}

		public SiteNavigationConfig GetConfiguration(string configName)
		{
			return (SiteNavigationConfig) configurations[configName];
		}
	}
}