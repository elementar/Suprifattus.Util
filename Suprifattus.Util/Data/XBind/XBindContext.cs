using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;

using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Reflection;

namespace Suprifattus.Util.Data.XBind
{
	public class XBindContext
	{
		private static readonly Regex rxSplit = new Regex(@"\s*[.]\s*", RegexOptions.Compiled);
		private static readonly Regex rxOr = new Regex(@"\s*[|]{2}\s*", RegexOptions.Compiled);

		[ThreadStatic] private static XBindContext ctx;

		public static XBindContext Current
		{
			get { return (ctx ?? (ctx = new XBindContext())); }
		}

		private readonly Hashtable ht = CollectionsUtil.CreateCaseInsensitiveHashtable();

		public object Resolve(string expression)
		{
			string[] s = rxSplit.Split(expression, 2);
			return Resolve(Get(s[0]), s[1]);
		}

		public object Resolve(object obj, string expression)
		{
			string[] s = rxSplit.Split(expression);

			object v = obj;
			foreach (string part in s)
				v = GetValue(obj, v, part, null);
			return v;
		}

		public void Update(string expression, object value)
		{
			string[] s = rxSplit.Split(expression, 2);
			Update(Get(s[0]), s[1], value);
		}

		public void Update(object obj, string expression, object value)
		{
			string[] s = rxSplit.Split(expression);

			object v = obj;
			foreach (string part in s)
				SetValue(v, part, null, value);
		}

		private static readonly ReflectionCache cache = new ReflectionCache();
		private const BindingFlags bf = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		private const MemberTypes mt = MemberTypes.Property | MemberTypes.Method | MemberTypes.Field;

		private object GetValue(object root, object obj, string prop, object[] parameters)
		{
			if (obj == null)
				return null;

			string[] or = rxOr.Split(prop);
			if (or.Length > 1)
				prop = or[0];

			MemberInfo mi = FindMemberInfo(obj, prop);

			if (mi == null)
				return null;

			object val = null;
			switch (mi.MemberType)
			{
				case MemberTypes.Property:
					val = ((PropertyInfo) mi).GetValue(obj, parameters);
					break;
				case MemberTypes.Field:
					val = ((FieldInfo) mi).GetValue(obj);
					break;
				case MemberTypes.Method:
					val = ((MethodInfo) mi).Invoke(obj, parameters);
					break;
			}

			if (val == null && or.Length > 1)
				val = Resolve(root, or[1]);

			return val;
		}

		private static MemberInfo FindMemberInfo(object obj, string prop)
		{
			Type t = obj.GetType();
			MemberInfo mi = cache.Get(t, prop);
			if (mi == null)
			{
				foreach (MemberInfo i in t.GetMembers(bf))
				{
					if ((mt & i.MemberType) != 0 && String.Compare(i.Name, prop, true) == 0)
					{
						mi = i;
						break;
					}
				}

				if (mi != null)
					cache.Set(t, prop, mi);
			}
			return mi;
		}

		private void SetValue(object obj, string prop, object[] parameters, object value)
		{
			if (obj == null)
				return;

			MemberInfo mi = FindMemberInfo(obj, prop);

			if (mi == null)
				return;

			switch (mi.MemberType)
			{
				case MemberTypes.Property:
					var pi = (PropertyInfo) mi;
					EnsureCompatibility(pi.PropertyType, ref value);
					pi.SetValue(obj, value, parameters);
					break;
				case MemberTypes.Field:
					var fi = (FieldInfo) mi;
					EnsureCompatibility(fi.FieldType, ref value);
					fi.SetValue(obj, value);
					break;
				default:
					throw new AppError("Erro Inesperado", "Tipo de membro indefinido: " + mi.MemberType);
			}
		}

		private void EnsureCompatibility(Type type, ref object value)
		{
			if (type.IsEnum && value is string)
				value = Enum.Parse(type, (string) value);
			if (type.IsEnum && !type.IsInstanceOfType(value))
				value = Convert.ChangeType(value, Enum.GetUnderlyingType(type));
			if ((type.IsPrimitive || type.IsEnum || type == typeof(String)) && !type.IsInstanceOfType(value))
				value = Convert.ChangeType(value, type);
		}

		#region IDictionary Delegating Members
		public void Add(object key, object value)
		{
			ht.Add(key, value);
		}

		public void Set(object key, object value)
		{
			ht[key] = value;
		}

		public object Get(object key)
		{
			return ht[key];
		}

		public void Clear()
		{
			ht.Clear();
		}

		public ICollection Keys
		{
			get { return ht.Keys; }
		}

		public ICollection Values
		{
			get { return ht.Values; }
		}

		public bool Contains(object key)
		{
			return ht.Contains(key);
		}

		public bool ContainsKey(object key)
		{
			return ht.ContainsKey(key);
		}

		public bool ContainsValue(object value)
		{
			return ht.ContainsValue(value);
		}

		public object this[object key]
		{
			get { return Get(key); }
			set { Set(key, value); }
		}
		#endregion
	}
}