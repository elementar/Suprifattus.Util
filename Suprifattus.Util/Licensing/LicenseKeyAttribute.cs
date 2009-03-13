using System;

namespace Suprifattus.Util.Licensing
{
	/// <summary>
	/// Permite a definição de uma chave de licença em um Assembly.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class LicenseKeyAttribute : Attribute
	{
		private string key;

		/// <summary>
		/// Cria uma nova chave de licença.
		/// </summary>
		/// <param name="key">A representação da chave</param>
		public LicenseKeyAttribute(string key)
		{
			this.key = key;
		}

		/// <summary>
		/// Busca a chave da licença.
		/// </summary>
		public string Key { get { return key; } }
	}
}
