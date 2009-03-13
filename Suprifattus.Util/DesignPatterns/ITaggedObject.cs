using System;

namespace Suprifattus.Util.DesignPatterns
{
	/// <summary>
	/// Marca objetos que tem a propriedade <see cref="Tag"/>.
	/// </summary>
	public interface ITaggedObject
	{
		/// <summary>
		/// A propriedade <see cref="Tag"/>.
		/// O conteúdo depende da implementação da classe.
		/// </summary>
		object Tag { get; set; }
	}
}
