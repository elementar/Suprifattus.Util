using System;
using System.Collections.Generic;

using Castle.MonoRail.Framework.Helpers;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	public class GroupHelper : AbstractHelper
	{
		private HashSet<object> s;

		public bool Once(object key)
		{
			if (s == null)
				s = new HashSet<object>();

			return s.Add(key);
		}

		public object PrintOnce(object obj)
		{
			return Once(obj) ? obj : String.Empty;
		}
	}
}