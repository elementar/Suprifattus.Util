using System;

using Castle.MicroKernel;

using Suprifattus.Util.Web.MonoRail.Components.Security;

namespace Suprifattus.Util.Web.MonoRail.Components.Security
{
	public class SecurityRulesProvider : ISecurityRule
	{
		IKernel kernel;
		ISecurityRule[] rules;

		public SecurityRulesProvider(IKernel kernel)
		{
			this.kernel = kernel;
			
			LoadRules();
		}

		private void LoadRules()
		{
			IHandler[] handlers = kernel.GetHandlers(typeof(ISecurityRule));
			rules = new ISecurityRule[handlers.Length];
			for (int i = 0; i < rules.Length; i++)
			{
				Type impl = handlers[i].ComponentModel.Implementation;
				if (typeof(SecurityRulesProvider).IsAssignableFrom(impl))
					continue;
				rules[i] = (ISecurityRule) handlers[i].Resolve(null);
			}
		}

		public bool IsAllowed(ISimpleAppUser user)
		{
			foreach (ISecurityRule rule in rules)
				if (rule != null && !rule.IsAllowed(user))
					return false;
			return true;
		}
	}
}