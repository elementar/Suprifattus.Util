using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Um enumerador de enumeradores, contendo todas as propriedades
	/// e métodos como virtuais.
	/// Simplesmente delega a responsabilidade de enumerar ao enumerador.
	/// Útil para utilizar como base para outros enumeradores.
	/// </summary>
	public class EnumeratorBase : IEnumerator, IEnumerable
	{
		private readonly IEnumerator en;

		public EnumeratorBase(IEnumerator en)
		{
			this.en = en;
		}

		public virtual object Current
		{
			get { return en.Current; }
		}

		public virtual bool MoveNext()
		{
			return en.MoveNext();
		}

		public virtual void Reset()
		{
			en.Reset();
		}

		public IEnumerator GetEnumerator()
		{
			return this;
		}
	}
}