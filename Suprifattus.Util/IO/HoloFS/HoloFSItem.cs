using System;

namespace Suprifattus.Util.IO.HoloFS
{
	/// <summary>
	/// Classe básica para itens no HoloFS.
	/// </summary>
	public abstract class HoloFSItem
	{
		/// <summary>
		/// Se o item existe.
		/// </summary>
		public abstract bool Exists { get; }

		/// <summary>
		/// Nome do item.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Nome completo do item, incluindo o nome de todos os seus pais.
		/// </summary>
		public abstract string FullName { get; }
	}
}
