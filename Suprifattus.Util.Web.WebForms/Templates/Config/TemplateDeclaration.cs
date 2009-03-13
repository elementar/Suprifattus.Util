using System;
using System.Xml;

namespace Suprifattus.Util.Web.Templates.Config
{
	public abstract class TemplateDeclaration
	{
		string name;

		public TemplateDeclaration(XmlElement el)
		{
			this.name = el.GetAttribute("name");
		}

		public string Name
		{
			get { return name; }
		}

		public abstract void ApplyTo(TemplatedPage page);
	}
}