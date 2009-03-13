using System;

namespace Suprifattus.Util.Web.Controls.GridPlugins
{
	public class GridPluginDeclaration
	{
		string pluginType;

		public string Type
		{
			get { return pluginType; }
			set { pluginType = value; }
		}

		public IGridPlugin Create(Grid grid)
		{
			IGridPlugin plugin = (IGridPlugin) Activator.CreateInstance(System.Type.GetType(Type, true, false));
			plugin.ActiveGrid = grid;
			return plugin;
		}
	}
}
