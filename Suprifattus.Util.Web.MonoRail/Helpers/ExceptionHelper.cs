using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

using Suprifattus.Util.AccessControl;
using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Xml;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	public class ExceptionHelper : AbstractHelper
	{
		private static readonly DictHelper dictHelper = new DictHelper();

		#region AddRelatedAction overrides
		public static IAction AddRelatedAction(string description, Controller c, string action)
		{
			return AddRelatedAction(description, c.AreaName, c.Name, action, (IDictionary) null);
		}

		public static IAction AddRelatedAction(string description, Controller c, string action, params string[] queryString)
		{
			IDictionary parameters = null;
			if (queryString != null && queryString.Length > 0)
				parameters = dictHelper.CreateDict(queryString);
			return AddRelatedAction(description, c.AreaName, c.Name, action, parameters);
		}

		public static IAction AddRelatedAction(string description, string area, string controller, string action, params string[] queryString)
		{
			IDictionary parameters = null;
			if (queryString != null && queryString.Length > 0)
				parameters = dictHelper.CreateDict(queryString);
			return AddRelatedAction(description, area, controller, action, parameters);
		}

		public static IAction AddRelatedAction(string description, string area, string controller, string action, IDictionary parameters)
		{
			const string RelatedActionsKey = "actions";

			IRailsEngineContext ctx = MonoRailHttpHandler.CurrentContext;

			IAction ac;
			var related = (IList) ctx.Flash[RelatedActionsKey];
			related = (related ?? new ArrayList());
			related.Add(ac = new MonoRailAction(description, area, controller, action, parameters));
			ctx.Flash[RelatedActionsKey] = related;

			return ac;
		}
		#endregion

		public bool IsApplicationDefinedException(Exception e)
		{
			return (e is BaseException);
		}

		/// <summary>
		/// Retorna o tipo de alerta para a exceção.
		/// </summary>
		/// <param name="e">A exceção</param>
		/// <returns>Um dos valores: 'alert', 'error' e 'success'</returns>
		public string GetAlertType(Exception e)
		{
			if (e is AppException)
				return "alert";
#pragma warning disable 618,612
			if (e is AccessDeniedException)
				return "alert";
#pragma warning restore 618,612

			return "error";
		}

		public string GetTitle(Exception e)
		{
			if (e is BaseException && !String.IsNullOrEmpty(((BaseException) e).Title))
				return ((BaseException) e).Title;

#pragma warning disable 618,612
			if (e is AccessDeniedException)
				return "Permissão Negada";
#pragma warning restore 618,612
			if (e is BusinessRuleViolationException)
				return "Violação de Regra de Negócio";
			if (e is AppException)
				return "Erro do Sistema";
			if (e is AppError)
				return "Erro Interno do Sistema";

			return "Erro Interno: " + e.GetType().Name;
		}

		public string Format(Exception e)
		{
			const string projDir = @".:\\(Documents and Settings|projects)\\\w+(\\My Documents\\Visual Studio Projects)?";
			string nl = Environment.NewLine;
			var rx = new Regex(" in " + projDir + "(.*$)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

			var s = new Stack();

			while (e != null)
			{
				s.Push(e);
				e = e.InnerException;
			}

			var sb = new StringBuilder();
			while (s.Count > 0 && (e = (Exception) s.Pop()) != null)
			{
				var be = e as BaseException;

				sb.AppendFormat("<strong class='exception-text'>{0}: ", XmlEncoder.Encode(e.GetType().FullName));
				if (be != null && !String.IsNullOrEmpty(be.Title))
					sb.AppendFormat("<span class='exception-title'>{0}:</span> ", be.Title);
				sb.AppendFormat("{0}</strong>", XmlEncoder.Encode(e.Message));
				if (be != null && !String.IsNullOrEmpty(be.AdditionalInfo))
					sb.AppendFormat(" <em class='exception-details'>({0})</em>", XmlEncoder.Encode(be.AdditionalInfo));
				sb.Append(nl);
				if (!String.IsNullOrEmpty(e.StackTrace))
				{
					string stackTrace = XmlEncoder.Encode(e.StackTrace);
					stackTrace = rx.Replace(stackTrace, nl + "\t<span class='exception-sourcefile'>in $3</span>");
					sb.Append(stackTrace).Append(nl);
				}
				sb.Append(nl);
			}

			return sb.ToString();
		}
	}

	public interface IAction
	{
		string Url { get; }
		string Description { get; }
	}

	public class MonoRailAction : IAction
	{
		private readonly string description;
		private readonly string area;
		private readonly string controller;
		private readonly string action;
		private readonly IDictionary parameters;

		public MonoRailAction(string description, string area, string controller, string action)
			: this(description, area, controller, action, null)
		{
		}

		public MonoRailAction(string description, string area, string controller, string action, IDictionary parameters)
		{
			this.description = description;
			this.area = area;
			this.controller = controller;
			this.action = action;
			this.parameters = parameters;
		}

		public MonoRailAction(string description, string controller, string action)
			: this(description, null, controller, action)
		{
		}

		public MonoRailAction(string description, string controller, string action, IDictionary parameters)
			: this(description, null, controller, action, parameters)
		{
		}

		public string Url
		{
			get
			{
				string url = new UrlInfo(area, controller, action).UrlRaw;
				if (parameters != null && parameters.Count > 0)
					url += "?" + BuildQueryString(parameters);
				return url;
			}
		}

		protected String BuildQueryString(IDictionary paramMap)
		{
			if (paramMap == null) return String.Empty;

			var sb = new StringBuilder();

			foreach (DictionaryEntry entry in paramMap)
			{
				if (entry.Value == null) continue;

				sb.AppendFormat("{0}={1}&amp;",
				                UrlEncode(entry.Key.ToString()), UrlEncode(entry.Value.ToString()));
			}

			return sb.ToString();
		}

		protected String UrlEncode(string s)
		{
			return MonoRailHttpHandler.CurrentContext.Server.UrlEncode(s);
		}

		public string Description
		{
			get { return description; }
		}
	}
}