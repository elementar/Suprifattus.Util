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

namespace Suprifattus.Util.IO
{
	/// <summary>
	/// Represents a circular byte data buffer - a growable queue of raw bytes.
	/// </summary>
	public class CircularBuffer
	{
		/// <summary>
		/// Specifies the default buffer size.
		/// </summary>
		public const int DefaultSize = 10240;

		private byte[] data;

		private int putIndex;

		/// <summary>
		/// Initializes a new instance of <see cref="CircularBuffer"/> with
		/// the <see cref="DefaultSize"/> buffer.
		/// </summary>
		public CircularBuffer()
			: this(DefaultSize)
		{
		}

		/// <summary>
		/// Creates a new instance of the <see cref="CircularBuffer"/>
		/// providing the <paramref name="initialCapacity"/> as it's
		/// initial capacity.
		/// </summary>
		/// <param name="initialCapacity">
		/// The initial capacity of the buffer.
		/// </param>
		public CircularBuffer(int initialCapacity)
		{
			if (initialCapacity <= 0)
				throw new ArgumentOutOfRangeException("initialCapacity");

			data = new byte[initialCapacity];
			putIndex = 0;
		}

		/// <summary>
		/// Clears the <see cref="CircularBuffer"/>'s contents.
		/// </summary>
		public void Clear()
		{
			putIndex = 0;
		}

		private void GrowTo(int newCapacity)
		{
			var newData = new byte[newCapacity];
			Array.Copy(data, newData, data.Length);
			data = newData;
		}

		/// <summary>
		/// Puts an array of bytes to the queue.
		/// </summary>
		/// <param name="buffer">
		/// The array of bytes that contains the data to be put to the this.data.
		/// </param>
		/// <param name="offset">
		/// The offset of the first byte in the this.data.
		/// </param>
		/// <param name="length">
		/// The number of bytes to put to the this.data.
		/// </param>
		public void PutBytes(byte[] buffer, int offset, int length)
		{
			if (null == buffer)
				throw new ArgumentNullException("buffer");
			if (0 > offset)
				throw new ArgumentOutOfRangeException("offset");
			if (0 > length)
				throw new ArgumentOutOfRangeException("length");
			if (offset > buffer.Length)
				throw new ArgumentOutOfRangeException("offset");
			if (offset + length > buffer.Length)
				throw new ArgumentOutOfRangeException();

			if ((buffer.Length == 0) || (0 == length))
				return;

			if (putIndex + length > Capacity)
				GrowTo(Math.Max((int) (Capacity*1.3), putIndex + length));
			Array.Copy(buffer, offset, data, putIndex, length);
			putIndex += length;
		}

		/// <summary>
		/// Gets an array of bytes and puts them to
		/// <paramref name="buffer"/> array.
		/// </summary>
		/// <param name="buffer">The array that receives the data.</param>
		/// <param name="offset">The offset of the first byte in the array.</param>
		/// <param name="length">The length of the data.</param>
		/// <returns>
		/// The number of bytes actually put into <paramref name="buffer"/>.
		/// </returns>
		public int GetBytes(byte[] buffer, int offset, int length)
		{
			if (null == buffer)
				throw new ArgumentNullException("buffer");
			if (0 > offset)
				throw new ArgumentOutOfRangeException("offset");
			if (0 > length)
				throw new ArgumentOutOfRangeException("length");
			if (offset > buffer.Length)
				throw new ArgumentOutOfRangeException("offset");
			if (offset + length > buffer.Length)
				throw new ArgumentOutOfRangeException();

			if ((buffer.Length == 0) || (0 == length))
				return 0;

			int result = Math.Min(length, putIndex);
			Array.Copy(data, 0, buffer, offset, result);
			Array.Copy(data, length, data, 0, putIndex - length);
			putIndex -= length;
			return result;
		}

		/// <summary>
		/// Discards the specified number of bytes from the
		/// <see cref="CircularBuffer"/>.
		/// </summary>
		/// <param name="bytesCount">The number of bytes to discard.</param>
		public void DropBytes(int bytesCount)
		{
			if (0 > bytesCount)
				throw new ArgumentOutOfRangeException("bytesCount");

			if (0 == bytesCount)
				return;
			int result = Math.Min(bytesCount, putIndex);
			Array.Copy(data, bytesCount, data, 0, putIndex - bytesCount);
			putIndex -= bytesCount;
		}

		/// <summary>
		/// Get number of bytes in the buffer.
		/// </summary>
		/// <value>
		/// Gets the number of elements actually contained in
		/// the <see cref="CircularBuffer"/>.
		/// </value>
		public int Count
		{
			get { return putIndex; }
		}

		/// <summary>
		/// Gets the overall capacity of the buffer.
		/// </summary>
		/// <value>
		/// The number of bytes that the <see cref="CircularBuffer"/>
		/// can contain.
		/// </value>
		public int Capacity
		{
			get { return data.Length; }
		}

		/// <summary>
		/// Finds a delimiter in the <see cref="CircularBuffer"/>.
		/// </summary>
		/// <param name="delimiter">
		/// A sequense of the bytes used as the delimiter.
		/// </param>
		/// <returns>
		/// The index ob the first byte of the delimiter or -1 if
		/// no delimiter found.
		/// </returns>
		public int FindDelimiter(byte[] delimiter)
		{
			if (null == delimiter)
				throw new ArgumentNullException("delimiter", "Parameter cannot be null");
			if (putIndex < delimiter.Length)
				throw new ArgumentException("Delimiter cannot be longer that data being tested");
			if (delimiter.Length == 0)
				return 0; // Empty pattern is there

			for (int i = 0; i < data.Length - delimiter.Length + 1; i++)
			{
				if (data[i] == delimiter[0])
				{
					bool found = true;
					for (int j = 1; j < delimiter.Length; j++)
					{
						if (data[i + j] != delimiter[j])
						{
							found = false;
							break;
						}
					}
					if (found)
						return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Gives low level access to the underlying buffer.
		/// </summary>
		/// <returns>
		/// An array of bytes in the buffer.
		/// </returns>
		/// <remarks>
		/// Be very careful while accessing the buffer. Use <see cref="Count"/>
		/// value to determine the size of the data. Although the buffer can be
		/// bigger than <c>Count</c>, it contains garbage beyond it.
		/// </remarks>
		public byte[] GetBuffer()
		{
			return data;
		}
	}
}