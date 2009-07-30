using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

using Suprifattus.Util.AccessControl;
using Suprifattus.Util.AccessControl.Impl;
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
			var principal = CriaIPrincipal(user, true);

			if (user == null)
			{
				// remove o usuário do log
				LogUtil.SetLoggingProperty("id", null);

				// realiza o logoff
				FormsAuthentication.SignOut();

				return principal;
			}

			// atribui o login do usuário ao log
			LogUtil.SetLoggingProperty("id", principal.Identity.Login);

			var userData = this.CriaDadosUsuario(user);

			// cria o ticket do FormsAuthentication
			var ticket = ConstroiTicket(user, userData, persist);

			// encripta o ticket e adiciona a um cookie
			var encTicket = FormsAuthentication.Encrypt(ticket);
			var cookie = this.CreateCookieForFormsAuth();
			cookie.Value = encTicket;
			AspNetContext.Response.Cookies.Add(cookie);

			// redireciona à página correta
			var redirectUrl = FormsAuthentication.GetRedirectUrl(principal.Identity.Login, false);
			if (!String.IsNullOrEmpty(redirectUrl) && redirectUrl.StartsWith(RailsContext.ApplicationPath))
				RailsContext.Response.Redirect(redirectUrl);
			else
				RailsContext.Response.Redirect(RailsContext.ApplicationPath + '/');

			return principal;
		}

		/// <summary>
		/// Cria os dados básicos do usuário.
		/// </summary>
		protected virtual IDictionary CriaDadosUsuario(ISimpleAppUser user)
		{
			IDictionary userData =
				new Hashtable
					{
						{ "id", user.Id },
						{ "papeis", user.Roles.Select(r => r.Name).ToArray() },
						{ "nomeCompleto", user.Name },
						{ "ip", AspNetContext.Request.UserHostAddress },
					};

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
				throw new SecurityException("Autenticação Forms do ASP.NET sem Cookies não é suportada.");

			var formsCookie = AspNetContext.Request.Cookies[FormsAuthentication.FormsCookieName];

			if (formsCookie == null)
			{
				Log.DebugFormat("Cookie '{0}' não presente na requisição. Sessão sem autenticação. Url: {1}", FormsAuthentication.FormsCookieName, AspNetContext.Request.Url);
				return null;
			}
			
			if (String.IsNullOrEmpty(formsCookie.Value))
			{
				Log.DebugFormat("Cookie '{0}' vazio. Sessão sem autenticação. Url: {1}", FormsAuthentication.FormsCookieName, AspNetContext.Request.Url);
				return null;
			}

			try
			{
				var ticket = FormsAuthentication.Decrypt(formsCookie.Value);

				return ExtraiPrincipalDeTicket(ticket);
			}
			catch (CryptographicException)
			{
				return null;
			}
		}

		private HttpCookie CreateCookieForFormsAuth()
		{
			return new HttpCookie(FormsAuthentication.FormsCookieName)
			       	{
			       		HttpOnly = true, 
								Path = RailsContext.ApplicationPath + "/",
			       	};
		}

		public override void Logout()
		{
			var principal = RailsContext.CurrentUser;
			if (principal != null)
				Log.WarnFormat("Usuário '{0}' desconectou.", principal.Identity.Name);
			else
				Log.WarnFormat("Usuário desconectou.");

			var usuario = this.LoadCurrentAppUser() as IAppUser;
			if (usuario != null)
			{
				usuario.AutoLoginHash = null;
				usuario.Save();
			}

			var emptyCookie = CreateCookieForFormsAuth();
			emptyCookie.Value = "-";
			emptyCookie.Expires = DateTime.Now.AddYears(-2);

			AspNetContext.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
			AspNetContext.Response.Cookies.Add(emptyCookie);
		}

		/// <summary>
		/// Constrói as informações de usuário que serão armazenadas no <see cref="FormsAuthenticationTicket"/>.
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

			// versões de tickets:
			// 1: apenas papeis e nome completo
			// 2: IDictionary para armazenar dados variáveis
			var ticket =
				new FormsAuthenticationTicket(
					VersaoTicket, // versão. aumentar a cada alteração para evitar erros
					user.Login, // nome do usuário
					DateTime.Now, // data do login
					DateTime.Now.AddHours(6), // expiração
					persist, // cookie persistente ou não
					serializedUserData, // informações do usuário
					FormsAuthentication.FormsCookiePath
					);

			return ticket;
		}

		private IPrincipal ExtraiPrincipalDeTicket(FormsAuthenticationTicket ticket)
		{
			if (ticket.Version != VersaoTicket)
			{
				Log.ErrorFormat("Ticket com versão diferente da atual. Versão atual = {0}, versão do ticket = {1}", VersaoTicket, ticket.Version);
				return null;
			}

			try
			{
				Log.Debug("Carregando informações do ticket");
				var serializedUserData = Convert.FromBase64String(ticket.UserData);

				IDictionary userData;
				using (var ms = new MemoryStream(serializedUserData))
				using (var gzs = new GZipStream(ms, CompressionMode.Decompress))
				{
					var fmt = new BinaryFormatter();
					userData = (IDictionary) fmt.Deserialize(gzs);
				}

				if (Log.IsDebugEnabled)
				{
					Log.Debug("Dados do usuário:");
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
				Log.Error("Erro ao deserializar dados do usuário conectado", ex);
				return null;
			}
			catch (Exception ex)
			{
				Log.Error("Erro ao recuperar informações do usuário conectado", ex);
				if (!(ex is AppError) && !(ex is SecurityException))
					throw;

				return null;
			}
		}

		/// <summary>
		/// Valida o <see cref="SuprifattusPrincipal"/> criado a partir do <paramref name="ticket"/>.
		/// A validação padrão verifica se o IP de criação do ticket é o mesmo, e, caso
		/// <typeparamref name="T"/> suporte <see cref="IAppUser"/>, verifica também o hash.
		/// </summary>
		protected virtual void ValidaPrincipalDeTicket(FormsAuthenticationTicket ticket, SuprifattusPrincipal principal)
		{
			var ip = (string) principal.Properties["ip"];
			if (ip != AspNetContext.Request.UserHostAddress)
				throw new SecurityException("Ticket inválido")
					.SetAdditionalInfo("ip original: {0}, ip atual: {1}", ip, AspNetContext.Request.UserHostAddress);

			if (!typeof(T).IsSubclassOf(typeof(IAppUser)))
				return;

			var hash = (string) principal.Properties["hash"];
			if (String.IsNullOrEmpty(hash))
				throw new AppError("Hash em branco", "É obrigatório o uso de hash com IAppUser");

			var usuario = (IAppUser) this.LoadAppUser(principal.Identity.UserID);
			if (hash != this.ObtemTokenParaLoginAutomatico(usuario, false, true))
				throw new SecurityException("Hashs não conferem");
		}
	}
}