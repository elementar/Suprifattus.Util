using System;

namespace Suprifattus.Util
{
	[CLSCompliant(false)]
	public delegate T WeakRefCreationDelegate<T>();

	[CLSCompliant(false)]
	public class WeakReference<T> : System.WeakReference
	{
		WeakRefCreationDelegate<T> del;

		public WeakReference(WeakRefCreationDelegate<T> createDelegate)
			: base(createDelegate())
		{
			this.del = createDelegate;
		}

		public new T Target
		{
			get { return (T) (base.Target == null ? base.Target = del() : base.Target); }
			set { base.Target = value; }
		}
	}
}
