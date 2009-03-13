using System;

namespace Suprifattus.Util.Web.MonoRail.Components.Security
{
	public interface ISimpleAppUser
	{
		int Id { get; }
		string Name { get; }
		string Login { get; }
		string Password { get; set; }
		DateTime LastLogin { get; }
		IAppRole[] Roles { get; }
		bool IsActive { get; }
		bool IsNew { get; }
		void SetLastLogin(DateTime date);
		void Save();
	}
	
	/// <summary>
	/// Representa um usuário.
	/// </summary>
	public interface IAppUser : ISimpleAppUser
	{
		string AutoLoginHash { get; set; }
	}
}