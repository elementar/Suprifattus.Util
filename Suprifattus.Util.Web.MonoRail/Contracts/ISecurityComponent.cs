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
	/// Cont�m regras utilizadas para implementar seguran�a.
	/// </summary>
	public interface ISecurityComponent
	{
		/// <summary>
		/// Efetua o login.
		/// </summary>
		/// <param name="username">Nome do usu�rio</param>
		/// <param name="password">Senha</param>
		/// <param name="savePassword">Salvar senha</param>
		/// <returns>Um <see cref="IExtendedPrincipal"/>, representando o usu�rio conectado</returns>
		/// <exception cref="SecurityException">Caso o login falhe por um motivo qualquer</exception>
		IExtendedPrincipal Login(string username, string password, bool savePassword);

		/// <summary>
		/// Tenta efetuar o login autom�tico, geralmente baseado em cookies.
		/// </summary>
		/// <returns>
		/// O usu�rio conectado automaticamente, ou <c>null</c> caso n�o
		/// seja poss�vel efetuar o login autom�tico.
		/// </returns>
		IExtendedPrincipal LoginAutomatico();

		/// <summary>
		/// Desconecta o usu�rio.
		/// </summary>
		void Logout();

		/// <summary>
		/// Redireciona para a p�gina de login.
		/// </summary>
		void RedirecionaParaLogin();
		
		/// <summary>
		/// Altera a senha.
		/// </summary>
		/// <param name="user">Usu�rio</param>
		/// <param name="senha">Senha</param>
		/// <param name="senha2">Confirma��o da senha</param>
		void AlteraSenha(ISimpleAppUser user, string senha, string senha2);

		/// <summary>
		/// Criptografa a senha especificada, para fins de compara��o ou
		/// armazenamento em banco de dados.
		/// </summary>
		/// <param name="senha">A senha descriptografada</param>
		/// <returns>A senha criptografada</returns>
		string CriptografaSenha(string senha);

		[Obsolete("Substitu�da pela propriedade 'Principal', que n�o obt�m da sess�o")]
		IExtendedPrincipal UsuarioConectado { get; }
		
		/// <summary>
		/// Obt�m o usu�rio atualmente conectado.
		/// </summary>
		IExtendedPrincipal Principal { get; }

		/// <summary>
		/// Define a URL da p�gina de login.
		/// </summary>
		string UrlLogin { get; set; }
		
		string UrlSSL { get; set; }
		string UrlNormal { get; set; }
		
		/// <summary>
		/// Prepara o objeto <see cref="IPrincipal"/>. Utilizado geralmente
		/// no in�cio de uma nova requisi��o web.
		/// </summary>
		/// <seealso cref="Principal"/>
		/// <seealso cref="BaseMonoRailApplication.LoadPrincipal"/>
		/// <seealso cref="HttpApplication.PreRequestHandlerExecute"/>
		IPrincipal PreparePrincipal();
	}
}