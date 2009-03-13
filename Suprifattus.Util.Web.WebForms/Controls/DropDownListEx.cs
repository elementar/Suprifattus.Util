using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Commons.Web.UI.Controls.Persistence;

using Suprifattus.Util.Web.Controls.Helpers;

namespace Suprifattus.Util.Web.Controls
{
	[ParseChildren(false)]
	[ControlBuilder(typeof(ListControlBuilder))]
	[ToolboxData("<{0}:DropDownListEx runat=server/>")]
	public class DropDownListEx : DropDownList, ISelectControl, IPersistentControl
	{
		private string dataAutoFillFields, dataGroupTextField;
		private string dataDetailIdFields;
		private string sourceWS;
		private string clientChange;
		private IList fixedFirst = new ArrayList();
		private IList fixedLast = new ArrayList();

		#region Internal Properties
		IList IListControl.FixedItemsFirst
		{
			get { return fixedFirst; }
		}

		IList IListControl.FixedItemsLast
		{
			get { return fixedLast; }
		}
		#endregion

		#region Public Properties
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		public string DataAutoFillFields
		{
			get { return dataAutoFillFields; }
			set { dataAutoFillFields = value; }
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		public string DataDetailIdFields
		{
			get { return dataDetailIdFields; }
			set { dataDetailIdFields = value; }
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		public string DataGroupTextField 
		{
			get { return dataGroupTextField; }
			set { dataGroupTextField = value; }
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue(null)]
		public string SourceWebService 
		{
			get { return sourceWS; }
			set { sourceWS = value; }
		}
		#endregion

		#region Client-Side Event Properties
		public string OnClientChange
		{
			get { return clientChange; }
			set { clientChange = value; }
		}
		#endregion

		protected override void AddParsedSubObject(object obj)
		{
			try 
			{
				if (!ListControlUtil.AddParsedSubObjectReplacement(this, obj))
					base.AddParsedSubObject(obj);
			}
			catch (Exception ex)
			{
				if (obj is LiteralControl)
					throw new Exception(String.Format("Error while trying to add a literal with text: '{0}'", ((LiteralControl) obj).Text), ex);
				throw new Exception("Error while trying to add subobject of type " + obj.GetType().FullName, ex);
			}
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			dataAutoFillFields = (string) ViewState["dataAutoFillFields"];
			dataGroupTextField = (string) ViewState["dataGroupTextField"];
			dataDetailIdFields = (string) ViewState["dataDetailIdFields"];
			sourceWS = (string) ViewState["sourceWS"];
			clientChange = (string) ViewState["clientChange"];
		}

		protected override object SaveViewState()
		{
			ViewState["dataAutoFillFields"] = dataAutoFillFields;
			ViewState["dataGroupTextField"] = dataGroupTextField;
			ViewState["dataDetailIdFields"] = dataDetailIdFields;
			ViewState["sourceWS"] = sourceWS;
			ViewState["clientChange"] = clientChange;

			return base.SaveViewState();
		}

		public override void DataBind()
		{
			base.DataBind();
			ListControlUtil.AddExtendedListItemProperties(this);
			ListControlUtil.InsertFixedItems(this);
			ClearSelection();
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			ListControlUtil.AddExtendedSelectControlAttributesToRender(this, writer);
			if (!Logic.StringEmpty(clientChange))
				Attributes["onchange"] += ";" + clientChange + ";";
			base.AddAttributesToRender(writer);
		}
		
		protected override void RenderContents(HtmlTextWriter writer)
		{
			ListControlUtil.RenderSelectOptions(this, writer);
		}

		#region IListControl implementation
		public ListSelectionMode SelectionMode
		{
			get { return ListSelectionMode.Single; }
		}
		#endregion

		#region IPersistentControl implementation
		public object SaveState()
		{
			return SelectedValue;
		}

		public void LoadState(object state)
		{
			ClearSelection();
			ControlUtil.TrySetValue(this, (string) state);
		}
		#endregion
	}
}
