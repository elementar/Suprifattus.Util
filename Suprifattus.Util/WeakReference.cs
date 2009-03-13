using System;

namespace Suprifattus.Util
{
	[CLSCompliant(false)]
	public delegate T WeakRefCreationDelegate<T>();

	[CLSCompliant(false)]
	public class WeakReference<T> : WeakReference
	{
		private readonly WeakRefCreationDelegate<T> del;

		public WeakReference(WeakRefCreationDelegate<T> createDelegate)
			: base(createDelegate())
		{
			this.del = createDelegate;
		}

		public new T Target
		{
			get { return (T) (base.Target ?? (base.Target = del())); }
			set { base.Target = value; }
		}
	}
}