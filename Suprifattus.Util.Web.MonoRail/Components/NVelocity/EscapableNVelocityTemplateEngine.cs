using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Web;

using Castle.Components.Common.TemplateEngine;
using Castle.Core.Logging;

using Commons.Collections;

using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace Suprifattus.Util.Web.MonoRail.Components.NVelocity
{
	public class EscapableNVelocityTemplateEngine : ITemplateEngine, ISupportInitialize
	{
		private VelocityEngine vengine;

		private String templateDir = ".";

		/// <summary>
		/// Constructs an EscapableNVelocityTemplateEngine instance
		/// assuming the default values.
		/// </summary>
		public EscapableNVelocityTemplateEngine()
		{
			this.Log = NullLogger.Instance;
			this.EnableCache = true;
		}

		/// <summary>
		/// Constructs a EscapableNVelocityTemplateEngine instance
		/// specifing the template directory.
		/// </summary>
		/// <param name="templateDir">The template directory</param>
		public EscapableNVelocityTemplateEngine(String templateDir)
		{
			this.Log = NullLogger.Instance;
			this.EnableCache = true;
			this.TemplateDir = templateDir;
		}

		/// <summary>
		/// Gets or sets the template directory
		/// </summary>
		public string TemplateDir
		{
			get { return templateDir; }
			set
			{
				if (vengine != null)
					throw new InvalidOperationException("Could not change the TemplateDir after Template Engine initialization.");

				templateDir = value;
			}
		}

		/// <summary>
		/// Enable/Disable caching. Default is <c>true</c>
		/// </summary>
		public bool EnableCache { get; set; }

		public ILogger Log { get; set; }

		/// <summary>
		/// Starts/configure NVelocity based on the properties.
		/// </summary>
		public void BeginInit()
		{
			vengine = new VelocityEngine();

			var props = new ExtendedProperties();

			var expandedTemplateDir = ExpandTemplateDir(templateDir);
			Log.InfoFormat("Initializing NVelocityTemplateEngine component using template directory: {0}", expandedTemplateDir);

			var propertiesFile = new FileInfo(Path.Combine(expandedTemplateDir, "nvelocity.properties"));
			if (propertiesFile.Exists)
			{
				Log.Info("Found 'nvelocity.properties' on template dir, loading as base configuration");
				using (Stream stream = propertiesFile.OpenRead())
					props.Load(stream);
			}

			props.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");
			props.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, expandedTemplateDir);
			props.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_CACHE, EnableCache ? "true" : "false");

			vengine.Init(props);
		}

		public void EndInit()
		{
		}

		/// <summary>
		/// Returns <c>true</c> only if the 
		/// specified template exists and can be used
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns></returns>
		public bool HasTemplate(String templateName)
		{
			if (vengine == null)
				throw new InvalidOperationException("Template Engine not yet initialized.");

			try
			{
				vengine.GetTemplate(templateName);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Process the template with data from the context.
		/// </summary>
		public bool Process(IDictionary context, String templateName, TextWriter output)
		{
			if (vengine == null)
				throw new InvalidOperationException("Template Engine not yet initialized.");

			var template = vengine.GetTemplate(templateName);
			var velocityContext = CreateContext(context);

			template.Merge(velocityContext, output);

			return true;
		}

		/// <summary>
		/// Process the input template with data from the context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="templateName">Name of the template.  Used only for information during logging</param>
		/// <param name="output">The output.</param>
		/// <param name="inputTemplate">The input template.</param>
		/// <returns></returns>
		public bool Process(IDictionary context, string templateName, TextWriter output, string inputTemplate)
		{
			return Process(context, templateName, output, new StringReader(inputTemplate));
		}

		/// <summary>
		/// Process the input template with data from the context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="templateName">Name of the template.  Used only for information during logging</param>
		/// <param name="output">The output.</param>
		/// <param name="inputTemplate">The input template.</param>
		/// <returns></returns>
		public bool Process(IDictionary context, string templateName, TextWriter output, TextReader inputTemplate)
		{
			if (vengine == null)
				throw new InvalidOperationException("Template Engine not yet initialized.");

			return vengine.Evaluate(CreateContext(context), output, templateName, inputTemplate);
		}

		private VelocityContext CreateContext(IDictionary context)
		{
			var velocityContext = new VelocityContext(new Hashtable(context));
			velocityContext.AttachEventCartridge(EscapeUtils.EscapableEventCartridge);

			return velocityContext;
		}

		private String ExpandTemplateDir(String templateDir)
		{
			Log.DebugFormat("Template directory before expansion: {0}", templateDir);

			// if nothing to expand, then exit
			if (templateDir == null)
				templateDir = String.Empty;

			// expand web application root
			if (templateDir.StartsWith("~/"))
			{
				var webContext = HttpContext.Current;
				if (webContext != null && webContext.Request != null)
					templateDir = webContext.Server.MapPath(templateDir);
			}

			// normalizes the path (including ".." notation, for parent directories)
			templateDir = new DirectoryInfo(templateDir).FullName;

			Log.Debug("Template directory after expansion: {0}", templateDir);
			return templateDir;
		}
	}
}