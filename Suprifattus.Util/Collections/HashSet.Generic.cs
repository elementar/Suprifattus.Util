using System;
using System.Collections;
using System.Collections.Generic;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Implementa��o de um conjunto com uma <see cref="Hashtable"/> por baixo.
	/// </summary>
	/// <typeparam name="T">O tipo que ser� armazenado neste <see cref="ISet"/></typeparam>
	[CLSCompliant(false)]
	[Serializable]
	public class HashSet<T> : ISet<T>, ISet, IDisposable, ICloneable
	{
		private Dictionary<T, object> ht;
		private static readonly object dummy = new object();

		/// <summary>
		/// Cria um novo conjunto com uma <see cref="Hashtable"/> por baixo, com
		/// a capacidade especificada.
		/// </summary>
		/// <param name="capacity">A capacidade inicial do conjunto.</param>
		public HashSet(int capacity)
		{
			ht = new Dictionary<T, object>(capacity);
		}

		/// <summary>
		/// Cria um novo conjunto com uma <see cref="Hashtable"/> por baixo.
		/// </summary>
		public HashSet()
		{
			ht = new Dictionary<T, object>();
		}

		/// <summary>
		/// Cria um novo conjunto com uma <see cref="Hashtable"/> por baixo,
		/// contendo os elementos especificados em <paramref name="initial"/>.
		/// </summary>
		/// <param name="initial">Os elementos iniciais do <see cref="ISet"/></param>
		public HashSet(params T[] initial)
			: this(initial.Length)
		{
			foreach (T item in initial)
				Add(item);
		}

		/// <summary>
		/// Cria um novo conjunto com uma <see cref="Hashtable"/> por baixo,
		/// contendo os elementos especificados em <paramref name="initial"/>.
		/// </summary>
		/// <param name="initial">Os elementos iniciais do <see cref="ISet"/></param>
		public HashSet(ICollection initial)
			: this(initial.Count)
		{
			foreach (T item in initial)
				Add(item);
		}

		/// <summary>
		/// Cria um novo conjunto com uma <see cref="Hashtable"/> por baixo,
		/// contendo os elementos especificados em <paramref name="initial"/>.
		/// </summary>
		/// <param name="initial">Os elementos iniciais do <see cref="ISet"/></param>
		public HashSet(ICollection<T> initial)
			: this(initial.Count)
		{
			foreach (T item in initial)
				Add(item);
		}

		public HashSet(params object[] initial)
			: this((ICollection) initial)
		{
		}
		
		/// <summary>
		/// A quantidade de itens do conjunto.
		/// </summary>
		public int Count
		{
			get { return ht.Keys.Count; }
		}

		/// <summary>
		/// Adiciona um item no conjunto.
		/// </summary>
		/// <param name="val">O valor a ser adicionado</param>
		/// <returns>Verdadeiro se o item n�o existia, falso se j� existia</returns>
		public bool Add(T val)
		{
			int oldCount = ht.Count;
			ht[val] = dummy;
			return ht.Count > oldCount;
		}

		/// <summary>
		/// Adiciona diversos itens no conjunto.
		/// </summary>
		/// <param name="vals">Os diversos itens a serem adicionados</param>
		public void AddRange(params T[] vals)
		{
			foreach (T val in vals)
				Add(val);
		}

		/// <summary>
		/// Adiciona diversos itens no conjunto.
		/// </summary>
		/// <param name="vals">Os diversos itens a serem adicionados</param>
		public void AddRange(IEnumerable<T> vals)
		{
			foreach (T val in vals)
				Add(val);
		}

		public bool Remove(T value)
		{
			return ht.Remove(value);
		}

		/// <summary>
		/// Limpa o conjunto.
		/// </summary>
		public void Clear()
		{
			ht.Clear();
		}

		/// <summary>
		/// Verifica se um item est� contido no conjunto.
		/// </summary>
		/// <param name="val">O item a ser verificado</param>
		/// <returns>Verdadeiro se o item se encontra no conjunto, falso caso contr�rio</returns>
		public bool Contains(T val)
		{
			return ht.ContainsKey(val);
		}

		/// <summary>
		/// Retorna um Enumerator para este <see cref="ISet"/>
		/// </summary>
		/// <returns>Um Enumerator para este <see cref="ISet"/></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return ht.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Libera os recursos utilizados pelo conjunto.
		/// </summary>
		public void Dispose()
		{
			ht.Clear();
			ht = null;
		}
		
		/// <summary>
		/// Obt�m uma c�pia deste conjunto.
		/// </summary>
		public HashSet<T> Clone()
		{
			return new HashSet<T>((ICollection<T>) this);
		}

		/// <summary>
		/// Obt�m uma c�pia deste conjunto.
		/// </summary>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#region ISet members
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
		
		#region ICollection Members
		/// <summary>
		/// Copia o conte�do deste <see cref="ISet"/> para um vetor.
		/// </summary>
		/// <param name="array">O vetor</param>
		/// <param name="index">A posi��o inicial do vetor "<paramref name="array"/>" onde a c�pia deve ser feita</param>
		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection) ht.Keys).CopyTo(array, index);
		}

		void ICollection<T>.Add(T value)
		{
			Add(value);
		}

		/// <summary>
		/// Copia o conte�do deste <see cref="ISet"/> para um vetor.
		/// </summary>
		/// <param name="array">O vetor</param>
		/// <param name="index">A posi��o inicial do vetor "<paramref name="array"/>" onde a c�pia deve ser feita</param>
		public void CopyTo(T[] array, int index)
		{
			ht.Keys.CopyTo(array, index);
		}

		/// <summary>
		/// Identifica se o <see cref="ISet"/> em quest�o � sincronizado ou n�o.
		/// </summary>
		/// <value>Verdadeiro se o <see cref="ISet"/> � sincronizado, falso caso contr�rio</value>
		public bool IsSynchronized
		{
			get { return ((IDictionary) ht).IsSynchronized; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// O objeto que pode ser utilizado para sincronizar o acesso ao <see cref="ISet"/>.
		/// </summary>
		public object SyncRoot
		{
			get { return ((ICollection) ht.Keys).SyncRoot; }
		}
		#endregion
	}
}