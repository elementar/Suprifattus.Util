using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

using Castle.DynamicProxy;

namespace Suprifattus.Util.Web.MonoRail.Components
{
	/// <summary>
	/// Dynamic Caster.
	/// </summary>
	/// <remarks>
	/// Got from: http://www.ayende.com/Blog/2005/06/03/DynamicCasterIsDone.aspx
	/// </remarks>
	public sealed class DynamicCaster
	{
		private DynamicCaster() { }
		
#if GENERICS
		public static T Cast<T>(object source)
		{
			return (T) Cast(typeof(T), source);
		}
#endif
		
		public static object Cast(Type destinationType, object source)
		{
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");

			if (source == null)
				throw new ArgumentNullException("source");

			if (source.GetType().GetInterface(destinationType.Name) != null)
				return source;

			ProxyGenerator generator = new ProxyGenerator();
			return generator.CreateProxy(destinationType, new DelegatingInterceptor(source.GetType()), source);
		}

		internal class DelegatingInterceptor : StandardInterceptor
		{
			private readonly Type type;
			private readonly IDictionary methodCache = new HybridDictionary();

			public DelegatingInterceptor(Type type)
			{
				this.type = type;
			}

			protected override void PreProceed(IInvocation invocation, params object[] args)
			{
				if (!DoesMethodExists(invocation.Method))
					throw new NotImplementedException(String.Format("Type '{0}' does not implement method '{1}'!", type.FullName, invocation.Method.ToString()));
			}

			private bool DoesMethodExists(MethodInfo method)
			{
				if (methodCache.Contains(method))
					return (bool) methodCache[method];

				ParameterInfo[] parameters = method.GetParameters();
				Type[] types = new Type[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
					types[i] = parameters[i].ParameterType;

				bool methodExists = type.GetMethod(method.Name, types) != null;
				methodCache.Add(method, methodExists);
				return methodExists;
			}
		}
	}
}