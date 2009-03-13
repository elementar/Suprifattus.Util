using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

using Castle.Core.Logging;
using Castle.Core.Resource;
using Castle.MonoRail.Framework;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Web.MonoRail.Attributes;
using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail
{
	public class BaseMonoRailContainer : WindsorContainer
	{
		protected readonly ILogger log;

		protected List<Assembly> assembliesToInspect = new List<Assembly>();
		private readonly Regex rxViewComponentName = new Regex(@"^(\w)(\w*?)(View)?Component$");

		public BaseMonoRailContainer(HttpApplication app, params Assembly[] assembliesToInspect)
			: base(new XmlInterpreter(new ConfigResource()))
		{
			log = Resolve<ILoggerFactory>().Create(GetType());
			log.Debug("Inicializando...");

			// inspeciona o assembly final, se for subclasse
			if (this.GetType() != typeof(BaseMonoRailContainer))
				this.assembliesToInspect.Add(this.GetType().Assembly);

			this.assembliesToInspect.AddRange(assembliesToInspect);

			Initialize(app);

			log.Info("Inicialização do container concluída");
		}

		private void Initialize(HttpApplication app)
		{
			Kernel.AddComponentInstance("this", typeof(IWindsorContainer), this);

			if (app != null)
				Kernel.AddComponentInstance("web.app", typeof(HttpApplication), app);

			Initialize();
		}

		protected virtual void Initialize()
		{
			RegisterBusinessRules();
			RegisterComponents();
			RegisterControllers();
			RegisterViewComponents();
			RegisterFilters();

			RegisterOther();
		}

		/// <summary>
		/// Registra todas as classes que implementam a interface <see cref="IFilter"/>.
		/// </summary>
		protected virtual void RegisterFilters()
		{
			foreach (Assembly assembly in assembliesToInspect)
				foreach (Type t in assembly.GetExportedTypes())
					if (typeof(IFilter).IsAssignableFrom(t) && !t.IsAbstract)
						AddComponent("filter:" + t.FullName, t);
		}

		/// <summary>
		/// Registra todas as classes derivadas de <see cref="Controller"/>.
		/// </summary>
		protected virtual void RegisterControllers()
		{
			foreach (Assembly assembly in assembliesToInspect)
				foreach (Type t in assembly.GetExportedTypes())
					if (typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract)
						AddComponent("controller:" + t.FullName, t);
		}

		/// <summary>
		/// Registra todas as classes marcadas com [<see cref="WindsorComponentAttribute"/>],
		/// que não derivem de <see cref="BusinessRule"/>.
		/// </summary>
		protected virtual void RegisterComponents()
		{
			Type rn = typeof(BusinessRule);
			foreach (Assembly assembly in assembliesToInspect)
				foreach (Type t in assembly.GetExportedTypes())
				{
					if (t.IsSubclassOf(rn) || t.IsAbstract)
						continue;
					AutoRegister(t, false);
				}
		}

		/// <summary>
		/// Registra todas as classes que derivam de <see cref="BusinessRule"/>,
		/// marcadas ou não.
		/// </summary>
		protected virtual void RegisterBusinessRules()
		{
			Type rn = typeof(BusinessRule);
			foreach (Assembly assembly in assembliesToInspect)
			{
				foreach (Type t in assembly.GetExportedTypes())
				{
					if (!t.IsSubclassOf(rn) || t.IsAbstract)
						continue;

					AutoRegister(t, true);
				}
			}
		}

		/// <summary>
		/// Registra todas as classes que derivam de <see cref="ViewComponent"/>.
		/// </summary>
		protected virtual void RegisterViewComponents()
		{
			Type viewComponentType = typeof(ViewComponent);
			foreach (Assembly assembly in assembliesToInspect)
			{
				foreach (Type t in assembly.GetTypes())
					if (t.IsSubclassOf(viewComponentType) && !t.IsAbstract)
						RegisterViewComponent(t);
			}
		}

		/// <summary>
		/// Método modelo, pode ser sobrescrito nas classes derivadas para registrar
		/// outro tipo de objeto.
		/// </summary>
		protected virtual void RegisterOther()
		{
		}

		/// <summary>
		/// Tenta registrar um componente automaticamente, através do atributo
		/// <see cref="WindsorComponentAttribute"/>.
		/// </summary>
		/// <param name="componentType">O tipo do componente</param>
		/// <param name="registerEvenIfNotTagged">Se verdadeiro, registra o componente mesmo que o mesmo não esteja marcado com o atributo <see cref="WindsorComponentAttribute"/>.</param>
		/// <returns>Verdadeiro se o componente foi registrado através do atributo, falso caso contrário</returns>
		protected bool AutoRegister(Type componentType, bool registerEvenIfNotTagged)
		{
			bool registeredWithAttribute = false;
			foreach (WindsorComponentAttribute attr in componentType.GetCustomAttributes(typeof(WindsorComponentAttribute), false))
			{
				if (attr.ComponentKey != null && (attr.ImplementedServices == null || attr.ImplementedServices.Length == 0))
					AddComponent(attr.ComponentKey, componentType);
				else if (attr.ComponentKey != null && attr.ImplementedServices.Length == 1)
					AddComponent(attr.ComponentKey, attr.ImplementedServices[0], componentType);
				else
				{
					foreach (Type serviceType in attr.ImplementedServices)
						AddComponent(String.Format("component:{0}:{1}", componentType.FullName, serviceType.FullName), serviceType, componentType);
				}

				registeredWithAttribute = true;
			}

			if (!registeredWithAttribute && registerEvenIfNotTagged)
				AddComponent("component:" + componentType.FullName, componentType);

			return registeredWithAttribute;
		}

		/// <summary>
		/// Registra um <see cref="ViewComponent"/>, dando o nome adequado
		/// para a chave de registro.
		/// </summary>
		/// <param name="t">O tipo do <see cref="ViewComponent"/>.</param>
		/// <remarks>
		/// Não é verificado se o tipo passado por parâmetro realmente deriva de <see cref="ViewComponent"/>,
		/// apenas é verificada sua nomenclatura, que deve terminar em <c>ViewComponent</c> ou apenas <c>Component</c>.
		/// </remarks>
		protected void RegisterViewComponent(Type t)
		{
			Match m = rxViewComponentName.Match(t.Name);
			if (!m.Success)
				throw new AppError("Componente visual incorreto", "Componente visual com nome incorreto: " + t.Name + ". Certifique-se que o nome da classe do componente termina com \"ViewComponent\".");
			AddComponent(m.Groups[1].Value.ToLower() + m.Groups[2].Value, t);
		}

		public override void Dispose()
		{
			log.Debug("Finalizando...");
			base.Dispose();
		}
	}
}