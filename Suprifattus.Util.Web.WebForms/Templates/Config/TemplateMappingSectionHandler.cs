using System;
using System.Configuration;
using System.Xml;

namespace Suprifattus.Util.Web.Templates.Config
{
	public class TemplateMappingSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			return new TemplateMappingConfig(section);
		}
	}
}
