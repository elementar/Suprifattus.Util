using System;
using System.Collections;
using System.Security.Principal;

namespace Suprifattus.Util.AccessControl.Impl
{
	public class SuprifattusPrincipal : GenericPrincipal, IExtendedPrincipal
	{
		private readonly IDictionary userData;
		private readonly string[] papeis;

		private IRole[] dummyRoles;

		public SuprifattusPrincipal(int id, string login, string nomeCompleto, string[] papeis, IDictionary userData)
			: base(new SuprifattusIdentity(id, login, nomeCompleto), papeis)
		{
			this.userData = userData;
			this.papeis = papeis;
		}

		public new IExtendedIdentity Identity
		{
			get { return (IExtendedIdentity) base.Identity; }
		}

		public IRole[] Roles
		{
			get { return dummyRoles ?? (dummyRoles = Array.ConvertAll<string, IRole>(papeis, delegate(string p) { return new DummyRole(p); })); }
		}

		public IDictionary Properties
		{
			get { return userData; }
		}

		#region DummyRole
		private class DummyRole : IRole
		{
			private readonly string roleName;

			public DummyRole(string roleName)
			{
				this.roleName = roleName;
			}

			public string RoleName
			{
				get { return roleName; }
			}

			public bool HasPermission(IPermission permission)
			{
				return false;
			}

			public bool HasPermission(string permissionId)
			{
				return false;
			}
		}
		#endregion
	}
}