using System;

using Suprifattus.Util.AccessControl.Impl;

namespace Suprifattus.Util.AccessControl
{
	/// <summary>
	/// Atributo utilizado para marcar um item que necessite de uma permissão específica.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
	public sealed class DemandPermissionsAttribute : Attribute
	{
		private IPermission[] permissions;

		/// <summary>
		/// Cria uma nova demanda de permissão com a permissão especificada.
		/// </summary>
		/// <param name="permission">A permissão</param>
		public DemandPermissionsAttribute(string permission)
		{
			this.permissions = new IPermission[] { Permission.GetPermission(permission) };
		}
		
		/// <summary>
		/// Cria uma nova demanda de permissão com as permissões especificadas.
		/// </summary>
		/// <param name="permissions">As permissões</param>
		public DemandPermissionsAttribute(params string[] permissions)
		{
			this.permissions = new IPermission[permissions.Length];
			for (int i=0; i < this.permissions.Length; i++)
				this.permissions[i] = Permission.GetPermission(permissions[i]);
		}

		/// <summary>
		/// Retorna as permissões demandadas por este atributo.
		/// </summary>
		public IPermission[] Permissions 
		{
			get { return permissions; }
		}
	}
}
