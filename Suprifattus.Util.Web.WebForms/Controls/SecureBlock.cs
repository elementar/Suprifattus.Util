using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Suprifattus.Util.Web.Controls
{
	[DefaultProperty("RequiredPermissions")]
	[ToolboxData("<{0}:SecureBlock runat=server></{0}:SecureBlock>")]
	[ParseChildren(true)]
	public class SecureBlock : WebControl
	{
		private string requiredPermissions = null;
		private bool ignore = false;
		PlaceHolder ok, fail;

		public SecureBlock()
		{
			EnsureChildControls();
		}
		
		[Bindable(true)]
		[Category("Security")]
		[DefaultValue(null)]
		public string RequiredPermissions
		{
			get { return requiredPermissions; }
			set { requiredPermissions = value; }
		}

		[Bindable(true)]
		[Category("Security")]
		[DefaultValue(false)]
		public bool Ignore
		{
			get { return ignore; }
			set { ignore = value; }
		}

		public PlaceHolder Success
		{
			get { return ok; }
		}

		public PlaceHolder Failure
		{
			get { return fail; }
		}

		protected override void CreateChildControls()
		{
			Controls.Add(this.ok   = new PlaceHolder());
			Controls.Add(this.fail = new PlaceHolder());
			
			base.CreateChildControls();
		}

		protected override object SaveViewState()
		{
			ViewState["requiredPermissions"] = requiredPermissions;
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			requiredPermissions = (string) ViewState["requiredPermissions"];
		}

		protected override void Render(HtmlTextWriter w)
		{
			EnsureChildControls();

			bool ok = true;

			if (!ignore)
				foreach (string perm in requiredPermissions.Split(','))
					if (!WebUtil.HasPermission(perm))
					{
						ok = false;
						break;
					}
			
			if (ok)
				this.ok.RenderControl(w);
			else
				this.fail.RenderControl(w);
		}

	}
}