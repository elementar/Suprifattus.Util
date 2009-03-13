using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Suprifattus.Util.Web.Handlers
{
	/// <summary>
	/// Source code stolen from:
	/// http://www.aspnetresources.com/articles/HttpFilters.aspx
	/// </summary>
	public class ContentFilter : FilterBase
	{
		public enum RegexSet
		{
			AspNet,
			JavaScript,
			Cleanup,
		}

		private static Replacer[] aspNetSpecializedReplacers =
			{
				new TitleIDRemover(),
				new ClientSideScriptBeginCommentBlockReplacer(),
				new ClientSideScriptEndCommendBlockReplacer(),
				new LanguageJavascriptAttributeRemover(),
				new ImageBorderAttributeRemover(),
				new ViewStateHiddenFieldsReplacer(),
				new PostBackFunctionReplacer(),
				new FormNameRemover(),
				new ApplicationPathReplacer(),
				new LineBreakTagReplacer(),
			};

		private static Replacer[] javascriptSpecializedReplacers =
			{
				new InnerHtmlCallReplacer(),
				new NullClassNameReplacer(),
			};
		
		private static Replacer[] cleanupSpecializedReplacers =
			{
				new UnecessaryWhitespaceRemover(),
			};
		
		private Replacer[] selectedReplacers;
		private StringBuilder responseHtml;

		public ContentFilter(Stream inputStream, RegexSet regexSet)
			: base(inputStream)
		{
			switch (regexSet)
			{
				case RegexSet.AspNet: 
					selectedReplacers = aspNetSpecializedReplacers; 
					break;
				case RegexSet.JavaScript: 
					selectedReplacers = javascriptSpecializedReplacers; 
					break;
				case RegexSet.Cleanup:
					selectedReplacers = cleanupSpecializedReplacers;
					break;
				default: 
					throw new ArgumentException("RegexSet inválido.");
			}

			responseHtml = new StringBuilder();
		}

		public ContentFilter(Stream inputStream)
			: this(inputStream, RegexSet.AspNet) { }

		public override void Write(byte[] buffer, int offset, int count)
		{
			string strBuffer = Encoding.Default.GetString(buffer, offset, count);

			responseHtml.Append(strBuffer);
		}

		public override void Close()
		{
			string finalHtml = responseHtml.ToString();

			foreach (Replacer r in selectedReplacers)
				finalHtml = r.Run(finalHtml);
				
			// Write the formatted HTML back
			byte[] data = Encoding.Default.GetBytes(finalHtml);

			responseStream.Write(data, 0, data.Length);

			base.Close();
		}


		#region Replacer Base Classes
		private abstract class Replacer
		{
			public Replacer()
			{
			}

			public abstract string Run(string s);
		}

		private abstract class RegexReplacer : Replacer
		{
			public RegexReplacer()
			{
				me = new MatchEvaluator(Evaluator);
			}

			protected Regex rx;
			protected MatchEvaluator me;

			public override string Run(string s)
			{
				return rx.Replace(s, me);
			}

			protected abstract string Evaluator(Match m);
		}

		private class RegexRemover : RegexReplacer
		{
			int group;

			public RegexRemover(int group, Regex rx)
			{
				this.group = group;
				this.rx = rx;
			}

			public RegexRemover(Regex rx)
				: this(-1, rx) { }

			protected override string Evaluator(Match m)
			{
				if (group != -1)
					return m.ToString().Replace(m.Groups[group].Value, string.Empty);
				else
					return String.Empty;
			}
		}
		#endregion

		#region Specialized Replacers
		private class TitleIDRemover : RegexRemover
		{
			public TitleIDRemover()
				: base(1, new Regex("<title(\\s+id=.+?)>", RegexOptions.IgnoreCase | RegexOptions.Compiled)) { }
		}

		private class ClientSideScriptBeginCommentBlockReplacer : RegexReplacer
		{
			public ClientSideScriptBeginCommentBlockReplacer()
			{
				rx = new Regex(@"<script[^>]*>\s*(<!--)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			}

			protected override string Evaluator(Match m)
			{
				return m.ToString().Replace(m.Groups[1].Value, "//<![CDATA[");
			}
		}

		private class InnerHtmlCallReplacer : RegexReplacer
		{
			public InnerHtmlCallReplacer()
			{
				rx = new Regex(@"\binnerHTML(?<ignore>(""|/\*\*/))?\b", RegexOptions.Compiled);
			}

			protected override string Evaluator(Match m)
			{
				return (m.Groups["ignore"].Success ? m.ToString() : "innerXHTML");
			}
		}

		private class NullClassNameReplacer : RegexReplacer
		{
			public NullClassNameReplacer()
			{
				rx = new Regex(@"\b(element\.className\.)\b", RegexOptions.Compiled);
			}

			protected override string Evaluator(Match m)
			{
				return "(element.className || '').";
			}
		}

		private class ClientSideScriptEndCommendBlockReplacer : RegexReplacer
		{
			public ClientSideScriptEndCommendBlockReplacer()
			{
				rx = new Regex(@"(-->)\s*</script>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			}

			protected override string Evaluator(Match m)
			{
				return m.ToString().Replace(m.Groups[1].Value, "]]>");
			}
		}

		private class LanguageJavascriptAttributeRemover : RegexRemover
		{
			public LanguageJavascriptAttributeRemover()
				: base(new Regex("language=[\"']javascript[\"']", RegexOptions.IgnoreCase | RegexOptions.Compiled)) { }
		}

		private class ImageBorderAttributeRemover : RegexRemover
		{
			public ImageBorderAttributeRemover()
				: base(1, new Regex("<img.*(border=\".*?\").*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled)) { }
		}

		private class ViewStateHiddenFieldsReplacer : RegexReplacer
		{
			public ViewStateHiddenFieldsReplacer()
			{
				rx = new Regex("(<form[^>]*>\\s*)(<input.*?__.*?/>)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
			}

			protected override string Evaluator(Match m)
			{
				return String.Concat(m.Groups[1].Value, "<div style='display: none'>", m.Groups[2].Value, "</div>");
			}
		}

		private class FormNameRemover : RegexRemover
		{
			public FormNameRemover()
				: base(1, new Regex("<form\\s+(name=.*?\\s)", RegexOptions.IgnoreCase | RegexOptions.Compiled)) { }
		}

		private class UnecessaryWhitespaceRemover : RegexRemover
		{
			public UnecessaryWhitespaceRemover()
				: base(new Regex(@"(?<=\S\s)(\s+)|\s+(?=[>])", RegexOptions.Compiled)) { }
		}

		private class PostBackFunctionReplacer : Replacer
		{
			public override string Run(string s)
			{
				// If __doPostBack is registered, replace the whole function
				if (s.IndexOf("__doPostBack") > -1) 
				{
					try 
					{
						int pos1 = s.IndexOf("var theform;");
						int pos2 = s.IndexOf("theform.__EVENTTARGET", pos1);
						string methodText = s.Substring(pos1, pos2 - pos1);

						s = s.Replace(methodText, "var theform = document.forms[0];\r\n\t\t");
					} 
					catch 
					{
					}
				}

				return s;
			}
		}

		private class ApplicationPathReplacer : RegexReplacer
		{
			public ApplicationPathReplacer()
			{
				rx = new Regex("((href|src)=[\"']?)~/", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			}

			protected override string Evaluator(Match m)
			{
				return WebUtil.ExpandTilde(m.ToString());
			}
		}

		private class LineBreakTagReplacer : RegexReplacer
		{
			public LineBreakTagReplacer()
			{
				rx = new Regex("<br>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			}

			protected override string Evaluator(Match m)
			{
				return "<br />";
			}
		}
		#endregion
	}
}