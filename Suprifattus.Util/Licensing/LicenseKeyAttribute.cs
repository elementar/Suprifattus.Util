using System;

namespace Suprifattus.Util.Licensing
{
	/// <summary>
	/// Permite a defini��o de uma chave de licen�a em um Assembly.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class LicenseKeyAttribute : Attribute
	{
		private string key;

		/// <summary>
		/// Cria uma nova chave de licen�a.
		/// </summary>
		/// <param name="key">A representa��o da chave</param>
		public LicenseKeyAttribute(string key)
		{
			this.key = key;
		}

		/// <summary>
		/// Busca a chave da licen�a.
		/// </summary>
		public string Key { get { return key; } }
	}
}
