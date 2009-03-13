using System;

using Castle.ActiveRecord;
using Castle.Core;

using Suprifattus.Util.AccessControl;
using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail.Components.Security
{
	[Transient]
	public class CurrentUser<T> : BusinessRuleWithLogging
		where T: class, ISimpleAppUser
	{
		#region Dependências
		private readonly ISecurityComponent securityComponent;

		public CurrentUser(ISecurityComponent securityComponent)
		{
			this.securityComponent = securityComponent;
		}
		#endregion

		public IExtendedPrincipal Principal
		{
			get { return securityComponent.Principal; }
		}

		[Obsolete("Utilizar propriedade Principal")]
		public virtual IExtendedPrincipal GetPrincipal()
		{
			return Principal;
		}

		public virtual T LoadUser()
		{
			IExtendedPrincipal principal = Principal;

			if (principal == null)
				throw new AppError("Erro ao obter usuário conectado", "Usuário conectado não disponível.");

			int uid = principal.Identity.UserID;
			return ActiveRecordMediator<T>.FindByPrimaryKey(uid, false);
		}
	}
}