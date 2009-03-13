using System;
using System.Security.Permissions;
using System.Security.Principal;

using Suprifattus.Util.Web.MonoRail.Components.Security.AspNetForms;

namespace Suprifattus.Util.Web.MonoRail.Components.Security
{
	/// <summary>
	/// Representa um papel.
	/// </summary>
	public interface IAppRole
	{
		/// <summary>
		/// O nome do papel. Não é a descrição do mesmo, mas sim um código que poderá ser
		/// utilizado por <see cref="IPrincipal.IsInRole"/> ou <see cref="PrincipalPermissionAttribute"/>.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// O perfil desse tipo de papel. Não é mais utilizado.
		/// </summary>
		[Obsolete("Foi utilizado apenas no Imuno")]
		IAppProfile Profile { get; }

		/// <summary>
		/// A lista de permissões. Não é mais utilizada em projetos que utilizam
		/// <see cref="AspNetFormsSecurityComponent{T}"/>, visto que a segurança
		/// passou a ser por papel.
		/// </summary>
		[Obsolete("A forma preferencial de lidar com segurança é através de papéis, utilizando [PrincipalPermission]")]
		IAppPermission[] Permissions { get; }
	}
}