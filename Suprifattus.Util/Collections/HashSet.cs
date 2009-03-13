using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Implementa��o de um conjunto com uma <see cref="Hashtable"/> por baixo.
	/// </summary>
	[Serializable]
	public class HashSet : ISet, IDisposable, ICloneable
	{
		private Hashtable ht;
		private static readonly object dummy = new object();

		/// <summary>
		/// Cria um novo conjunto com uma <see cref="Hashtable"/> por baixo, com
		/// a capacidade especificada.
		/// </summary>
		/// <param name="capacity">A capacidade inicial do conjunto.</param>
		public HashSet(int capacity)
		{
			ht = new Hashtable(capacity);
		}

		/// <summary>
		/// Cria um novo conjunto com uma <see cref="Hashtable"/> por baixo.
		/// </summary>
		public HashSet() 
		{
			ht = new Hashtable();
		}

		/// <summary>
		/// Cria um novo conjunto com uma <see cref="Hashtable"/> por baixo,
		/// contendo os elementos especificados em <paramref name="initial"/>.
		/// </summary>
		/// <param name="initial">Os elementos iniciais do <see cref="ISet"/></param>
		public HashSet(ICollection initial)
			: this(initial.Count)
		{
			foreach (object item in initial)
				Add(item);
		}

		public HashSet(params object[] initial)
			: this((ICollection) initial) { }

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
		public bool Add(object val)
		{
			int oldCount = ht.Count;
			ht[val] = dummy;
			return ht.Count > oldCount;
		}

		/// <summary>
		/// Adiciona diversos itens no conjunto.
		/// </summary>
		/// <param name="vals">Os diversos itens a serem adicionados</param>
		public void AddRange(params object[] vals)
		{
			this.AddRange((IEnumerable) vals);
		}

		/// <summary>
		/// Adiciona diversos itens no conjunto.
		/// </summary>
		/// <param name="vals">Os diversos itens a serem adicionados</param>
		public void AddRange(IEnumerable vals)
		{
			foreach (object val in vals)
				Add(val);
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
		public bool Contains(object val)
		{
			return ht[val] != null;
		}

		/// <summary>
		/// Retorna um Enumerator para este <see cref="ISet"/>
		/// </summary>
		/// <returns>Um Enumerator para este <see cref="ISet"/></returns>
    public IEnumerator GetEnumerator()
    {
      return ht.Keys.GetEnumerator();
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
		public HashSet Clone()
		{
			return new HashSet((ICollection) this);
		}

		/// <summary>
		/// Obt�m uma c�pia deste conjunto.
		/// </summary>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#region ICollection Members

		/// <summary>
		/// Copia o conte�do deste <see cref="ISet"/> para um vetor.
		/// </summary>
		/// <param name="array">O vetor</param>
		/// <param name="index">A posi��o inicial do vetor "<paramref name="array"/>" onde a c�pia deve ser feita</param>
		public void CopyTo(Array array, int index)
		{
			ht.Keys.CopyTo(array, index);
		}

		/// <summary>
		/// Identifica se o <see cref="ISet"/> em quest�o � sincronizado ou n�o.
		/// </summary>
		/// <value>Verdadeiro se o <see cref="ISet"/> � sincronizado, falso caso contr�rio</value>
		public bool IsSynchronized
		{
			get { return ht.IsSynchronized; }
		}

		/// <summary>
		/// O objeto que pode ser utilizado para sincronizar o acesso ao <see cref="ISet"/>.
		/// </summary>
		public object SyncRoot
		{
			get { return ht.Keys.SyncRoot; }
		}

		#endregion
	}
}
