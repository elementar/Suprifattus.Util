using System;

namespace Suprifattus.Util.Web.Controls.GridPlugins
{
	public interface IGridPlugin
	{
		Grid ActiveGrid { get; set; }
	}

	public interface IColumnStyleGridPlugin : IGridPlugin
	{
		void ApplyColumnStyles();
	}

	public interface IDataFormatterPlugin : IGridPlugin
	{
		object Format(GridColumn col, object value);
	}
}
