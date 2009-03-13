using System;
using System.Security;
using System.Security.Permissions;

namespace Suprifattus.Util.AccessControl
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method)]
	public class ApplicationDefinedPermissionAttribute : CodeAccessSecurityAttribute
	{
		string[] neededPermissions;

		public ApplicationDefinedPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public ApplicationDefinedPermissionAttribute(SecurityAction action, params string[] neededPermissions)
			: base(action)
		{
			this.neededPermissions = neededPermissions;
		}

		public override System.Security.IPermission CreatePermission()
		{
			System.Security.IPermission result = null;
			foreach (string perm in neededPermissions) 
			{
				ApplicationDefinedPermission p = new ApplicationDefinedPermission(PermissionState.Unrestricted);
				p.Name = perm;
				result = (result == null ? p : result.Intersect(p));
			}
			return result;
		}
	}

	[Serializable]
	public class ApplicationDefinedPermission : CodeAccessPermission, System.Security.IPermission
	{
		PermissionState state;
		string name;

		public ApplicationDefinedPermission(PermissionState state)
		{
			this.state = state;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public override bool IsSubsetOf(System.Security.IPermission target)
		{
			ApplicationDefinedPermission p = target as ApplicationDefinedPermission;
			
			if (p == null || p.Name == null || Name == null)
				return false;

			return 
				this.Name.Length > p.Name.Length && 
				this.Name.Substring(0, p.Name.Length) == p.Name &&
				this.name[p.name.Length] == '.';
		}

		public override System.Security.IPermission Copy()
		{
			ApplicationDefinedPermission p = new ApplicationDefinedPermission(state);
			p.Name = this.Name;
			return p;
		}

		public override System.Security.IPermission Intersect(System.Security.IPermission target)
		{
			throw new NotImplementedException();
		}

		public override SecurityElement ToXml()
		{
			throw new NotImplementedException();
		}

		public override void FromXml(SecurityElement elem)
		{
			throw new NotImplementedException();
		}
	}

}
