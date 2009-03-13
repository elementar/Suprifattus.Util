using System;

namespace Suprifattus.Util.AccessControl.Impl
{
	/// <summary>
	/// Representa os dados de um usuário.
	/// </summary>
	[Serializable]
	public class IdentityBase : IExtendedIdentity
	{
		private readonly int userId;
		private readonly string userName;
		private readonly string login;
		private readonly bool isAuth;

		/// <summary>
		/// Cria um novo usuário.
		/// </summary>
		/// <param name="userId">O código do usuário.</param>
		/// <param name="isAuth">Verdadeiro se o usuário deverá ser marcado como autenticado, Falso caso contrário.</param>
		/// <param name="userName">O nome do usuário.</param>
		public IdentityBase(int userId, bool isAuth, string userName)
			: this(userId, isAuth, userName, null)
		{
		}

		/// <summary>
		/// Cria um novo usuário.
		/// </summary>
		/// <param name="userId">O código do usuário.</param>
		/// <param name="isAuth">Verdadeiro se o usuário deverá ser marcado como autenticado, Falso caso contrário.</param>
		/// <param name="userName">O nome do usuário.</param>
		/// <param name="login">O login do usuário</param>
		public IdentityBase(int userId, bool isAuth, string userName, string login)
		{
			this.userId = userId;
			this.isAuth = isAuth;
			this.userName = userName;
			this.login = login;
		}

		/// <summary>
		/// Cria um novo usuário.
		/// </summary>
		/// <param name="userId">O código do usuário.</param>
		/// <param name="isAuth">Verdadeiro se o usuário deverá ser marcado como autenticado, Falso caso contrário.</param>
		public IdentityBase(int userId, bool isAuth)
			: this(userId, isAuth, "Default User")
		{
		}

		/// <summary>
		/// Retorna o identificador único do usuário.
		/// </summary>
		public virtual int UserID
		{
			get { return userId; }
		}

		/// <summary>
		/// Retorna o identificador único do usuário, o mesmo que o <see cref="UserID"/>
		/// </summary>
		/// <returns>O identificador único do usuário.</returns>
		public override int GetHashCode()
		{
			return userId;
		}

		/// <summary>
		/// Retorna um valor booleano indicando se o usuário está autenticado ou não.
		/// </summary>
		public virtual bool IsAuthenticated
		{
			get { return isAuth; }
		}

		/// <summary>
		/// O nome do usuário.
		/// </summary>
		public virtual string Name
		{
			get { return userName; }
		}

		/// <summary>
		/// O login do usuário.
		/// </summary>
		public string Login
		{
			get { return login; }
		}

		/// <summary>
		/// O tipo de autenticação realizada.
		/// </summary>
		public virtual string AuthenticationType
		{
			get { return "Suprifattus Default"; }
		}
	}
}