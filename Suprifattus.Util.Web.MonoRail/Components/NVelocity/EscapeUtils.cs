using System;
using System.Collections;

using NVelocity.App.Events;

using Suprifattus.Util.Xml;

namespace Suprifattus.Util.Web.MonoRail.Components.NVelocity
{
	public static class EscapeUtils
	{
		private static readonly EventCartridge ec;

		static EscapeUtils()
		{
			ec = new EventCartridge();
			ec.ReferenceInsertion += ec_ReferenceInsertion;
		}

		public static EventCartridge EscapableEventCartridge
		{
			get { return ec; }
		}

		private static void ec_ReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
		{
			if (e.OriginalValue == null)
				return;

			var s = e.GetCopyOfReferenceStack();
			while (s.Count > 0)
			{
				var current = s.Pop();
				if (!(current is IEscapable))
					continue;

				e.NewValue = XmlEncoder.Encode(Convert.ToString(e.OriginalValue));
				return;
			}
		}

		public static void XmlEscapeStringValues(IDictionary dict)
		{
			foreach (var key in new ArrayList(dict.Keys))
			{
				var value = dict[key];
				if (value is string)
					dict[key] = XmlEncoder.Encode(value as string);
			}
		}
	}
}