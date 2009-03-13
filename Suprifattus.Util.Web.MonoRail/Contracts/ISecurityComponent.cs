using System;
using System.Security.Principal;
using System.Web;

using Suprifattus.Util.AccessControl;
using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Web.MonoRail;
using Suprifattus.Util.Web.MonoRail.Components.Security;

namespace Suprifattus.Util.Web.MonoRail.Contracts
{
	/// <summary>
	/// Contém regras utilizadas para implementar segurança.
	/// </summary>
	public interface ISecurityComponent
	{
		/// <summary>
		/// Efetua o login.
		/// </summary>
		/// <param name="username">Nome do usuário</param>
		/// <param name="password">Senha</param>
		/// <param name="savePassword">Salvar senha</param>
		/// <returns>Um <see cref="IExtendedPrincipal"/>, representando o usuário conectado</returns>
		/// <exception cref="SecurityException">Caso o login falhe por um motivo qualquer</exception>
		IExtendedPrincipal Login(string username, string password, bool savePassword);

		/// <summary>
		/// Tenta efetuar o login automático, geralmente baseado em cookies.
		/// </summary>
		/// <returns>
		/// O usuário conectado automaticamente, ou <c>null</c> caso não
		/// seja possível efetuar o login automático.
		/// </returns>
		IExtendedPrincipal LoginAutomatico();

		/// <summary>
		/// Desconecta o usuário.
		/// </summary>
		void Logout();

		/// <summary>
		/// Redireciona para a página de login.
		/// </summary>
		void RedirecionaParaLogin();
		
		/// <summary>
		/// Altera a senha.
		/// </summary>
		/// <param name="user">Usuário</param>
		/// <param name="senha">Senha</param>
		/// <param name="senha2">Confirmação da senha</param>
		void AlteraSenha(ISimpleAppUser user, string senha, string senha2);

		/// <summary>
		/// Criptografa a senha especificada, para fins de comparação ou
		/// armazenamento em banco de dados.
		/// </summary>
		/// <param name="senha">A senha descriptografada</param>
		/// <returns>A senha criptografada</returns>
		string CriptografaSenha(string senha);

		[Obsolete("Substituída pela propriedade 'Principal', que não obtém da sessão")]
		IExtendedPrincipal UsuarioConectado { get; }
		
		/// <summary>
		/// Obtém o usuário atualmente conectado.
		/// </summary>
		IExtendedPrincipal Principal { get; }

		/// <summary>
		/// Define a URL da página de login.
		/// </summary>
		string UrlLogin { get; set; }
		
		string UrlSSL { get; set; }
		string UrlNormal { get; set; }
		
		/// <summary>
		/// Prepara o objeto <see cref="IPrincipal"/>. Utilizado geralmente
		/// no início de uma nova requisição web.
		/// </summary>
		/// <seealso cref="Principal"/>
		/// <seealso cref="BaseMonoRailApplication.LoadPrincipal"/>
		/// <seealso cref="HttpApplication.PreRequestHandlerExecute"/>
		IPrincipal PreparePrincipal();
	}
}