using System;
using System.Collections;

using Castle.Core;
using Castle.Core.Logging;
using Castle.MonoRail.Framework;

namespace Suprifattus.Util.Web.MonoRail.Filters
{
	/// <summary>
	/// Filtro que exibe uma tela informando o usu�rio
	/// que o sistema em quest�o apenas com a engine <c>Gecko</c>.
	/// </summary>
	[Singleton]
	public class MozillaOnlyFilter : IFilter
	{
		/// <summary>
		/// Equivalente ao Firefox 1.5.0.1
		/// </summary>
		private const long DefaultMinimumGeckoRelease = 20060111;

		private static readonly Hashtable cache = new Hashtable();
		private static bool logged;

		public MozillaOnlyFilter()
		{
			Log = new NullLogger();
			Enabled = true;
			Redirect = true;

			RedirectController = "home";
			RedirectAction = "mozilla";

			MinGeckoRelease = DefaultMinimumGeckoRelease;
		}

		/// <summary>
		/// O <em>release</em> m�nimo do Gecko para este <see cref="UserAgent"/>
		/// ser considerado v�lido.
		/// </summary>
		public long MinGeckoRelease { get; set; }

		public ILogger Log { get; set; }
		public bool Enabled { get; set; }

		public bool Redirect { get; set; }
		public string RedirectArea { get; set; }
		public string RedirectController { get; set; }
		public string RedirectAction { get; set; }

		/// <summary>
		/// Executa o filtro.
		/// </summary>
		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			if (!Enabled)
				return true;

			UserAgent ua = null;

			var uaString = context.UnderlyingContext.Request.UserAgent;
			if (uaString != null)
			{
				ua = (UserAgent) cache[uaString];
				if (ua == null)
					cache[uaString] = ua = new UserAgent(uaString);
			}

			if (IsValid(ua))
				return true;

			if (!logged)
			{
				Log.Error("Tentativa de acesso atrav�s de browser n�o suportado: {0}", uaString);
				logged = true;
			}

			controller.PropertyBag["invalidGecko"] = true;
			if (!Redirect)
				return true;

			throw new Exception("redir: " + Redirect);

			RedirectToNotice(controller);
			return false;
		}

		/// <summary>
		/// Verifica se o <see cref="UserAgent"/> � v�lido.
		/// </summary>
		/// <param name="ua">O <see cref="UserAgent"/>. Pode ser nulo e inv�lido.</param>
		/// <returns>
		/// Verdadeiro se o <see cref="UserAgent"/> deva ser
		/// considerado v�lido, falso caso contr�rio.
		/// </returns>
		protected virtual bool IsValid(UserAgent ua)
		{
			return ua != null && ua.Valid && ua.GeckoRelease >= MinGeckoRelease;
		}

		/// <summary>
		/// Redireciona para a p�gina que avisa sobre a restri��o do browser.
		/// </summary>
		/// <param name="controller">O <see cref="Controller"/> que n�o passou na valida��o</param>
		protected virtual void RedirectToNotice(Controller controller)
		{
			if (String.IsNullOrEmpty(RedirectArea))
				controller.Redirect(RedirectController, RedirectAction);
			else
				controller.Redirect(RedirectArea, RedirectController, RedirectAction);
		}
	}
}