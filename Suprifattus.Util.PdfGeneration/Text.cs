using System;
using System.Web.UI.WebControls;

namespace Suprifattus.Util.PdfGeneration
{
	public class Text
	{
		#region Constructors
		public Text(Unit size, string contents)
		{
			Style = new TextStyle();
			this.Style.Size = size;
			this.Contents = contents;
		}

		public Text(int size, string contents)
			: this(Unit.Point(size), contents)
		{
		}

		public Text(string contents)
			: this(8, contents)
		{
		}

		public Text()
			: this(8, null)
		{
		}
		#endregion

		public TextStyle Style { get; set; }
		public string Contents { get; set; }
	}
}