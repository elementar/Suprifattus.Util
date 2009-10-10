using System;

using Suprifattus.Util.Web.MonoRail.Components.NVelocity;

namespace Suprifattus.Util.Web.MonoRail
{
	/// <summary>
	/// Classes marcadas com essa interface ser�o "escapadas"
	/// pelo <see cref="EscapableNVelocityViewEngine"/>. �til em <c>DTO</c>s
	/// e <c>ActiveRecord</c>s.
	/// </summary>
	public interface IEscapable
	{
	}

	[Obsolete("Utilizar EscapableNVelocityViewEngine")]
	public class CustomNVelocityViewEngine : EscapableNVelocityViewEngine
	{
	}
}