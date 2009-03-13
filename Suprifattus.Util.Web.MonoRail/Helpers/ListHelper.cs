using System;
using System.Collections;

using Castle.ActiveRecord;
using Castle.MonoRail.Framework.Helpers;

using Iesi.Collections;

using NHibernate;

using Suprifattus.Util.Collections;
using Suprifattus.Util.Data.XBind;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	/// <summary>
	/// Helper com métodos auxiliares para lidar com listas e enumerações.
	/// </summary>
	public class ListHelper : AbstractHelper
	{
#if GENERICS
		#region Filter e FilterScalar
		public IEnumerable Filter(object collection, string filterString)
		{
			NHibernateDelegate call =
				delegate(ISession session, object obj)
					{
						IQuery q = session.CreateFilter(collection, filterString);
						return q.List();
					};

			return (IEnumerable) ActiveRecordMediator.Execute(typeof(ActiveRecordBase), call, null);
		}

		public object FilterScalar(object collection, string filterString)
		{
			foreach (object r in Filter(collection, filterString))
				return r;

			return null;
		}
		#endregion
#endif

		#region Distinct
		public IEnumerable Distinct(IEnumerable source, string propertyName)
		{
			return new DistinctEnumerable(source, propertyName);
		}

		public IEnumerable Distinct(IEnumerable source)
		{
			HashedSet s = new HashedSet();
			foreach (object item in source)
				s.Add(item);
			return s;
		}
		#endregion

		#region Concat
		public IEnumerable Concat(IEnumerable source1, IEnumerable source2)
		{
			return new ConcatEnumerator(source1, source2);
		}

		public IEnumerable Concat(params IEnumerable[] sources)
		{
			return new ConcatEnumerator(sources);
		}
		#endregion

		#region Reverse
		public IEnumerable Reverse(IEnumerable en)
		{
			return new ReverseEnumerable(en);
		}
		#endregion

		#region Page
		public IEnumerable Page(IEnumerable source, int pageSize)
		{
			return new PagedEnumerable(source, pageSize);
		}
		#endregion

		#region Columns
		public IEnumerable Columns(IList list, int columns)
		{
			return new ColumnEnumerable(list, columns);
		}
		#endregion

		#region GroupBy
		public IEnumerable GroupBy(IEnumerable source, string propertyName)
		{
			if (source == null)
				return null;
			return new GroupByEnumerable(source, propertyName);
		}

		public IEnumerable GroupBy(IEnumerable source, string propertyName, int maxPerGroup)
		{
			if (source == null)
				return null;
			return new GroupByEnumerable(source, propertyName, maxPerGroup);
		}
		#endregion

		#region Sum
		public decimal Sum(IEnumerable en, string propertyName)
		{
			decimal sum = 0;

			XBindContext xb = new XBindContext();
			foreach (object obj in en)
			{
				object val = xb.Resolve(obj, propertyName);
				if (val != null)
					sum += Convert.ToDecimal(val);
			}

			return sum;
		}
		#endregion

		#region Range
		public IEnumerable Range(int begin, int end, int step)
		{
			return new RangeEnumerator(begin, end, step);
		}

		public IEnumerable Range(int begin, int end)
		{
			return new RangeEnumerator(begin, end, begin <= end ? 1 : -1);
		}

		public IEnumerable Range(int length)
		{
			return new RangeEnumerator(0, length, 1);
		}
		#endregion
	}
}