#if APACHE
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using java.io;

using org.apache.fop.apps;
using org.apache.fop.configuration;
using org.xml.sax;

using Suprifattus.Util.IO;

namespace Suprifattus.Util.PdfGeneration
{
	public class XslFoPdfGenerator : IPdfGenerator
	{
		public void SetConfiguration(string key, object value)
		{
			Configuration.put(key, value);
		}

		public int Generate(XmlDocument sourceDocument, Stream output)
		{
			var source = new CircularStream();
			XmlWriter w = new XmlTextWriter(source, Encoding.Default);
			sourceDocument.WriteTo(w);

			return Generate(source, output);
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
			var jIn = new InputSource(GetReader(source));
			OutputStream jOut = new StreamOutputStream(output);
			var driver = new Driver(jIn, jOut);
			driver.setRenderer(Driver.RENDER_PDF);
			driver.run();
			var pageCount = driver.getResults().getPageCount();
			jOut.flush();

			return pageCount;
		}

		private Reader GetReader(Stream source)
		{
			return new InputStreamReader(new StreamInputStream(source), "UTF8");
		}

		#region StreamOutputStream
		private class StreamOutputStream : OutputStream
		{
			private readonly Stream s;

			public StreamOutputStream(Stream s)
			{
				this.s = s;
			}

			public override void write(int b)
			{
				s.WriteByte((byte) b);
			}
		}
		#endregion

		#region StreamInputStream
		private class StreamInputStream : InputStream
		{
			private readonly Stream s;
			private readonly byte[] buf = new byte[1];

			public StreamInputStream(Stream s)
			{
				this.s = s;
			}

			public override int read()
			{
				var c = s.Read(buf, 0, 1);
				if (c == 0)
					return -1;
				return buf[0];
			}

			public override int read(sbyte[] b)
			{
				return read(b, 0, b.Length);
			}

			public override int read(sbyte[] b, int off, int count)
			{
				var bc = new byte[count];
				var n = s.Read(bc, off, count);
				for (var i = 0; i < count; i++)
					b[off++] = (sbyte) bc[i];
				return n;
			}

			public override int available()
			{
				return 0;
			}
		}
		#endregion
	}
}
#endif
