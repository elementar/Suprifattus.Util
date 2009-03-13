using System;

using Suprifattus.Util.AccessControl.Impl;

namespace Suprifattus.Util.AccessControl
{
	/// <summary>
	/// Atributo utilizado para marcar um item que necessite de uma permiss�o espec�fica.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
	public sealed class DemandPermissionsAttribute : Attribute
	{
		private IPermission[] permissions;

		/// <summary>
		/// Cria uma nova demanda de permiss�o com a permiss�o especificada.
		/// </summary>
		/// <param name="permission">A permiss�o</param>
		public DemandPermissionsAttribute(string permission)
		{
			this.permissions = new IPermission[] { Permission.GetPermission(permission) };
		}
		
		/// <summary>
		/// Cria uma nova demanda de permiss�o com as permiss�es especificadas.
		/// </summary>
		/// <param name="permissions">As permiss�es</param>
		public DemandPermissionsAttribute(params string[] permissions)
		{
			this.permissions = new IPermission[permissions.Length];
			for (int i=0; i < this.permissions.Length; i++)
				this.permissions[i] = Permission.GetPermission(permissions[i]);
		}

		/// <summary>
		/// Retorna as permiss�es demandadas por este atributo.
		/// </summary>
		public IPermission[] Permissions 
		{
			get { return permissions; }
		}
	}
}
