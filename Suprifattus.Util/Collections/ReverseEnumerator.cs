using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	public class ReverseEnumerable : IEnumerable
	{
		IEnumerable source;

		public ReverseEnumerable(IEnumerable source)
		{
			this.source = source;
		}

		public IEnumerator GetEnumerator()
		{
			return new ReverseEnumerator(source);
		}
	}

	public class ReverseEnumerator : EnumeratorBase
	{
		public ReverseEnumerator(IEnumerable en)
			: base(Reverse(en))
		{
		}

		private static IEnumerator Reverse(IEnumerable en)
		{
			ArrayList al = new ArrayList();
			foreach (object item in en)
				al.Add(item);
			al.Reverse();
			return al.GetEnumerator();
		}
	}
}