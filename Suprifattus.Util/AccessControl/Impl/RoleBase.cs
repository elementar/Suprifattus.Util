using System;
using System.Collections.Generic;

namespace Suprifattus.Util.AccessControl.Impl
{
	/// <summary>
	/// Papel
	/// </summary>
	[Serializable]
	public class RoleBase : IRole
	{
		private readonly string roleName;
		private readonly HashSet<IPermission> permissions;

		/// <summary>
		/// Cria um novo papel.
		/// </summary>
		/// <param name="roleName">O nome do papel</param>
		/// <param name="permissions">As permissões</param>
		public RoleBase(string roleName, params string[] permissions)
			: this(roleName, new HashSet<IPermission>(GetPermissions(permissions)))
		{
		}

		/// <summary>
		/// Cria um novo papel.
		/// </summary>
		/// <param name="roleName">O nome do papel</param>
		/// <param name="permissions">As permissões</param>
		public RoleBase(string roleName, HashSet<IPermission> permissions)
		{
			this.roleName = roleName;
			this.permissions = permissions;
		}

		private static IEnumerable<IPermission> GetPermissions(IEnumerable<string> permissions)
		{
			foreach (string perm in permissions)
				yield return Permission.GetPermission(perm);
		}

		/// <summary>
		/// Nome do Papel.
		/// </summary>
		public string RoleName
		{
			get { return roleName; }
		}

		/// <summary>
		/// Verifica se este papel tem uma permissão específica.
		/// </summary>
		/// <param name="permission">O nome da permissão</param>
		/// <returns>Verdadeiro se o papel tem a permissão, falso caso contrário.</returns>
		public bool HasPermission(IPermission permission)
		{
			return permissions.Contains(permission);
		}

		/// <summary>
		/// Verifica se este papel tem uma permissão específica.
		/// </summary>
		/// <param name="permission">O nome da permissão</param>
		/// <returns>Verdadeiro se o papel tem a permissão, falso caso contrário.</returns>
		public bool HasPermission(string permission)
		{
			return permissions.Contains(Permission.GetPermission(permission));
		}

		/// <summary>
		/// Retorna a representação string do papel.
		/// </summary>
		/// <returns>O nome do papel</returns>
		public override string ToString()
		{
			return RoleName;
		}
	}
}