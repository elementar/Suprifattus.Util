using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;

using Fonet;

using Suprifattus.Util.IO;

namespace Suprifattus.Util.PdfGeneration
{
	public class FoNetPdfGenerator : IPdfGenerator
	{
		private readonly Regex rxPages = new Regex(@"produced (\d+) page", RegexOptions.Compiled);

		public void SetConfiguration(string key, object value)
		{
		}

		public int Generate(XmlDocument sourceDocument, Stream output)
		{
			var driver = FonetDriver.Make();
			var pageCount = 0;
			driver.OnInfo += (s, i) => UpdatePageCountHack(i, ref pageCount);
			driver.Render(sourceDocument, output);

			return pageCount;
		}

		public int Generate(XslCompiledTransform transform, XmlDocument sourceDocument, Stream output)
		{
			var source = new CircularStream();
			XmlWriter w = new XmlTextWriter(source, Encoding.Default);

			transform.Transform(sourceDocument, w);

			return Generate(source, output);
		}

		public int Generate(Stream source, Stream output)
		{
			var driver = FonetDriver.Make();
			var pageCount = 0;
			driver.OnInfo += (s, i) => UpdatePageCountHack(i, ref pageCount);
			driver.Render(source, output);

			return pageCount;
		}

		private int UpdatePageCountHack(FonetEventArgs i, ref int pages)
		{
			var m = rxPages.Match(i.GetMessage());
			if (m != null && m.Success)
				pages += Convert.ToInt32(m.Groups[1].Value);
			return pages;
		}
	}
}