using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;
using System.Web;

using Castle.ActiveRecord;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

using Suprifattus.Util.AccessControl;
using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail
{
	/// <summary>
	/// Classe b�sica para projetos utilizando MonoRail.
	/// Suporta Windsor e ActiveRecord.
	/// </summary>
	public class BaseMonoRailApplication : HttpApplication, IContainerAccessor
	{
		private bool initializingWindsor;
		private const string WindsorContainerAppKey = "windsorContainer";

		/// <summary>
		/// Obt�m o <see cref="IWindsorContainer"/> atribu�do
		/// a aplica��o executando atualmente.
		/// </summary>
		public static IWindsorContainer CurrentWindsorContainer
		{
			get
			{
				if (HttpContext.Current == null)
					return null;

				var ca = (IContainerAccessor) HttpContext.Current.ApplicationInstance;
				return ca == null ? null : ca.Container;
			}
		}

		/// <summary>
		/// Construtor. Respons�vel por inicializar os eventos b�sicos
		/// e criar o Windsor Container, chamando <see cref="CreateWindsorContainer"/>.
		/// </summary>
		public BaseMonoRailApplication()
		{
			this.AuthenticateRequest += Application_AuthenticateRequest;
			this.BeginRequest += Application_BeginRequest;
			this.EndRequest += Application_EndRequest;
			this.Error += Application_Error;
		}

		public override void Init()
		{
			EnsureWindsorContainer();
			using (new WebApplicationLock(this))
				WindsorReferenceCount++;

			base.Init();
		}

		private void EnsureWindsorContainer()
		{
			if (StoredContainer != null || initializingWindsor)
				return;

			using (new WebApplicationLock(this))
			{
				if (StoredContainer != null || initializingWindsor)
					return;

				initializingWindsor = true;
				try
				{
					StoredContainer = CreateWindsorContainer();
				}
				finally
				{
					initializingWindsor = false;
				}
			}
		}

		/// <summary>
		/// Obt�m o <see cref="IWindsorContainer"/> atribu�do
		/// a esta aplica��o.
		/// </summary>
		public virtual IWindsorContainer Container
		{
			get
			{
				EnsureWindsorContainer();
				return StoredContainer;
			}
		}

		protected virtual IWindsorContainer StoredContainer
		{
			get { return (IWindsorContainer) Application[WindsorContainerAppKey]; }
			set { Application[WindsorContainerAppKey] = value; }
		}

		private int WindsorReferenceCount
		{
			get { return ((int?) Application["MR_windsorRefCount"]) ?? 0; }
			set { Application["MR_windsorRefCount"] = Math.Max(value, 0); }
		}

		#region Create & Dispose Windsor Container
		/// <summary>
		/// Cria um objeto implementando <see cref="IWindsorContainer"/>.
		/// Deve ser sobrescrito para retornar um <c>Windsor Container</c>
		/// especializado para a aplica��o. A implementa��o padr�o chama:
		/// <code>new WindsorContainer(new XmlInterpreter())</code>
		/// <seealso cref="IWindsorContainer"/>
		/// </summary>
		protected virtual IWindsorContainer CreateWindsorContainer()
		{
			return new WindsorContainer(new XmlInterpreter());
		}

		/// <summary>
		/// Libera o <c>Windsor Container</c>.
		/// A implementa��o padr�o apenas chama 
		/// <see cref="IWindsorContainer.Dispose"/>.
		/// <seealso cref="IWindsorContainer"/>
		/// </summary>
		protected virtual void DisposeWindsorContainer()
		{
			using (new WebApplicationLock(this))
				if (StoredContainer != null && (--WindsorReferenceCount == 0))
				{
					StoredContainer.Dispose();
					StoredContainer = null;
				}
		}
		#endregion

		/// <summary>
		/// Libera os recursos utilizados por esta aplica��o.
		/// Chama tamb�m o m�todo <see cref="DisposeWindsorContainer"/>.
		/// </summary>
		public override void Dispose()
		{
			DisposeWindsorContainer();
			base.Dispose();
		}

		#region Create & Dispose AR SessionScope
		/// <summary>
		/// Cria um <see cref="ISessionScope"/> do <c>ActiveRecord</c>.
		/// � chamado toda vez que uma requisi��o � iniciada, no evento
		/// <see cref="HttpApplication.BeginRequest"/>.
		/// </summary>
		protected virtual ISessionScope CreateActiveRecordSessionScope()
		{
			return new SessionScope();
		}

		/// <summary>
		/// Libera um <see cref="ISessionScope"/> do <c>ActiveRecord</c>.
		/// � chamado toda vez que uma requisi��o � finalizada, no evento
		/// <see cref="HttpApplication.EndRequest"/>.
		/// </summary>
		protected virtual void DisposeActiveRecordSessionScope(SessionScope scope)
		{
			scope.Dispose();
		}
		#endregion

		/// <summary>
		/// Carrega uma implementa��o de <see cref="IPrincipal"/>.
		/// � chamado antes de qualquer <c>Handler</c> ser executado,
		/// no evento <see cref="HttpApplication.PreRequestHandlerExecute"/>.
		/// </summary>
		/// <remarks>
		/// A implementa��o padr�o resolve pela propriedade <see cref="StoredContainer"/>
		/// um componente implementando o servi�o <see cref="ISecurityComponent"/>,
		/// e obt�m o <see cref="IPrincipal"/> chamando o m�todo
		/// <see cref="ISecurityComponent.PreparePrincipal"/>.
		/// </remarks>
		protected virtual IPrincipal LoadPrincipal()
		{
			EnsureWindsorContainer();

			// se n�o encontrar o componente, mant�m o mesmo usu�rio
			if (!Container.Kernel.HasComponent(typeof(ISecurityComponent)))
				return Context.User;

			var sec = Container.Resolve<ISecurityComponent>();

			var principal = sec.PreparePrincipal();

			this.SetUserNameOnLogs();

			return principal;
		}

		private void SetUserNameOnLogs()
		{
			string userName = null;

			var p = Context.User;
			if (p != null && p.Identity != null && p.Identity.IsAuthenticated)
			{
				var pex = p as IExtendedPrincipal;
				userName = pex != null ? pex.Identity.Login : p.Identity.Name;
			}

			LogUtil.SetLoggingProperty("id", userName);
			LogUtil.SetLoggingProperty("user", userName);
		}

		#region ASP.NET Event Handlers
		private void Application_BeginRequest(Object sender, EventArgs e)
		{
			lock (Context)
			{
				var s = Context.Items["nh.sessionscope"] as Stack;
				if (s == null)
					Context.Items["nh.sessionscope"] = s = new Stack();
				s.Push(CreateActiveRecordSessionScope());
			}
			Context.Trace.Write("Castle.ActiveRecord", "Created SessionScope");
		}

		private void Application_AuthenticateRequest(object sender, EventArgs e)
		{
			var newPrincipal = LoadPrincipal();
			if (newPrincipal != null)
				Thread.CurrentPrincipal = Context.User = newPrincipal;

			this.SetUserNameOnLogs();
		}

		private void Application_EndRequest(Object sender, EventArgs e)
		{
			lock (Context)
			{
				var s = (Stack) Context.Items["nh.sessionscope"];

				try
				{
					SessionScope scope;
					while ((scope = (SessionScope) s.Pop()) != null)
					{
						DisposeActiveRecordSessionScope(scope);
						Context.Trace.Write("Castle.ActiveRecord", "Disposed SessionScope");
					}
				}
				catch (ThreadAbortException)
				{
					throw;
				}
				catch (Exception ex)
				{
					Context.Trace.Warn("ActiveRecord", "Error while disposing Session Scope", ex);
				}
			}
		}

		protected void Application_Error(Object sender, EventArgs e)
		{
			const string loggedContextFlag = "suprifattus:apperror:logged";

			if (Context.Items.Contains(loggedContextFlag))
				return;

			var ex = Context.Server.GetLastError();
			var log = LogUtil.GetLogger();
			log.Error("Erro na requisi��o: " + Context.Request.Url.PathAndQuery, ex);

			// se o log de erro est� desabilitado e o Trace do ASP.NET est�, loga no Trace
			if (!log.IsErrorEnabled && Context.Trace.IsEnabled)
				Context.Trace.Write("Suprifattus.Framework", "Error: ", ex);
			Context.Items[loggedContextFlag] = true;
		}
		#endregion
	}
}