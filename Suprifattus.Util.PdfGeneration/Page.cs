using System;

using Suprifattus.Util.PdfGeneration.Objects.Tables;

namespace Suprifattus.Util.PdfGeneration
{
	[CLSCompliant(false)]
	public class Page
	{
		private float width, height;
		private float marginLeft = 1, marginRight = 1, marginTop = 1, marginBottom = 1;
		private readonly PageSize size;

		#region Constructors
		public Page(PageSize pageSize)
		{
			this.size = pageSize;

			switch (pageSize & ~PageSize.Landscape)
			{
				case PageSize.A4:
					this.width = 21f;
					this.height = 29.7f;
					break;
				default:
					throw new Exception("Unrecognized page size: " + pageSize);
			}

			if ((pageSize & PageSize.Landscape) != 0)
			{
				// inverte largura e altura
				Logic.Swap(ref this.width, ref this.height);
			}
		}

		public Page(float width, float height)
		{
			this.size = PageSize.Custom;
			this.width = width;
			this.height = height;
		}
		#endregion

		/// <summary>
		/// Escalona a largura das colunas para o tamanho da página.
		/// </summary>
		/// <param name="cols">As colunas</param>
		public void Fit(ColumnCollection cols)
		{
			cols.ScaleBy(0.17, width - 2);
		}

		#region Public Properties
		public float Width
		{
			get { return width; }
			set { width = value; }
		}

		public float Height
		{
			get { return height; }
			set { height = value; }
		}

		public float MarginLeft
		{
			get { return marginLeft; }
			set { marginLeft = value; }
		}

		public float MarginRight
		{
			get { return marginRight; }
			set { marginRight = value; }
		}

		public float MarginTop
		{
			get { return marginTop; }
			set { marginTop = value; }
		}

		public float MarginBottom
		{
			get { return marginBottom; }
			set { marginBottom = value; }
		}
		#endregion

		public PageSize Size
		{
			get { return size & ~PageSize.Landscape; }
		}

		public PageSize Orientation
		{
			get { return size & PageSize.Landscape; }
		}
	}
}