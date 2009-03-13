using System;

namespace Suprifattus.Util.AccessControl.Impl
{
	public class SuprifattusIdentity : IdentityBase
	{
		public SuprifattusIdentity(int id, string login, string nomeCompleto)
			: base(id, true, nomeCompleto, login)
		{
		}
	}
}