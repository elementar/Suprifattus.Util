using System;

namespace Suprifattus.Util
{
	public class Lazy<T>
	{
		public delegate T LazyCreationDelegate();

		private LazyCreationDelegate del;
		private T value;

		public Lazy(LazyCreationDelegate del)
		{
			this.del = del;
		}

		public T Value
		{
			get { return value != null ? value : (value = del()); }
		}
		
		public static implicit operator T(Lazy<T> lazy)
		{
			return lazy.Value;
		}
	}
}
