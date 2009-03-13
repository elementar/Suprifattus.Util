using System;
using System.IO;

namespace Suprifattus.Util.IO
{
	public sealed class Streams
	{
		private const int DefaultBufferSize = 1024 * 20;

		public static int SaveStream(Stream from, Stream to)
		{
			return SaveStream(DefaultBufferSize, from, to);
		}
		
		public static int SaveStream(int bufSize, Stream from, Stream to)
		{
			var buf = new byte[bufSize];
			int written = 0;
			int l;
			while ((l = from.Read(buf, 0, buf.Length)) > 0)
			{
				to.Write(buf, 0, l);
				written += l;
			}

			return written;
		}

		public static int SaveStream(Stream from, Stream to, int limit)
		{
			return SaveStream(DefaultBufferSize, from, to, limit);
		}

		public static int SaveStream(int bufSize, Stream from, Stream to, int limit)
		{
			var buf = new byte[bufSize];
			int written = 0;
			int l;
			while (written < limit && (l = from.Read(buf, 0, buf.Length)) > 0)
			{
				if (written + l > limit)
					l = limit - written;
				to.Write(buf, 0, l);
				written += l;
			}

			return written;
		}
	}
}