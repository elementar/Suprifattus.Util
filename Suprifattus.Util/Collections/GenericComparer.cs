using System;
using System.Collections.Generic;

namespace Suprifattus.Util.Collections
{
	public sealed class GenericComparer<T> : IComparer<T>
	{
		private readonly Comparison<T> comparsion;

		public GenericComparer(Comparison<T> comparsion)
		{
			this.comparsion = comparsion;
		}

		public int Compare(T x, T y)
		{
			return comparsion(x, y);
		}
	}
}