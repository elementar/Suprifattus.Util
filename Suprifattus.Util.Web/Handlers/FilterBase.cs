using System;
using System.IO;

namespace Suprifattus.Util.Web.Handlers
{
	public abstract class FilterBase : Stream
	{
		long position;

		protected Stream responseStream;

		public FilterBase(Stream inputStream)
		{
			responseStream = inputStream;
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void Close()
		{
			responseStream.Close();
		}

		public override void Flush()
		{
			responseStream.Flush();
		}

		public override long Length
		{
			get { return 0; }
		}

		public override long Position
		{
			get { return position; }
			set { position = value; }
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return responseStream.Seek(offset, origin);
		}

		public override void SetLength(long length)
		{
			responseStream.SetLength(length);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return responseStream.Read(buffer, offset, count);
		}
	}
}