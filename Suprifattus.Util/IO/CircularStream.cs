#region License
/*  Pvax.Net
    Copyright (c) 2005, Alexey A. Popov
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are
    permitted provided that the following conditions are met:

    - Redistributions of source code must retain the above copyright notice, this list
      of conditions and the following disclaimer.

    - Redistributions in binary form must reproduce the above copyright notice, this list
      of conditions and the following disclaimer in the documentation and/or other materials
      provided with the distribution.

    - Neither the name of the Alexey A. Popov nor the names of its contributors may be used to
      endorse or promote products derived from this software without specific prior written
      permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS *AS IS* AND ANY EXPRESS
    OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
    AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
    IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.IO;

namespace Suprifattus.Util.IO
{
	/// <summary>
	/// A <see cref="Stream"/> that uses a <see cref="CircularBuffer"/> as
	/// it's underlying storage.
	/// </summary>
	public class CircularStream : Stream
	{
		private readonly CircularBuffer buffer;

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularStream"/>
		/// class.
		/// </summary>
		public CircularStream()
		{
			buffer = new CircularBuffer();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularStream"/>
		/// class providing an existing <see cref="CircularBuffer"/> as
		/// it's underlying storage.
		/// </summary>
		/// <param name="buffer">A <see cref="CircularBuffer"/>.</param>
		public CircularStream(CircularBuffer buffer)
		{
			this.buffer = buffer;
		}

		/// <summary>
		/// Gets the underlying <see cref="CircularBuffer"/>.
		/// </summary>
		public CircularBuffer Buffer
		{
			get { return buffer; }
		}

		#region Stream abstract class implementation
		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		/// <value>
		/// Always <c>true</c>.
		/// </value>
		public override bool CanRead
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		/// <value>
		/// Always <c>false</c>.
		/// </value>
		public override bool CanSeek
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <value>
		/// Always <c>true</c>.
		/// </value>
		public override bool CanWrite
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		/// <remarks>
		/// This implementation returns a number of bytes accumulated in
		/// the underlying storage.
		/// </remarks>
		public override long Length
		{
			get { return buffer.Count; }
		}

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		/// <remarks>
		/// It is not possible to change the position in a circular stream, so
		/// access to this property always throws a
		/// <see cref="NotSupportedException"/>.
		/// </remarks>
		/// <exception cref="NotSupportedException">
		/// Always thrown by design.
		/// </exception>
		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances
		/// the current position within this stream by the number of bytes
		/// written.
		/// </summary>
		/// <param name="buffer">An array of bytes.</param>
		/// <param name="offset">The zero-based byte offset in
		/// <paramref name="buffer"/> at which to begin copying bytes to
		/// the current stream.</param>
		/// <param name="count">The number of bytes to be written to
		/// the current stream.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.buffer.PutBytes(buffer, offset, count);
		}

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances
		/// the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">An array of bytes.</param>
		/// <param name="offset">The zero-based byte offset in
		/// <paramref name="buffer"/> at which to begin storing the data read
		/// from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from
		/// the current stream.</param>
		/// <returns>The total number of bytes read into the buffer.</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			int length = Math.Min(this.buffer.Count, count);
			return this.buffer.GetBytes(buffer, offset, length);
		}

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">
		/// The desired length of the current stream in bytes.
		/// </param>
		/// <exception cref="NotSupportedException">
		/// Always thrown by design.
		/// </exception>
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">
		/// A byte offset relative to the <paramref name="origin"/> parameter.
		/// </param>
		/// <param name="origin">
		/// A value of type <see cref="SeekOrigin"/> indicating the reference
		/// point used to obtain the new position.
		/// </param>
		/// <returns>The new position within the current stream.</returns>
		/// <exception cref="NotSupportedException">
		/// Always thrown by design.
		/// </exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Clears all buffers for this stream and causes any buffered data
		/// to be written to the underlying device.
		/// </summary>
		/// <remarks>
		/// This implementation does nothing.
		/// </remarks>
		public override void Flush()
		{
		}
		#endregion
	}
}