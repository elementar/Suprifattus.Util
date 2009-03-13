using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Suprifattus.Util.Web.Controls
{
	public class GridPaging : UserControl
	{
		[DefaultValue(1)]
		public int CurrentPage
		{
			get { return (int) ViewState["current"]; }
			set { ViewState["current"] = value; }
		}

		[DefaultValue(1)]
		public int PageCount
		{
			get { return (int) ViewState["count"]; }
			set { ViewState["count"] = value; }
		}

		[DefaultValue(0)]
		public int TotalRecords
		{
			get { return Convert.ToInt32(ViewState["total"]); }
			set { ViewState["total"] = value; }
		}

		#region Delegate and EventArgs
		public class PageChangedEventArgs : EventArgs
		{
			public PageChangedEventArgs(int oldPageIndex, int newPageIndex)
			{
				this.OldPageIndex = oldPageIndex;
				this.NewPageIndex = newPageIndex;
			}

			public readonly int OldPageIndex;
			public int NewPageIndex;
		}
		
		public delegate void PageChangedDelegate(object sender, PageChangedEventArgs e);
		#endregion
		
		public event PageChangedDelegate OnPageChanged;
		
		protected void DoPageChanged(bool absolute, int n) 
		{
			PageChangedEventArgs args = new PageChangedEventArgs(CurrentPage, absolute ? n : CurrentPage + n);
			if (OnPageChanged != null)
				OnPageChanged(this, args);
			CurrentPage = args.NewPageIndex;
		}
		
		protected void MoveFirstClick(object sender, ImageClickEventArgs e)
		{
			if (OnPageChanged != null && CurrentPage != 1)
				DoPageChanged(true, 1);
		}

		protected void MoveLastClick(object sender, ImageClickEventArgs e)
		{
			if (OnPageChanged != null && CurrentPage != PageCount)
				DoPageChanged(true, PageCount);
		}

		protected void MovePreviousClick(object sender, ImageClickEventArgs e)
		{
			if (OnPageChanged != null && CurrentPage > 1)
				DoPageChanged(false, -1);
		}

		protected void MoveNextClick(object sender, ImageClickEventArgs e)
		{
			if (OnPageChanged != null && CurrentPage < PageCount)
				DoPageChanged(false, 1);
		}

		protected void CurrentPageTextChanged(object sender, EventArgs e)
		{
			if (OnPageChanged != null)
				DoPageChanged(true, Convert.ToInt32((sender as TextBox).Text));
		}

	}
}
