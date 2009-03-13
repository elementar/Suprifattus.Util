using System;
using System.Reflection;

namespace Suprifattus.Util.Web.Templates
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PageTemplateAttribute : Attribute
	{
		string templateName;
		string skinID;

		public PageTemplateAttribute()
			: this(null, null)
		{
		}
			
		public PageTemplateAttribute(string templateName)
			: this(templateName, null)
		{
		}

		public PageTemplateAttribute(string templateName, string skinID)
		{
			this.templateName = templateName;
			this.skinID = skinID;
		}

		public string TemplateName
		{
			get { return templateName; }
			set { templateName = value; }
		}

		public string SkinID
		{
			get { return skinID; }
			set { skinID = value; }
		}

		public static PageTemplateAttribute GetAttribute(MemberInfo mi)
		{
			return (PageTemplateAttribute) Attribute.GetCustomAttribute(mi, typeof(PageTemplateAttribute), true);
		}
	}
}
