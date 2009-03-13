using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;

using Suprifattus.Util.Text;

namespace Suprifattus.Util.PdfGeneration.Objects.Tables
{
	public delegate void ColumnWidthChangingEventHandler(Column col, ref double newWidth);

	/// <summary>
	/// Representa uma coluna.
	/// </summary>
	public class Column
	{
		private double width = -1;

		private Hashtable mapValues;

		public event ColumnWidthChangingEventHandler WidthChanging;

		#region Public Properties
		public double Width
		{
			get { return width; }
			set
			{
				if (WidthChanging != null)
					WidthChanging(this, ref value);
				width = value;
			}
		}

		public TextStyle Style { get; set; }
    public Text Header { get; set; }
    public string FormatString { get; set; }
    public string NullValue { get; set; }
    public bool AlwaysFullVisible { get; set; }
    public string Expression { get; set; }
		#endregion

		#region Constructors
		public Column(string dataColumnName)
		{
			Header = new Text();
			Style = new TextStyle();
			this.Header.Contents = this.Expression = dataColumnName;
		}

		public Column(DataColumn col, int width)
		{
			Header = new Text();
			Style = new TextStyle();
			this.width = width;
			this.Header.Contents = this.Expression = col.ColumnName;
		}

		public Column(string dataColumnName, int width)
		{
			Header = new Text();
			Style = new TextStyle();
			this.width = width;
			this.Header.Contents = this.Expression = dataColumnName;
		}
		#endregion

		public void MapValue(string value, string text)
		{
			if (mapValues == null)
				mapValues = CollectionsUtil.CreateCaseInsensitiveHashtable();
			mapValues[value] = text;
		}

		public string FormatValue(object obj)
		{
			if (NullableHelper.IsNull(obj))
				obj = null;

			if (mapValues != null)
			{
				var val = (string) mapValues[Convert.ToString(obj)];
				if (val != null)
					obj = val;
			}

			if (obj == null && NullValue != null)
				obj = NullValue;

			using (new CultureSwitch(CultureInfo.CurrentUICulture))
			{
				if (FormatString != null)
					return String.Format(PluggableFormatProvider.Instance, FormatString, obj);

				return Convert.ToString(obj);
			}
		}
	}
}