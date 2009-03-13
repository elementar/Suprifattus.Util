using System;
using System.Web.UI.WebControls;

using Commons.Web.UI.Controls.Persistence;

namespace Suprifattus.Util.Web.Controls
{
	public class CheckBoxEx : CheckBox, IPersistentControl
	{
		object IPersistentControl.SaveState()
		{
			return Checked;
		}

		void IPersistentControl.LoadState(object state)
		{
			Checked = (bool) state;
		}
	}
}
