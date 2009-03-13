using System;
using System.CodeDom.Compiler;

using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Representa uma ordena��o em uma cl�usula SQL.
	/// </summary>
	[Serializable]
	public class SqlOrder : ISqlRenderable
	{
		string[] order;

		/// <summary>
		/// Cria um novo campo de ordena��o na consulta SQL.
		/// </summary>
		/// <param name="order">O par�metro de ordena��o</param>
		public SqlOrder(params string[] order)
		{
			this.order = order;
		}

		/// <summary>
		/// A ordena��o.
		/// </summary>
		public string[] Order
		{
			get { return order; }
		}

		/// <summary>
		/// Retorna a representa��o <see cref="String"/> desta cl�usula de ordena��o.
		/// </summary>
		/// <returns>A representa��o <see cref="String"/> desta cl�usula de ordena��o.</returns>
		public override string ToString()
		{
			return CollectionUtils.Join(order, ", ");
		}

		/// <summary>
		/// Renderiza a cl�usula ORDER BY.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public void Render(IndentedTextWriter tw)
		{
			for (int i=0; i < order.Length; i++) 
				tw.WriteLine(order[i] + (i+1 < order.Length ? ", " : ""));
		}
	}
}
