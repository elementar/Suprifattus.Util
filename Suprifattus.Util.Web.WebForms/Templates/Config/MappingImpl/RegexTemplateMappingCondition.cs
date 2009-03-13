using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Suprifattus.Util.Web.Templates.Config
{
	public class RegexTemplateMappingCondition : ITemplateMappingCondition
	{
		Regex urlRegex = null;

		public RegexTemplateMappingCondition(XmlElement el)
		{
			string urlMatch = el.GetAttribute("url");
			if (!Logic.StringEmpty(urlMatch))
				urlRegex = new Regex(urlMatch);
		}

		public bool Satisfied(HttpContext ctx)
		{
			return urlRegex != null && urlRegex.IsMatch(ctx.Request.ServerVariables["SCRIPT_NAME"]);
		}
	}
}