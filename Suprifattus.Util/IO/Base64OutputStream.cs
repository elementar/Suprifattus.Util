//#define SAFE

using System;
using System.IO;
using System.Xml;

namespace Suprifattus.Util.IO
{
	public class Base64OutputStream : Stream
	{
#if SAFE
		private readonly MemoryStream ms = new MemoryStream();
#endif

		private readonly XmlWriter xmlWriter;
		private readonly TextWriter textWriter;

		public Base64OutputStream(XmlWriter writer)
		{
			this.xmlWriter = writer;
		}

		public Base64OutputStream(TextWriter writer)
		{
			this.textWriter = writer;
		}

		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void Flush()
		{
#if SAFE
			byte[] allBytes = ms.ToArray();
#endif

			if (xmlWriter != null)
			{
#if SAFE
				xmlWriter.WriteBase64(allBytes, 0, allBytes.Length);
#endif
				xmlWriter.Flush();
			}

			if (textWriter != null)
			{
#if SAFE
				textWriter.Write(Convert.ToBase64String(allBytes));
#endif
				textWriter.Flush();
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
#if SAFE
			ms.Write(buffer, offset, count);
#else
			if (xmlWriter != null)
				xmlWriter.WriteBase64(buffer, offset, count);
			if (textWriter != null)
				textWriter.Write(Convert.ToBase64String(buffer, offset, count));
#endif
		}

#if SAFE
		public override void WriteByte(byte value)
		{
			ms.WriteByte(value);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
				ms.Dispose();
		}
#endif

		#region Not implemented methods
		public override long Length
		{
			get { throw new NotImplementedException(); }
		}

		public override long Position
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}