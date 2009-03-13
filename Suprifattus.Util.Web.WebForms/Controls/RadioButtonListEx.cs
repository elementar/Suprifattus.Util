using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using Commons.Web.UI.Controls.Persistence;

using Suprifattus.Util.Web.Controls.Helpers;

namespace Suprifattus.Util.Web.Controls
{
	[ControlBuilder(typeof(ListControlBuilder))]
	[ParseChildren(false)]
	public class RadioButtonListEx : RadioButtonList, IListControl, IPersistentControl
	{
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

		protected override void AddParsedSubObject(object obj)
		{
			if (!ListControlUtil.AddParsedSubObjectReplacement(this, obj))
				base.AddParsedSubObject(obj);
		}

		public override void DataBind()
		{
			base.DataBind();
			ListControlUtil.InsertFixedItems(this);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
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
			SelectedValue = (string) state;
		}
		#endregion
	}
}
