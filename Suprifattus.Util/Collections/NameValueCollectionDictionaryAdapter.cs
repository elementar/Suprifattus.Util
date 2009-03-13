using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace Suprifattus.Util.Collections
{
	public class DictionaryWrapper : HybridDictionary
	{
		/// <summary>
		/// Recebe o <see cref="NameValueCollection"/> em <paramref name="nvc"/>, 
		/// lê todos os seus itens e adiciona no <see cref="HybridDictionary"/> interno.
		/// </summary>
		/// <param name="nvc">O <see cref="NameValueCollection"/></param>
		/// <seealso cref="NameValueCollectionDictionaryAdapter"/>
		public DictionaryWrapper(NameValueCollection nvc)
		{
			foreach (string key in nvc.Keys)
				if (key != null)
					Add(key, nvc[key]);
		}
	}

	public class NameValueCollectionDictionaryAdapter : IDictionary
	{
		private readonly NameValueCollection nvc;

		/// <summary>
		/// Armazenao o <see cref="NameValueCollection"/> em <paramref name="nvc"/>,
		/// delegando a implementação de <see cref="IDictionary"/> para os membros
		/// equivalentes em <see cref="NameValueCollection"/>.
		/// </summary>
		/// <param name="nvc">O <see cref="NameValueCollection"/></param>
		/// <seealso cref="DictionaryWrapper"/>
		public NameValueCollectionDictionaryAdapter(NameValueCollection nvc)
		{
			this.nvc = nvc;
		}

		public bool Contains(object key)
		{
			return nvc[key.ToString()] != null;
		}

		public void Add(object key, object value)
		{
			nvc.Add(key.ToString(), value.ToString());
		}

		public void Clear()
		{
			nvc.Clear();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new DictionaryEnumerator(nvc);
		}

		private class DictionaryEnumerator : EnumeratorBase, IDictionaryEnumerator
		{
			public DictionaryEnumerator(NameValueCollection nvc)
				: base(Enumerate(nvc))
			{
			}

			private static IEnumerator Enumerate(NameValueCollection nvc)
			{
				foreach (string key in nvc.Keys)
					if (key != null)
						yield return new DictionaryEntry(key, nvc[key]);
			}

			public object Key
			{
				get { return Entry.Key; }
			}

			public object Value
			{
				get { return Entry.Value; }
			}

			public DictionaryEntry Entry
			{
				get { return (DictionaryEntry) Current; }
			}
		}

		public void Remove(object key)
		{
			nvc.Remove(key.ToString());
		}

		public object this[object key]
		{
			get { return nvc[key.ToString()]; }
			set { nvc[key.ToString()] = Convert.ToString(value); }
		}

		public ICollection Keys
		{
			get { return nvc.AllKeys; }
		}

		public ICollection Values
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { return (bool) typeof(NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(nvc, null); }
		}

		public bool IsFixedSize
		{
			get { return IsReadOnly; }
		}

		public void CopyTo(Array array, int index)
		{
			nvc.CopyTo(array, index);
		}

		public int Count
		{
			get { return nvc.Count; }
		}

		public object SyncRoot
		{
			get { return ((ICollection) nvc).SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return ((ICollection) nvc).IsSynchronized; }
		}

		public IEnumerator GetEnumerator()
		{
			return nvc.GetEnumerator();
		}
	}
}