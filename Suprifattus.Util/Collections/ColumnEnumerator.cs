using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	public class ColumnEnumerable : IEnumerable
	{
		private readonly IList list;
		private readonly int columnSize;

		public ColumnEnumerable(IList list, int columnSize)
		{
			this.list = list;
			this.columnSize = columnSize;
		}

		public IEnumerator GetEnumerator()
		{
			return new ColumnEnumerator(list, columnSize);
		}
	}
	
	public class ColumnEnumerator : IEnumerator
	{
		private readonly IList list;
		private readonly int columnSize;
		private int currentIndex = -1;
		private object current;

		public ColumnEnumerator(IList list, int columnSize) 
		{
			this.list = list;
			this.columnSize = columnSize;
		}

		public bool MoveNext()
		{
			bool hasNext = ++currentIndex < list.Count;
			
			if (hasNext)
				current = list[(list.Count / columnSize) * (currentIndex % columnSize) + (int) Math.Floor((float) currentIndex / columnSize)];
			
			return hasNext;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		public object Current
		{
			get { return current; }
		}
	}
}
