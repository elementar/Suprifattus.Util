using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Classe copiada descaradamente dos builtins do Boo:
	/// svn://svn.boo.codehaus.org/boo/scm/boo/trunk/src/Boo.Lang/Builtins.cs
	/// </summary>
	public class ConcatEnumerator : IEnumerator, IEnumerable
	{
		private int _index;
		private readonly IEnumerable[] _enumerables;
		private IEnumerator _current;

		public ConcatEnumerator(params IEnumerable[] args)
		{
			_enumerables = args;
			Reset();
		}

		public void Reset()
		{
			_index = 0;
			_current = _enumerables[_index].GetEnumerator();
		}

		public bool MoveNext()
		{
			if (_current.MoveNext())
			{
				return true;
			}

			while (++_index < _enumerables.Length)
			{
				_current = _enumerables[_index].GetEnumerator();
				if (_current.MoveNext())
					return true;
			}
			return false;
		}

		public IEnumerator GetEnumerator()
		{
			return this;
		}

		public object Current
		{
			get { return _current.Current; }
		}
	}
}