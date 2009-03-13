using System;
using System.Web;

namespace Suprifattus.Util.Web.Templates.Config
{
	public interface ITemplateMappingCondition
	{
		bool Satisfied(HttpContext ctx);
	}
}