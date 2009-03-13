using System;

namespace Suprifattus.Util.AccessControl
{
	public interface IRole
	{
		string RoleName { get; }

		bool HasPermission(IPermission permission);
		bool HasPermission(string permissionId);
	}
}