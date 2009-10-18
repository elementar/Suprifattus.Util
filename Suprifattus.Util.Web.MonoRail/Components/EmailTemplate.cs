using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;

using Castle.Components.Common.EmailSender;
using Castle.Components.Common.TemplateEngine;
using Castle.Core;

using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Text;
using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail.Components
{
	/// <summary>
	/// Componente utilizado para montar templates de email.
	/// </summary>
	public class EmailTemplate : BusinessRuleWithLogging, IInitializable
	{
		private readonly IEmailSender emailSender;
		private readonly ITemplateEngine templateEngine;

		private static readonly Regex
			rxEmailSeparator = new Regex(@"\s*;\s*", RegexOptions.Compiled),
			rxSimpleEmail = new Regex(@"^[\w._%+-]+@\w+(\.\w+)+$", RegexOptions.Compiled),
			rxEmailWithName = new Regex(@"^.* <[\w._%+-]+@[\w+-]+(\.[\w+-]+)+>$", RegexOptions.Compiled);

		private static readonly Singleton<XslCompiledTransform>
			xslHtmlEmail = new Singleton<XslCompiledTransform>(CriaXsltHtml40);

		public EmailTemplate(IEmailSender emailSender, ITemplateEngine templateEngine, string from)
		{
			this.emailSender = emailSender;
			this.templateEngine = templateEngine;

			this.From = from;
		}

		public string Subject { get; set; }
		public string To { get; set; }
		public string From { get; private set; }
		public string Cc { get; set; }
		public string Bcc { get; set; }

		public string BaseUrl { get; set; }
		public Format Format { get; set; }
		public string Template { get; set; }

		public void Initialize()
		{
			// preenche o BaseUrl, se não preenchido
			if (String.IsNullOrEmpty(this.BaseUrl))
				this.BaseUrl = RailsContext.Request.Uri.GetLeftPart(UriPartial.Authority) + RailsContext.ApplicationPath;
		}

		public virtual void Send(Message mail)
		{
			if (!AreValidEmailAddresses(mail.From, false))
				throw new AppException("E-mail inválido", "Endereço de e-mail inválido no campo 'De': " + mail.From);
			if (!AreValidEmailAddresses(mail.To, false))
				throw new AppException("E-mail inválido", "Endereço de e-mail inválido no campo 'Para': " + mail.To);
			if (!AreValidEmailAddresses(mail.Cc, true))
				throw new AppException("E-mail inválido", "Endereço de e-mail inválido no campo 'CC': " + mail.Cc);
			if (!AreValidEmailAddresses(mail.Bcc, true))
				throw new AppException("E-mail inválido", "Endereço de e-mail inválido no campo 'BCC': " + mail.Bcc);

			emailSender.Send(mail);
		}

		public virtual Message BuildMessage(IDictionary context, bool degrade)
		{
			var mail = new Message { From = From, To = To, Cc = Cc, Bcc = Bcc, Subject = Subject, Format = Format };
			AdjustMailHeaders(mail);
			mail.Body = ComposeBody(mail, context, degrade);

			return mail;
		}

		protected virtual string ComposeBody(Message msg, IDictionary context, bool degrade)
		{
			string body;
			using (var sw = new StringWriter())
			{
				templateEngine.Process(context, Template, sw);
				body = sw.ToString();
			}

			if (degrade)
				body = DegradeToHtml4(body);

			return body;
		}

		/// <summary>
		/// Aplica o XSLT de degradação para HTML 4.
		/// Importante para e-mais HTML, para poder ser lido em diversos browsers e clientes de e-mail.
		/// </summary>
		protected string DegradeToHtml4(string body)
		{
			if (xslHtmlEmail == null)
				throw new AppError("Arquivo Faltante", "O arquivo XSLT para degradação para HTML4 não foi encontrado.");

			// aplica o XSL de email
			using (var sr = new StringReader(body))
			using (var sw = new StringWriter())
			using (var r = XmlReader.Create(sr))
			{
				xslHtmlEmail.Instance.Transform(r, null, sw);

				return sw.ToString();
			}
		}

		private void AdjustMailHeaders(Message mail)
		{
			mail.Headers.Add("X-Suprifattus-Gen", DateTime.Now.ToString("s"));

			mail.From = RemoveAccents(mail.From);
			mail.To = RemoveAccents(mail.To);

			if (!String.IsNullOrEmpty(mail.Cc))
				mail.Cc = RemoveAccents(mail.Cc);

			if (!String.IsNullOrEmpty(mail.Bcc))
				mail.Cc = RemoveAccents(mail.Bcc);
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

		private static XslCompiledTransform CriaXsltHtml40()
		{
			var xsl = new XslCompiledTransform();
			using (var s = Utils.GetResourceStream("Resources.html-email.xsl"))
			using (var r = XmlReader.Create(s))
				xsl.Load(r);
			return xsl;
		}
	}
}