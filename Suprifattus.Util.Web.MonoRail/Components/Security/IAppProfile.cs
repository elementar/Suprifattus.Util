using System;

namespace Suprifattus.Util.Web.MonoRail.Components.Security
{
	/// <summary>
	/// Representa um perfil de usu�rio.
	/// </summary>
	public interface IAppProfile
	{
		string Skin { get; }
		string Home { get; }
	}
}