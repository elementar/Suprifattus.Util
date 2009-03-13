using System;
using System.Collections;

using Suprifattus.Util.Web.Controls.GridPlugins;

namespace Suprifattus.Util.Web.Controls
{
	/// <summary>
	/// Plugins.
	/// </summary>
	public class GridPluginCollection : CollectionBase
	{
		public GridPluginCollection()
		{
		}

		public IGridPlugin this[int index] 
		{
			get { return (IGridPlugin) InnerList[index]; }
			set { InnerList[index] = value; }
		}

		public int Add(IGridPlugin col)
		{
			return InnerList.Add(col);
		}

		public GridPluginEnumerator Select(Type pluginType)
		{
			return new GridPluginEnumerator(this, pluginType);
		}

		protected override void OnValidate(object value)
		{
			if (!(value is IGridPlugin))
				throw new ArgumentException("Value must be an IGridPlugin");
		}

	}
}
