using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Um conjunto implementado com um <see cref="ArrayList"/> por baixo.
	/// </summary>
	public class ListSet : ISet, IDisposable
	{
		private readonly ArrayList innerList = new ArrayList();

		#region Constructors
		public ListSet()
		{
		}

		public ListSet(params object[] vals)
		{
			AddRange(vals);
		}

		public ListSet(IEnumerable vals)
		{
			AddRange(vals);
		}
		#endregion

		public bool Add(object val)
		{
			bool add = !innerList.Contains(val);
			if (add)
				innerList.Add(val);
			return add;
		}

		public void Clear()
		{
			innerList.Clear();
		}

		public void AddRange(params object[] vals)
		{
			AddRange((IEnumerable) vals);
		}

		public void AddRange(IEnumerable vals)
		{
			foreach (object val in vals)
				Add(val);
		}

		public bool Contains(object val)
		{
			return innerList.Contains(val);
		}

		public int Count
		{
			get { return innerList.Count; }
		}

		public IEnumerator GetEnumerator()
		{
			return innerList.GetEnumerator();
		}

		void IDisposable.Dispose()
		{
			Clear();
		}

		#region ICollection implementation
		public void CopyTo(Array array, int index)
		{
			innerList.CopyTo(array, index);
		}

		object ICollection.SyncRoot
		{
			get { return innerList.SyncRoot; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}
		#endregion
	}
}