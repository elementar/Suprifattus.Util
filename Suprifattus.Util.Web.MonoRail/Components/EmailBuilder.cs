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
	public interface IEmailBuilder
	{
		string To { get; set; }
		string From { get; }
		string[] Cc { get; set; }
		string BaseUrl { get; set; }
		string XsltHtml4 { get; set; }
	}

	/// <summary>
	/// Componente construtor de emails.
	/// </summary>
	/// <remarks>
	/// Pode ser utilizado como serviço, mas o ideal é utilizá-lo como
	/// classe básica para criar um serviço, permitindo, assim, isolar
	/// as propriedades de configuração de forma mais fácil.
	/// </remarks>
	[Obsolete("Usar EmailTemplate")]
	public class EmailBuilder : BusinessRuleWithLogging, IInitializable, IEmailBuilder
	{
		private static readonly Regex
			rxEmailSeparator = new Regex(@"\s*;\s*", RegexOptions.Compiled),
			rxSimpleEmail = new Regex(@"^[\w._%+-]+@\w+(\.\w+)+$", RegexOptions.Compiled),
			rxEmailWithName = new Regex(@"^.* <[\w._%+-]+@[\w+-]+(\.[\w+-]+)+>$", RegexOptions.Compiled);

		private XslCompiledTransform xslHtmlEmail;

		public EmailBuilder(string from)
		{
			this.XsltHtml4 = "~/res/xsl/html-email.xsl";
			this.From = from;
		}

		public string To { get; set; }
		public string From { get; private set; }
		public string[] Cc { get; set; }
		public string BaseUrl { get; set; }
		public string XsltHtml4 { get; set; }

		public void Initialize()
		{
			// preenche o BaseUrl, se não preenchido
			if (String.IsNullOrEmpty(this.BaseUrl))
				this.BaseUrl = RailsContext.Request.Uri.GetLeftPart(UriPartial.Authority) + RailsContext.ApplicationPath;

			// cria o XSL transform, se existir
			var xslPath = RailsContext.Server.MapPath(this.XsltHtml4);

			if (!File.Exists(xslPath))
				return;

			this.xslHtmlEmail = new XslCompiledTransform();
			this.xslHtmlEmail.Load(xslPath);
		}

		/// <summary>
		/// Aplica o XSLT de degradação para HTML 4.
		/// Importante para e-mais HTML, para poder ser lido em diversos browsers e clientes de e-mail.
		/// </summary>
		protected void DegradeToHtml4(Message mail)
		{
			if (xslHtmlEmail == null)
				throw new AppError("Arquivo Faltante", "O arquivo XSLT para degradação para HTML4 não foi encontrado.");

			// aplica o XSL de email
			using (var sr = new StringReader(mail.Body))
			using (var sw = new StringWriter())
			using (var r = XmlReader.Create(sr))
			{
				xslHtmlEmail.Transform(r, null, sw);

				sw.Flush();
				mail.Body = sw.ToString();
			}
		}

		protected bool AreValidEmailAddresses(string emailAddresses, bool allowEmptyEmailList)
		{
			if (String.IsNullOrEmpty(emailAddresses))
				return allowEmptyEmailList;

			foreach (var email in rxEmailSeparator.Split(emailAddresses))
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