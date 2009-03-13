using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Implementação de um conjunto com uma <see cref="Hashtable"/> por baixo.
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
		/// <returns>Verdadeiro se o item não existia, falso se já existia</returns>
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
		/// Verifica se um item está contido no conjunto.
		/// </summary>
		/// <param name="val">O item a ser verificado</param>
		/// <returns>Verdadeiro se o item se encontra no conjunto, falso caso contrário</returns>
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
		/// Obtém uma cópia deste conjunto.
		/// </summary>
		public HashSet Clone()
		{
			return new HashSet((ICollection) this);
		}

		/// <summary>
		/// Obtém uma cópia deste conjunto.
		/// </summary>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#region ICollection Members

		/// <summary>
		/// Copia o conteúdo deste <see cref="ISet"/> para um vetor.
		/// </summary>
		/// <param name="array">O vetor</param>
		/// <param name="index">A posição inicial do vetor "<paramref name="array"/>" onde a cópia deve ser feita</param>
		public void CopyTo(Array array, int index)
		{
			ht.Keys.CopyTo(array, index);
		}

		/// <summary>
		/// Identifica se o <see cref="ISet"/> em questão é sincronizado ou não.
		/// </summary>
		/// <value>Verdadeiro se o <see cref="ISet"/> é sincronizado, falso caso contrário</value>
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
