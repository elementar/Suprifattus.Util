using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Suprifattus.Util.Web.Controls
{
	using JavaScript;

	[ToolboxData("<{0}:JavaScriptValidation runat=server/>")]
	public class JavaScriptValidation : System.Web.UI.Control
	{
		protected override void OnLoad(EventArgs e)
		{
			JavaScriptHttpHandler.Dependencies.AddRange("validate");
		}

		#region Properties
		#endregion
	}
}
