using System;

namespace Suprifattus.Util.Web.MonoRail.Components.Security
{
	/// <summary>
	/// Representa uma permiss�o.
	/// </summary>
	public interface IAppPermission
	{
		string Id { get; }
		string Name { get; }
	}
}