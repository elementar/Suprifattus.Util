using System;
using System.Security.Cryptography;

using Castle.ActiveRecord;

using NHibernate.Expression;

using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Web.MonoRail.Components.Security
{
	public class SecurityComponentBase<T> : SecurityComponentBase
		where T: class, ISimpleAppUser
	{
		protected SecurityComponentBase(HashAlgorithm hashAlg)
			: base(hashAlg)
		{
		}

		public virtual T BuscaUsuarioConectado()
		{
			var u = Principal;
			if (u == null)
				throw new AppException("Usuário Desconectado", "O usuário não está conectado ao sistema.\nVerifique se a sessão expirou.");

			return (T) LoadAppUser(u.Identity.UserID);
		}

		protected override ISimpleAppUser LoadAppUser(int id)
		{
			return ActiveRecordMediator<T>.FindByPrimaryKey(id, false);
		}

		protected override ISimpleAppUser LoadAppUser(string username)
		{
			T[] them = ActiveRecordMediator<T>.FindAll(Expression.Eq("Login", username));
			if (them == null || them.Length < 1)
				return null;
			return them[0];
		}

		protected override ISimpleAppUser LoadCurrentAppUser()
		{
			if (Principal == null || Principal.Identity == null || !Principal.Identity.IsAuthenticated)
				return null;

			return LoadAppUser(Principal.Identity.UserID);
		}
	}
}