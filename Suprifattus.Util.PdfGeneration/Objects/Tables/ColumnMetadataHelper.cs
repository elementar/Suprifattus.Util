using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;

namespace Suprifattus.Util.PdfGeneration.Objects.Tables
{
	/// <summary>
	/// Classe que auxilia no preenchimento de metadados de colunas.
	/// </summary>
	public class ColumnMetadataHelper
	{
		private readonly XmlDocument doc;

		#region Constructors
		/// <summary>
		/// Cria um novo auxiliar de metadados de colunas, baseado em
		/// metadados embutidos em um resource.
		/// </summary>
		/// <param name="asm">O assembly que contém o resource</param>
		/// <param name="resPath">O caminho para o resource</param>
		public ColumnMetadataHelper(Assembly asm, string resPath)
			: this(new XmlDocument())
		{
			using (var s = asm.GetManifestResourceStream(resPath))
				doc.Load(s);
		}

		/// <summary>
		/// Cria um novo auxiliar de metadados de colunas.
		/// </summary>
		/// <param name="s">O stream de onde vem os metadados</param>
		public ColumnMetadataHelper(Stream s)
			: this(new XmlDocument())
		{
			doc.Load(s);
		}

		/// <summary>
		/// Cria um novo auxiliar de metadados de colunas.
		/// </summary>
		/// <param name="filename">O nome do arquivo XML contendo os metadados</param>
		public ColumnMetadataHelper(string filename)
			: this(new XmlDocument())
		{
			if (filename.StartsWith("~"))
				filename = HttpContext.Current.Server.MapPath(filename);

			doc.Load(filename);
		}

		/// <summary>
		/// Cria um novo auxiliar de metadados de colunas.
		/// </summary>
		/// <param name="doc">O documento que contém os metadados</param>
		public ColumnMetadataHelper(XmlDocument doc)
		{
			this.doc = doc;
		}
		#endregion

		/// <summary>
		/// Complementa o estilo de uma coluna, baseado nos metadados.
		/// </summary>
		/// <param name="column">A coluna</param>
		public void ComplementStyle(Column column)
		{
			var xpath = String.Format("/ReportMetadata/Fields/Field[@Name='{0}']", column.Expression);
			var el = doc.SelectSingleNode(xpath) as XmlElement;
			if (el != null)
			{
				var caption = el.GetAttributeNode("Caption");
				var width = el.GetAttributeNode("Width");
				var format = el.GetAttributeNode("FormatString");
				var nullValue = el.GetAttributeNode("NullValue");
				var textAlign = el.GetAttributeNode("TextAlignment");
				var headerAlign = el.GetAttributeNode("HeaderAlignment");

				if (caption != null)
					column.Header.Contents = caption.Value;

				if (width != null)
					column.Width = Convert.ToDouble(width.Value, CultureInfo.InvariantCulture);

				if (format != null)
					column.FormatString = format.Value;

				if (nullValue != null)
					column.NullValue = nullValue.Value;

				if (textAlign != null)
					column.Style.Alignment = (TextAlignment) Enum.Parse(typeof(TextAlignment), textAlign.Value, true);

				if (headerAlign != null)
					column.Header.Style.Alignment = (TextAlignment) Enum.Parse(typeof(TextAlignment), headerAlign.Value, true);

				foreach (XmlElement map in el.SelectNodes("Map"))
					column.MapValue(map.GetAttribute("Value"), map.GetAttribute("To"));
			}
			else
				throw new ArgumentException(String.Format("Report Metadata for column named {0} not found.", column.Expression));
		}
	}
}