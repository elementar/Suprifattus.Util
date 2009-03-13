using System;
using System.Collections;

using Castle.MonoRail.Framework.Views.NVelocity;

using NVelocity;
using NVelocity.App;
using NVelocity.App.Events;
using NVelocity.Context;

using Suprifattus.Util.Xml;

namespace Suprifattus.Util.Web.MonoRail
{
	/// <summary>
	/// Classes marcadas com essa interface serão "escapadas"
	/// pelo <see cref="CustomNVelocityViewEngine"/>. Útil em <c>DTO</c>s
	/// e <c>ActiveRecord</c>s.
	/// </summary>
	public interface IEscapable
	{
	}

	public class CustomNVelocityViewEngine : NVelocityViewEngine
	{
		private EventCartridge ec;

		protected override void BeforeMerge(VelocityEngine velocity, Template template, IContext context)
		{
			((VelocityContext) context).AttachEventCartridge(CreateEventCartridge());

			base.BeforeMerge(velocity, template, context);
		}

		protected virtual EventCartridge CreateEventCartridge()
		{
			if (ec == null)
			{
				ec = new EventCartridge();
				ec.ReferenceInsertion += ec_ReferenceInsertion;
			}
			return ec;
		}

		private void ec_ReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
		{
			Stack s = e.GetCopyOfReferenceStack();
			while (s.Count > 0)
			{
				object current = s.Pop();
				if (current is IEscapable)
				{
					e.NewValue = XmlEncoder.Encode(Convert.ToString(e.OriginalValue));
					return;
				}
			}
		}
	}
}