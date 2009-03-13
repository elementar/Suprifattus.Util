using System;
using System.Web.UI.WebControls;

namespace Suprifattus.Util.PdfGeneration
{
	public class TextStyle
	{
		#region Constructors
		public TextStyle()
		{
			Alignment = TextAlignment.Left;
			Size = Unit.Point(8);
		}

		public TextStyle(Unit size)
		{
			Alignment = TextAlignment.Left;
			this.Size = size;
		}

		public TextStyle(TextAlignment alignment)
		{
			Size = Unit.Point(8);
			this.Alignment = alignment;
		}

		public TextStyle(Unit size, TextAlignment alignment)
		{
			this.Size = size;
			this.Alignment = alignment;
		}
		#endregion

		public Unit Size { get; set; }
		public TextAlignment Alignment { get; set; }
	}
}