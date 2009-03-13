using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
	public class ReflectionCache
	{
		bool caseSensitive;
		Hashtable ht = new Hashtable();

		public ReflectionCache(bool caseSensitive)
		{
			this.caseSensitive = caseSensitive;
		}

		public ReflectionCache()
			: this(true) { }

		public void Set(Type type, string memberName, MemberInfo mi)
		{
			IDictionary l = ht[type] as IDictionary;
			if (l == null)
				ht[type] = l = CreateHashTable();
			l[memberName] = mi;
		}

		private Hashtable CreateHashTable()
		{
			return caseSensitive
				? new Hashtable() 
				: CollectionsUtil.CreateCaseInsensitiveHashtable();
		}

		public IDictionary Get(Type type)
		{
			return ht[type] as IDictionary;
		}

		public MemberInfo Get(Type type, string memberName)
		{
			IDictionary td = Get(type);
			if (td == null)
				return null;
			return (MemberInfo) td[memberName];
		}

		public IDictionary this[Type key]
		{
			get { return Get(key); }
		}

		public void Clear()
		{
			ht.Clear();
		}
	}
}
