using System;

using Suprifattus.Util.AccessControl.Impl;

namespace Suprifattus.Util.AccessControl
{
	/// <summary>
	/// Exce��o lan�ada quando o acesso a determinado recurso � negado.
	/// </summary>
	[Obsolete("Utilizada apenas no projeto CCS")]
	public class AccessDeniedException : ApplicationException
	{
		private const string DefaultMessageFormat = "O usu�rio '{0}' n�o tem a permiss�o necess�ria:\n- {1}";

		private readonly IExtendedPrincipal user;
		private readonly IPermission perm;

		/// <summary>
		/// Cria uma nova exce��o de acesso negado.
		/// </summary>
		/// <param name="user">O usu�rio que tentou realizar o acesso.</param>
		/// <param name="perm">A permiss�o que foi negada ao usu�rio.</param>
		/// <param name="msgFormat">Formato da mensagem de erro. 
		/// Utilizar: {0} para o id do usu�rio e {1} para o nome da permiss�o.</param>
		public AccessDeniedException(IExtendedPrincipal user, IPermission perm, string msgFormat)
			: base(String.Format((msgFormat ?? DefaultMessageFormat), user, perm))
		{
			this.user = user;
			this.perm = perm;
		}

		/// <summary>
		/// Cria uma nova exce��o de acesso negado.
		/// </summary>
		/// <param name="user">O usu�rio que tentou realizar o acesso.</param>
		/// <param name="perm">A permiss�o que foi negada ao usu�rio.</param>
		[Obsolete]
		public AccessDeniedException(IExtendedPrincipal user, string perm)
			: this(user, Permission.GetPermission(perm), DefaultMessageFormat)
		{
		}

		/// <summary>
		/// O usu�rio que tentou realizar o acesso.
		/// </summary>
		public IExtendedPrincipal User
		{
			get { return user; }
		}

		/// <summary>
		/// O nome da permiss�o que foi negada ao usu�rio.
		/// </summary>
		public IPermission DeniedPermission
		{
			get { return perm; }
		}
	}
}