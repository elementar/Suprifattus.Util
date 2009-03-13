using System;
using System.Collections;
using System.Web.UI;

namespace Suprifattus.Util.PdfGeneration.Objects.Tables
{
	public class TableRowWrapper
	{
		private readonly object row;
		private readonly ColumnCollection cols;

		public TableRowWrapper(ColumnCollection cols, object row)
		{
			this.row = row;
			this.cols = cols;
		}

		public ColumnCollection Columns
		{
			get { return cols; }
		}

		public virtual object Get(Column col)
		{
			return col.FormatValue(DataBinder.Eval(row, col.Expression));
		}
	}

	public class TableRowEnumerator : IEnumerator
	{
		private readonly IEnumerator rows;
		private readonly ColumnCollection cols;
		private TableRowWrapper currentWrapper;

		public TableRowEnumerator(IEnumerable rows, ColumnCollection cols)
		{
			this.rows = rows.GetEnumerator();
			this.cols = cols;
		}

		public bool MoveNext()
		{
			currentWrapper = null;
			return rows.MoveNext();
		}

		public void Reset()
		{
			currentWrapper = null;
			rows.Reset();
		}

		public object Current
		{
			get { return (currentWrapper ?? (currentWrapper = new TableRowWrapper(cols, rows.Current))); }
		}
	}
}