using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Commons.Web.UI.Controls.Persistence;

using Suprifattus.Util.Data.XBind;

namespace Suprifattus.Util.Web.Controls
{
	[ToolboxData("<{0}:TextBoxEx runat=server/>")]
	public class TextBoxEx : TextBox, IPersistentControl, IXBindAware, IDatabaseFieldRelated
	{
		bool autocomplete = true;
		string afExpr;
		string mask;
		string valType;
		string dbField;
		string xbindExpression;

		#region Public Properties - Data
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue(null)]
		public string AutoFillExpr
		{
			get { return afExpr; }
			set { afExpr = value; }
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue(null)]
		public string DatabaseField
		{
			get { return dbField; }
			set { dbField = value; }
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue(null)]
		public string XBindExpression
		{
			get { return xbindExpression; }
			set { xbindExpression = value; }
		}
		#endregion

		#region Public properties - Behaviour
		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue(true)]
		public bool AutoComplete
		{
			get { return autocomplete; }
			set { autocomplete = value; }
		}

		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue(null)]
		public string Mask
		{
			get { return mask; }
			set { mask = value; }
		}

		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue(null)]
		public string ValidationType
		{
			get { return valType; }
			set { valType = value; }
		}
		#endregion

		#region Load & Save from ViewState
		protected override object SaveViewState()
		{
			ViewState["autocomplete"] = autocomplete;
			ViewState["autofillexpr"] = afExpr;
			ViewState["mask"] = mask;
			ViewState["valType"] = valType;
			ViewState["dbField"] = dbField;
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			autocomplete = (bool) ViewState["autocomplete"];
			afExpr = (string) ViewState["autofillexpr"];
			mask = (string) ViewState["mask"];
			valType = (string) ViewState["valType"];
			dbField = (string) ViewState["dbField"];
		}
		#endregion

		public override void DataBind()
		{
			base.DataBind();
			((IXBindAware) this).Bind();
		}

		void IXBindAware.Bind()
		{
			if (!Logic.StringEmpty(xbindExpression))
				Text = Convert.ToString(XBindContext.Current.Resolve(xbindExpression));
		}

		void IXBindAware.Update()
		{
			if (!Logic.StringEmpty(xbindExpression))
				XBindContext.Current.Update(xbindExpression, Text);
		}
		
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (!autocomplete)
				Attributes.Add("autocomplete", "off");

			if (afExpr != null && afExpr.Length > 0)
				Attributes.Add("jsmasterdetail:autofill-expr", afExpr);

			base.AddAttributesToRender(writer);
		}

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			if (mask != null) 
			{
				writer.WriteBeginTag("jsmaskedit:Bind");
				writer.WriteAttribute("Control", ClientID);
				writer.WriteAttribute("Mask", Mask);
				writer.Write(HtmlTextWriter.SelfClosingTagEnd);
			}
			if (valType != null)
			{
				writer.WriteBeginTag("jsvalidation:RegexValidation");
				writer.WriteAttribute("Type", valType);
				writer.Write(HtmlTextWriter.SelfClosingTagEnd);
			}

			base.RenderBeginTag(writer);
		}


		#region IPersistentControl implementation
		public object SaveState()
		{
			return Text;
		}

		public void LoadState(object state)
		{
			Text = (string) state;
		}
		#endregion
	}
}
