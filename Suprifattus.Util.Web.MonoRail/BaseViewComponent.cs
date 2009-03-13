using System;

using Castle.MonoRail.Framework;
using Castle.Core.Logging;

namespace Suprifattus.Util.Web.MonoRail
{
	public abstract class BaseViewComponent : ViewComponent
	{
		private ILogger log;

		public ILogger Log
		{
			get { return (log != null ? log : (log = LogUtil.GetLogger(GetType()))); }
			set { log = value; }
		}
	}
}