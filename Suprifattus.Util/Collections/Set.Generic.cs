using System;
using System.Collections.Generic;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Um conjunto.
	/// </summary>
	/// <typeparam name="T">O tipo de dados utilizado pelo conjunto.</typeparam>
	public interface ISet<T> : ICollection<T>
	{
		/// <summary>
		/// Adiciona um item no conjunto.
		/// </summary>
		/// <param name="val">O valor a ser adicionado</param>
		/// <returns>Verdadeiro se o item não existia, falso se já existia</returns>
		new bool Add(T val);

		/// <summary>
		/// Adiciona diversos itens no conjunto.
		/// </summary>
		/// <param name="vals">Os diversos itens a serem adicionados</param>
		void AddRange(params T[] vals);

		/// <summary>
		/// Adiciona diversos itens no conjunto.
		/// </summary>
		/// <param name="vals">Os diversos itens a serem adicionados</param>
		void AddRange(IEnumerable<T> vals);
	}
}