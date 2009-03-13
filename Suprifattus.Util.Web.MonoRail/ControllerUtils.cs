using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

using Castle.MonoRail.ActiveRecordSupport.Pagination;
using Castle.MonoRail.Framework.Helpers;

using NHibernate.Expression;

using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Web.MonoRail
{
	public class ControllerUtils : AbstractHelper
	{
		private static readonly Regex rxNotNumbers = new Regex(@"\D", RegexOptions.Compiled);

		#region Paginate com ARPaginable
		public IPaginatedPage Paginate(Type targetType, string hql, params object[] parameters)
		{
			IARPaginableDataSource q = new ARPaginableSimpleQuery(targetType, hql, parameters);
			return Paginate(q);
		}

		public IPaginatedPage Paginate(Type type, Order order, params ICriterion[] criterions)
		{
			return Paginate(type, new[] { order }, criterions);
		}

		public IPaginatedPage Paginate(Type type, Order order1, Order order2, params ICriterion[] criterions)
		{
			return Paginate(type, new[] { order1, order2 }, criterions);
		}

		public IPaginatedPage Paginate(Type type, Order order1, Order order2, Order order3, params ICriterion[] criterions)
		{
			return Paginate(type, new[] { order1, order2, order3 }, criterions);
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

		public void RenderXml<T>(IEnumerable<T> lista, string rootXmlElement, Action<EasyXmlWriter, T> onItem)
		{
			RenderXml(lista, rootXmlElement, "", null, onItem);
		}

		public void RenderXml<T>(IEnumerable<T> lista, string rootXmlElement, string rootXmlNS, Action<EasyXmlWriter, T> onItem)
		{
			RenderXml(lista, rootXmlElement, rootXmlNS, null, onItem);
		}

		public void RenderXml<T>(IEnumerable<T> lista,
		                         string rootXmlElement, string rootXmlNS,
		                         Action<EasyXmlWriter> onFirstItem,
		                         Action<EasyXmlWriter, T> onItem)
		{
			Controller.CancelView();
			Controller.Response.ContentType = "text/xml";

			var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };

			using (XmlWriter w = XmlTextWriter.Create(Controller.Response.Output, settings))
			{
				if (w == null)
					throw new AppAssertionError("Erro", "Erro ao criar XmlWriter");

				var ew = new EasyXmlWriter(w);

				w.WriteStartElement(rootXmlElement, rootXmlNS);
				if (onFirstItem != null)
					onFirstItem(ew);
				if (lista != null)
					foreach (T obj in lista)
						onItem(ew, obj);
				w.WriteEndElement();
			}
		}
#endif
		#endregion
	}

	public class EasyXmlWriter
	{
		private readonly XmlWriter w;

		public EasyXmlWriter(XmlWriter w)
		{
			this.w = w;
		}

		public XmlWriter Writer
		{
			get { return w; }
		}

		public EasyXmlWriter EmptyElement(string localName)
		{
			w.WriteStartElement(localName);
			w.WriteEndElement();
			return this;
		}

		public EasyXmlWriter StartElement(string localName)
		{
			w.WriteStartElement(localName);
			return this;
		}

		public EasyXmlWriter Attribute(string localName, object value)
		{
			w.WriteAttributeString(localName, Convert.ToString(value));
			return this;
		}

		public EasyXmlWriter Value(object value)
		{
			w.WriteValue(value);
			return this;
		}

		public EasyXmlWriter EndElement()
		{
			w.WriteEndElement();
			return this;
		}
	}
}