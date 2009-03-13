using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

using Castle.ActiveRecord.Queries;
using Castle.MonoRail.ActiveRecordSupport.Pagination;
using Castle.MonoRail.Framework.Helpers;

using NHibernate.Expression;

namespace Suprifattus.Util.Web.MonoRail
{
	public class ControllerUtils : AbstractHelper
	{
		static Regex rxNotNumbers = new Regex(@"\D", RegexOptions.Compiled);

		#region Paginate com ARPaginable
		public IPaginatedPage Paginate(Type targetType, string hql, params object[] parameters)
		{
			IARPaginableDataSource q = new ARPaginableSimpleQuery(targetType, hql, parameters);
			return Paginate(q);
		}
		
		public IPaginatedPage Paginate(Type type, Order order, params ICriterion[] criterions)
		{
			return Paginate(type, new Order[] {order}, criterions);
		}
		
		public IPaginatedPage Paginate(Type type, Order order1, Order order2, params ICriterion[] criterions)
		{
			return Paginate(type, new Order[] {order1, order2}, criterions);
		}
		
		public IPaginatedPage Paginate(Type type, Order order1, Order order2, Order order3, params ICriterion[] criterions)
		{
			return Paginate(type, new Order[] {order1, order2, order3}, criterions);
		}
		
		public IPaginatedPage Paginate(Type type, Order[] orders, params ICriterion[] criterions)
		{
			IARPaginableDataSource q = new ARPaginableCriteria(type, orders, criterions);
			return Paginate(q);
		}
		
		public IPaginatedPage Paginate(IARPaginableDataSource q)
		{
			int pageSize = ((CustomBaseController) Controller).Config.PageSize;
			return Paginate(q, pageSize);
		}

		public IPaginatedPage Paginate(IARPaginableDataSource q, int pageSize)
		{
			return ARPaginationHelper.CreatePagination(pageSize, q);
		}
		#endregion

		#region Paginate com IList
		/// <summary>
		/// Faz a paginação da lista especificada.
		/// </summary>
		public IPaginatedPage Paginate(IList source)
		{
			int pageSize = ((CustomBaseController) Controller).Config.PageSize;
			return Paginate(source, pageSize);
		}

		/// <summary>
		/// Faz a paginação da lista especificada, com o tamanho de página especificado.
		/// </summary>
		/// <remarks>
		/// Para utilizar o tamanho de página padrão, chamar
		/// o overload sem o parâmetro <paramref name="pageSize" />:
		/// <see cref="Paginate(IList)"/>.
		/// </remarks>
		public IPaginatedPage Paginate(IList source, int pageSize)
		{
			return PaginationHelper.CreatePagination(this.Controller, source, pageSize);
		}
		#endregion

		public string RemoveNonNumeric(string data)
		{
			if (data == null)
				return data;
			return rxNotNumbers.Replace(data, "");
		}

		public string MakeHqlLikeExpression(string s)
		{
			if (Logic.StringEmpty(s))
				return null;
			return s.Replace(" ", "% ") + "%";
		}

		public ICriterion MakeLikeExpression(string property, string s)
		{
			s = MakeHqlLikeExpression(s);
			if (s == null)
				return null;
			return Expression.Like(property, s);
		}
		
		public object CreateComponent(string componentName)
		{
			return BaseMonoRailApplication.CurrentWindsorContainer.Resolve(componentName);
		}
		
		public object CreateComponent(Type serviceType)
		{
			return BaseMonoRailApplication.CurrentWindsorContainer.Resolve(serviceType);
		}
		
#if GENERICS
		public T CreateComponent<T>()
		{
			return (T) CreateComponent(typeof(T));
		}

		public T CreateComponent<T>(string componentName)
		{
			return (T) CreateComponent(componentName);
		}
#endif

		#region MakeXml
#if GENERICS
		public delegate void MakeXmlFirstItemDelegate(XmlWriter w);
		public delegate void MakeXmlItemDelegate<T>(XmlWriter w, T obj);

		public void RenderXml<T>(IEnumerable<T> lista, string rootXmlElement, MakeXmlItemDelegate<T> onItem)
		{
			RenderXml(lista, rootXmlElement, "", null, onItem);
		}

		public void RenderXml<T>(IEnumerable<T> lista, string rootXmlElement, string rootXmlNS, MakeXmlItemDelegate<T> onItem)
		{
			RenderXml(lista, rootXmlElement, rootXmlNS, null, onItem);
		}

		public void RenderXml<T>(IEnumerable<T> lista, 
		                         string rootXmlElement, string rootXmlNS, 
		                         MakeXmlFirstItemDelegate onFirstItem,
		                         MakeXmlItemDelegate<T> onItem)
		{
			Controller.CancelView();
			Controller.Response.ContentType = "text/xml";
			
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			
			using (XmlWriter w = XmlTextWriter.Create(Controller.Response.Output, settings))
			{
				w.WriteStartElement(rootXmlElement, rootXmlNS);
				if (onFirstItem != null)
					onFirstItem(w);
				if (lista != null)
					foreach (T obj in lista)
						onItem(w, obj);
				w.WriteEndElement();
			}
		}
#endif
		#endregion
	}
}
