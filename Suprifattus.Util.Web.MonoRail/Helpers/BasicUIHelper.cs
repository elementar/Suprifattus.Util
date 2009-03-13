using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using Castle.MonoRail.Framework.Helpers;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	public class BasicUIHelper : AbstractHelper
	{
		private static readonly Regex rxAction = new Regex(@"(\w+)[.](\w+)($|[?])");
		private static readonly object defaultZebra = new object();

		private HybridDictionary zebras;
		private HashSet<object> once;

		public string ReferrerAction
		{
			get
			{
				var ra = Controller.Request["referrerAction"];
				if (!String.IsNullOrEmpty(ra))
					return ra;

				var url = Controller.Context.UrlReferrer;
				if (!String.IsNullOrEmpty(url))
					return rxAction.Match(url).Result("$1");

				return null;
			}
		}

		public object Zebra(object v1, object v2)
		{
			return Zebra(defaultZebra, v1, v2);
		}

		public object Zebra(object key, object v1, object v2)
		{
			if (zebras == null)
				zebras = new HybridDictionary(2);

			var current = zebras[key];
			return (zebras[key] = (current == null || Equals(current, v2) ? v1 : v2));
		}

		public string BindAutoComplete(string fieldId, string hiddenId, string url)
		{
			return String.Format(@"
				<div id=""{0}_autocomplete""></div>
				<script type=""text/javascript"">
				// <![CDATA[
					new Ajax.Autocompleter('{0}', '{0}_autocomplete', '{2}', {{
						onSelect: function(i) {{
							$('{0}').value = i.textContent;
							$('{1}').value = i.value;
						}}
					}});
				// ]]>
				</script>
			", fieldId, hiddenId, url);
		}

		protected virtual string UrlJSMasterDetail
		{
			get { return "/res/js/util.ashx?castle-masterdetail"; }
		}

		public string BindMasterDetail(string masterId, string detailId, string url, string parameters)
		{
			var urlJS = Controller.Context.ApplicationPath + UrlJSMasterDetail;

			return String.Format(@"
				<script type=""text/javascript"" src=""{0}""></script>
				<script type=""text/javascript"">
				// <![CDATA[
					MasterDetailDropDown.register('{1}', '{2}', '{3}', ""{4}"");
				// ]]>
				</script>
			", urlJS, masterId, detailId, url, parameters);
		}

		public string BindValidation(string validationType)
		{
			return String.Format("<jsvalidation:RegexValidation Type=\"{0}\" />", validationType);
		}

		public string BindMask(string id, string mask)
		{
			return String.Format("<jsmaskedit:Bind Control=\"{0}\" Mask=\"{1}\" />", id, mask);
		}

		public string BindCalendarButton(string id)
		{
			return String.Empty;
		}

		public bool Once(object key)
		{
			if (once == null)
				once = new HashSet<object>();

			return once.Add(key);
		}
	}
}