using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using Suprifattus.Util.Reflection;
using Suprifattus.Util.Text;
using Suprifattus.Util.Web.Controls.Helpers;

namespace Suprifattus.Util.Web.Controls.Data
{
	public class DataBinderEx
	{
		public static string Eval(DataRowView row, string field)
		{
			if (row == null)
				return null;
			object val = row[field];
			if (val is DBNull)
				return null;
			else
				return Convert.ToString(val);
		}

		public static string Eval(DataRowView row, string field, string format)
		{
			if (row == null)
				return null;
			object val = row[field];
			if (val is DBNull)
				return null;
			else
				return String.Format(PluggableFormatProvider.Instance, format, val);
		}

		public static string Eval(DataRow row, string field)
		{
			if (row == null)
				return null;
			object val = row[field];
			if (val is DBNull)
				return null;
			else
				return Convert.ToString(val);
		}

		public static string Eval(DataRow row, string field, string format)
		{
			if (row == null)
				return null;
			object val = row[field];
			if (val is DBNull)
				return null;
			else
				return String.Format(PluggableFormatProvider.Instance, format, val);
		}

		public static object Eval(object dataItem, string expression)
		{
			if (dataItem is DataRowView)
				return Eval((DataRowView) dataItem, expression);
			if (dataItem is DataRow)
				return Eval((DataRow) dataItem, expression);
			
			return DataBinder.Eval(dataItem, expression);
		}

		public static object Eval(object dataItem, string expression, string format)
		{
			if (dataItem is DataRowView)
				return Eval((DataRowView) dataItem, expression, format);
			if (dataItem is DataRow)
				return Eval((DataRow) dataItem, expression, format);
			
			return String.Format(PluggableFormatProvider.Instance, format, DataBinder.Eval(dataItem, expression));
		}

		/// <summary>
		/// Faz o DataBind dos valores dos controles de acordo com o ID dos mesmos.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="dataItem"></param>
		public static void BindControlsByID(Control parent, object dataItem)
		{
			Condition cond = 
				new TypeCondition(typeof(TextBox)) |
				new TypeCondition(typeof(ListControl)) |
				new TypeCondition(typeof(CheckBox));

			foreach (Control ctl in ObjectQuery.SelectRecursive(parent.Controls, "Controls", cond)) 
			{
				if (!Logic.StringEmpty(ctl.ID))
					if (ControlUtil.IsEditable(ctl))
						BindControl(ctl, dataItem, ctl.ID.Substring(3));
			}
		}

		public static void BindControl(Control ctl, object dataItem, string expression)
		{
			try 
			{
				ControlUtil.SetValue(ctl, Eval(dataItem, expression));
			}
			catch
			{
			}
		}
	}
}
