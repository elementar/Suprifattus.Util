using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using Commons.Web.UI.Controls.Persistence;

using Suprifattus.Util.Reflection;

namespace Suprifattus.Util.Web.Controls.Helpers
{
	[Serializable]
	public class WebFormParser
	{
		Hashtable ht;
		[NonSerialized] Control ctl;
		object data;

		public WebFormParser()
			: this(true) { }
		
		public WebFormParser(bool saveParsedValues)
		{
			if (saveParsedValues)
				ht = new Hashtable();
		}
		
		public void Reset()
		{
			if (ht != null)
				ht.Clear();
		}
		
		public WebFormParser Parse(Control parent, string childID)
		{
			return Parse(parent.FindControl(childID));
		}
		
		public WebFormParser Parse(Control ctl)
		{
			this.ctl = ctl;
			this.data = null;

			if (ht != null)
				if (ctl is IPersistentControl)
					ht[ctl.ClientID] = ((IPersistentControl) ctl).SaveState();

			return this;
		}

		public void ReloadControlValues(Page page)
		{
			if (ht == null)
				throw new InvalidOperationException("This WebFormParser is not saving parsed values. Reloading is not allowed.");

			foreach (IPersistentControl ctl in ObjectQuery.SelectRecursive(page.Controls, "Controls", new TypeCondition(typeof(IPersistentControl))))
				if (ht.ContainsKey(ctl.ClientID))
					ctl.LoadState(ht[ctl.ClientID]);
		}

		public bool IsEmpty
		{
			get { return ControlUtil.IsEmpty(ctl); }
		}

		#region Simple Acessors
		public string String
		{
			get
			{
				if (!(data is string))
					data = ControlUtil.GetString(ctl);
					
				return (string) data;
			}
		}

		public string[] StringArray
		{
			get
			{
				if (!(data is string[]))
					data = ControlUtil.GetStringArray((ListControl) ctl);
					
				return (string[]) data;
			}
		}

		public DateTime Date
		{
			get
			{
				if (!(data is DateTime))
					data = ControlUtil.GetDate(ctl);
					
				return (DateTime) data;
			}
		}

		public byte Byte
		{
			get
			{
				if (!(data is byte))
					data = ControlUtil.GetByte(ctl);

				return (byte) data;
			}
		}

		public Decimal Decimal
		{
			get
			{
				if (!(data is Decimal))
					data = ControlUtil.GetDecimal(ctl);

				return (Decimal) data;
			}
		}

		public Int32 Int32
		{
			get
			{
				if (!(data is Int32))
					data = ControlUtil.GetInt32(ctl);

				return (Int32) data;
			}
		}

		public Int32[] Int32Array
		{
			get
			{
				if (!(data is Int32[]))
					data = ControlUtil.GetInt32Array((ListControl) ctl);

				return (Int32[]) data;
			}
		}

		public Int16 Int16
		{
			get
			{
				if (!(data is Int16))
					data = ControlUtil.GetInt16(ctl);

				return (Int16) data;
			}
		}

		public Int16[] Int16Array
		{
			get
			{
				if (!(data is Int16[]))
					data = ControlUtil.GetInt16Array((ListControl) ctl);

				return (Int16[]) data;
			}
		}

		public bool Boolean
		{
			get
			{
				if (!(data is Boolean))
					data = ControlUtil.GetBoolean(ctl);

				return (bool) data;
			}
		}

		public Enum Enum(Type enumType)
		{
			if (!(data is Enum))
				data = ControlUtil.GetEnum(ctl, enumType);

			return (Enum) data;
		}
		#endregion

		#region Extended Acessors
		public string Digits
		{
			get { return Regex.Replace(this.String, @"\D+", String.Empty); }
		}
		#endregion
	}
}
