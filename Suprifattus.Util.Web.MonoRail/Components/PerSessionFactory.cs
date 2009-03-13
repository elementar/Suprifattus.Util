using System;

using Castle.Core;
using Castle.Core.Logging;
using Castle.MonoRail.Framework;
using Castle.Windsor;

namespace Suprifattus.Util.Web.MonoRail.Components
{
	[Transient]
	public class PerSessionFactory<T>
	{
		private readonly IWindsorContainer windsor;
		private readonly string sessionKey;

		public PerSessionFactory(IWindsorContainer windsor)
		{
			this.Log = NullLogger.Instance;
			this.windsor = windsor;
			this.sessionKey = "SessionComponent_" + typeof(T).AssemblyQualifiedName;
		}

		public ILogger Log { get; set; }

		public T Get()
		{
			var session = MonoRailHttpHandler.CurrentContext.Session;

			lock (this)
				return (T) (session[this.sessionKey] ?? (session[this.sessionKey] = CreateNewInstance()));
		}

		private T CreateNewInstance()
		{
			Log.Debug("Criando nova instância");
			return windsor.Resolve<T>();
		}
	}
}