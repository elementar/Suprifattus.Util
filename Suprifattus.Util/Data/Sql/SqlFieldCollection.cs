using System;
using System.Collections;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Armazena uma cole��o de campos de uma consulta SQL.
	/// </summary>
	[Serializable]
	public class SqlFieldCollection : CollectionBase
	{
		/// <summary>
		/// Cria uma nova cole��o de campos.
		/// </summary>
		internal SqlFieldCollection()
		{
		}

		/// <summary>
		/// Adiciona um novo campo � lista.
		/// </summary>
		/// <param name="field">O novo campo.</param>
		/// <returns>O �ndice onde o campo foi inserido</returns>
		public int Add(SqlField field)
		{
			return InnerList.Add(field);
		}

		/// <summary>
		/// Cria e adiciona um novo campo � lista.
		/// </summary>
		/// <param name="fieldName">O nome do novo campo.</param>
		/// <returns>O �ndice onde o campo foi inserido</returns>
		public int Add(string fieldName)
		{
			return Add(new SqlField(fieldName));
		}

		/// <summary>
		/// Adiciona v�rios campos � lista.
		/// </summary>
		/// <param name="fields">Os campos a serem adicionados</param>
		public void AddRange(params SqlField[] fields)
		{
			InnerList.AddRange(fields);
		}

		/// <summary>
		/// Cria e adiciona v�rios campos � lista.
		/// </summary>
		/// <param name="fields">O nome dos campos que devem ser criados e adicionados</param>
		public void AddRange(params string[] fields)
		{
			foreach (string fieldName in fields)
				Add(fieldName);
		}
		
		/// <summary>
		/// Verifica se existe um <see cref="SqlField"/> com o nome especificado.
		/// </summary>
		/// <param name="fieldName">O nome do campo</param>
		/// <returns>Verdadeiro se a lista cont�m um <see cref="SqlField"/> com o nome especificado, Falso caso contr�rio</returns>
		public bool Contains(string fieldName)
		{
			return FindByName(fieldName) != null;
		}

		/// <summary>
		/// Tenta localizar um objeto <see cref="SqlField"/> na cole��o, que tenha
		/// como nome o <c>fieldName</c> especificado. A compara��o � <c>case-insensitive</c>.
		/// </summary>
		/// <param name="fieldName">O nome do campo</param>
		/// <returns>O objeto <see cref="SqlField"/>, ou <c>null</c></returns>
		public SqlField FindByName(string fieldName)
		{
			foreach (SqlField field in InnerList)
				if (String.Compare(field.Name, fieldName, false) == 0)
					return field;
			return null;
		}
		
		/// <summary>
		/// Retorna o campo na posi��o especificada.
		/// </summary>
		public SqlField this[int index]
		{
			get { return (SqlField) InnerList[index]; }
			set { InnerList[index] = value; }
		}

		/// <summary>
		/// Valida os elementos que s�o inseridos na cole��o.
		/// </summary>
		/// <param name="value">O novo elemento</param>
		protected override void OnValidate(object value)
		{
			if (!(value is SqlField))
				throw new ArgumentException("Value must be of type " + typeof(SqlField).FullName);
		}
	}
}
