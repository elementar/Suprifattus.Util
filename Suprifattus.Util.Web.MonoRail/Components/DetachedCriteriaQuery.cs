using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Castle.ActiveRecord;
using Castle.MonoRail.ActiveRecordSupport.Pagination;

using NHibernate;
using NHibernate.Expression;
using NHibernate.Transform;

using Suprifattus.Util.DesignPatterns;

namespace Suprifattus.Util.Web.MonoRail.Components
{

	#region DetachedCriteriaQuery<ARType>
	public class DetachedCriteriaQuery<ARType> : DetachedCriteriaQuery<ARType, ARType>
		where ARType: class
	{
		public DetachedCriteriaQuery(DetachedCriteria criteria)
			: base(criteria)
		{
		}

		public DetachedCriteriaQuery(ObjectFactory<DetachedCriteria>.CreateDelegate detachedCriteriaCreateDelegate, params Order[] orders)
			: base(detachedCriteriaCreateDelegate, orders)
		{
		}

		public DetachedCriteriaQuery(Order[] orders, DetachedCriteria criteria)
			: base(orders, criteria)
		{
		}

		public DetachedCriteriaQuery(Order order, DetachedCriteria criteria)
			: base(order, criteria)
		{
		}

		public DetachedCriteriaQuery(DetachedCriteria criteria, params Order[] orders)
			: base(criteria, orders)
		{
		}
	}
	#endregion

	public class DetachedCriteriaQuery<ARType, T> : IActiveRecordQuery<T[]>, IARPaginableDataSource
		where ARType: class
	{
		private readonly ObjectFactory<DetachedCriteria>.CreateDelegate createDelegate;
		private readonly Order[] orders;

		#region Constructors
		public DetachedCriteriaQuery(ObjectFactory<DetachedCriteria>.CreateDelegate detachedCriteriaCreateDelegate, params Order[] orders)
		{
			this.createDelegate = detachedCriteriaCreateDelegate;
			this.orders = orders;
		}

		public DetachedCriteriaQuery(DetachedCriteria criteria, params Order[] orders)
		{
			this.createDelegate = (() => criteria);
			this.orders = orders;
		}

		public DetachedCriteriaQuery(DetachedCriteria criteria)
		{
			this.createDelegate = (() => criteria);
		}

		public DetachedCriteriaQuery(Order[] orders, DetachedCriteria criteria)
		{
			this.createDelegate = (() => criteria);
			this.orders = orders;
		}

		public DetachedCriteriaQuery(Order order, DetachedCriteria criteria)
		{
			this.createDelegate = (() => criteria);
			this.orders = new[] { order };
		}
		#endregion

		public T[] Execute()
		{
			return ActiveRecordMediator<ARType>.ExecuteQuery2(this);
		}

		#region IActiveRecordQuery
		object IActiveRecordQuery.Execute(ISession session)
		{
			return InternalExecute(session).ToArray();
		}

		IEnumerable IActiveRecordQuery.Enumerate(ISession session)
		{
			return InternalExecute(session);
		}
		#endregion

		#region IActiveRecordQuery<T[]>
		T[] IActiveRecordQuery<T[]>.Execute(ISession session)
		{
			return InternalExecute(session).ToArray();
		}
		#endregion

		#region IARPaginableDataSource (implicit)
		int IARPaginableDataSource.ObtainCount()
		{
			return this.Count();
		}

		IEnumerable IARPaginableDataSource.ListAll()
		{
			return this.ListAll();
		}

		IEnumerable IARPaginableDataSource.Paginate(int pageSize, int currentPage)
		{
			return this.Paginate(pageSize, currentPage);
		}
		#endregion

		public int Count()
		{
			NHibernateDelegate call =
				(session, instance) =>
					{
						var q = CriteriaTransformer.TransformToRowCount(this.BuildCriteria(session, false));
						return q.UniqueResult();
					};

			return Convert.ToInt32(ActiveRecordMediator.Execute(typeof(ARType), call, null));
		}

		public IEnumerable<T> ListAll()
		{
			return (IEnumerable<T>) ActiveRecordMediator<ARType>.Execute((s, i) => InternalExecute(s), null);
		}

		public IEnumerable<T> PaginatedListAll(int pageSize)
		{
			return PaginatedListAll(pageSize, 1);
		}

		public IEnumerable<T> PaginatedListAll(int pageSize, int startPage)
		{
			bool repeat;
			do
			{
				repeat = false;
				foreach (T obj in Paginate(pageSize, startPage++))
				{
					repeat = true;
					yield return obj;
				}
			} while (repeat);
		}

		public IEnumerable<T> Paginate(int pageSize, int currentPage)
		{
			NHibernateDelegate call =
				delegate(ISession session, object instance)
					{
						var q = this.BuildCriteria(session, true);
						q.SetFirstResult(pageSize * (currentPage - 1));
						q.SetMaxResults(pageSize);
						return q.List<T>();
					};

			return (IEnumerable<T>) ActiveRecordMediator<ARType>.Execute(call, null);
		}

		private IList<T> InternalExecute(ISession session)
		{
			var q = this.BuildCriteria(session, true);
			return q.List<T>();
		}

		public Type RootType
		{
			get { return typeof(ARType); }
		}

		public int? CommandTimeout { get; set; }

		private ICriteria BuildCriteria(ISession session, bool forList)
		{
			var c = createDelegate().GetExecutableCriteria(session);
			if (this.CommandTimeout.HasValue)
				c.SetTimeout(this.CommandTimeout.Value);
			if (forList)
			{
				if (orders != null)
					foreach (Order order in orders)
						c.AddOrder(order);
				if (typeof(T) != typeof(object[]) && typeof(T) != typeof(ARType))
					c.SetResultTransformer(Transformers.AliasToBean(typeof(T)));
			}

			return c;
		}
	}
}