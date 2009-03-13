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
	[ToolboxData("<{0}:ListBoxEx runat=server/>")]
	public class ListBoxEx : ListBox, ISelectControl, IPersistentControl
	{
		private string dataAutoFillFields, dataGroupTextField;
		private string dataDetailIdFields;
		private string sourceWS;
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

		protected override void AddParsedSubObject(object obj)
		{
			if (!ListControlUtil.AddParsedSubObjectReplacement(this, obj))
				base.AddParsedSubObject(obj);
		}

		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);
			ListControlUtil.AddExtendedListItemProperties(this);
			ListControlUtil.InsertFixedItems(this);
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			ListControlUtil.AddExtendedSelectControlAttributesToRender(this, writer);
			base.AddAttributesToRender(writer);
		}
		
		protected override void RenderContents(HtmlTextWriter writer)
		{
			ListControlUtil.RenderSelectOptions(this, writer);
		}

		public string[] SelectedValues
		{
			get { return ListControlUtil.GetSelectedValues(this); }
			set { ListControlUtil.SetSelectedValues(this, value); }
		}

		#region IPersistentControl implementation
		public object SaveState()
		{
			return SelectedValues;
		}

		public void LoadState(object state)
		{
			SelectedValues = (string[]) state;
		}
		#endregion
	}
}
