using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using Commons.Web.UI.Controls.Persistence;

using Suprifattus.Util.Web.Controls.Helpers;

namespace Suprifattus.Util.Web.Controls
{
	[ParseChildren(false)]
	[ControlBuilder(typeof(ListControlBuilder))]
	public class CheckBoxListEx : CheckBoxList, IListControl, IPersistentControl
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

		public string[] SelectedValues
		{
			get { return ListControlUtil.GetSelectedValues(this); }
			set { ListControlUtil.SetSelectedValues(this, value); }
		}

		protected override void AddParsedSubObject(object obj)
		{
			if (!ListControlUtil.AddParsedSubObjectReplacement(this, obj))
				base.AddParsedSubObject(obj);
		}

		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);
			ListControlUtil.InsertFixedItems(this);
		}

		#region IListControl implementation
		public ListSelectionMode SelectionMode
		{
			get { return ListSelectionMode.Multiple; }
		}
		#endregion

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
