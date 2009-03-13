using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

using Castle.MonoRail.Framework;

using Suprifattus.Util.Web.MonoRail.Attributes;
using Suprifattus.Util.Web.MonoRail.Contracts;

namespace Suprifattus.Util.Web.MonoRail.Components
{
	[WindsorComponent("component.resourceProcessor", typeof(IWebResourceProcessor))]
	public class WebResourceProcessor : IWebResourceProcessor
	{
		private static readonly Regex
			rxVar = new Regex(@"\$(\w+)", RegexOptions.Compiled),
			rxRoot = new Regex(@"(~|\$!?siteRoot)/", RegexOptions.Compiled),
			rxIf = new Regex(@"^(.*)/[*]\s*if\s+(.*?)\s*[*]/\s*$", RegexOptions.Compiled | RegexOptions.Multiline),
			rxCond = new Regex(@"^ (?<not> [!] \s*)? (?<id>\w+?) ( (?<major>\d+) (?<minor>[.]\d+)? (?<plus>[+-])? )? $", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

		private bool debug;

		public bool Debug
		{
			get { return debug; }
			set { debug = value; }
		}

		public string ProcessCss(string css, IRailsEngineContext ctx, IDictionary parameters)
		{
			return new ItemToProcess(this, css, ctx, parameters).Process();
		}

		public string ProcessJs(string js, IRailsEngineContext ctx, IDictionary parameters)
		{
			return new ItemToProcess(this, js, ctx, parameters).Process();
		}

		private class ItemToProcess
		{
			private readonly string contents;
			private readonly WebResourceProcessor proc;
			private readonly IRailsEngineContext ctx;
			private readonly IDictionary parameters;

			public ItemToProcess(WebResourceProcessor proc, string contents, IRailsEngineContext ctx, IDictionary parameters)
			{
				this.proc = proc;
				this.contents = contents;
				this.ctx = ctx;
				this.parameters = parameters;
			}

			public string Contents
			{
				get { return contents; }
			}

			public IDictionary Parameters
			{
				get { return parameters; }
			}

			public string Process()
			{
				string c = Contents;
				c = rxVar.Replace(c, ReplaceVars);
				c = rxIf.Replace(c, ReplaceIf);
				c = rxRoot.Replace(c, ReplaceRoot);
				return c;
			}

			private string ReplaceVars(Match m)
			{
				string var = m.Groups[1].Value;
				if (Parameters.Contains(var))
					return Convert.ToString(Parameters[var]);
				else
					return m.Value;
			}
			
			private string ReplaceIf(Match m)
			{
				string cond = m.Groups[2].Value;
				HttpBrowserCapabilities browser = ctx.UnderlyingContext.Request.Browser;

				Match mCond = rxCond.Match(cond);
				
				bool ok = mCond.Success;
				string id = mCond.Groups["id"].Value;
				if (id == "FF" || id == "FFX")
					id = "Firefox";
				else if (id == "OP")
					id = "Opera";
				else if (id == "SA")
					id = "Safari";
				
				ok &= String.Compare(browser.Browser, id, true) == 0;

				int? major = null;
				float? minor = null;

				if (ok && mCond.Groups["major"].Success)
				{
					bool plus = mCond.Groups["plus"].Value == "+";
					major = Convert.ToInt32(mCond.Groups["major"].Value);
					if (mCond.Groups["minor"].Success)
						minor = Convert.ToSingle("0" + mCond.Groups["minor"].Value, CultureInfo.InvariantCulture);
					
					bool majorExact = browser.MajorVersion == major;
					ok &= majorExact || (plus && browser.MajorVersion > major);
					if (minor.HasValue)
						ok &= !majorExact || browser.MinorVersion == minor.Value || (plus && browser.MinorVersion > minor.Value);
				}
				
				if (mCond.Groups["not"].Success)
					ok = !ok;
				
				string rule = m.Groups[1].Value;
				if (ok)
					return rule;
				else if (proc.Debug)
					return String.Format(CultureInfo.InvariantCulture, "/* [debug] browser: '{0}{1}{2:.0}' cond: '{3}{4}{5:.0}' rule: '{6}' */", browser.Browser, browser.MajorVersion, browser.MinorVersion, id, major, minor, rule);
				else
					return String.Empty;
			}
			
			private string ReplaceRoot(Match m)
			{
				return ctx.ApplicationPath + "/";
			}
		}
	}
}