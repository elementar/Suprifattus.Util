using System;
using System.Collections;
using System.Configuration;

using Commons.Web.UI;

using Suprifattus.Util.Web.Templates.Config;

namespace Suprifattus.Util.Web.Templates
{
	public class TemplatedPage : PluggablePage, ISkinnable
	{
		string skinID = "biblia";
		bool applied = false;

		#region Public Properties
		public string SkinID
		{
			get { return skinID; }
			set { skinID = value; }
		}
		#endregion

		#region Static Initialization
		static TemplateDeclarationConfig templates;
		static TemplateMappingConfig mappings;
		
		static TemplatedPage()
		{
			templates = (TemplateDeclarationConfig) ConfigurationSettings.GetConfig("suprifattus.templates/templates");
			mappings = (TemplateMappingConfig) ConfigurationSettings.GetConfig("suprifattus.templates/mappings");
		}
		#endregion
		
		protected override void OnInit(EventArgs e)
		{
			TrackViewState();
			base.OnInit(e);
			ApplyTemplate();
		}

		private TemplateDeclaration ApplyTemplate()
		{
			if (applied)
				return null;

			if (templates == null)
			{
				applied = true;
				return null;
			}

			PageTemplateAttribute attr = PageTemplateAttribute.GetAttribute(GetType());

			if (attr != null)
			{
				this.SkinID = attr.SkinID;

				if (Logic.StringEmpty(attr.TemplateName))
				{
					applied = true;
					return null;
				}

				TemplateDeclaration template = templates[attr.TemplateName];
				if (template != null) 
				{
					template.ApplyTo(this);
					applied = true;
				}
				return template;
			}

			foreach (TemplateDeclaration template in templates)
			{
				if (mappings != null) 
				{
					IList m = (IList) mappings.Mappings[template.Name];
					if (m != null) 
					{
						foreach (ITemplateMappingCondition cond in m)
							if (cond.Satisfied(Context)) 
							{
								template.ApplyTo(this);
								applied = true;
								return template;
							}
					}
				}
			}

			if (templates.Default != null) 
			{
				templates.Default.ApplyTo(this);
				applied = true;
			}
			return templates.Default;
		}
	}
}
