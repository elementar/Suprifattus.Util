using System;

using Suprifattus.Util.Web.MonoRail.Components.Security;

namespace Suprifattus.Util.Web.MonoRail.Components.Security
{
	public interface ISecurityRule
	{
		bool IsAllowed(ISimpleAppUser user);
	}
}