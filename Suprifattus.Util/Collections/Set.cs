using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Um conjunto.
	/// </summary>
	public interface ISet : ICollection
	{
		/// <summary>
		/// Adiciona um item no conjunto.
		/// </summary>
		/// <param name="val">O valor a ser adicionado</param>
		/// <returns>Verdadeiro se o item n�o existia, falso se j� existia</returns>
		bool Add(object val);
		
		/// <summary>
		/// Limpa o conjunto.
		/// </summary>
		void Clear();
		
		/// <summary>
		/// Adiciona diversos itens no conjunto.
		/// </summary>
		/// <param name="vals">Os diversos itens a serem adicionados</param>
		void AddRange(params object[] vals);
		
		/// <summary>
		/// Adiciona diversos itens no conjunto.
		/// </summary>
		/// <param name="vals">Os diversos itens a serem adicionados</param>
		void AddRange(IEnumerable vals);

		/// <summary>
		/// Verifica se um item est� contido no conjunto.
		/// </summary>
		/// <param name="val">O item a ser verificado</param>
		/// <returns>Verdadeiro se o item se encontra no conjunto, falso caso contr�rio</returns>
		bool Contains(object val);
	}
}
