using System;
using System.Collections;
using System.Security.Principal;

namespace Suprifattus.Util.AccessControl
{
	public interface IExtendedPrincipal : IPrincipal
	{
		new IExtendedIdentity Identity { get; }
		IRole[] Roles { get; }
		IDictionary Properties { get; }
	}
}