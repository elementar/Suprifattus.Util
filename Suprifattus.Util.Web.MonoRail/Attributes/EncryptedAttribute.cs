using System;
using System.Reflection;
using System.Text;

using Castle.MonoRail.Framework;

using Suprifattus.Util.Encryption;

namespace Suprifattus.Util.Web.MonoRail.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public class EncryptedAttribute : Attribute, IParameterBinder
	{
		private readonly string key;
		private string parameterName;

		public EncryptedAttribute(string key)
		{
			this.key = key;
		}

		public EncryptedAttribute(string key, string parameterName)
		{
			this.key = key;
			this.parameterName = parameterName;
		}

		public int CalculateParamPoints(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			if (String.IsNullOrEmpty(parameterName))
				parameterName = parameterInfo.Name;

			return String.IsNullOrEmpty(controller.Request.Params[parameterName]) ? 0 : 100;
		}

		public object Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			if (String.IsNullOrEmpty(parameterName))
				parameterName = parameterInfo.Name;

			string value = controller.Request.Params[parameterName];

			if (String.IsNullOrEmpty(value))
				return null;

			using (var s3des = new SimpleTripleDES(key))
				return s3des.DecryptObjectFromBase64(AdjustBase64ForUrlUsage(false, value));
		}

		public static string Encrypt(string key, object obj)
		{
			using (var s3des = new SimpleTripleDES(key))
				return AdjustBase64ForUrlUsage(true, s3des.EncryptObjectToBase64(obj));
		}

		private static string AdjustBase64ForUrlUsage(bool escape, string s)
		{
			char[] m1 = { '+', '=', '/' };
			char[] m2 = { '-', '!', '_' };

			char[] from = (escape ? m1 : m2);
			char[] to = (escape ? m2 : m1);

			var sb = new StringBuilder(s);
			for (int i = 0; i < sb.Length; i++)
				for (int j = 0; j < from.Length; j++)
					if (sb[i] == from[j])
						sb[i] = to[j];

			return sb.ToString();
		}
	}
}