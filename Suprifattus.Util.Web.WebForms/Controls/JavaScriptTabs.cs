using System;
using System.Web.UI;

using Suprifattus.Util.Web.JavaScript;

namespace Suprifattus.Util.Web.Controls
{
	[ToolboxData("<{0}:JavaScriptTabs runat=server/>")]
	public class JavaScriptTabs : Control
	{
		protected override void OnLoad(EventArgs e)
		{
			JavaScriptHttpHandler.Dependencies.AddRange("tabs");
		}
	}
}
