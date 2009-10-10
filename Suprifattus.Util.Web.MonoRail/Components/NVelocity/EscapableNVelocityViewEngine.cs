using System;

using Castle.MonoRail.Framework.Views.NVelocity;

using NVelocity;
using NVelocity.App;
using NVelocity.Context;

namespace Suprifattus.Util.Web.MonoRail.Components.NVelocity
{
	public class EscapableNVelocityViewEngine : NVelocityViewEngine
	{
		protected override void BeforeMerge(VelocityEngine velocity, Template template, IContext context)
		{
			((VelocityContext) context).AttachEventCartridge(EscapeUtils.EscapableEventCartridge);

			base.BeforeMerge(velocity, template, context);
		}
	}
}