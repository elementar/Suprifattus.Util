using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Suprifattus.Util.PdfGeneration
{
	/// <summary>
	/// Generates a PDF document.
	/// </summary>
	public interface IPdfGenerator
	{
		/// <summary>
		/// Sets a configuration setting.
		/// </summary>
		void SetConfiguration(string key, object value);

		/// <summary>
		/// Generates a PDF document on the <paramref name="output"/>, and then
		/// return the number of the generated pages.
		/// </summary>
		int Generate(XmlDocument sourceDocument, Stream output);

		/// <summary>
		/// Generates a PDF document on the <paramref name="output"/>, and then
		/// return the number of the generated pages.
		/// </summary>
		int Generate(XslCompiledTransform transform, XmlDocument sourceDocument, Stream output);

		/// <summary>
		/// Generates a PDF document on the <paramref name="output"/>, and then
		/// return the number of the generated pages.
		/// </summary>
		int Generate(Stream source, Stream output);
	}
}