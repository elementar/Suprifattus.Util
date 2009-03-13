using System;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;

using Castle.MonoRail.Framework;

using Suprifattus.Util.AccessControl;
using Suprifattus.Util.AccessControl.Impl;
using Suprifattus.Util.Encryption;
using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail.Components.Security
{
	public abstract class SecurityComponentBase : BusinessRuleWithLogging, ISecurityComponent
	{
		public const string AutoLoginCookieKey = "login:auto";
		public const string CurrentUserSessionKey = "currentUser";

		private readonly HashAlgorithm hashAlg;
		private SecurityRulesProvider securityRulesProvider;
		private bool hashedOnClient;

		protected SecurityComponentBase(HashAlgorithm hashAlg)
		{
			this.hashAlg = hashAlg;
		}

		protected abstract ISimpleAppUser LoadAppUser(string username);
		protected abstract ISimpleAppUser LoadAppUser(int id);
		protected abstract ISimpleAppUser LoadCurrentAppUser();

		#region Propriedades p/ Configura��o
		public bool HashedOnClient
		{
			get { return hashedOnClient; }
			set { hashedOnClient = value; }
		}

		public string UrlLogin { get; set; }

		public string UrlSSL { get; set; }

		public string UrlNormal { get; set; }

		public SecurityRulesProvider SecurityRulesProvider
		{
			get { return securityRulesProvider; }
			set { securityRulesProvider = value; }
		}

		public string PermissionForLogin { get; set; }

		public string PermissionForChangePasswords { get; set; }
		#endregion

		#region Redireciona para Login
		public virtual void RedirecionaParaLogin()
		{
			if (Logic.StringEmpty(UrlLogin))
			{
				Log.Info("Redirecionando � p�gina de login padr�o: /home/login");
				RailsContext.Response.Redirect("home", "login");
			}
			else
			{
				Log.Info("Redirecionando � p�gina de login: " + UrlLogin);
				RailsContext.Response.Redirect(UrlLogin);
			}
		}
		#endregion

		#region Altera Senha
		public void AlteraSenha(ISimpleAppUser user, string senha, string senha2)
		{
			CheckPermission(Principal, PermissionForChangePasswords, "User does not have the required privileges to change other user's password");
			if (user.IsNew)
			{
				// se o usu�rio � novo, o preenchimento da senha � obrigat�rio
				if (Logic.StringEmpty(senha))
					throw new RequiredFieldNotFilledException("Senha");
			}
			else
			{
				// se o usu�rio j� existe, a senha n�o � obrigat�ria.
				// neste caso, mant�m a senha j� cadastrada.
				if (Logic.StringEmpty(senha) && Logic.StringEmpty(senha2))
					return;
			}

			if (Logic.StringEmpty(senha))
				throw new RequiredFieldNotFilledException("Senha");
			if (Logic.StringEmpty(senha2))
				throw new RequiredFieldNotFilledException("Confirma��o da Senha");
			if (senha != senha2)
				throw new BusinessRuleViolationException("Confirma��o de senha incorreta", "Favor confirmar corretamente o campo \"senha\".");

			user.Password = CriptografaSenha(senha);
			if (Log.IsWarnEnabled)
			{
				if (user.IsNew)
				{
					var roleNames = new StringBuilder(); // = user.Role != null ? "'" + user.Role.Name + "'" : "(nenhum)";
					foreach (IAppRole role in user.Roles)
						roleNames.Append(role.Name).Append(", ");
					if (roleNames.Length > 0)
						roleNames.Length -= 2;
					Log.WarnFormat("Criado novo usu�rio, papeis: {0}, login: '{1}', nome: '{2}'", roleNames, user.Login, user.Name);
				}
				else
					Log.WarnFormat("Alterada senha do usu�rio #{0}, login: '{1}', nome: '{2}'", user.Id, user.Login, user.Name);

				var request = RailsContext.UnderlyingContext.Request;
				Log.WarnFormat("Endere�o IP da solicita��o: {0} ({1})", request.UserHostAddress, request.UserHostName);
			}
		}
		#endregion

		#region Logout
		public virtual void Logout()
		{
			Log.Warn("Usu�rio desconectou.");
			DefineUsuarioConectado(null, false, false);
			RailsContext.UnderlyingContext.Session.Abandon();
		}
		#endregion

		#region Login
		public virtual IExtendedPrincipal Login(string username, string password, bool savePassword)
		{
			if (Log.IsWarnEnabled)
			{
				var request = RailsContext.UnderlyingContext.Request;
				Log.WarnFormat("Solicitado login {0} do usu�rio '{1}', atrav�s do IP: {2} ({3})", savePassword ? "persistente" : "n�o persistente", username, request.UserHostAddress, request.UserHostName);
			}

			DefineUsuarioConectado(null, false, false);

			var user = LoadAppUser(username);
			if (user == null)
			{
				Log.ErrorFormat("Usu�rio '{0}' inexistente ou inv�lido", username);
				throw new SecurityException("Usu�rio inexistente ou senha inv�lida");
			}

			if (password == null)
				password = "";

			if (!ComparaSenha(user, password))
			{
				Log.ErrorFormat("Tentativa de login com senha inv�lida para o usu�rio '{0}' (hashedOnClient = {1})", username, HashedOnClient);
				throw new SecurityException("Usu�rio inexistente ou senha inv�lida");
			}

			VerificaPermissaoLogin(user);

			user.SetLastLogin(DateTime.Now);
			user.Save();

			var p = DefineUsuarioConectado(user, savePassword, false);

			if (Log.IsWarnEnabled)
			{
				var request = RailsContext.UnderlyingContext.Request;
				Log.WarnFormat("Usu�rio #{0} ({1}) autenticado com sucesso, atrav�s do IP: {2} ({3}).", user.Id, user.Login, request.UserHostAddress, request.UserHostName);
			}

			return p;
		}

		protected virtual bool ComparaSenha(ISimpleAppUser user, string password)
		{
			return user.Password == (hashedOnClient ? password : CriptografaSenha(password));
		}
		#endregion

		#region VerificaPermissaoLogin
		private void VerificaPermissaoLogin(ISimpleAppUser user)
		{
			CheckPermission(user, PermissionForLogin, "Usu�rio '{0}' n�o tem permiss�o para efetuar login", user.Name);

			if (!user.IsActive)
			{
				Log.ErrorFormat("Tentativa de login de usu�rio INATIVO: '{0}'", user.Name);
				throw new SecurityException("Usu�rio inativo");
			}

			if (securityRulesProvider != null && !securityRulesProvider.IsAllowed(user))
			{
				Log.WarnFormat("Login do usu�rio '{0}' negado por regra espec�fica", user.Name);
				throw new SecurityException(String.Format("Login n�o permitido para o usu�rio '{0}'", user.Name));
			}
		}
		#endregion

		#region Login Autom�tico
		public IExtendedPrincipal LoginAutomatico()
		{
			var ctx = RailsContext;

			if (Log.IsWarnEnabled)
			{
				var request = ctx.UnderlyingContext.Request;
				Log.WarnFormat("Solicitado login autom�tico atrav�s do IP: {0} ({1})", request.UserHostAddress, request.UserHostName);
			}

			var autologin = ctx.Request.ReadCookie(AutoLoginCookieKey);

			if (autologin == null)
				return null;

			var parts = autologin.Split(':');
			if (parts.Length != 2)
				return null;
			if (!Logic.IsNumeric(parts[0]))
				return null;

			var user = LoadAppUser(Convert.ToInt32(parts[0]));
			if (user == null)
				return null;

			if (user is IAppUser)
			{
				// usu�rios com �ltimo login mais antigo que 1 dia tem o token invalidado
				if (user.LastLogin < DateTime.Now.AddDays(-1))
				{
					Log.Error("Token de login autom�tico expirado.");
					LimpaTokenAutoLogin(ctx, (IAppUser) user);
					return null;
				}

				if (!ValidaLoginAutomatico((IAppUser) user, autologin))
				{
					Log.Error("Token de login autom�tico inv�lido.");
					return null;
				}
			}

			VerificaPermissaoLogin(user);

			return DefineUsuarioConectado(user, false, true);
		}
		#endregion

		#region Obt�m Token para Login Autom�tico
		/// <summary>
		/// Gera um token (uma string) que pode ser usada para login autom�tico,
		/// e salva no registro do usu�rio especificado.
		/// </summary>
		/// <param name="u">O usu�rio para gerar ou obter o hash</param>
		/// <param name="novo">Se <c>true</c>, cria um novo hash, sen�o, retorna o hash j� salvo</param>
		/// <param name="apenasHash">Se <c>true</c>, retorna apenas o hash, sem criar o token</param>
		protected virtual string ObtemTokenParaLoginAutomatico(IAppUser u, bool novo, bool apenasHash)
		{
			if (novo)
			{
				Log.InfoFormat("Gerado novo token de login autom�tico para o usu�rio #{0} ({1})", u.Id, u.Login);
				u.AutoLoginHash = GeraHashParaLoginAutomatico();
				u.Save();
			}

			var hash = CriptografaSenha(u.AutoLoginHash + ":" + u.Login + ":" + u.Password);
			return (apenasHash ? (hash) : (u.Id + ":" + hash));
		}
		#endregion

		#region Valida Login Autom�tico
		protected virtual bool ValidaLoginAutomatico(IAppUser u, string token)
		{
			return ObtemTokenParaLoginAutomatico(u, false, false).Equals(token);
		}
		#endregion

		#region Gera Hash para Login Autom�tico
		protected virtual string GeraHashParaLoginAutomatico()
		{
			var rnd = new Random();
			var salt = new byte[64];
			rnd.NextBytes(salt);
			return HashUtil.HashToString(salt);
		}
		#endregion

		#region Limpa Token de Login Autom�tico
		protected virtual void LimpaTokenAutoLogin(IRailsEngineContext ctx, IAppUser u)
		{
			Log.Info("Limpando cookie e token de auto-login para o usu�rio");

			DefineCookieAutoLogin(null, DateTime.Today.AddDays(-10));
			if (u != null)
			{
				u.AutoLoginHash = null;
				u.Save();
			}
		}
		#endregion

		#region Cria IPrincipal
		/// <summary>
		/// Cria um objeto <see cref="IExtendedPrincipal"/>, com base no <see cref="ISimpleAppUser"/>
		/// passado. Dificilmente ser� necess�rio sobrecrever esse m�todo.
		/// </summary>
		protected virtual IExtendedPrincipal CriaIPrincipal(ISimpleAppUser user, bool autenticado)
		{
			if (user == null)
				return null;

			var bld = new UserBuilder(user.Id, autenticado, user.Name) { Login = user.Login };

			if (user.Roles != null)
			{
				foreach (IAppRole role in user.Roles)
#pragma warning disable 618,612
					foreach (IAppPermission p in role.Permissions)
						bld.AddPermission(role.Name, Permission.SetPermission(p.Id, p.Name));
#pragma warning restore 618,612
			}

			IExtendedPrincipal u = bld.Build();
			if (user.Roles != null)
			{
				foreach (IAppRole role in user.Roles)
				{
#pragma warning disable 618,612
					if (role != null && role.Profile != null)
					{
						u.Properties["skin"] = role.Profile.Skin;
						u.Properties["home"] = role.Profile.Home;
					}
#pragma warning restore 618,612
				}
			}
			return u;
		}
		#endregion

		#region Define Usu�rio Conectado
		protected virtual IExtendedPrincipal DefineUsuarioConectado(ISimpleAppUser user, bool persist, bool fromAutoLogin)
		{
			var principal = CriaIPrincipal(user, true);

			var ctx = RailsContext;

			ctx.Session[CurrentUserSessionKey] = principal;
			Thread.CurrentPrincipal = ctx.CurrentUser = principal;

			if (user == null)
			{
				// remove o cookie e o token do auto-login
				LimpaTokenAutoLogin(ctx, null);
				// remove o usu�rio do log
				LogUtil.SetLoggingProperty("id", null);
			}
			else
			{
				// atribui o login do usu�rio ao log
				LogUtil.SetLoggingProperty("id", principal.Identity.Login);

				if (user is IAppUser)
				{
					// se � auto-login, n�o limpa nem recria o token
					if (!fromAutoLogin)
					{
						if (!persist)
							LimpaTokenAutoLogin(ctx, (IAppUser) user);
						else
							DefineCookieAutoLogin(ObtemTokenParaLoginAutomatico((IAppUser) user, true, false), DateTime.Now.AddDays(1));
					}
				}
			}

			return principal;
		}
		#endregion

		#region Propriedade "Principal"
		[Obsolete("Substitu�da pela propriedade 'Principal', que n�o obt�m da sess�o")]
		public IExtendedPrincipal UsuarioConectado
		{
			get { return RailsContext.Session[CurrentUserSessionKey] as IExtendedPrincipal; }
		}

		public IExtendedPrincipal Principal
		{
			get { return RailsContext.CurrentUser as IExtendedPrincipal; }
		}
		#endregion

		#region Criptografa Senha
		/// <summary>
		/// Criptografa a senha, utilizando o algoritmo de criptografia
		/// selecionado na configura��o.
		/// </summary>
		/// <param name="senha">A senha a ser criptografada</param>
		/// <returns>A senha, criptografada com o algoritmo configurado</returns>
		public string CriptografaSenha(string senha)
		{
			lock (hashAlg)
				return HashUtil.GetHash(hashAlg, senha);
		}
		#endregion

		#region CheckPermission
		public void CheckPermission(IExtendedPrincipal principal, string permission, string errorMessage, params object[] errorMessageArguments)
		{
			if (Logic.StringEmpty(permission))
				return;

			if (principal == null)
				throw new ArgumentNullException("principal", "Par�metro 'principal' n�o pode ser nulo");

#pragma warning disable 618,612
			if (PermissionChecker.HasPermission(principal, permission))
				return;
#pragma warning restore 618,612

			throw new PermissionDeniedException(String.Format(errorMessage, errorMessageArguments));
		}

		public void CheckPermission(ISimpleAppUser user, string permission, string errorMessage, params object[] errorMessageArguments)
		{
			if (Logic.StringEmpty(permission))
				return;

			if (user == null)
				throw new ArgumentNullException("user", "Par�metro 'user' n�o pode ser nulo");

			if (user.Roles != null)
			{
				foreach (IAppRole role in user.Roles)
				{
#pragma warning disable 618,612
					if (role.Permissions != null)
					{
						foreach (IAppPermission perm in role.Permissions)
							if (perm.Id == permission)
								return;
					}
#pragma warning restore 618,612
				}
			}

			throw new PermissionDeniedException(String.Format(errorMessage, errorMessageArguments));
		}
		#endregion

		#region DefineCookieAutoLogin
		private void DefineCookieAutoLogin(string valor, DateTime expiracao)
		{
			var ctx = RailsContext;

			var cookie =
				new HttpCookie(AutoLoginCookieKey, valor)
					{
						HttpOnly = true,
						Expires = expiracao,
						Path = (ctx.ApplicationPath + "/")
					};

			ctx.Response.CreateCookie(cookie);
		}
		#endregion

		public virtual IPrincipal PreparePrincipal()
		{
			var context = HttpContext.Current;
			if (context.Handler is IRequiresSessionState)
				return (IPrincipal) context.Session[CurrentUserSessionKey];

			return null;
		}
	}
}