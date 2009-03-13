using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	public class PagedEnumerable : IEnumerable
	{
		IEnumerable en;
		int pageSize;

		public PagedEnumerable(IEnumerable en, int pageSize)
		{
			this.en = en;
			this.pageSize = pageSize;
		}

		public IEnumerator GetEnumerator()
		{
			return new PagedEnumerator(en, pageSize);
		}
	}
	
	public class PagedEnumerator : IEnumerator
	{
		readonly int pageSize;
		readonly IEnumerable source;

		IEnumerator en;
		int pageIndex;
		PageEnumerable currentPage;

		public PagedEnumerator(IEnumerable en, int pageSize)
		{
			this.source = en;
			this.pageSize = pageSize;

			this.Reset();
		}

		public bool MoveNext()
		{
			// se não tem mais elementos, não tem mais grupos
			if (!en.MoveNext())
				return false;

			ArrayList page = new ArrayList(pageSize);
			int n = pageSize;
			do 
			{
				page.Add(en.Current);
			} while (--n > 0 && en.MoveNext());

			this.currentPage = new PageEnumerable(page, pageIndex++);
			return true;
		}

		public void Reset()
		{
			this.en = this.source.GetEnumerator();
			this.currentPage = null;
			this.pageIndex = 0;
		}

		public object Current
		{
			get { return currentPage; }
		}

		private class PageEnumerable : IEnumerable
		{
			IEnumerable contents;
			int pageIndex;

			public PageEnumerable(IEnumerable contents, int pageIndex)
			{
				this.contents = contents;
				this.pageIndex = pageIndex;
			}

			public IEnumerator GetEnumerator()
			{
				return new PageEnumerator(contents, pageIndex);
			}
		}
		
		private class PageEnumerator : EnumeratorBase
		{
			int pageIndex;

			public PageEnumerator(IEnumerable contents, int pageIndex)
				: base(contents.GetEnumerator())
			{
				this.pageIndex = pageIndex;
			}

			public int Number
			{
				get { return pageIndex; }
			}
		}
	}
}
