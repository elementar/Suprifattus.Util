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
		#region Depend�ncias
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
				throw new AppError("Erro ao obter usu�rio conectado", "Usu�rio conectado n�o dispon�vel.");

			int uid = principal.Identity.UserID;
			return ActiveRecordMediator<T>.FindByPrimaryKey(uid, false);
		}
	}
}