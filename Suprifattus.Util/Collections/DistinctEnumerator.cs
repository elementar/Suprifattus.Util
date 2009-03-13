using System;
using System.Collections;

using Suprifattus.Util.Data.XBind;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Enumera coleções, considerando apenas elementos distintos.
	/// Elementos considerados iguais são retornados apenas uma vez
	/// pelo enumerador.
	/// </summary>
	/// <remarks>
	/// Dependendo do uso de um <see cref="IComparer"/>,
	/// é utilizado um <see cref="DistinctEnumerator"/>
	/// ou um <see cref="DistinctEnumeratorWithComparer"/>.
	/// </remarks>
	public class DistinctEnumerable : IEnumerable
	{
		private readonly IEnumerable source;
		private readonly string propertyName;
		private readonly IComparer comparer;

		public DistinctEnumerable(IEnumerable source, string propertyName)
		{
			this.source = source;
			this.propertyName = propertyName;
		}

		public DistinctEnumerable(IEnumerable source, string propertyName, IComparer comparer)
			: this(source, propertyName)
		{
			this.comparer = comparer;
		}

		public IEnumerator GetEnumerator()
		{
			if (comparer != null)
				return new DistinctEnumeratorWithComparer(source, propertyName, comparer);

			return new DistinctEnumerator(source, propertyName);
		}
	}

	public class DistinctEnumerator : EnumeratorBase
	{
		protected static readonly object first = new object();
		protected static readonly XBindContext ctx = new XBindContext();

		protected readonly string propertyName;

		protected object last = first;

		public DistinctEnumerator(IEnumerable source, string propertyName)
			: base(source.GetEnumerator())
		{
			this.propertyName = propertyName;
		}

		public override bool MoveNext()
		{
			if (!base.MoveNext())
				return false;

			if (last == first)
			{
				last = ctx.Resolve(base.Current, propertyName);
				return true;
			}
			
			object old = last;
			bool next = true;
			do
			{
				last = ctx.Resolve(base.Current, propertyName);
			} while (IsEqualToLast(old) && (next = base.MoveNext()));
			return next;
		}

		protected virtual bool IsEqualToLast(object old)
		{
			return
				(last == old) ||
				(last == null && old == null) ||
				(last != null && last.Equals(old)) ||
				(old != null && old.Equals(last));
		}
	}

	public class DistinctEnumeratorWithComparer : DistinctEnumerator
	{
		private readonly IComparer comparer;

		public DistinctEnumeratorWithComparer(IEnumerable source, string propertyName, IComparer comparer)
			: base(source, propertyName)
		{
			this.comparer = comparer;
		}

		protected override bool IsEqualToLast(object old)
		{
			return comparer.Compare(last, old) == 0;
		}
	}
}