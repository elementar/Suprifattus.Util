using System;
using System.Collections;
using System.Collections.Generic;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Um conjunto implementado com um <see cref="List{T}"/> por baixo.
	/// </summary>
	public class ListSet<T> : ISet<T>, ISet, IDisposable
	{
		List<T> innerList = new List<T>();

		public bool Add(T val)
		{
			bool add = innerList.Contains(val);
			if (add)
				innerList.Add(val);
			return add;
		}

		public void AddRange(params T[] vals)
		{
			AddRange((IEnumerable<T>) vals);
		}

		public void AddRange(IEnumerable<T> vals)
		{
			foreach (T val in vals)
				Add(val);
		}

		public void Clear()
		{
			innerList.Clear();
		}

		public bool Contains(T item)
		{
			return innerList.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			innerList.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return innerList.Remove(item);
		}

		public int Count
		{
			get { return innerList.Count; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return innerList.GetEnumerator();
		}

		#region ICollection<T> implementation
		void ICollection<T>.Add(T item)
		{
			this.Add(item);
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}
		#endregion
		
		#region IEnumerator implementation
		IEnumerator IEnumerable.GetEnumerator()
		{
			return innerList.GetEnumerator();
		}
		#endregion

		void IDisposable.Dispose()
		{
			this.Clear();
		}

		#region ISet implementation
		bool ISet.Add(object val)
		{
			return this.Add((T) val);
		}

		void ISet.AddRange(params object[] vals)
		{
			foreach (T val in vals)
				this.Add(val);
		}

		void ISet.AddRange(IEnumerable vals)
		{
			foreach (T val in vals)
				this.Add(val);
		}

		bool ISet.Contains(object val)
		{
			return this.Contains((T) val);
		}
		#endregion

		#region ICollection implementation
		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyTo((T[]) array, index);
		}

		object ICollection.SyncRoot
		{
			get { return ((ICollection) innerList).SyncRoot; }
		}

		bool ICollection.IsSynchronized
		{
			get { return ((ICollection) innerList).IsSynchronized; }
		}
		#endregion
	}
}