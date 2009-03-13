using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;

using Castle.MonoRail.Framework.Helpers;

using Suprifattus.Util.Collections;
using Suprifattus.Util.Exceptions;
using Suprifattus.Util.Text;
using Suprifattus.Util.Timing;
using Suprifattus.Util.Xml;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	using PF = PluralForm;
#if GENERICS
	using NullableInt32 = Nullable<int>;
	using NullableDateTime = Nullable<DateTime>;

#else
	using Nullables;
#endif

	public class BasicFormatHelper : AbstractHelper
	{
		protected static readonly Regex rxFirstName = new Regex(@"^\S+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		protected static readonly Regex rxLineBreaks = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);
		protected static readonly Regex rxLineContents = new Regex("^.*$", RegexOptions.Compiled);

		protected IFormatProvider fp
		{
			get { return PluggableFormatProvider.Instance; }
		}

		public IClock clock = SystemClock.Instance;

		#region ConvertLineBreaks & EncloseLines
		public string ConvertLineBreaks(string s, string br)
		{
			if (s == null) return null;
			return rxLineBreaks.Replace(s, br);
		}

		public string EncloseLines(string s, string prefixAndSuffix)
		{
			return EncloseLines(s, prefixAndSuffix, prefixAndSuffix);
		}

		public string EncloseLines(string s, string prefix, string suffix)
		{
			return EncloseLines(s, prefix, suffix, null);
		}

		public string EncloseLines(string s, string prefix, string suffix, string middle)
		{
			if (s == null) return null;
			s = rxLineContents.Replace(s, prefix + "$0" + suffix);
			if (middle != null)
				s = ConvertLineBreaks(s, middle);
			return s;
		}
		#endregion

		#region GetDatePart
		public NullableInt32 GetDatePart(NullableDateTime dt, string part)
		{
			if (!dt.HasValue) return null;

			switch (part)
			{
				case "d":
					return dt.Value.Day;
				case "M":
					return dt.Value.Month;
				case "y":
					return dt.Value.Year;

				case "H":
					return dt.Value.Hour;
				case "m":
					return dt.Value.Minute;
				case "s":
					return dt.Value.Second;
			}

			return null;
		}
		#endregion

		#region GetAge
		public string GetAge(NullableDateTime val)
		{
			if (!val.HasValue) return null;

			DateTime hoje = clock.Get();
			DateTime nasc = val.Value.Date;

			if (nasc > hoje)
			{
				// TODO: adicionar log
				//				throw new SGLabException("Data de Nascimento Inválida", "Não é permitido utilizar data de nascimento superior a data atual.");
				return "--";
			}

			int idade = hoje.Year - nasc.Year;
			DateTime aniv = nasc.AddYears(idade);
			if (hoje < aniv) // se ainda não passou o aniversário
				idade -= 1; // subtrai um ano

			if (idade >= 1) // se a idade é maior ou igual a 1 ano, apresenta apenas o ano
				return PF.Format(idade, "{0} ano[:s]", idade);

			// para menores de 1 ano, apresenta apenas os meses
			int meses = hoje.Month - nasc.Month;
			if ((meses == 0 && hoje.Year > nasc.Year) || meses < 0)
				meses += 12;
			if (hoje < nasc.AddMonths(meses))
				meses -= 1;
			return PF.Format(meses, "{0} [mês:meses]", meses);
		}
		#endregion

		#region GetFirstName
		public string GetFirstName(string fullname)
		{
			return rxFirstName.Match(fullname).Value;
		}
		#endregion

		#region ToJSArray
		public string ToJSArray(string str)
		{
			if (str == null)
				return "[]";

			string[] a = rxLineBreaks.Split(str);
			if (a.Length == 0)
				return "[]";

			return "['" + CollectionUtils.Join(a, "', '", new FormatDelegate(Escape)) + "']";
		}
		#endregion

		#region Escape
		public string Escape(object s)
		{
			if (s == null)
				return null;
			return s.ToString().Replace("'", "\\'").Replace("\"", "\\\"");
		}
		#endregion

		#region HtmlEncode & XmlEncode
		public string HtmlEncode(object s)
		{
			if (s == null)
				return null;

			return Controller.Context.Server.HtmlEncode(s.ToString());
		}

		public string XmlEncode(object s)
		{
			if (s == null)
				return null;

			return XmlEncoder.Encode(s.ToString());
		}
		#endregion

		#region Transform
		public string Transform(object source, string xsltViewPath)
		{
			if (source == null)
				return null;

			var doc = new XmlDocument();
			doc.LoadXml(source.ToString());

#if GENERICS
			var tr = new XslCompiledTransform();
#else
			XslTransform tr = new XslTransform();
#endif
			using (var w = new StringWriter())
			{
				Controller.InPlaceRenderSharedView(w, xsltViewPath);
				w.Flush();
				XmlReader r = new XmlTextReader(new StringReader(w.GetStringBuilder().ToString()));
				tr.Load(r, null, null);
			}

			using (var w = new StringWriter())
			{
#if GENERICS
				tr.Transform(doc, null, w);
#else
				tr.Transform(doc, null, w, null);
#endif
				w.Flush();
				return w.GetStringBuilder().ToString();
			}
		}
		#endregion

		#region Coalesce
		public object Coalesce(params object[] objs)
		{
			foreach (object obj in objs)
				if (!NullableHelper.IsNull(obj))
					return obj;

			return null;
		}
		#endregion

		#region Case
		public object Case(params object[] args)
		{
			for (int i = 1; i < args.Length; i += 2)
			{
				if (Logic.RepresentsTrue(args[i - 1]))
					return args[i];
			}
			if (args.Length % 2 == 1)
				return args[args.Length - 1];
			return null;
		}
		#endregion

		#region Substring
		public string Substring(object s, int start, int length)
		{
			if (s == null) return null;

			string ss = Convert.ToString(s);
			return ss.Substring(start, Math.Min(ss.Length, length));
		}
		#endregion

		#region Replace
		public string Replace(string source, string regex, string replacement)
		{
			return Regex.Replace(source, regex, replacement);
		}
		#endregion

		#region Pad, PadLeft & PadRight
		public string Pad(object obj, int spaces)
		{
			string s = Convert.ToString(obj);

			if (s == null)
				return new string(' ', spaces);

			if (s.Length == spaces)
				return s;

			if (s.Length > spaces)
				return s.Substring(0, spaces);

			var sb = new StringBuilder(s);
			sb.Append(' ', spaces - sb.Length);
			return sb.ToString();
		}

		public string PadLeft(object obj, string paddingChar, int length)
		{
			if (paddingChar == null || paddingChar.Length != 1)
				throw new AppError("Parâmetro Inválido", "Parâmetro inválido para o método 'PadLeft'");

			string s = Convert.ToString(obj);
			if (s.Length == length)
				return s;

			if (s.Length > length)
				return s.Substring(0, length);

			return s.PadLeft(length, paddingChar[0]);
		}

		public string PadRight(object obj, string paddingChar, int length)
		{
			if (paddingChar == null || paddingChar.Length != 1)
				throw new AppError("Parâmetro Inválido", "Parâmetro inválido para o método 'PadRight'");

			string s = Convert.ToString(obj);
			if (s.Length == length)
				return s;

			if (s.Length > length)
				return s.Substring(s.Length - length, length);

			return s.PadRight(length, paddingChar[0]);
		}
		#endregion

		#region Cut
		public string Cut(object value, int length)
		{
			return Cut(value, length, "...");
		}

		public string Cut(object value, int length, string trail)
		{
			string s = Convert.ToString(value);
			if (s == null || s.Length <= length)
				return s;
			return s.Substring(0, length - trail.Length) + trail;
		}
		#endregion

		#region Join
		public string Join(IDictionary dict, string keyValueSeparator, string entrySeparator)
		{
			if (dict == null)
				return null;
			return CollectionUtils.Join(dict, keyValueSeparator, entrySeparator);
		}

		public string JoinIds(IEnumerable en, string separator)
		{
			if (en == null)
				return null;
			return CollectionUtils.Join(en, separator, o => ((IRecord) o).Id.ToString());
		}
		#endregion

		#region ToLower & ToUpper
		public string ToLower(object s)
		{
			return (s == null ? null : Convert.ToString(s).ToLower());
		}

		public string ToUpper(object s)
		{
			return (s == null ? null : Convert.ToString(s).ToUpper());
		}
		#endregion

		#region FormatInvariant
		public string FormatInvariant(object o)
		{
			using (new CultureSwitch(CultureInfo.InvariantCulture))
				return Format(o);
		}

		public string FormatInvariant(object o, string format)
		{
			using (new CultureSwitch(CultureInfo.InvariantCulture))
				return Format(o, format);
		}

		public string FormatInvariant(object o, string format, object defaultValue)
		{
			using (new CultureSwitch(CultureInfo.InvariantCulture))
				return Format(o, format, defaultValue);
		}
		#endregion

		#region Format
		public string Format(object o)
		{
			if (o == null)
				return null;

#if !GENERICS
			if (o is INullableType)
			{
				INullableType val = o as INullableType;
				return val == null || !val.HasValue || val.Value == null 
					? null 
					: Format(val.Value);
			}
#endif

			return Convert.ToString(o, fp);
		}

		public string Format(object o, string format)
		{
			if (o == null)
				return null;

#if !GENERICS
			if (o is INullableType)
			{
				INullableType val = o as INullableType;
				return val == null || !val.HasValue || val.Value == null 
					? null 
					: Format(val.Value, format);
			}
#endif

			return String.Format(fp, "{0:" + format + "}", o);
		}

		public string Format(object o, string format, object defaultValue)
		{
			return Format((o ?? defaultValue), format);
		}
		#endregion

		#region GetDescription
		public string GetDescription(MemberInfo memberInfo)
		{
			if (memberInfo == null)
				return null;

			foreach (DescriptionAttribute descAtt in memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), true))
				return descAtt.Description;

			return memberInfo.Name;
		}
		#endregion

		#region PluralForm
		public string PluralForm(long n, string format, params object[] args)
		{
			return PF.Format(n, fp, format, args);
		}
		#endregion

		#region Now
		public string Now(string format)
		{
			return Format(DateTime.Now, format);
		}
		#endregion
	}
}