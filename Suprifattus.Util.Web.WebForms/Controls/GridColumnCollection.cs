using System;
using System.Collections;
using System.ComponentModel;

namespace Suprifattus.Util.Web.Controls
{
	using Collections;
	using Reflection;

	/// <summary>
	/// Colunas de uma tabela.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(CollectionConverter))]
	[ValueCondition("value", typeof(GridColumn))]
	[CLSCompliant(false)]
	public class GridColumnCollection : CollectionBaseEx
	{
		public GridColumnCollection()
		{
		}

		public int Add(GridColumn col)
		{
			return List.Add(col);
		}

		public GridColumn this[int index] 
		{
			get { return (GridColumn) List[index]; }
			set { List[index] = value; }
		}
	}
}
