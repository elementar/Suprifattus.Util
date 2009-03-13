using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Xml;

namespace Suprifattus.Util.Web.Controls.Data
{
	[ToolboxData(@"<{0}:DataRecord runat=""server"" ItemSource=""<%# Container.DataItem %>""/>")]
	public class DataRecord : Control
	{
		#region struct RecordData
		[Serializable]
		struct RecordData
		{
			[Serializable]
			public struct RecordDataItem
			{
				public string Field;
				public string Value;
				public string XsdType;
			}

			public string Id;
			public RecordDataItem[] Items;
		}
		#endregion

		public const string DefaultXmlNsPrefix = "jsdatabind";
		
		private object itemSource;
		private string xmlNsPrefix = DefaultXmlNsPrefix;
		private RecordData data;
		private bool indent = false;

		[Bindable(true), Category("Data"), DefaultValue(null)] 
		public object ItemSource 
		{
			get { return itemSource; }
			set { itemSource = value; }
		}

		[Bindable(true), Category("Data"), DefaultValue(DefaultXmlNsPrefix)]
		public string XmlNamespacePrefix 
		{
			get { return xmlNsPrefix; }
			set { xmlNsPrefix = value; }
		}

		[Bindable(true), Category("Appearance"), DefaultValue(false)]
		public bool Indent
		{
			get { return indent; }
			set { indent = value; }
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			data = (RecordData) ViewState["RecordData"];
		}

		protected string XmlRepres(object obj) 
		{
			if (obj is DateTime) return XmlConvert.ToString((DateTime) obj, "dd/MM/yyyy");
			if (obj is int) return XmlConvert.ToString((int) obj);
			if (obj is long) return XmlConvert.ToString((long) obj);
			if (obj is Guid) return XmlConvert.ToString((Guid) obj);
			if (obj is TimeSpan) return XmlConvert.ToString((TimeSpan) obj);
			if (obj is double) return XmlConvert.ToString((double) obj);
			if (obj is float) return XmlConvert.ToString((float) obj);
			if (obj is long) return XmlConvert.ToString((long) obj);
			return XmlEscape(Convert.ToString(obj));
		}

		protected string XmlEscape(string s)
		{
			return s
				.Replace("&",  "&amp;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;")
				.Replace("\"", "&quot;")
				.Replace("'",  "&#39;")
				.Replace("\r\n", "&#10;")
				.Replace("\n", "&#10;");
		}

		protected string XsdType(Type t)
		{
			string r = t.Name;
			r = Char.ToLower(r[0]) + r.Substring(1);

			switch (r) 
			{
				case "timeSpan": r = "duration"; break;
				case "sByte": r = "byte"; break;
				case "byte": r = "unsignedByte"; break;
				case "uInt16": r = "unsignedShort"; break;
				case "uInt32": r = "unsignedInt"; break;
				case "uInt64": r = "unsignedLong"; break;
				case "int16": r = "short"; break;
				case "int32": r = "int"; break;
				case "int64": r = "long"; break;
			}
			return r;
		}
		
		public override void DataBind()
		{
			base.DataBind();

			DataRowView drv = ItemSource as DataRowView;
			if (drv == null)
				return;

			DataColumnCollection drvCols = drv.Row.Table.Columns;

			data = new RecordData();

			if (drv.Row.Table.PrimaryKey.Length > 0)
				data.Id = Convert.ToString(drv.Row[drv.Row.Table.PrimaryKey[0]]);

			data.Items = new RecordData.RecordDataItem[drvCols.Count];

			for (int i = 0; i < drvCols.Count; i++) 
			{ 
				data.Items[i].Field = drvCols[i].ColumnName;
				data.Items[i].Value = XmlRepres(drv.Row[drvCols[i]]);
				data.Items[i].XsdType = XsdType(drvCols[i].DataType);
			}

			ViewState["RecordData"] = data;
		}
		
		protected override void Render(HtmlTextWriter output)
		{
			if (data.Items == null) 
				return;

			output.WriteBeginTag(XmlNamespacePrefix + ":Record");
		
			if (data.Id != null && data.Id.Length > 0)
				output.WriteAttribute("Id", data.Id);
		
			output.Write(HtmlTextWriter.TagRightChar);
			if (indent)
			{
				output.WriteLine();
				output.Indent++;
			}

			foreach (RecordData.RecordDataItem item in data.Items) 
			{
				output.WriteBeginTag(XmlNamespacePrefix + ":" + item.Field);
				output.WriteAttribute("xsi:type", "xsd:" + item.XsdType);
				output.WriteAttribute("Value", item.Value, false);
				output.Write(HtmlTextWriter.SelfClosingTagEnd);
				if (indent)
					output.WriteLine();
			}

			if (indent)
			{
				output.Indent--;
			}

			output.WriteEndTag(XmlNamespacePrefix + ":Record");
		}
	}
}
