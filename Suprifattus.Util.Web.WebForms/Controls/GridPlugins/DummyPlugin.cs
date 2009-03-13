using System;
using System.Diagnostics;

namespace Suprifattus.Util.Web.Controls.GridPlugins
{
	public sealed class DummyPlugin : IGridPlugin
	{
		Grid grid;

		public DummyPlugin()
		{
			Debug.WriteLine("Dummy Grid Plugin Created!");
		}

		public Grid ActiveGrid
		{
			get { return grid; }
			set { grid = value; }
		}
	}
}