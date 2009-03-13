using System;

namespace Suprifattus.Util.AccessControl.Impl
{
	/// <summary>
	/// Representa os dados de um usu�rio.
	/// </summary>
	[Serializable]
	public class IdentityBase : IExtendedIdentity
	{
		private readonly int userId;
		private readonly string userName;
		private readonly string login;
		private readonly bool isAuth;

		/// <summary>
		/// Cria um novo usu�rio.
		/// </summary>
		/// <param name="userId">O c�digo do usu�rio.</param>
		/// <param name="isAuth">Verdadeiro se o usu�rio dever� ser marcado como autenticado, Falso caso contr�rio.</param>
		/// <param name="userName">O nome do usu�rio.</param>
		public IdentityBase(int userId, bool isAuth, string userName)
			: this(userId, isAuth, userName, null)
		{
		}

		/// <summary>
		/// Cria um novo usu�rio.
		/// </summary>
		/// <param name="userId">O c�digo do usu�rio.</param>
		/// <param name="isAuth">Verdadeiro se o usu�rio dever� ser marcado como autenticado, Falso caso contr�rio.</param>
		/// <param name="userName">O nome do usu�rio.</param>
		/// <param name="login">O login do usu�rio</param>
		public IdentityBase(int userId, bool isAuth, string userName, string login)
		{
			this.userId = userId;
			this.isAuth = isAuth;
			this.userName = userName;
			this.login = login;
		}

		/// <summary>
		/// Cria um novo usu�rio.
		/// </summary>
		/// <param name="userId">O c�digo do usu�rio.</param>
		/// <param name="isAuth">Verdadeiro se o usu�rio dever� ser marcado como autenticado, Falso caso contr�rio.</param>
		public IdentityBase(int userId, bool isAuth)
			: this(userId, isAuth, "Default User")
		{
		}

		/// <summary>
		/// Retorna o identificador �nico do usu�rio.
		/// </summary>
		public virtual int UserID
		{
			get { return userId; }
		}

		/// <summary>
		/// Retorna o identificador �nico do usu�rio, o mesmo que o <see cref="UserID"/>
		/// </summary>
		/// <returns>O identificador �nico do usu�rio.</returns>
		public override int GetHashCode()
		{
			return userId;
		}

		/// <summary>
		/// Retorna um valor booleano indicando se o usu�rio est� autenticado ou n�o.
		/// </summary>
		public virtual bool IsAuthenticated
		{
			get { return isAuth; }
		}

		/// <summary>
		/// O nome do usu�rio.
		/// </summary>
		public virtual string Name
		{
			get { return userName; }
		}

		/// <summary>
		/// O login do usu�rio.
		/// </summary>
		public string Login
		{
			get { return login; }
		}

		/// <summary>
		/// O tipo de autentica��o realizada.
		/// </summary>
		public virtual string AuthenticationType
		{
			get { return "Suprifattus Default"; }
		}
	}
}