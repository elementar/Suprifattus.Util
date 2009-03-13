using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Suprifattus.Util.Web.Controls.Helpers
{
	public class ListControlBuilder : ControlBuilder
	{
		public override bool AllowWhitespaceLiterals()
		{
			return false;
		}
		
		public override Type GetChildControlType(string tagName, IDictionary attribs)
		{
			if (tagName == "supri:FixedListItem")
				return typeof(FixedListItem);
			else if (tagName.EndsWith("ListItem"))
				return typeof(ListItem);

			return base.GetChildControlType(tagName, attribs);
		}
	}
}