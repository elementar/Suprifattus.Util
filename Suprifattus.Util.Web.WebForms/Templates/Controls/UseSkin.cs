using System;
using System.ComponentModel;
using System.Web.UI;

namespace Suprifattus.Util.Web.Templates.Controls
{
	[DefaultProperty("Name")]
	[ToolboxData("<{0}:UseSkin runat=server />")]
	public class UseSkin : Control
	{
		private string skinID;

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(null)]
		public string SkinID
		{
			get { return skinID; }
			set { skinID = value; }
		}
	}
}