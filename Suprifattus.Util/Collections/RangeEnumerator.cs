using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Classe copiada descaradamente dos builtins do Boo:
	/// svn://svn.boo.codehaus.org/boo/scm/boo/trunk/src/Boo.Lang/Builtins.cs
	/// </summary>
	public class RangeEnumerator : IEnumerator, IEnumerable
	{
		private int _index;
		private readonly int _begin;
		private readonly int _end;
		private readonly int _step;

		public RangeEnumerator(int begin, int end, int step)
		{
			if (step > 0)
			{
				_end = begin + (step * (int) Math.Ceiling(Math.Abs(end - begin) / ((float) step)));
			}
			else
			{
				_end = begin + (step * (int) Math.Ceiling(Math.Abs(begin - end) / ((float) Math.Abs(step))));
			}

			_end -= step;
			_begin = begin - step;
			_step = step;
			_index = _begin;
		}

		public void Reset()
		{
			_index = _begin;
		}

		public bool MoveNext()
		{
			if (_index != _end)
			{
				_index += _step;
				return true;
			}
			return false;
		}

		public object Current
		{
			get { return _index; }
		}

		public IEnumerator GetEnumerator()
		{
			return this;
		}
	}
}