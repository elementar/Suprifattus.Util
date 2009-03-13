using System;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Xml;

namespace Suprifattus.Util.Web.Templates.Config
{
	public class HeaderAndFooterTemplateDeclaration : TemplateDeclaration
	{
		string header;
		string footer;

		public HeaderAndFooterTemplateDeclaration(TemplateDeclarationConfig cfg, XmlElement el)
			: base(el)
		{
			this.header = el.GetAttribute("header");
			this.footer = el.GetAttribute("footer");

			Regex rxIsRoot = new Regex("^[/~]");
			if (!rxIsRoot.IsMatch(this.header))
				this.header = cfg.BaseDirectory + "/" + this.header;
			if (!rxIsRoot.IsMatch(this.footer))
				this.footer = cfg.BaseDirectory + "/" + this.footer;
		}

		public string Header
		{
			get { return header; }
		}

		public string Footer
		{
			get { return footer; }
		}

		public override void ApplyTo(TemplatedPage page)
		{
			Control ctlHeader = page.LoadControl(header);
			Control ctlFooter = page.LoadControl(footer);

			ctlHeader.ID = "__ctlTemplateHeader";
			ctlFooter.ID = "__ctlTemplateFooter";

			if (ctlHeader is ISkinnable) ((ISkinnable) ctlHeader).SkinID = page.SkinID;
			if (ctlFooter is ISkinnable) ((ISkinnable) ctlFooter).SkinID = page.SkinID;

			if (page.Controls.Count >= 3) 
			{
				page.Controls.RemoveAt(0);
				page.Controls.RemoveAt(page.Controls.Count - 1);
			}
			
			page.Controls.AddAt(0, ctlHeader);
			page.Controls.Add(ctlFooter);
		}
	}
}