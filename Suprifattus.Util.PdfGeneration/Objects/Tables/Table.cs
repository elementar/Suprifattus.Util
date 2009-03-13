using System;
using System.Collections;

namespace Suprifattus.Util.PdfGeneration.Objects.Tables
{
	[CLSCompliant(false)]
	public class Table : IEnumerable
	{
		private IEnumerable rows;
		private readonly ColumnCollection cols;

		#region Constructors
		public Table(ColumnCollection cols)
		{
			this.cols = cols;
		}

		public Table(string columnMetadataPath)
		{
			this.cols = new ColumnCollection(new ColumnMetadataHelper(columnMetadataPath));
		}
		#endregion

		#region Public Properties
		public IEnumerable Rows
		{
			get { return rows; }
			set { rows = value; }
		}

		public ColumnCollection Columns
		{
			get { return cols; }
		}
		#endregion

		public void Fit(Page page)
		{
			page.Fit(cols);
		}

		public IEnumerator GetEnumerator()
		{
			return new TableRowEnumerator(rows, cols);
		}
	}
}