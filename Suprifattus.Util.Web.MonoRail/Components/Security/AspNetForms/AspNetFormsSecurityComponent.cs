using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

using Suprifattus.Util.AccessControl;
using Suprifattus.Util.AccessControl.Impl;
using Suprifattus.Util.Collections;
using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Web.MonoRail.Components.Security.AspNetForms
{
	public class AspNetFormsSecurityComponent<T> : SecurityComponentBase<T>
		where T: class, ISimpleAppUser
	{
		public const int VersaoTicket = 2;

		public AspNetFormsSecurityComponent(HashAlgorithm hashAlg)
			: base(hashAlg)
		{
		}

		protected override IExtendedPrincipal DefineUsuarioConectado(ISimpleAppUser user, bool persist, bool fromAutoLogin)
		{
			IExtendedPrincipal principal = CriaIPrincipal(user, true);

			if (user == null)
			{
				// remove o usu�rio do log
				LogUtil.SetLoggingProperty("id", null);

				// realiza o logoff
				FormsAuthentication.SignOut();

				return principal;
			}

			// atribui o login do usu�rio ao log
			LogUtil.SetLoggingProperty("id", principal.Identity.Login);

			IDictionary userData = this.CriaDadosUsuario(user);

			// cria o ticket do FormsAuthentication
			FormsAuthenticationTicket ticket = ConstroiTicket(user, userData, persist);

			// encripta o ticket e adiciona a um cookie
			string encTicket = FormsAuthentication.Encrypt(ticket);
			HttpCookie cookie = this.CreateCookieForFormsAuth();
			cookie.Value = encTicket;
			AspNetContext.Response.Cookies.Add(cookie);

			// redireciona � p�gina correta
			string redirectUrl = FormsAuthentication.GetRedirectUrl(principal.Identity.Login, false);
			if (!String.IsNullOrEmpty(redirectUrl) && redirectUrl.StartsWith(RailsContext.ApplicationPath))
				RailsContext.Response.Redirect(redirectUrl);
			else
				RailsContext.Response.Redirect(RailsContext.ApplicationPath + '/');
			return principal;
		}

		/// <summary>
		/// Cria os dados b�sicos do usu�rio.
		/// </summary>
		protected virtual IDictionary CriaDadosUsuario(ISimpleAppUser user)
		{
			IDictionary userData = new Hashtable();

			string[] papeis = CollectionUtils.ToArray(user.Roles, r => r.Name);

			userData.Add("id", user.Id);
			userData.Add("papeis", papeis);
			userData.Add("nomeCompleto", user.Name);
			userData.Add("ip", AspNetContext.Request.UserHostAddress);

			if (user is IAppUser)
				userData.Add("hash", this.ObtemTokenParaLoginAutomatico((IAppUser) user, true, true));

			ComplementaDadosUsuario((T) user, userData);

			return userData;
		}

		protected virtual void ComplementaDadosUsuario(T usuario, IDictionary dados)
		{
		}

		public override IPrincipal PreparePrincipal()
		{
			if (!FormsAuthentication.CookiesSupported)
				throw new SecurityException("Autentica��o Forms do ASP.NET sem Cookies n�o � suportada.");

			HttpCookie formsCookie = AspNetContext.Request.Cookies[FormsAuthentication.FormsCookieName];

			if (formsCookie == null)
			{
				Log.DebugFormat("Cookie '{0}' n�o presente na requisi��o. Sess�o sem autentica��o. Url: {1}", FormsAuthentication.FormsCookieName, AspNetContext.Request.Url);
				return null;
			}

			if (String.IsNullOrEmpty(formsCookie.Value))
			{
				Log.DebugFormat("Cookie '{0}' vazio. Sess�o sem autentica��o. Url: {1}", FormsAuthentication.FormsCookieName, AspNetContext.Request.Url);
				return null;
			}

			try
			{
				FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(formsCookie.Value);

				return ExtraiPrincipalDeTicket(ticket);
			}
			catch (CryptographicException)
			{
				return null;
			}
		}

		private HttpCookie CreateCookieForFormsAuth()
		{
			var cookie =
				new HttpCookie(FormsAuthentication.FormsCookieName)
					{
						HttpOnly = true,
						Path = (RailsContext.ApplicationPath + "/")
					};
			return cookie;
		}

		public override void Logout()
		{
			IPrincipal principal = RailsContext.CurrentUser;
			if (principal != null)
				Log.WarnFormat("Usu�rio '{0}' desconectou.", principal.Identity.Name);
			else
				Log.WarnFormat("Usu�rio desconectou.");

			var usuario = this.LoadCurrentAppUser() as IAppUser;
			if (usuario != null)
			{
				usuario.AutoLoginHash = null;
				usuario.Save();
			}

			HttpCookie emptyCookie = CreateCookieForFormsAuth();
			emptyCookie.Value = "-";
			emptyCookie.Expires = DateTime.Now.AddYears(-2);

			AspNetContext.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
			AspNetContext.Response.Cookies.Add(emptyCookie);
		}

		/// <summary>
		/// Constr�i as informa��es de usu�rio que ser�o armazenadas no <see cref="FormsAuthenticationTicket"/>.
		/// </summary>
		private FormsAuthenticationTicket ConstroiTicket(ISimpleAppUser user, IDictionary userData, bool persist)
		{
			string serializedUserData;
			using (var ms = new MemoryStream())
			{
				using (var gzs = new GZipStream(ms, CompressionMode.Compress))
				{
					var fmt = new BinaryFormatter();
					fmt.Serialize(gzs, userData);
				}
				ms.Flush();
				serializedUserData = Convert.ToBase64String(ms.ToArray());
			}

			// vers�es de tickets:
			// 1: apenas papeis e nome completo
			// 2: IDictionary para armazenar dados vari�veis
			var ticket =
				new FormsAuthenticationTicket(
					VersaoTicket, // vers�o. aumentar a cada altera��o para evitar erros
					user.Login, // nome do usu�rio
					DateTime.Now, // data do login
					DateTime.Now.AddHours(6), // expira��o
					persist, // cookie persistente ou n�o
					serializedUserData, // informa��es do usu�rio
					FormsAuthentication.FormsCookiePath
					);

			return ticket;
		}

		private IPrincipal ExtraiPrincipalDeTicket(FormsAuthenticationTicket ticket)
		{
			if (ticket.Version != VersaoTicket)
			{
				Log.ErrorFormat("Ticket com vers�o diferente da atual. Vers�o atual = {0}, vers�o do ticket = {1}", VersaoTicket, ticket.Version);
				return null;
			}

			try
			{
				Log.Debug("Carregando informa��es do ticket");
				byte[] serializedUserData = Convert.FromBase64String(ticket.UserData);

				IDictionary userData;
				using (var ms = new MemoryStream(serializedUserData))
				using (var gzs = new GZipStream(ms, CompressionMode.Decompress))
				{
					var fmt = new BinaryFormatter();
					userData = (IDictionary) fmt.Deserialize(gzs);
				}

				if (Log.IsDebugEnabled)
				{
					Log.Debug("Dados do usu�rio:");
					foreach (DictionaryEntry de in userData)
						Log.DebugFormat("  {0} = {1}", de.Key, de.Value);
				}

				var id = (int) userData["id"];
				var nomeCompleto = (string) userData["nomeCompleto"];
				var papeis = (string[]) userData["papeis"];

				var principal = new SuprifattusPrincipal(id, ticket.Name, nomeCompleto, papeis, userData);

				this.ValidaPrincipalDeTicket(ticket, principal);

				return principal;
			}
			catch (SerializationException ex)
			{
				Log.Error("Erro ao deserializar dados do usu�rio conectado", ex);
				return null;
			}
			catch (Exception ex)
			{
				Log.Error("Erro ao recuperar informa��es do usu�rio conectado", ex);
				if (!(ex is AppError) && !(ex is SecurityException))
					throw;

				return null;
			}
		}

		/// <summary>
		/// Valida o <see cref="SuprifattusPrincipal"/> criado a partir do <paramref name="ticket"/>.
		/// A valida��o padr�o verifica se o IP de cria��o do ticket � o mesmo, e, caso
		/// <typeparamref name="T"/> suporte <see cref="IAppUser"/>, verifica tamb�m o hash.
		/// </summary>
		protected virtual void ValidaPrincipalDeTicket(FormsAuthenticationTicket ticket, SuprifattusPrincipal principal)
		{
			var ip = (string) principal.Properties["ip"];
			if (ip != AspNetContext.Request.UserHostAddress)
				throw new SecurityException("Ticket inv�lido");

			if (typeof(T).IsSubclassOf(typeof(IAppUser)))
			{
				var hash = (string) principal.Properties["hash"];
				if (String.IsNullOrEmpty(hash))
					throw new AppError("Hash em branco", "� obrigat�rio o uso de hash com IAppUser");

				var usuario = (IAppUser) this.LoadAppUser(principal.Identity.UserID);
				if (hash != this.ObtemTokenParaLoginAutomatico(usuario, false, true))
					throw new SecurityException("Hashs n�o conferem");
			}
		}
	}
}