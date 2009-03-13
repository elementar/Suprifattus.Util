using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;

using Castle.Core.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Adapters;
using Castle.Windsor;

using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Web.MonoRail
{
	public class PerSessionFactory<T>
	{
		private readonly IWindsorContainer windsor;
		private ILogger log = NullLogger.Instance;

		public PerSessionFactory(IWindsorContainer windsor)
		{
			this.windsor = windsor;
		}

		public ILogger Log
		{
			get { return log; }
			set { log = value; }
		}

		public T Get()
		{
			//string sessionId = MonoRailHttpHandler.CurrentContext.UnderlyingContext.Session.SessionID;
			string sessionKey = "SessionComponent_" + typeof(T).AssemblyQualifiedName;
			
			//Log.Debug("Sessão: {0}, obtendo componente: {1}", sessionId, sessionKey);

			IDictionary session = MonoRailHttpHandler.CurrentContext.Session;
			
			lock (this)
				return (T) (session[sessionKey] ?? (session[sessionKey] = CreateNewInstance()));
		}

		private T CreateNewInstance()
		{
			Log.Debug("Criando nova instância");
			return windsor.Resolve<T>();
		}
	}
	
	[Obsolete("Não está funcionando corretamente, utilizar PerSessionFactory por enquanto", true)]
	public class PerSessionLifestyleHandler : AbstractLifestyleManager
	{
		private string sessionKey;

		public override object Resolve(CreationContext context)
		{
			return (ManagedObject != null ? ManagedObject : (ManagedObject = base.Resolve(context)));
		}

		public override void Dispose()
		{
			if (ManagedObject is IDisposable)
				((IDisposable) ManagedObject).Dispose();
		}

		private void EnsureSessionKey()
		{
			if (sessionKey == null)
			{
				lock (this)
				{
					if (sessionKey != null)
						return;

					do
					{
						sessionKey = "SessionComponent_" + Guid.NewGuid().ToString("D");
					} while (Session.Contains(sessionKey));
				}
			}
		}

		protected object ManagedObject
		{
			get
			{
				EnsureSessionKey();
				return Session[sessionKey];
			}
			set
			{
				EnsureSessionKey();
				Session[sessionKey] = value;
			}
		}

		protected IDictionary Session
		{
			get
			{
				IDictionary session = MonoRailHttpHandler.CurrentContext.Session;

				if (session == null)
				{
					HttpSessionState httpSession = HttpContext.Current.Session;

					if (httpSession == null)
						throw new AppAssertionError("Sessão indisponível", "A sessão não está disponível no contexto: " + HttpContext.Current.Request.Url);

					session = new SessionAdapter(httpSession);
				}

				return session;
			}
		}
	}
}