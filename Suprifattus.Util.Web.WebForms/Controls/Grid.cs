using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

using Suprifattus.Util.Collections;
using Suprifattus.Util.Text;
using Suprifattus.Util.Web.Controls.Data;
using Suprifattus.Util.Web.Controls.GridPlugins;

namespace Suprifattus.Util.Web.Controls
{
	public enum GridEditingMode
	{
		Disabled,
		PostBack,
		Ajax,
		Custom
	}

	public delegate void GridRowEditingEventHandler(object sender, GridRowEditingEventArgs e);

	public class GridRowEditingEventArgs : EventArgs
	{
		readonly string primaryKeyValue;

		public GridRowEditingEventArgs(string pkVal)
		{
			this.primaryKeyValue = pkVal;
		}

		#region Public Properties
		public string PrimaryKeyValue
		{
			get { return primaryKeyValue; }
		}
		#endregion
	}
	
	[ParseChildren(false)]
	[ControlBuilder(typeof(GridControlBuilder))]
	public class Grid : UserControl, IPostBackEventHandler
	{
		IEnumerable dataSource;
		string primaryKey;
		string activeInactiveCol;
		bool allowPaging, checkboxes = true;
		bool allowFilter = false;
		bool autoGenerateColumns = false;
		bool scroll = false;
		int pageSize = DefaultPageSize, currentPage = 1;
		Unit width = Unit.Percentage(100);
		GridColumnCollection cols = new GridColumnCollection();
		GridPluginCollection plugins = new GridPluginCollection();
		GridEditingMode editMode;

		protected TextBox txtFilter;
		protected Button btnFilter;

		#region Events
		public event GridRowEditingEventHandler RowEditing;
		#endregion
		
		#region Appearance and Behavior
		[DefaultValue(true)]
		public bool Checkboxes 
		{
			get { return checkboxes; }
			set { checkboxes = value; }
		}

		[Obsolete("Use EditMode property instead")]
		[DefaultValue(true)]
		public bool AllowEditing
		{
			get { return editMode != GridEditingMode.Disabled; }
			set { editMode = (value ? GridEditingMode.Custom : GridEditingMode.Disabled); }
		}

		[DefaultValue(GridEditingMode.Disabled)]
		public GridEditingMode EditMode
		{
			get { return editMode; }
			set { editMode = value; }
		}

		[DefaultValue(null)]
		public string ActiveInactiveColumn
		{
			get { return activeInactiveCol; }
			set { activeInactiveCol = value; }
		}

		[DefaultValue(false)]
		public bool Scroll
		{
			get { return scroll; }
			set { scroll = value; }
		}

		public Unit Width
		{
			get { return width; }
			set { width = value; }
		}
		#endregion

		#region Data Source and Columns
		public IEnumerable DataSource 
		{
			get { return dataSource; }
			set { dataSource = value; }
		}

		public bool AutoGenerateColumns
		{
			get{return autoGenerateColumns;}
			set{autoGenerateColumns = value;}
		}

		[CLSCompliant(false)]
		public GridColumnCollection Columns 
		{
			get { return cols; }
			set { cols = value; }
		}

		public string PrimaryKey 
		{
			get { return primaryKey; }
			set { primaryKey = value; }
		}
		#endregion

		#region Paging
		[DefaultValue(false)]
		public bool AllowPaging 
		{
			get { return allowPaging; }
			set { allowPaging = value; }
		}

		[DefaultValue(20)]
		public int PageSize 
		{
			get { return pageSize; }
			set { pageSize = value; }
		}

		[DefaultValue(1)]
		public int CurrentPage 
		{
			get { return currentPage; }
			set { currentPage = value; }
		}
		#endregion

		#region Filtering
		public bool AllowFilter
		{
			get { return allowFilter; }
			set { allowFilter = value; }
		}

		public string DataFilter
		{
			get { return (txtFilter == null ? null : txtFilter.Text); }
			set { txtFilter.Text = value; }
		}
		#endregion

		#region ViewState Management
		protected override object SaveViewState()
		{
			ViewState["cols"] = cols;
			ViewState["currentPage"] = currentPage;
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			cols = (GridColumnCollection) ViewState["cols"];
			currentPage = ViewState["currentPage"] == null ? 1 : (int) ViewState["currentPage"];
		}
		#endregion

		#region Default Values
		public static int DefaultPageSize 
		{
			get 
			{ 
				string defPgSize = ConfigurationSettings.AppSettings["Suprifattus.Util.Web.Controls.Grid.DefaultPageSize"];
				return (defPgSize != null && defPgSize.Length > 0 ? Convert.ToInt32(defPgSize) : 20);
			}
		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			if (btnFilter != null)
				btnFilter.Click += new EventHandler(btnFilter_Click);
		}

		protected override void AddParsedSubObject(object obj)
		{
			if (obj is GridColumn) 
			{
				GridColumn col = (GridColumn) obj;
				col.ColumnIndex = cols.Count;
				cols.Add(col);
			}
			else if (obj is GridPluginDeclaration)
				plugins.Add(((GridPluginDeclaration) obj).Create(this));
			else
				base.AddParsedSubObject(obj);
		}

		#region Rendering Helpers
		protected virtual string GetEditReference(RepeaterItem Container)
		{
			string pkVal = DataBinder.Eval(Container.Parent.Parent, "DataItem." + PrimaryKey, "{0}");

			switch (EditMode) 
			{
				case GridEditingMode.Custom:
					return "JsDataBind.onEditClick(this, event)";
				case GridEditingMode.PostBack:
					return Page.GetPostBackEventReference(this, pkVal);
				case GridEditingMode.Ajax:
					return String.Format("JsDataBind.onAjaxEditClick(this, event, '{0}?ajaxEdit={1}')", Request.ServerVariables["SCRIPT_NAME"], pkVal);
				default:
					return null;
			}
		}

		protected virtual string GetRowClickReference(RepeaterItem Container)
		{
			StringCollection r = new StringCollection();
			if (Checkboxes)
				r.Add("JsDataBind.onCheckClick(this, event)");

			return CollectionUtils.Join(r, ";");
		}

		protected virtual string GetCSSClassForRow(RepeaterItem item)
		{
			string css = null;

			if (!Logic.StringEmpty(activeInactiveCol))
			{
				bool val = Convert.ToBoolean(DataBinderEx.Eval(item.DataItem, activeInactiveCol));
				css += " " + (val ? "active" : "inactive");
			}

			return css;
		}

		protected virtual object GetValue(object viewRepeaterItem, RepeaterItem columnRepeaterItem)
		{
			RepeaterItem ri = (RepeaterItem) viewRepeaterItem;
			GridColumn col = (GridColumn) columnRepeaterItem.DataItem;
			
			return GetValue(ri.DataItem, col);
		}

		protected virtual object GetValue(object obj, GridColumn col)
		{
			object val = DataBinderEx.Eval(obj, col.DataField);

			foreach (IDataFormatterPlugin plugin in plugins.Select(typeof(IDataFormatterPlugin)))
				val = plugin.Format(col, val);

			if (col.DataFormatString != null)
				val = String.Format(PluggableFormatProvider.Instance, col.DataFormatString, val);

			if (!col.AllowHtml)
				val = Server.HtmlEncode(Convert.ToString(val, CultureInfo.InvariantCulture));

			return val;
		}
		#endregion

		protected virtual void OnRowEditing(GridRowEditingEventArgs e)
		{
			if (RowEditing != null)
				RowEditing(this, e);
		}
		
		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);

			if (allowPaging && dataSource != null && !(dataSource is PagedDataSource))
			{
				PagedDataSource pds = new PagedDataSource();
				pds.DataSource = this.dataSource;
				pds.AllowPaging = allowPaging;
				pds.PageSize = pageSize;

				this.dataSource = pds;
			}

			if (autoGenerateColumns && Columns.Count == 0) 
				RegenerateColumns();

			foreach (IColumnStyleGridPlugin plugin in plugins.Select(typeof(IColumnStyleGridPlugin)))
				plugin.ApplyColumnStyles();
		}

		public void RegenerateColumns()
		{
			object ds = dataSource;
			if (ds is PagedDataSource)
				ds = ((PagedDataSource) dataSource).DataSource;

			DataTable dt;
			if (ds is DataView)
				dt = ((DataView) ds).Table;
			else if (ds is DataTable)
				dt = ((DataTable) ds);
			else if (ds == null)
				throw new ArgumentNullException("dataSource", "DataSource can't be null.");
			else
				throw new ArgumentException("DataSource type not supported: " + ds.GetType().FullName);
	
			Columns.Clear();
			foreach (DataColumn col in dt.Columns) 
			{
				if (this.PrimaryKey != col.ColumnName)
				{
					GridColumn gcol = new GridColumn();
					gcol.SourceDataColumn = col;
					gcol.ColumnIndex = this.Columns.Count;
					gcol.DataField = col.ColumnName;
					gcol.HeaderText = col.ColumnName;
					this.Columns.Add(gcol);
				}
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}

		public void RaisePostBackEvent(string eventArgument)
		{
			OnRowEditing(new GridRowEditingEventArgs(eventArgument));
		}

		#region Events
		private void btnFilter_Click(object sender, EventArgs e)
		{
			WebUtil.RegisterStartupFocus(txtFilter);
		}
		#endregion
	}
}
