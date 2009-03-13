using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Castle.MonoRail.Framework.Helpers;

using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	public class RescueHelper : AbstractHelper
	{
		private Exception exception;

		public Exception Exception
		{
			get
			{
				if (exception == null)
				{
					exception = Controller.Context.LastException;
					while (IsEncapsulationException(exception))
						exception = exception.InnerException;
				}

				return exception;
			}
		}

		public string ExceptionTypeDescription
		{
			get
			{
				if (Exception is AppError)
					return "Erro do Sistema";
				if (Exception is AppException)
					return "Erro do Usuário";

				return "Erro Interno";
			}
		}

		public string FormatException(Exception e)
		{
			return FormatException(e, true);
		}

		public string FormatException(Exception e, bool usingHTML)
		{
			const string projDir = @".:\\(Documents and Settings|projects)\\\w+(\\My Documents\\Visual Studio Projects)?";
			string nl = Environment.NewLine;

			string
				fmtTitleStart = usingHTML ? "<strong class='exception-text'>" : null,
				fmtTitleEnd = usingHTML ? "</strong>" : null,
				fmtDataStart = usingHTML ? "<em class='exception-data'>" : null,
				fmtDataEnd = usingHTML ? "</em>" : null,
				fmtSourceFileStart = usingHTML ? "<span class='exception-sourcefile'>" : null,
				fmtSourceFileEnd = usingHTML ? "</span>" : null;

			var rx = new Regex(" in " + projDir + "(.*$)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

			var s = new Stack<Exception>();

			while (e != null)
			{
				s.Push(e);
				e = e.InnerException;
			}

			var sb = new StringBuilder();
			while (s.Count > 0)
			{
				e = s.Pop();
				sb.AppendFormat("{0}{1}: ", fmtTitleStart, Controller.Context.Server.HtmlEncode(e.GetType().FullName));
				sb.AppendFormat("{0}{1}", Controller.Context.Server.HtmlEncode(e.Message), fmtTitleEnd);
				sb.Append(nl);
				foreach (DictionaryEntry de in e.Data)
					sb.AppendFormat("    {0}{1} = {2}{3}", fmtDataStart, de.Key, de.Value, fmtDataEnd).Append(nl);
				if (e.StackTrace != null || e.StackTrace.Length > 0)
				{
					string stackTrace;
					stackTrace = Controller.Context.Server.HtmlEncode(e.StackTrace);
					stackTrace = rx.Replace(stackTrace, nl + "\t" + fmtSourceFileStart + "in $3" + fmtSourceFileEnd);
					sb.Append(stackTrace).Append(nl);
				}
				sb.Append(nl);
			}

			return sb.ToString();
		}

		private bool IsEncapsulationException(Exception ex)
		{
			return ex is TargetInvocationException;
		}
	}
}