using System;

namespace Suprifattus.Util.Web.Controls.Helpers
{
	/// <summary>
	/// SELECT based control, like DropDownList and ListBox.
	/// </summary>
	public interface ISelectControl : IListControl
	{
		string DataAutoFillFields { get; }
		string DataDetailIdFields { get; }
		string DataGroupTextField { get; }
		string SourceWebService { get; }

		object DataSource { get; }
		string DataMember { get; }
	}
}
