using System;

namespace Suprifattus.Util
{
	public class Singleton<T>
		where T: class
	{
		public delegate T SingletonCreateDelegate();

		private readonly SingletonCreateDelegate createDelegate;
		private T instance;

		public Singleton(SingletonCreateDelegate createDelegate)
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