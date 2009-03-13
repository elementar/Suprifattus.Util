using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Web.Controls.Helpers
{
	/// <summary>
	/// Utility methods for controls.
	/// </summary>
	internal class ListControlUtil
	{
		public static void SetSelectedValues(ListControl ctl, string[] value)
		{
			foreach (ListItem li in ctl.Items)
				li.Selected = false;
	
			foreach (string val in value)
				ctl.Items.FindByValue(val).Selected = true;
		}

		public static string[] GetSelectedValues(ListControl ctl)
		{
			ArrayList al = new ArrayList();
			foreach (ListItem li in ctl.Items)
				if (li.Selected)
					al.Add(li.Value);
	
			return (string[]) CollectionUtils.ToArray(typeof(string), al);
		}
		
		public static bool AddParsedSubObjectReplacement(IListControl ctl, object obj)
		{
			if (obj is FixedListItem) 
			{
				switch (((FixedListItem) obj).Position)
				{
					case FixedListItemPosition.First:
						ctl.FixedItemsFirst.Add(obj);
						break;
					case FixedListItemPosition.Last:
						ctl.FixedItemsLast.Add(obj);
						break;
					default:
						throw new InvalidOperationException("Invalid FixedListItem position.");
				}
				return true;
			}
			else if (obj is ListItem) 
			{
				ctl.Items.Add((ListItem) obj);
				return true;
			}
			else
				return false;
		}

		public static void InsertFixedItems(IListControl ctl)
		{
			int i = 0;
			int prevIndex = (ctl is DropDownList ? ((DropDownList) ctl).SelectedIndex : -1);
			foreach (FixedListItem li in ctl.FixedItemsFirst)
			{
				ListItem newItem = new ListItem(li.Text, li.Value);
				newItem.Selected = false;
				ctl.Items.Insert(i++, newItem);
			}

			foreach (FixedListItem li in ctl.FixedItemsLast)
			{
				ListItem newItem = new ListItem(li.Text,  li.Value);
				newItem.Selected = false;
				ctl.Items.Add(newItem);
			}

			if (ctl is DropDownList)
				((DropDownList) ctl).SelectedIndex = prevIndex + i;
		}

		public static void AddExtendedListItemProperties(ISelectControl ctl)
		{
			if (ctl.DataSource == null ||
				(ctl.DataAutoFillFields == null || ctl.DataAutoFillFields.Length == 0) && 
				(ctl.DataGroupTextField == null || ctl.DataGroupTextField.Length == 0) && 
				(ctl.DataDetailIdFields == null || ctl.DataDetailIdFields.Length == 0 ))
				return;

			int i = 0;
			foreach (object obj in GetResolvedDataSource(ctl.DataSource, ctl.DataMember))
			{
				if (ctl.DataAutoFillFields != null && ctl.DataAutoFillFields.Length > 0)
					ctl.Items[i].Attributes.Add("jsmasterdetail:autofill-data", BuildFieldData(obj, ctl.DataAutoFillFields));
				if (ctl.DataGroupTextField != null && ctl.DataGroupTextField.Length > 0)
					ctl.Items[i].Attributes.Add("-group-name", Convert.ToString(DataBinder.GetPropertyValue(obj, ctl.DataGroupTextField)));
				if (ctl.DataDetailIdFields != null && ctl.DataDetailIdFields.Length > 0)
					ctl.Items[i].Attributes.Add("jsmasterdetail:detail-data", BuildFieldData(obj, ctl.DataDetailIdFields));
				i++;
			}
		}
		
		public static void RenderSelectOptions(ISelectControl ctl, HtmlTextWriter writer)
		{
			bool alreadySelected = false;
			bool enableGrouping = ctl.DataGroupTextField != null && ctl.DataGroupTextField.Length > 0;
			string lastGroup = null;
			for (int i = 0; i < ctl.Items.Count; i++)
			{
				ListItem li = ctl.Items[i];
				if (enableGrouping && li.Attributes["-group-name"] != lastGroup) 
				{
					if (i > 1)
						writer.WriteEndTag("optgroup");
					lastGroup = li.Attributes["-group-name"];
					writer.WriteBeginTag("optgroup");
					writer.WriteAttribute("label", lastGroup, true);
					writer.Write(HtmlTextWriter.TagRightChar);
				}

				writer.WriteBeginTag("option");
				if (li.Selected)
				{
					if (alreadySelected && ctl.SelectionMode != ListSelectionMode.Multiple)
						throw new InvalidProgramException(String.Format("Multiple ListItems selected in {0} '{1}'", ctl.GetType().FullName, ctl.ID));
					alreadySelected = true;
					writer.WriteAttribute("selected", "true", false);
				}
				writer.WriteAttribute("value", li.Value, true);

				foreach (string attrKey in li.Attributes.Keys)
					if (!attrKey.StartsWith("-"))
						writer.WriteAttribute(attrKey, li.Attributes[attrKey], true);
				
				writer.Write(HtmlTextWriter.TagRightChar);
				HttpUtility.HtmlEncode(li.Text, writer);
				writer.WriteEndTag("option");
				writer.WriteLine();
			}

			if (enableGrouping && ctl.Items.Count > 1)
				writer.WriteEndTag("optgroup");
		}
		
		private static string BuildFieldData(object obj, string fieldList)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string fld in fieldList.Split(','))
				sb.AppendFormat("{0}={1},", fld, Convert.ToString(DataBinder.GetPropertyValue(obj, fld)));
			sb.Length -= 1;
			return sb.ToString();
		}
		
		private static IEnumerable GetResolvedDataSource(object dataSource, string dataMember)
		{
			if (dataSource != null)
			{
				IListSource source1 = dataSource as IListSource;
				if (source1 != null)
				{
					IList list1 = source1.GetList();
					if (!source1.ContainsListCollection)
					{
						return list1;
					}
					if ((list1 != null) && (list1 is ITypedList))
					{
						ITypedList list2 = (ITypedList) list1;
						PropertyDescriptorCollection collection1 = list2.GetItemProperties(new PropertyDescriptor[0]);
						if ((collection1 == null) || (collection1.Count == 0))
						{
							throw new HttpException("ListSource_Without_DataMembers");
						}
						PropertyDescriptor descriptor1 = null;
						if ((dataMember == null) || (dataMember.Length == 0))
						{
							descriptor1 = collection1[0];
						}
						else
						{
							descriptor1 = collection1.Find(dataMember, true);
						}
						if (descriptor1 != null)
						{
							object obj1 = list1[0];
							object obj2 = descriptor1.GetValue(obj1);
							if ((obj2 != null) && (obj2 is IEnumerable))
							{
								return (IEnumerable) obj2;
							}
						}
						throw new HttpException("ListSource_Missing_DataMember");
					}
				}
				if (dataSource is IEnumerable)
				{
					return (IEnumerable) dataSource;
				}
			}
			return null;
		}

		public static void AddExtendedSelectControlAttributesToRender(ISelectControl ctl, HtmlTextWriter writer)
		{
			ctl.Attributes["onchange"] += "if (typeof(JsMasterDetail) != 'undefined') { if (!JsMasterDetail.handleEvent(this)) if(typeof(MasterDetail_MasterChanged) != 'undefined') MasterDetail_MasterChanged(this); } else if (typeof(MasterDetail_MasterChanged) != 'undefined') MasterDetail_MasterChanged(this);";
			if (ctl.SourceWebService != null)
				ctl.Attributes["jsdatabind:source-ws"] = new NormalizeControlNames(ctl).Normalize(ctl.SourceWebService);
		}

		/// <summary>
		/// </summary>
		/// <remarks>
		/// Esta classe não será mais necessária se pudermos utilizar "closures",
		/// como no C# 2.0 ou na linguagem Boo.
		/// 
		/// Código em Boo:
		/// <code>
		///		def NormalizeControlNames(ctl as Control, expr as string):
		///			/(\$\{)([^}]+)(\})/.Replace(expr, def(m as Match):
		///				id = m.Groups[2].Value
		///				trueCtl = ctl.Parent.FindControl(id)
		///				id = trueCtl.ClientID if trueCtl
		///				return m.Result("$1${id}$3")
		///			)
		/// </code>
		/// </remarks>
		private sealed class NormalizeControlNames
		{
			static readonly Regex rx = new Regex(@"(\$\{)([^}]+)(\})", RegexOptions.Compiled);
			readonly Control ctl;

			public NormalizeControlNames(Control ctl)
			{
				this.ctl = ctl;
			}

			public NormalizeControlNames(IListControl ctl)
				: this((Control) ctl) { }

			public string Normalize(string expr)
			{
				return rx.Replace(expr, new MatchEvaluator(Evaluator));
			}

			private string Evaluator(Match m)
			{
				string ctlID = m.Groups[2].Value;
				Control trueCtl = ctl.Parent.FindControl(ctlID);
				string newID = (trueCtl != null ? trueCtl.ClientID : ctlID);
				return m.Result("$1" + newID + "$3");
			}
		}
	}
}
