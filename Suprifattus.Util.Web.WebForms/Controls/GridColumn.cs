using System;
using System.Data;

namespace Suprifattus.Util.Web.Controls
{
	/// <summary>
	/// Uma coluna de uma tabela.
	/// </summary>
	[Serializable]
	public class GridColumn
	{
		[NonSerialized] DataColumn dc;
		int index;
		bool allowHtml, allowFilter;
		string headerText, headerStyle;
		string dataField;
		string dataFormatString;
		string filterFormatString;

		public GridColumn()
		{
		}

		public int ColumnIndex
		{
			get { return index; }
			set { index = value; }
		}

		public bool AllowHtml
		{
			get { return allowHtml; }
			set { allowHtml = value; }
		}

		public bool AllowFilter
		{
			get { return allowFilter; }
			set { allowFilter = value; }
		}

		public DataColumn SourceDataColumn
		{
			get { return dc; }
			set { dc = value; }
		}

		public string HeaderText
		{
			get { return headerText; }
			set { headerText = value; }
		}

		public string HeaderStyle
		{
			get { return headerStyle; }
			set { headerStyle = value; }
		}

		public string HeaderStyleXML
		{
			get
			{
				if (headerStyle != null && headerStyle.Length > 0)
					return String.Format("style=\"{0}\"", headerStyle);
				return String.Empty;
			}
		}

		[Obsolete("Use DataField instead")]
		public string ColumnName
		{
			get { return dataField; }
			set { dataField = value; }
		}

		public string DataField
		{
			get { return dataField; }
			set { dataField = value; }
		}

		public string DataFormatString
		{
			get { return dataFormatString; }
			set { dataFormatString = value; }
		}

		public string FilterFormatString
		{
			get { return filterFormatString; }
			set { filterFormatString = value; }
		}
	}
}