using System;
using System.Collections;
using System.Configuration;
using System.Xml;

namespace Suprifattus.Util.Web.Templates.Config
{
	public class TemplateMappingConfig
	{
		Hashtable maps = new Hashtable();

		public TemplateMappingConfig(XmlNode section)
		{
			foreach (XmlElement mapElement in section.SelectNodes("map"))
			{
				string templateName = mapElement.GetAttribute("templateName");
				ArrayList conds = new ArrayList(5);

				foreach (XmlElement mapConditions in mapElement.SelectNodes("*"))
					switch (mapConditions.Name)
					{
						case "regexMatch": 
							conds.Add(new RegexTemplateMappingCondition(mapConditions));
							break;
						default:
							throw new ConfigurationException("Map condition not recognized: " + mapConditions.Name, mapConditions);
					}

				conds.TrimToSize();
				maps.Add(templateName, conds);
			}
		}

		public IDictionary Mappings
		{
			get { return maps; }
		}
	}
}