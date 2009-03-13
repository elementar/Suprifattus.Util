using System;

using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail.Components
{
	public sealed class NoOpObjectFormatter : IObjectFormatter
	{
		public static readonly IObjectFormatter Instance = new NoOpObjectFormatter();

		private NoOpObjectFormatter()
		{
		}

		public object Format(object arg)
		{
			return arg;
		}
	}

	public class ObjectFormatter : IObjectFormatter
	{
		public delegate object ObjectFormatterDelegate(object arg);

		private readonly ObjectFormatterDelegate del;

		public ObjectFormatter(ObjectFormatterDelegate del)
		{
			this.del = del;
		}

		public object Format(object arg)
		{
			return del(arg);
		}
	}

#if GENERICS
	public class ObjectFormatter<T> : IObjectFormatter
	{
		public delegate object ObjectFormatterDelegate(T arg);

		private readonly ObjectFormatterDelegate del;

		public ObjectFormatter(ObjectFormatterDelegate del)
		{
			this.del = del;
		}

		public object Format(object arg)
		{
			return del((T) arg);
		}
	}
#endif
}