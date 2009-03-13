using System;
using System.Security.Principal;
using System.Text.RegularExpressions;

using Castle.Core.Logging;
using Castle.MonoRail.Framework;

using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail.Filters
{
	/// <summary>
	/// Filtro de Segurança.
	/// Responsável por realizar o auto-login e por redirecionar
	/// para a página de login, caso o usuário esteja desconectado.
	/// </summary>
	public class SecurityFilter : IFilter
	{
		private const RegexOptions ControllersToSkipRxOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
		
		private readonly ISecurityComponent securityComponent;
		private ILogger log = NullLogger.Instance;
		private Regex controllersToSkip = new Regex(@"home$", ControllersToSkipRxOptions);
		
		public SecurityFilter(ISecurityComponent securityComponent)
		{
			this.securityComponent = securityComponent;
		}

		public ILogger Log
		{
			get { return log; }
			set { log = value; }
		}

		public Regex ControllersToSkip
		{
			get { return controllersToSkip; }
			set { controllersToSkip = value; }
		}

		public string ControllersToSkipPattern
		{
			get { return controllersToSkip.ToString(); }
			set { controllersToSkip = new Regex(value, ControllersToSkipRxOptions); }
		}

		public bool Perform(ExecuteEnum exec, IRailsEngineContext ctx, Controller c)
		{
			if (!controllersToSkip.IsMatch(GetControllerName(c)))
				if (ctx.CurrentUser == null || !ctx.CurrentUser.Identity.IsAuthenticated)
					return TryAutoLogin();

			return true;
		}

		private bool TryAutoLogin()
		{
			log.Debug("User not authenticated, trying to perform auto-login");
			try
			{
				IPrincipal auto = securityComponent.LoginAutomatico();

				if (auto != null && auto.Identity.IsAuthenticated)
				{
					log.Info("AutoLogin performed for user: " + auto.Identity.Name);
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error trying to perform auto-login, will redirect to login page", ex);
			}

			securityComponent.RedirecionaParaLogin();
			return false;
		}

		private string GetControllerName(Controller c)
		{
			string controllerName = c.Name;
			if (!Logic.StringEmpty(c.AreaName))
				controllerName = c.AreaName + "/" + controllerName;
			return controllerName;
		}
	}
}
