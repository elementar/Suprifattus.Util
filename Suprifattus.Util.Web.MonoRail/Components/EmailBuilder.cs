using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;

using Castle.Components.Common.EmailSender;
using Castle.Core;

using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Text;
using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail.Components
{
	/// <summary>
	/// Componente construtor de emails.
	/// </summary>
	/// <remarks>
	/// Pode ser utilizado como serviço, mas o ideal é utilizá-lo como
	/// classe básica para criar um serviço, permitindo, assim, isolar
	/// as propriedades de configuração de forma mais fácil.
	/// </remarks>
	public class EmailBuilder : BusinessRuleWithLogging, IInitializable
	{
		private static readonly Regex
			rxEmailSeparator = new Regex(@"\s*;\s*", RegexOptions.Compiled),
			rxSimpleEmail = new Regex(@"^[\w._%+-]+@\w+(\.\w+)+$", RegexOptions.Compiled),
			rxEmailWithName = new Regex(@"^.* <[\w._%+-]+@[\w+-]+(\.[\w+-]+)+>$", RegexOptions.Compiled);

		private readonly string from;
		private string to;
		private string[] cc;
		private string baseUrl;
		private string xsltHtml4 = "~/res/xsl/html-email.xsl";

		private XslCompiledTransform xslHtmlEmail;
		
		public EmailBuilder(string from)
		{
			this.from = from;
		}

		public string To
		{
			get { return to; }
			set { to = value; }
		}

		public string From
		{
			get { return from; }
		}

		public string[] Cc
		{
			get { return cc; }
			set { cc = value; }
		}

		public string BaseUrl
		{
			get { return baseUrl; }
			set { baseUrl = value; }
		}

		public string XsltHtml4
		{
			get { return xsltHtml4; }
			set { xsltHtml4 = value; }
		}

		public void Initialize()
		{
			// preenche o BaseUrl, se não preenchido
			if (String.IsNullOrEmpty(this.BaseUrl))
				this.BaseUrl = RailsContext.Request.Uri.GetLeftPart(UriPartial.Authority) + RailsContext.ApplicationPath;

			// cria o XSL transform, se existir
			string xslPath = RailsContext.Server.MapPath(this.XsltHtml4);
			if (File.Exists(xslPath))
			{
				this.xslHtmlEmail = new XslCompiledTransform();
				this.xslHtmlEmail.Load(xslPath);
			}
		}

		/// <summary>
		/// Aplica o XSLT de degradação para HTML 4.
		/// Importante para e-mais HTML, para poder ser lido em diversos browsers.
		/// </summary>
		protected void DegradeToHtml4(Message mail)
		{
			if (xslHtmlEmail == null)
				throw new AppError("Arquivo Faltante", "O arquivo XSLT para degradação para HTML4 não foi encontrado.");
			
			// aplica o XSL de email
			using (StringReader sr = new StringReader(mail.Body))
			using (StringWriter sw = new StringWriter())
			using (XmlReader r = XmlReader.Create(sr))
			{
				xslHtmlEmail.Transform(r, null, sw);

				sw.Flush();
				mail.Body = sw.ToString();
			}
		}

		protected bool AreValidEmailAddresses(string emailAddresses, bool allowEmptyEmailList)
		{
			if (String.IsNullOrEmpty(emailAddresses) && allowEmptyEmailList)
				return true;

			foreach (string email in rxEmailSeparator.Split(emailAddresses))
				if (!rxEmailWithName.IsMatch(email) && !rxSimpleEmail.IsMatch(email))
					return false;

			return true;
		}

		protected string RemoveAccents(string s)
		{
			return String.Format(PluggableFormatProvider.Instance, "{0:noacc}", (s ?? "").Trim());
		}
	}
}