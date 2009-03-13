using System;

using Castle.Core;
using Castle.Core.Logging;

namespace Suprifattus.Util.Web.MonoRail.Contracts
{
	/// <summary>
	/// Representa uma regra de negócio.
	/// </summary>
	[Transient]
	public abstract class BusinessRuleWithLogging : BusinessRule
	{
		private ILogger log = NullLogger.Instance;

		public ILogger Log
		{
			get { return log; }
			set { log = value; }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (log is IDisposable)
					((IDisposable) log).Dispose();
			}

			base.Dispose(disposing);
		}
	}
}