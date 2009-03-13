using System;
using System.Collections;
using System.Security.Principal;

namespace Suprifattus.Util.AccessControl.Impl
{
	/// <summary>
	/// Usuário.
	/// </summary>
	[Serializable]
	public class UserBase : IExtendedPrincipal
	{
		private readonly IExtendedIdentity id;
		private readonly IRole[] userRoles;
		private readonly Hashtable customProps = new Hashtable();

		/// <summary>
		/// Cria um novo usuário.
		/// </summary>
		/// <param name="userId">O ID do usuário</param>
		/// <param name="isAuth">Se o usuário está autenticado ou não.</param>
		public UserBase(int userId, bool isAuth)
			: this(new IdentityBase(userId, isAuth), new IRole[0])
		{
		}

		/// <summary>
		/// Cria um novo usuário com as informações fornecidas.
		/// </summary>
		/// <param name="userId">O ID do usuário</param>
		/// <param name="isAuth">Se o usuário está autenticado ou não.</param>
		/// <param name="userName">O nome do usuário</param>
		/// <param name="userRoles">Os papéis do usuário</param>
		public UserBase(int userId, bool isAuth, string userName, IRole[] userRoles)
			: this(userId, isAuth, userName, null, userRoles)
		{
		}

		/// <summary>
		/// Cria um novo usuário com as informações fornecidas.
		/// </summary>
		/// <param name="userId">O ID do usuário</param>
		/// <param name="isAuth">Se o usuário está autenticado ou não.</param>
		/// <param name="userName">O nome do usuário</param>
		/// <param name="userLogin">O login do usuário</param>
		/// <param name="userRoles">Os papéis do usuário</param>
		public UserBase(int userId, bool isAuth, string userName, string userLogin, IRole[] userRoles)
			: this(new IdentityBase(userId, isAuth, userName, userLogin), userRoles)
		{
		}

		/// <summary>
		/// Cria um novo usuário com as informações fornecidas.
		/// </summary>
		/// <param name="id">A identidade do usuário</param>
		/// <param name="userRoles">Os papéis do usuário</param>
		public UserBase(IExtendedIdentity id, IRole[] userRoles)
		{
			this.id = id;
			this.userRoles = userRoles;
		}

		/// <summary>
		/// Retorna o objeto <see cref="IExtendedIdentity"/> que contém dados sobre
		/// a identidade do usuário - nome, ID, etc.
		/// </summary>
		public IExtendedIdentity Identity
		{
			get { return id; }
		}

		/// <summary>
		/// Papéis do usuário.
		/// </summary>
		public IRole[] Roles
		{
			get { return userRoles; }
		}

		/// <summary>
		/// Retorna o objeto <see cref="IIdentity"/> que contém dados sobre
		/// a identidade do usuário - nome, ID, etc.
		/// </summary>
		IIdentity IPrincipal.Identity
		{
			get { return id; }
		}

		/// <summary>
		/// Propriedades personalizadas do usuário.
		/// </summary>
		public IDictionary Properties
		{
			get { return customProps; }
		}

		/// <summary>
		/// Verifica se um usuário tem um papel específico.
		/// </summary>
		/// <param name="role">O nome do papel</param>
		/// <returns>Verdadeiro se o usuário tem o papel, falso caso contrário.</returns>
		public bool IsInRole(string role)
		{
			foreach (IRole r in userRoles)
				if (String.Compare(r.RoleName, role, true) == 0)
					return true;
			return false;
		}

		/// <summary>
		/// Retorna a representação string deste usuário - que é o próprio nome dele.
		/// </summary>
		/// <returns>A representação string deste usuário.</returns>
		public override string ToString()
		{
			return Identity.Name;
		}
	}
}