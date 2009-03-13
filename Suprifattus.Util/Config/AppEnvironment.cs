using System;

namespace Suprifattus.Util.Config
{
	/// <summary>
	/// Define os possíveis ambientes em que a aplicação poderá estar sendo executada.
	/// </summary>
	public enum AppEnvironment
	{
		/// <summary>
		/// Ambiente de Produção
		/// </summary>
		Production,
		/// <summary>
		/// Ambiente de Simulação
		/// </summary>
		Simulation,
		/// <summary>
		/// Ambiente de Desenvolvimento
		/// </summary>
		Development,
		/// <summary>
		/// Ambiente de Integração
		/// </summary>
		Integration,
		/// <summary>
		/// Ambiente de Testes
		/// </summary>
		Test,
	}
}
