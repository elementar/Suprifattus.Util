using System;
using System.Web;
using System.Reflection;

using Suprifattus.Util.DesignPatterns;

namespace Suprifattus.Util.Web.Cache
{
	[AttributeUsage(AttributeTargets.Property)]
	[CLSCompliant(false)]
	public class ApplicationLazyInitAttribute : LazyInitAttribute 
	{
		string key;

		public ApplicationLazyInitAttribute(string key, Type type, params object[] pars) 
			: base(type, pars)
		{
			this.key = key;
		}

		protected override bool NeedInit(object obj, MemberInfo mi)
		{
			return base.NeedInit(obj, mi) || HttpContext.Current.Application[key] == null;
		}

		protected override void SetValue(object obj, MemberInfo mi, object val)
		{
			HttpContext.Current.Application[key] = val;
			base.SetValue(obj, mi, val);
		}
	}
	
	/// <summary>
	/// Atributo para inicialização preguiçosa.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[CLSCompliant(false)]
	public class LazyInitAttribute : Attribute
	{
		protected readonly Type t;
		protected readonly object[] pars;

		public LazyInitAttribute(Type type, params object[] pars)
		{
			this.t = type;
			this.pars = pars;
		}

		public static void Init(object obj) 
		{
			foreach (MemberInfo mi in obj.GetType().GetMembers())
				if (Attribute.IsDefined(mi, typeof(LazyInitAttribute))) 
				{
					LazyInitAttribute attr = (LazyInitAttribute) Attribute.GetCustomAttribute(mi, typeof(LazyInitAttribute));
					if (attr.NeedInit(obj, mi))
						attr.SetValue(obj, mi, attr.Create());
				}
		}

		protected virtual bool NeedInit(object obj, MemberInfo mi) 
		{
			if (mi is FieldInfo)
				return ((FieldInfo) mi).GetValue(obj) == null;
			else if (mi is PropertyInfo)
				return ((PropertyInfo) mi).GetValue(obj, null) == null;
			else
				throw new NotImplementedException();
		}
		
		protected virtual void SetValue(object obj, MemberInfo mi, object val) 
		{
			if (mi is FieldInfo)
				((FieldInfo) mi).SetValue(obj, val);
			else if (mi is PropertyInfo) 
			{
				if (((PropertyInfo) mi).CanWrite) ((PropertyInfo) mi).SetValue(obj, val, null);
			}
			else
				throw new NotImplementedException();
		}
			
		public virtual object Create() 
		{
			return ((IObjectFactory) Activator.CreateInstance(t, pars)).Create();
		}
	}
}
