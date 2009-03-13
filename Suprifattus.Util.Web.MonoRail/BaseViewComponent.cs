using System;

using Castle.Core.Logging;
using Castle.MonoRail.Framework;

namespace Suprifattus.Util.Web.MonoRail
{
	public abstract class BaseViewComponent : ViewComponent
	{
		private ILogger log;

		public ILogger Log
		{
			get { return (log ?? (log = LogUtil.GetLogger(GetType()))); }
			set { log = value; }
		}
	}
}