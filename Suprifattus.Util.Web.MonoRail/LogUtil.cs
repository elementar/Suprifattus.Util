using System;
using System.Diagnostics;

using Castle.Core.Logging;
using Castle.Services.Logging.Log4netIntegration;
using Castle.Windsor;

using log4net;
using log4net.Config;

namespace Suprifattus.Util.Web.MonoRail
{
	/// <summary>
	/// Classe utilit�ria para obter <see cref="ILogger"/>s.
	/// Desenvolvida para funcionar em aplica��es que herdam de
	/// <see cref="BaseMonoRailApplication"/> e utilizam
	/// <see cref="IWindsorContainer"/>.
	/// </summary>
	public class LogUtil
	{
		private static ILoggerFactory defaultFactory;

		[Obsolete("Isolando a configura��o em log4net.config, n�o � mais necess�rio chamar Reconfigure()")]
		public static void Reconfigure()
		{
			XmlConfigurator.Configure();
		}

		/// <summary>
		/// Define uma propriedade no <c>LogicalThreadContext</c>
		/// do <c>log4net</c>.
		/// </summary>
		public static void SetLoggingProperty(string name, object value)
		{
			LogicalThreadContext.Properties[name] = value;
		}

		/// <summary>
		/// Obt�m um <see cref="ILogger"/> para o m�todo em execu��o no momento.
		/// </summary>
		public static ILogger GetLogger()
		{
			return GetLogger(1);
		}

		/// <summary>
		/// Obt�m um <see cref="ILogger"/> para o m�todo em execu��o 
		/// <paramref name="framesToSkip"/> chamadas abaixo.
		/// </summary>
		/// <param name="framesToSkip">O n�mero de chamadas abaixo</param>
		public static ILogger GetLogger(int framesToSkip)
		{
			Type t = new StackTrace(framesToSkip + 1).GetFrame(0).GetMethod().ReflectedType;
			return GetLogger(t);
		}

		/// <summary>
		/// Obt�m um <see cref="ILogger"/> para a classe especificada.
		/// </summary>
		/// <param name="logClass">A classe</param>
		public static ILogger GetLogger(Type logClass)
		{
			ILoggerFactory f = GetLoggerFactory();
			return f.Create(logClass);
		}

		/// <summary>
		/// Obt�m um <see cref="ILogger"/> com o nome especificado.
		/// </summary>
		/// <param name="name">O nome</param>
		public static ILogger GetLogger(string name)
		{
			var f = GetLoggerFactory();
			return f.Create(name);
		}

		private static ILoggerFactory GetLoggerFactory()
		{
			var c = GetWindsorContainer();
			if (c != null)
				return (ILoggerFactory) c.Resolve(typeof(ILoggerFactory));

			return defaultFactory ?? (defaultFactory = new Log4netFactory());
		}

		private static IWindsorContainer GetWindsorContainer()
		{
			return BaseMonoRailApplication.CurrentWindsorContainer;
		}
	}
}