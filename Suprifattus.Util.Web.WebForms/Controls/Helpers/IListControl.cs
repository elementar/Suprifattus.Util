using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Suprifattus.Util.Web.Controls.Helpers
{
	/// <summary>
	/// Any list control, like RadioButtonList or DropDownList
	/// </summary>
	public interface IListControl
	{
		string ID { get; }
		ListSelectionMode SelectionMode { get; }

		AttributeCollection Attributes { get; }

		ListItemCollection Items { get; }
		IList FixedItemsFirst { get; }
		IList FixedItemsLast { get; }
	}
}