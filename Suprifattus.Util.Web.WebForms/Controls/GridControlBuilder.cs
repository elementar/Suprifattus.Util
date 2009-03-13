using System;
using System.Collections;
using System.Web.UI;

using Suprifattus.Util.Web.Controls.GridPlugins;

namespace Suprifattus.Util.Web.Controls
{
	public class GridControlBuilder : ControlBuilder
	{
		public override Type GetChildControlType(string tagName, IDictionary attribs)
		{
			if (tagName == "Column")
				return typeof(GridColumn);
			
			if (tagName == "Plugin")
				return typeof(GridPluginDeclaration);

			return null;
		}
	}
}
