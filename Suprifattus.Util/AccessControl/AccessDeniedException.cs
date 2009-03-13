using System;

using Suprifattus.Util.AccessControl.Impl;

namespace Suprifattus.Util.AccessControl
{
	/// <summary>
	/// Exceção lançada quando o acesso a determinado recurso é negado.
	/// </summary>
	[Obsolete("Utilizada apenas no projeto CCS")]
	public class AccessDeniedException : ApplicationException
	{
		private const string DefaultMessageFormat = "O usuário '{0}' não tem a permissão necessária:\n- {1}";

		private readonly IExtendedPrincipal user;
		private readonly IPermission perm;

		/// <summary>
		/// Cria uma nova exceção de acesso negado.
		/// </summary>
		/// <param name="user">O usuário que tentou realizar o acesso.</param>
		/// <param name="perm">A permissão que foi negada ao usuário.</param>
		/// <param name="msgFormat">Formato da mensagem de erro. 
		/// Utilizar: {0} para o id do usuário e {1} para o nome da permissão.</param>
		public AccessDeniedException(IExtendedPrincipal user, IPermission perm, string msgFormat)
			: base(String.Format((msgFormat ?? DefaultMessageFormat), user, perm))
		{
			this.user = user;
			this.perm = perm;
		}

		/// <summary>
		/// Cria uma nova exceção de acesso negado.
		/// </summary>
		/// <param name="user">O usuário que tentou realizar o acesso.</param>
		/// <param name="perm">A permissão que foi negada ao usuário.</param>
		[Obsolete]
		public AccessDeniedException(IExtendedPrincipal user, string perm)
			: this(user, Permission.GetPermission(perm), DefaultMessageFormat)
		{
		}

		/// <summary>
		/// O usuário que tentou realizar o acesso.
		/// </summary>
		public IExtendedPrincipal User
		{
			get { return user; }
		}

		/// <summary>
		/// O nome da permissão que foi negada ao usuário.
		/// </summary>
		public IPermission DeniedPermission
		{
			get { return perm; }
		}
	}
}