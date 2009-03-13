using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Suprifattus.Util.Web.Controls
{
	public enum FixedListItemPosition
	{
		First, Last
	}

	[Serializable]
	[ControlBuilder(typeof(ListItemControlBuilder))]
	public class FixedListItem : Control
	{
		string value, text;
		FixedListItemPosition position;

		public FixedListItem()
		{
		}

		#region Public Properties
		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public FixedListItemPosition Position
		{
			get { return position; }
			set { position = value; }
		}
		#endregion

		protected override void AddParsedSubObject(object obj)
		{
			if (obj is LiteralControl)
				Text += ((LiteralControl) obj).Text;

			base.AddParsedSubObject(obj);
		}
	}
}
