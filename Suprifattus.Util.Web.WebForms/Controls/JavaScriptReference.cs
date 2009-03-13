using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Suprifattus.Util.Web.Controls
{
	using JavaScript;
	using Collections;

	[ToolboxData("<{0}:JavaScriptReference runat=server/>")]
	public class JavaScriptReference : System.Web.UI.Control
	{
		private string url;

		public string HandlerUrl
		{
			get { return url; }
			set { url = value; }
		}

		public JavaScriptReference()
		{
		}

		public JavaScriptReference(string handlerUrl)
		{
			this.HandlerUrl = handlerUrl;
		}

		protected string JavaScripts 
		{
			get { return CollectionUtils.Join(JavaScriptHttpHandler.Dependencies, ","); }
		}
		
		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteBeginTag("script");
			writer.WriteAttribute("type", "text/javascript");
			writer.WriteAttribute("src", url + "?" + JavaScripts);
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteEndTag("script");
			writer.WriteLine();
		}

	}
}
