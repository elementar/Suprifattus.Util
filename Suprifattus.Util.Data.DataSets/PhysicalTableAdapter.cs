using System;
using System.Data;

namespace Suprifattus.Util.Data
{
	/// <summary>
	/// Recupera o nome físico de uma <see cref="DataTable"/>
	/// e, no <see cref="Dispose"/>, retorna ao nome virtual.
	/// </summary>
	/// <remarks>
	///	Ideal para ser utilizado em conjunto com o comando <c>using</c>.
	/// </remarks>
	public sealed class PhysicalTableAdapter : IDisposable
	{
		ConnectorBase conn;
		DataTable dt;
		string originalTableName;

		/// <summary>
		/// Cria um novo <see cref="PhysicalTableAdapter"/>.
		/// </summary>
		/// <param name="conn">O <see cref="ConnectorBase"/></param>
		/// <param name="dt">A <see cref="DataTable"/></param>
		public PhysicalTableAdapter(ConnectorBase conn, DataTable dt)
		{
			this.conn = conn;
			this.dt = dt;

			Begin();
		}

		/// <summary>
		/// Finaliza a <see cref="PhysicalTableAdapter"/>, devolvendo a
		/// <see cref="DataTable"/> a seu estado original.
		/// </summary>
		public void Dispose()
		{
			End();
		}

		void Begin()
		{
			dt.TableName = conn.GetPhysicalTableName(originalTableName = dt.TableName);
		}

		void End()
		{
			dt.TableName = originalTableName;
		}
	}
}
