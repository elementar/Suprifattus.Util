using System;
using System.Security.Principal;

namespace Suprifattus.Util.AccessControl
{
	public interface IExtendedIdentity : IIdentity
	{
		string Login { get; }
		int UserID { get; }
	}
}