using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Internal;
using Castle.ActiveRecord.Queries;

using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail.Components
{
	public class ActiveRecordTests : BusinessRule
	{
		public QueryResults ExecuteQuery(string hql)
		{
			var q = new SimpleQuery(typeof(ActiveRecordBase), typeof(object), hql);
			return new QueryResults(q);
		}

		#region "QueryResults" Class
		public class QueryResults : IEnumerable
		{
			private readonly Array r;

			public QueryResults(IActiveRecordQuery q)
			{
				r = (Array) ActiveRecordMediator.ExecuteQuery(q);
			}

			public IEnumerable<string> Columns
			{
				get
				{
					if (r == null)
						yield break;

					Type t = r.GetType().GetElementType();
					if (t == typeof(object))
					{
						if (r.Length == 0)
							yield break;

						object first = r.GetValue(0);
						t = first.GetType();

						if (t.IsArray)
						{
							int i = 0;
							foreach (object col in (Array) first)
								yield return "Column " + (++i);
							yield break;
						}
					}

					ActiveRecordModel arModel = ActiveRecordModel.GetModel(t);
					if (arModel != null && arModel.PrimaryKey != null)
						yield return arModel.PrimaryKey.Property.Name;

					foreach (PropertyInfo pi in t.GetProperties())
					{
						bool nestedOK = false;
						if (arModel != null)
						{
							foreach (NestedModel nestedModel in arModel.Components)
								if (nestedModel.Property == pi)
								{
									nestedOK = true;
									foreach (PropertyModel nestedPropertyModel in nestedModel.Model.Properties)
										yield return pi.Name + "." + nestedPropertyModel.Property.Name;
								}
						}

						if (!nestedOK)
							yield return pi.Name;
					}
				}
			}

			public IEnumerator GetEnumerator()
			{
				foreach (object val in r)
					yield return new QueryTuple(this, val);
			}

			public class QueryTuple : IEnumerable
			{
				private readonly QueryResults queryResults;
				private readonly object value;

				public QueryTuple(QueryResults queryResults, object value)
				{
					this.queryResults = queryResults;
					this.value = value;
				}

				public IEnumerator GetEnumerator()
				{
					foreach (string column in queryResults.Columns)
					{
						object v;
						if (column.StartsWith("Column "))
						{
							int i = Convert.ToInt32(column.Substring(7));
							v = ((Array) value).GetValue(i - 1);
						}
						else
							v = DataBinder.Eval(value, column);
						yield return FormatValue(v);
					}
				}

				private object FormatValue(object v)
				{
					if (v == null)
						return "(null)";
					if (v.GetType().IsGenericType && typeof(IList<>).MakeGenericType(v.GetType().GetGenericArguments()[0]).IsAssignableFrom(v.GetType()))
						return String.Format("<em>IList&lt;{0}&gt;</em>", v.GetType().GetGenericArguments()[0].Name);
					if (v is IRecord)
						return String.Format("<a href='?hql=from+{0}+r+where+r.id+=+{1}'>{0}#{1}</a>", v.GetType().Name, ((IRecord) v).Id);
					
					return v;
				}
			}
		}
		#endregion
	}
}