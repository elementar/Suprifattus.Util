using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Suprifattus.Util.Web.Controls
{
	using JavaScript;

	[ToolboxData("<{0}:JavaScriptDataBind runat=server/>")]
	public class JavaScriptDataBind : System.Web.UI.Control
	{
		bool singleRecord;

		protected override void OnLoad(EventArgs e)
		{
			JavaScriptHttpHandler.Dependencies.AddRange("maskedit", "masterdetail", "databind", "webservices");
			
			if (SingleRecord)
				JavaScriptHttpHandler.Dependencies.Add("databind-1rec");
		}

		#region Properties
		[DefaultValue(false)]
		public bool SingleRecord
		{
			get { return singleRecord; }
			set { singleRecord = value; }
		}
		#endregion
	}
}
