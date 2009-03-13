using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.ComponentModel;

namespace Suprifattus.Util.Web.Controls
{
	using Util;
	using Util.Collections;

	public enum FlashQuality 
	{
		High, Medium, Low
	}
	
	[CLSCompliant(false)]
	public class NameValue : KeyValue
	{
		public NameValue() : base(null, null) { }

		public string Name { get { return Key; } set { Key = value; } }
	}
	
	public class FlashMovieControlBuilder : ControlBuilder
	{
		public override Type GetChildControlType(string tagName, IDictionary attribs)
		{
			if (tagName == "FlashVar")
				return typeof(NameValue);
			if (tagName == "AlternateContent")
				return typeof(Literal);
			return null;
		}
	}
	
	/// <summary>
	/// Apresenta uma apresentação flash.
	/// </summary>
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:FlashMovie runat=server></{0}:FlashMovie>")]
	[ParseChildren(false)]
	[ControlBuilder(typeof(FlashMovieControlBuilder))]
	public class FlashMovie : System.Web.UI.Control
	{
		private Unit width, height;
		private string flashMovie;
		private FlashQuality flashQuality;
		private IList flashVars = new ArrayList(), flashParams = new ArrayList();
		private string alternateContent = "";

		private KeyValueFormatter nameValueFormatter = new KeyValueFormatter("{0}={1}");
	
		#region Public Properties
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(200)]
		public Unit Width 
		{
			get { return width; }
			set { width = value; }
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(200)]
		public Unit Height 
		{
			get { return height; }
			set { height = value; }
		}

		[Bindable(true)]
		[Category("Flash")]
		[DefaultValue(FlashQuality.High)]
		public FlashQuality Quality 
		{
			get { return flashQuality; }
			set { flashQuality = value; }
		}

		[Bindable(true)]
		[Category("Flash")]
		[DefaultValue("")]
		public string Movie 
		{
			get { return flashMovie; }
			set { flashMovie = value; }
		}

		[Bindable(true)]
		[Category("Flash")]
		[DefaultValue(null)]
		public IList FlashVar 
		{
			get { return flashVars; }
			set { flashVars = value; }
		}

		[Bindable(true)]
		[Category("Flash")]
		[DefaultValue(null)]
		public IList FlashParam 
		{
			get { return flashParams; }
			set { flashParams = value; }
		}
		#endregion
		
		protected override void AddParsedSubObject(object obj)
		{
			if (obj is NameValue)
				flashVars.Add(obj);
			if (obj is Literal)
				alternateContent += ((Literal) obj).Text;
		}
		
		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to</param>
		protected override void Render(HtmlTextWriter output)
		{
			output.Indent++;
			output.WriteBeginTag("object");
			output.WriteAttribute("type", "application/x-shockwave-flash");
			output.WriteAttribute("data", flashMovie, true);
			output.WriteAttribute("width", Width.ToString());
			output.WriteAttribute("height", Height.ToString());
			//output.WriteAttribute("id", ClientID);
			output.Write(HtmlTextWriter.TagRightChar);
			output.WriteLine();

			output.Indent++;

			WriteParam(output, "allowScriptAccess", "sameDomain");
			WriteParam(output, "movie", flashMovie, true);
			
			if (flashVars != null)
				WriteParam(output, "flashvars", CollectionUtils.Join(flashVars, "&", nameValueFormatter), true);
			
			WriteParam(output, "quality", flashQuality.ToString().ToLower());
			WriteParam(output, "wmode", "transparent");
			WriteParam(output, "wmode", "opaque");
			WriteParam(output, "scale", "noscale");
			WriteParam(output, "salign", "LT");

			output.Indent--;

			output.Write(alternateContent);

			output.WriteEndTag("object");
			output.Indent--;
		}

		private void WriteParam(HtmlTextWriter output, string name, string value) 
		{
			WriteParam(output, name, value, false);
		}
			
		private void WriteParam(HtmlTextWriter output, string name, string value, bool encodeValue) 
		{
			output.WriteBeginTag("param");
			output.WriteAttribute("name", name);
			output.WriteAttribute("value", value, encodeValue);
			output.Write(HtmlTextWriter.SelfClosingTagEnd);
			output.WriteLine();
		}
	}
}
