using System;
using System.Web.UI;

using Suprifattus.Util.Web.JavaScript;

namespace Suprifattus.Util.Web.Controls
{
	[ToolboxData("<{0}:JavaScriptMaskEdit runat=server/>")]
	public class JavaScriptMaskEdit : Control
	{
		string focusOnStart;

		public string FocusOnStart
		{
			get { return focusOnStart; }
			set { focusOnStart = value; }
		}

		protected override void OnLoad(EventArgs e)
		{
			JavaScriptHttpHandler.Dependencies.AddRange("maskedit");
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (!Logic.StringEmpty(focusOnStart))
			{
				writer.WriteBeginTag("jsmaskedit:FocusOnStart");
				writer.WriteAttribute("Control", focusOnStart);
				writer.Write(HtmlTextWriter.SelfClosingTagEnd);
			}
		}
	}
}
