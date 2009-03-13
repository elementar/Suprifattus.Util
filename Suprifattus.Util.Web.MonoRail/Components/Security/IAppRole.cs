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
		/// O nome do papel. N�o � a descri��o do mesmo, mas sim um c�digo que poder� ser
		/// utilizado por <see cref="IPrincipal.IsInRole"/> ou <see cref="PrincipalPermissionAttribute"/>.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// O perfil desse tipo de papel. N�o � mais utilizado.
		/// </summary>
		[Obsolete("Foi utilizado apenas no Imuno")]
		IAppProfile Profile { get; }

		/// <summary>
		/// A lista de permiss�es. N�o � mais utilizada em projetos que utilizam
		/// <see cref="AspNetFormsSecurityComponent{T}"/>, visto que a seguran�a
		/// passou a ser por papel.
		/// </summary>
		[Obsolete("A forma preferencial de lidar com seguran�a � atrav�s de pap�is, utilizando [PrincipalPermission]")]
		IAppPermission[] Permissions { get; }
	}
}