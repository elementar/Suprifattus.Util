using System;
using System.Collections;
using System.Security.Principal;

namespace Suprifattus.Util.AccessControl.Impl
{
	/// <summary>
	/// Usu�rio.
	/// </summary>
	[Serializable]
	public class UserBase : IExtendedPrincipal
	{
		private readonly IExtendedIdentity id;
		private readonly IRole[] userRoles;
		private readonly Hashtable customProps = new Hashtable();

		/// <summary>
		/// Cria um novo usu�rio.
		/// </summary>
		/// <param name="userId">O ID do usu�rio</param>
		/// <param name="isAuth">Se o usu�rio est� autenticado ou n�o.</param>
		public UserBase(int userId, bool isAuth)
			: this(new IdentityBase(userId, isAuth), new IRole[0])
		{
		}

		/// <summary>
		/// Cria um novo usu�rio com as informa��es fornecidas.
		/// </summary>
		/// <param name="userId">O ID do usu�rio</param>
		/// <param name="isAuth">Se o usu�rio est� autenticado ou n�o.</param>
		/// <param name="userName">O nome do usu�rio</param>
		/// <param name="userRoles">Os pap�is do usu�rio</param>
		public UserBase(int userId, bool isAuth, string userName, IRole[] userRoles)
			: this(userId, isAuth, userName, null, userRoles)
		{
		}

		/// <summary>
		/// Cria um novo usu�rio com as informa��es fornecidas.
		/// </summary>
		/// <param name="userId">O ID do usu�rio</param>
		/// <param name="isAuth">Se o usu�rio est� autenticado ou n�o.</param>
		/// <param name="userName">O nome do usu�rio</param>
		/// <param name="userLogin">O login do usu�rio</param>
		/// <param name="userRoles">Os pap�is do usu�rio</param>
		public UserBase(int userId, bool isAuth, string userName, string userLogin, IRole[] userRoles)
			: this(new IdentityBase(userId, isAuth, userName, userLogin), userRoles)
		{
		}

		/// <summary>
		/// Cria um novo usu�rio com as informa��es fornecidas.
		/// </summary>
		/// <param name="id">A identidade do usu�rio</param>
		/// <param name="userRoles">Os pap�is do usu�rio</param>
		public UserBase(IExtendedIdentity id, IRole[] userRoles)
		{
			this.id = id;
			this.userRoles = userRoles;
		}

		/// <summary>
		/// Retorna o objeto <see cref="IExtendedIdentity"/> que cont�m dados sobre
		/// a identidade do usu�rio - nome, ID, etc.
		/// </summary>
		public IExtendedIdentity Identity
		{
			get { return id; }
		}

		/// <summary>
		/// Pap�is do usu�rio.
		/// </summary>
		public IRole[] Roles
		{
			get { return userRoles; }
		}

		/// <summary>
		/// Retorna o objeto <see cref="IIdentity"/> que cont�m dados sobre
		/// a identidade do usu�rio - nome, ID, etc.
		/// </summary>
		IIdentity IPrincipal.Identity
		{
			get { return id; }
		}

		/// <summary>
		/// Propriedades personalizadas do usu�rio.
		/// </summary>
		public IDictionary Properties
		{
			get { return customProps; }
		}

		/// <summary>
		/// Verifica se um usu�rio tem um papel espec�fico.
		/// </summary>
		/// <param name="role">O nome do papel</param>
		/// <returns>Verdadeiro se o usu�rio tem o papel, falso caso contr�rio.</returns>
		public bool IsInRole(string role)
		{
			foreach (IRole r in userRoles)
				if (String.Compare(r.RoleName, role, true) == 0)
					return true;
			return false;
		}

		/// <summary>
		/// Retorna a representa��o string deste usu�rio - que � o pr�prio nome dele.
		/// </summary>
		/// <returns>A representa��o string deste usu�rio.</returns>
		public override string ToString()
		{
			return Identity.Name;
		}
	}
}