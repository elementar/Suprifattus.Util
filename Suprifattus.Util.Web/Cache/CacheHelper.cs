using System;
using System.Web.Caching;
using System.Xml;

namespace Suprifattus.Util.Web.Cache
{
	public class CacheHelper : HttpContextBound
	{
		string prefix;
		TimeSpan slidingExpiration;

		public CacheHelper(string prefix, TimeSpan slidingExpiration)
		{
			this.prefix = prefix;
			this.slidingExpiration = slidingExpiration;
		}

		public CacheHelper(Type clazz, TimeSpan slidingExpiration)
			: this(clazz.FullName, slidingExpiration) { }

		public XmlDocument GetXmlDocument(string id)
		{
			return (XmlDocument) Page.Cache.Get(prefix + "." + id);
		}

		public void SetXmlDocument(string id, XmlDocument doc)
		{
			SetXmlDocument(id, doc, null);
		}

		public void SetXmlDocument(string id, XmlDocument doc, CacheDependency deps)
		{
			Page.Cache.Insert(prefix + "." + id, doc, deps, DateTime.MaxValue, slidingExpiration, CacheItemPriority.Low, null);
		}
	}
}
