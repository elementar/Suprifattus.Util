using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Collections
{
	public class SoftDictionary<T, V> : IDictionary<T, V>
		where V: struct
	{
		Dictionary<T, V> inner;

		public SoftDictionary()
			: this(new Dictionary<T, V>())
		{
		}

		public SoftDictionary(Dictionary<T, V> inner)
		{
			this.inner = inner;
		}

		public V this[T key]
		{
			get
			{
				if (!inner.ContainsKey(key))
					return default(V);
				else
					return inner[key];
			}
			set
			{
				if (default(V).Equals(value))
				{
					if (inner.ContainsKey(key))
						inner.Remove(key);
				}
				else if (!inner.ContainsKey(key))
					inner.Add(key, value);
				else
					inner[key] = value;
			}
		}

		#region Membros Delegados
		public void Add(T key, V value)
		{
			inner.Add(key, value);
		}

		public void Clear()
		{
			inner.Clear();
		}

		public bool ContainsKey(T key)
		{
			return inner.ContainsKey(key);
		}

		public bool ContainsValue(V value)
		{
			return inner.ContainsValue(value);
		}

		public IEnumerator<KeyValuePair<T, V>> GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			inner.GetObjectData(info, context);
		}

		public void OnDeserialization(object sender)
		{
			inner.OnDeserialization(sender);
		}

		public bool Remove(T key)
		{
			return inner.Remove(key);
		}

		public bool TryGetValue(T key, out V value)
		{
			return inner.TryGetValue(key, out value);
		}

		public IEqualityComparer<T> Comparer
		{
			get { return inner.Comparer; }
		}

		public int Count
		{
			get { return inner.Count; }
		}

		public ICollection<T> Keys
		{
			get { return inner.Keys; }
		}

		public ICollection<V> Values
		{
			get { return inner.Values; }
		}

		void ICollection<KeyValuePair<T, V>>.Add(KeyValuePair<T, V> item)
		{
			((ICollection<KeyValuePair<T, V>>) inner).Add(item);
		}

		void ICollection<KeyValuePair<T, V>>.Clear()
		{
			((ICollection<KeyValuePair<T, V>>) inner).Clear();
		}

		bool ICollection<KeyValuePair<T, V>>.Contains(KeyValuePair<T, V> item)
		{
			return ((ICollection<KeyValuePair<T, V>>) inner).Contains(item);
		}

		void ICollection<KeyValuePair<T, V>>.CopyTo(KeyValuePair<T, V>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<T, V>>) inner).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<T, V>>.Remove(KeyValuePair<T, V> item)
		{
			return ((ICollection<KeyValuePair<T, V>>) inner).Remove(item);
		}

		int ICollection<KeyValuePair<T, V>>.Count
		{
			get { return ((ICollection<KeyValuePair<T, V>>) inner).Count; }
		}

		bool ICollection<KeyValuePair<T, V>>.IsReadOnly
		{
			get { return ((ICollection<KeyValuePair<T, V>>) inner).IsReadOnly; }
		}
		#endregion
	}
}