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
		#region Depend�ncias
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
		/// Avan�a para a pr�xima a��o.
		/// A a��o pode estar definida no par�metro <c>@next</c>, ou ser�
		/// utilizado o especificado em <paramref name="defaultAction"/>.
		/// </summary>
		/// <param name="defaultAction">A a��o padr�o, caso nenhuma seja especificada em <c>@next</c></param>
		public void SwitchToAction(string defaultAction)
		{
			SwitchToAction(defaultAction, null);
		}

		/// <summary>
		/// Avan�a para a pr�xima a��o, com os par�metros especificados.
		/// A a��o pode estar definida no par�metro <c>@next</c>, ou ser�
		/// utilizado o especificado em <paramref name="defaultAction"/>.
		/// </summary>
		/// <param name="defaultAction">A a��o padr�o, caso nenhuma seja especificada em <c>@next</c></param>
		/// <param name="parameters">Os par�metros</param>
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

		#region Verifica��o de Permiss�es
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
					PermissionChecker.CheckPermissions((IExtendedPrincipal) Context.CurrentUser, perms, "O usu�rio '{0}' n�o possui a permiss�o necess�ria.");
				else
					throw new SecurityException("Usu�rio n�o conectado.");
			}
		}
#endif
		#endregion

		#region Extens�es no comportamento padr�o do MonoRail
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
			// sobrescrevemos o InvokeMethod para verificar pelas permiss�es
			// antes de executar o m�todo.
			CheckPermissions(method);

			base.InvokeMethod(method, request, methodArgs);
		}
#endif

		protected override object[] BuildMethodArguments(ParameterInfo[] parameters, IRequest request, IDictionary actionArgs)
		{
			// sobrescrevemos o BuildMethodArguments para capturar exce��es
			// causadas por par�metros com dados incorretos, como erros no
			// atributo [ARFetch] ou tipos inv�lidos de dados.
			try
			{
				return base.BuildMethodArguments(parameters, request, actionArgs);
			}
			catch (RailsException ex)
			{
				if (ex.InnerException is NotFoundException)
					throw new AppError("Registro N�o Encontrado", "N�o foi encontrado o registro solicitado.\nAlguns dados podem ter sido digitados incorretamente, ou pode haver um problema com o sistema.", ex);

				if (ex.InnerException is ActiveRecordException)
					throw new AppError("Problemas com os Dados", "Ocorreu um erro ao manipular o banco de dados.\nAlguns dados podem ter sido digitados incorretamente, ou pode haver um problema com o sistema.", ex);

				if (ex.GetBaseException() is FormatException || ex.Message.StartsWith("Error building method arguments."))
					throw new AppError("Dados Inv�lidos", "Ocorreu um erro ao realizar a a��o.\nAlguns dados podem ter sido digitados incorretamente, ou pode haver um problema com o sistema.", ex);

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