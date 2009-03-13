using System;

namespace Suprifattus.Util.AccessControl
{
	public interface IPermission
	{
		void Check(IExtendedPrincipal user);
		bool Test(IExtendedPrincipal user);

		string ID { get; }
		string Name { get; }
	}
}