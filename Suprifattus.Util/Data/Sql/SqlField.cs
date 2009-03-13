using System;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Representa um campo em uma consulta SQL.
	/// </summary>
	[Serializable]
	public class SqlField
	{
		string name, alias;

		/// <summary>
		/// Cria um novo campo de uma consulta SQL.
		/// </summary>
		/// <param name="name">O nome do campo</param>
		public SqlField(string name)
			: this(name, null) { }

		/// <summary>
		/// Cria um novo campo de uma consulta SQL.
		/// </summary>
		/// <param name="name">O nome do campo</param>
		/// <param name="alias">O <c>alias</c> do campo</param>
		public SqlField(string name, string alias)
		{
			this.name = name;
			this.alias = alias;
		}

		/// <summary>
		/// O <c>Nome</c> do campo.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// O <c>Alias</c> do campo.
		/// </summary>
		public string Alias
		{
			get { return alias; }
		}

		/// <summary>
		/// Retorna a representação <see cref="String"/> deste campo.
		/// </summary>
		/// <returns>A representação <see cref="String"/> deste campo.</returns>
		public override string ToString()
		{
			if (alias != null)
				return String.Format("{0} AS {1}", name, alias);
			else
				return name;
		}

	}
}
