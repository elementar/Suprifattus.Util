using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Suprifattus.Util.Web.Controls
{
	[ToolboxData("<{0}:LinkButtonEx runat=server/>")]
	public class LinkButtonEx : LinkButton
	{
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

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (!Logic.StringEmpty(OnClientClick))
				base.Attributes["onclick"] += String.Format(";{0};", OnClientClick);
			if (forceJSValidation)
				base.Attributes["onclick"] += ";if (typeof(JsValidation) != 'undefined') if (!JsValidation.validate()) EventUtil.cancel(event);";
			if (!CausesValidation)
				base.Attributes["jsvalidation:ignore"] += "true";
			base.AddAttributesToRender(writer);
		}
	}
}