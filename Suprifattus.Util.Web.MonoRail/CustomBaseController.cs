using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.MonoRail.Framework;

using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Web.MonoRail.Helpers;

namespace Suprifattus.Util.Web.MonoRail
{
	[Helper(typeof(ListHelper))]
	[Helper(typeof(MathHelper))]
	[Helper(typeof(BasicFormatHelper))]
	[Helper(typeof(ExceptionHelper))]
	[Helper(typeof(RescueHelper), "rescue")]
	[Helper(typeof(RegexHelper))]
	[Helper(typeof(GroupHelper))]
	[Helper(typeof(ControllerUtils))]
	[Helper(typeof(UrlHelper), "u")]
	[Helper(typeof(RuntimeInfoHelper), "runtimeInfo")]
	public class CustomBaseController : SmartDispatcherController
	{
		#region Dependências
		private CustomBaseControllerConfig config = CustomBaseControllerConfig.Instance;

		public CustomBaseControllerConfig Config
		{
			get { return config; }
			set { config = value; }
		}
		#endregion

		public ControllerUtils Utils
		{
			get { return (ControllerUtils) Helpers["ControllerUtils"]; }
		}

		public IDictionary ControllerSession
		{
			get
			{
				string csKey = "ControllerSession:" + AreaName + "/" + Name;
				lock (Session.SyncRoot)
				{
					IDictionary cs = (IDictionary) Session[csKey];
					if (cs == null)
						Session[csKey] = cs = new HybridDictionary();
					return cs;
				}
			}
		}

		#region SwitchToAction
		/// <summary>
		/// Avança para a próxima ação.
		/// A ação pode estar definida no parâmetro <c>@next</c>, ou será
		/// utilizado o especificado em <paramref name="defaultAction"/>.
		/// </summary>
		/// <param name="defaultAction">A ação padrão, caso nenhuma seja especificada em <c>@next</c></param>
		public void SwitchToAction(string defaultAction)
		{
			SwitchToAction(defaultAction, null);
		}

		/// <summary>
		/// Avança para a próxima ação, com os parâmetros especificados.
		/// A ação pode estar definida no parâmetro <c>@next</c>, ou será
		/// utilizado o especificado em <paramref name="defaultAction"/>.
		/// </summary>
		/// <param name="defaultAction">A ação padrão, caso nenhuma seja especificada em <c>@next</c></param>
		/// <param name="parameters">Os parâmetros</param>
		public void SwitchToAction(string defaultAction, IDictionary parameters)
		{
			string action = Logic.Coalesce(Request.Params["@next"], defaultAction);

			RedirectToAction(action, parameters);
		}
		#endregion

		#region RedirectToRoot
		public void RedirectToRoot()
		{
			Redirect(Context.ApplicationPath + '/');
		}
		#endregion

		#region Verificação de Permissões
#if PERMISSOES_ANTIGAS
		private static IDictionary permCache = new Hashtable();

		private void CheckPermissions(MethodInfo method)
		{
			object[] perms = (object[]) permCache[method];
			if (perms == null)
				permCache.Add(method, perms = method.GetCustomAttributes(typeof(DemandPermissionsAttribute), true));

			if (perms.Length > 0)
			{
				if (Context.CurrentUser is IExtendedPrincipal)
					PermissionChecker.CheckPermissions((IExtendedPrincipal) Context.CurrentUser, perms, "O usuário '{0}' não possui a permissão necessária.");
				else
					throw new SecurityException("Usuário não conectado.");
			}
		}
#endif
		#endregion

		#region Extensões no comportamento padrão do MonoRail
		protected override MethodInfo SelectMethod(string action, IDictionary actions, IRequest request, IDictionary actionArgs)
		{
			string actionOverride = Request["@action"];
			if (actionOverride != null && actionOverride.Length > 0)
				action = actionOverride;

			return base.SelectMethod(action, actions, request, actionArgs);
		}

#if PERMISSOES_ANTIGAS
		protected override void InvokeMethod(MethodInfo method, IRequest request, IDictionary methodArgs)
		{
			// sobrescrevemos o InvokeMethod para verificar pelas permissões
			// antes de executar o método.
			CheckPermissions(method);

			base.InvokeMethod(method, request, methodArgs);
		}
#endif

		protected override object[] BuildMethodArguments(ParameterInfo[] parameters, IRequest request, IDictionary actionArgs)
		{
			// sobrescrevemos o BuildMethodArguments para capturar exceções
			// causadas por parâmetros com dados incorretos, como erros no
			// atributo [ARFetch] ou tipos inválidos de dados.
			try
			{
				return base.BuildMethodArguments(parameters, request, actionArgs);
			}
			catch (RailsException ex)
			{
				if (ex.InnerException is NotFoundException)
					throw new AppError("Registro Não Encontrado", "Não foi encontrado o registro solicitado.\nAlguns dados podem ter sido digitados incorretamente, ou pode haver um problema com o sistema.", ex);

				if (ex.InnerException is ActiveRecordException)
					throw new AppError("Problemas com os Dados", "Ocorreu um erro ao manipular o banco de dados.\nAlguns dados podem ter sido digitados incorretamente, ou pode haver um problema com o sistema.", ex);

				if (ex.GetBaseException() is FormatException || ex.Message.StartsWith("Error building method arguments."))
					throw new AppError("Dados Inválidos", "Ocorreu um erro ao realizar a ação.\nAlguns dados podem ter sido digitados incorretamente, ou pode haver um problema com o sistema.", ex);

				throw;
			}
		}

		public override void Redirect(string url)
		{
			if (Context.RequestType == "POST")
			{
				const string js = "<script type='text/javascript'>location.href='{0}';</script>";
				RenderText(String.Format(js, url.Replace("'", "\\'")));
			}
			else
				base.Redirect(url);
		}
		#endregion
	}
}