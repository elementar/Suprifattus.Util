using System;
using System.Collections;

namespace Suprifattus.Util.Web.Controls
{
	public class GridPluginEnumerator : IEnumerator, IEnumerable
	{
		Type pluginType;
		IEnumerator baseEnum;

		public GridPluginEnumerator(GridPluginCollection col, Type pluginType)
		{
			this.pluginType = pluginType;
			this.baseEnum = col.GetEnumerator();
		}

		public bool MoveNext()
		{
			while (baseEnum.MoveNext())
				if (pluginType.IsAssignableFrom(baseEnum.Current.GetType()))
					return true;

			return false;
		}

		public void Reset()
		{
			baseEnum.Reset();
		}

		public object Current
		{
			get { return baseEnum.Current; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
	}
}