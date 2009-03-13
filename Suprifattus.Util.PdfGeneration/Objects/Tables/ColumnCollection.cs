using System;
using System.Collections;
using System.Data;

namespace Suprifattus.Util.PdfGeneration.Objects.Tables
{
	public class ColumnCollection : CollectionBase
	{
		private readonly Hashtable map = new Hashtable();
		private readonly ColumnStyleInferer inferer;

		public ColumnCollection()
		{
		}

		public ColumnCollection(ColumnMetadataHelper helper)
		{
			this.inferer = new ColumnStyleInferer(helper, this);
		}

		/// <summary>
		/// Calcula a largura física real das colunas fixas, 
		/// e distribui o espaço restante entre os elementos
		/// que não tem largura fixa.
		/// </summary>
		/// <param name="aproxCharLength">A largura aproximada dos caracteres na página.</param>
		/// <param name="maxWidth">A largura máxima</param>
		public void ScaleBy(double aproxCharLength, double maxWidth)
		{
			inferer.ScaleBy(aproxCharLength, maxWidth);
		}

		public Column InferAndAdd(string columnName, Type columnDataType)
		{
			var col = inferer.Infer(columnName, columnDataType);
			Add(col);
			return col;
		}

		public Column InferAndAdd(DataColumn column)
		{
			var col = inferer.Infer(column);
			Add(col);
			return col;
		}

		public int Add(Column column)
		{
			return List.Add(column);
		}

		#region OnCollectionChange Overrides
		protected override void OnRemoveComplete(int index, object value)
		{
			map.Remove(((Column) value).Expression);
			base.OnRemoveComplete(index, value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			map.Add(((Column) value).Expression, value);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			base.OnSetComplete(index, oldValue, newValue);
			map.Remove(((Column) oldValue).Expression);
			map.Add(((Column) newValue).Expression, newValue);
		}

		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			map.Clear();
		}
		#endregion

		public Column this[int index]
		{
			get { return (Column) InnerList[index]; }
			set { InnerList[index] = value; }
		}

		public Column this[string dataColumnName]
		{
			get { return (Column) map[dataColumnName]; }
		}

		public Column this[DataColumn dataColumn]
		{
			get { return this[dataColumn.ColumnName]; }
		}

		protected override void OnValidate(object value)
		{
			if (!(value is Column))
				throw new ArgumentException("Value must be of type ColumnStyle");
		}
	}
}