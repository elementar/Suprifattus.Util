using System;
using System.Text.RegularExpressions;

using Castle.MonoRail.Framework;

using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Web.MonoRail.Filters
{
	[Obsolete("Use the new IEscapable interface to permit 'unsafe' characters to be displayed correctly.")]
	public class SafePostFilter : IFilter
	{
		private readonly Regex rxUnsafe = new Regex("[&\"'<>]", RegexOptions.Compiled);

		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller c)
		{
			foreach (string key in c.Request.Form.AllKeys)
				foreach (string value in c.Request.Form.GetValues(key))
					if (rxUnsafe.IsMatch(value))
						throw new AppException("Caracteres inv�lidos no formul�rio", "Voc� digitou caracteres inv�lidos no formul�rio.\nOs caracteres < (menor-que), > (maior-que), \" (aspas) e & (\"e\" comercial) s�o inv�lidos.");

			return true;
		}
	}
}