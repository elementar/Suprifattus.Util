using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace Suprifattus.Util.Web.Templates.Config
{
	public class TemplateDeclarationConfig : IEnumerable
	{
		string baseDir;
		TemplateDeclaration defaultTemplate;
		Hashtable templates = CollectionsUtil.CreateCaseInsensitiveHashtable();

		public TemplateDeclarationConfig(XmlNode section)
		{
			XmlElement root = (XmlElement) section;
			baseDir = root.GetAttribute("baseDirectory");
			if (Logic.StringEmpty(baseDir))
				baseDir = "~";

			string defaultID = root.GetAttribute("default");

			foreach (XmlElement templateEl in root.SelectNodes("*"))
			{
				TemplateDeclaration t;
				switch (templateEl.Name)
				{
					case "headerAndFooterTemplate":
						t = new HeaderAndFooterTemplateDeclaration(this, templateEl);
						templates.Add(t.Name, t);
						break;
					default:
						throw new ConfigurationException("Template declaration not recognized: " + templateEl.Name, templateEl);
				}
				if (String.Compare(defaultID, t.Name, true) == 0)
					defaultTemplate = t;
			}
		}

		public string BaseDirectory
		{
			get { return baseDir; }
		}

		public TemplateDeclaration Default
		{
			get { return defaultTemplate; }
		}

		public TemplateDeclaration this[string templateName]
		{
			get { return (TemplateDeclaration) templates[templateName]; }
		}

		public IEnumerator GetEnumerator()
		{
			return templates.Values.GetEnumerator();
		}
	}
}