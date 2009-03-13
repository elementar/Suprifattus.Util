using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.ComponentModel;

namespace Suprifattus.Util.Web.Controls
{
	/// <summary>
	/// Summary description for AttributeAppend.
	/// </summary>
	[ToolboxData("<{0}:AttributeAppend Runat=server AttributeName=\"\" AttributeValue=\"\" />")]
	public class AttributeAppend : System.Web.UI.Control
	{
		private string attributeName;
		private object attributeValue;
		private string select;
	
		[Bindable(true)]
		[Category("Logic")]
		[DefaultValue("")] 
		public string AttributeName
		{
			get { return attributeName; }
			set { attributeName = value; }
		}

		[Bindable(true)]
		[Category("Logic")]
		[DefaultValue(null)] 
		public object AttributeValue 
		{
			get { return attributeValue; }
			set { attributeValue = value; }
		}

		[Bindable(true)]
		[Category("Logic")]
		[DefaultValue("")] 
		public string Select 
		{
			get { return select; }
			set { select = value; }
		}

		protected override void OnPreRender(EventArgs e)
		{
			int index = -1, i = -1;
			while (index == -1 && ++i < Parent.Controls.Count)
				if (Parent.Controls[i] == this)
					index = i;

			Debug.Assert(index != -1);

			if (Parent.Controls.Count > index) 
			{
				WebControl ctl = Parent.Controls[index+1] as WebControl;
				if (ctl != null)
					ctl.Attributes[AttributeName] = Convert.ToString(AttributeValue);
			}
		}

	}
}
