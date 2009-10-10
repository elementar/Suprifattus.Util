using System;

namespace Suprifattus.Util
{
	public class Singleton<T>
		where T: class
	{
		private readonly Func<T> createDelegate;
		private T instance;

		public Singleton(Func<T> createDelegate)
		{
			this.createDelegate = createDelegate;
		}

		public T Instance
		{
			get
			{
				EnsureInitialized();

				return instance;
			}
		}

		public void EnsureInitialized()
		{
			if (instance == null)
				lock (this)
					if (instance == null)
						instance = createDelegate();
		}
	}
}