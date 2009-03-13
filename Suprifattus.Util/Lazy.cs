using System;

namespace Suprifattus.Util
{
	public class Lazy<T>
		where T: class
	{
		public delegate T LazyCreationDelegate();

		private readonly LazyCreationDelegate del;
		private T value;

		public Lazy(LazyCreationDelegate del)
		{
			this.del = del;
		}

		public T Value
		{
			get { return value ?? (value = del()); }
		}

		public static implicit operator T(Lazy<T> lazy)
		{
			return lazy.Value;
		}
	}
}