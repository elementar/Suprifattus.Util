using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using AttributeCollection = System.Web.UI.AttributeCollection;

namespace Suprifattus.Util.Web.Controls
{
	[ToolboxData("<{0}:ButtonEx runat=server/>")]
	public class ButtonEx : Button
	{
		static Regex rxAccessKey = new Regex(@"\[(.)\]", RegexOptions.Compiled);

		bool forceJSValidation = false;
		string onClientClick;
		
		#region Behavior
		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue("")]
		public string OnClientClick
		{
			get { return onClientClick; }
			set { onClientClick = value; }
		}

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool ForceJSValidation
		{
			get { return forceJSValidation; }
			set { forceJSValidation = value; }
		}
		#endregion

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			this.AddAttributesToRender(writer);

			writer.RenderBeginTag("button");
		}

		protected override void RenderContents(HtmlTextWriter w)
		{
			if (rxAccessKey.IsMatch(Text))
				w.Write(rxAccessKey.Replace(Text, "<u>$1</u>"));
			else
				w.Write(Text);

			base.RenderContents(w);
		}

		protected override void AddAttributesToRender(HtmlTextWriter w)
		{
			if (!Logic.StringEmpty(OnClientClick))
				base.Attributes["onclick"] += String.Format(";{0};", OnClientClick);
			if (forceJSValidation)
				base.Attributes["onclick"] += ";if (typeof(JsValidation) != 'undefined') if (!JsValidation.validate()) EventUtil.cancel(event);";
			if (!CausesValidation)
				base.Attributes["jsvalidation:ignore"] += "true";

			if (!Logic.StringEmpty(AccessKey) && rxAccessKey.IsMatch(Text))
				base.AccessKey = rxAccessKey.Match(Text).Groups[1].Value;

			base.AddAttributesToRender(w);
		}
	}
}
