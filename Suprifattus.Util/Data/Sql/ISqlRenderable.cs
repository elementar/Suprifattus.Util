using System;
using System.CodeDom.Compiler;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Interface que deve ser implementada por objetos que participem
	/// da renderização de uma cláusula SQL.
	/// </summary>
	public interface ISqlRenderable
	{
		/// <summary>
		/// Renderiza este objeto no <see cref="IndentedTextWriter"/> especificado.
		/// </summary>
		/// <param name="writer"></param>
		void Render(IndentedTextWriter writer);
	}
}
