using System;

namespace Suprifattus.Util.Web.MonoRail.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class WindsorComponentAttribute : Attribute
	{
		private readonly string componentKey;
		private readonly Type[] implementedServices;

		public WindsorComponentAttribute(string componentKey)
		{
			ValidateComponentKey(componentKey);
			this.componentKey = componentKey;
		}

		public WindsorComponentAttribute(params Type[] implementedServices)
		{
			ValidateMultipleServices(implementedServices);
			this.implementedServices = implementedServices;
		}

		public WindsorComponentAttribute(string componentKey, Type implementedService)
		{
			ValidateComponentKey(componentKey);
			ValidateSingleService(implementedService);
			this.componentKey = componentKey;
			this.implementedServices = new Type[] { implementedService };
		}

		public string ComponentKey
		{
			get { return componentKey; }
		}

		public Type[] ImplementedServices
		{
			get { return implementedServices; }
		}

		private static void ValidateComponentKey(string componentKey)
		{
			if (String.IsNullOrEmpty(componentKey))
				throw new ArgumentException("O nome do componente, se especificado, n�o pode ser vazio.");
		}

		private static void ValidateMultipleServices(Type[] implementedServices)
		{
			if (implementedServices == null || implementedServices.Length == 0)
				throw new ArgumentException("Deve ser especificado pelo menos um servi�o implementado.");
		}

		private static void ValidateSingleService(Type implementedService)
		{
			if (implementedService == null)
				throw new ArgumentNullException("Se especificado, o servi�o implementado n�o pode ser nulo.");
		}
	}
}