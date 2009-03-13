using System;
using System.CodeDom.Compiler;

using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Representa uma ordenação em uma cláusula SQL.
	/// </summary>
	[Serializable]
	public class SqlOrder : ISqlRenderable
	{
		string[] order;

		/// <summary>
		/// Cria um novo campo de ordenação na consulta SQL.
		/// </summary>
		/// <param name="order">O parâmetro de ordenação</param>
		public SqlOrder(params string[] order)
		{
			this.order = order;
		}

		/// <summary>
		/// A ordenação.
		/// </summary>
		public string[] Order
		{
			get { return order; }
		}

		/// <summary>
		/// Retorna a representação <see cref="String"/> desta cláusula de ordenação.
		/// </summary>
		/// <returns>A representação <see cref="String"/> desta cláusula de ordenação.</returns>
		public override string ToString()
		{
			return CollectionUtils.Join(order, ", ");
		}

		/// <summary>
		/// Renderiza a cláusula ORDER BY.
		/// </summary>
		/// <param name="tw">O <see cref="IndentedTextWriter"/></param>
		public void Render(IndentedTextWriter tw)
		{
			for (int i=0; i < order.Length; i++) 
				tw.WriteLine(order[i] + (i+1 < order.Length ? ", " : ""));
		}
	}
}
