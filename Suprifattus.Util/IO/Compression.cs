using System;
using System.IO;
using System.Text;

namespace Suprifattus.Util.IO
{
	/// <summary>
	/// Tipos de compactação suportados
	/// </summary>
	public enum CompressionType
	{
		/// <summary>
		/// GZip
		/// </summary>
		GZip,
		/// <summary>
		/// BZip2
		/// </summary>
		BZip2,
		/// <summary>
		/// Zip
		/// </summary>
		Zip
	}

	/// <summary>
	/// Realiza compactação e descompactação, permitindo selecionar o algoritmo.
	/// </summary>
	public class Compression
	{
		/// <summary>
		/// Qual o tipo de compactação que será utilizado?
		/// </summary>
		public CompressionType CompressionProvider;

		public Compression()
			: this(CompressionType.BZip2)
		{
		}

		public Compression(CompressionType compressionProvider)
		{
			this.CompressionProvider = compressionProvider;
		}

		private Stream OutputStream(Stream inputStream)
		{
			const string asm = "ICSharpCode.SharpZipLib";
			object[] p = new object[] {inputStream};

			switch (CompressionProvider)
			{
				case CompressionType.BZip2:
					return (Stream) Activator.CreateInstance(asm, asm + ".BZip2.BZip2OutputStream", p).Unwrap();
				case CompressionType.GZip:
					return (Stream) Activator.CreateInstance(asm, asm + ".GZip.GZipOutputStream", p).Unwrap();
				case CompressionType.Zip:
					return (Stream) Activator.CreateInstance(asm, asm + ".Zip.ZipOutputStream", p).Unwrap();
				default:
					throw new Exception("Tipo de compressão não suportado: " + CompressionProvider);
			}
		}

		private Stream InputStream(Stream inputStream)
		{
			const string asm = "ICSharpCode.SharpZipLib";
			object[] p = new object[] {inputStream};

			switch (CompressionProvider)
			{
				case CompressionType.BZip2:
					return (Stream) Activator.CreateInstance(asm, asm + ".BZip2.BZip2InputStream", p).Unwrap();
				case CompressionType.GZip:
					return (Stream) Activator.CreateInstance(asm, asm + ".GZip.GZipInputStream", p).Unwrap();
				case CompressionType.Zip:
					return (Stream) Activator.CreateInstance(asm, asm + ".Zip.ZipInputStream", p).Unwrap();
				default:
					throw new Exception("Tipo de compressão não suportado: " + CompressionProvider);
			}
		}

		/// <summary>
		/// Compacta um conjunto de bytes.
		/// </summary>
		/// <param name="bytesToCompress">Os bytes a serem compactados</param>
		/// <returns>Um array com os bytes compactados</returns>
		public byte[] Compress(byte[] bytesToCompress)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (Stream s = OutputStream(ms))
					s.Write(bytesToCompress, 0, bytesToCompress.Length);
				return ms.ToArray();
			}
		}

		/// <summary>
		/// Compacta uma string em uma string codificada em Base64.
		/// </summary>
		/// <param name="stringToCompress">A String a ser compactada</param>
		/// <returns>Uma string codificada em Base64 contendo o resultado da compactação</returns>
		public string CompressToBase64(string stringToCompress)
		{
			byte[] compressedData = CompressToByte(stringToCompress);
			string strOut = Convert.ToBase64String(compressedData);
			return strOut;
		}

		/// <summary>
		/// Compacta um Stream em uma string codificada em Base64.
		/// </summary>
		/// <param name="streamToCompress">O Stream a ser compactado</param>
		/// <returns>Uma string codificada em Base64 contendo o resultado da compactação</returns>
		public string CompressToBase64(Stream streamToCompress)
		{
			byte[] inBytes = new byte[streamToCompress.Length];
			streamToCompress.Read(inBytes, 0, inBytes.Length);
			return CompressToBase64(inBytes);
		}

		/// <summary>
		/// Compacta um array de bytes em uma string codificada em Base64.
		/// </summary>
		/// <param name="bytesToCompress">O array de bytes a ser compactado</param>
		/// <returns>Uma string codificada em Base64 contendo o resultado da compactação</returns>
		public string CompressToBase64(byte[] bytesToCompress)
		{
			return Convert.ToBase64String(Compress(bytesToCompress));
		}

		/// <summary>
		/// Compacta uma string em um array de bytes
		/// </summary>
		/// <param name="stringToCompress">A string a ser compactada</param>
		/// <returns>Um array de bytes contendo o resultado da compactação</returns>
		public byte[] CompressToByte(string stringToCompress)
		{
			byte[] bytData = Encoding.Unicode.GetBytes(stringToCompress);
			return Compress(bytData);
		}

		/// <summary>
		/// Descompacta uma string com dados codificados em Base64 para uma string normal.
		/// </summary>
		/// <param name="stringToDecompress">A string compactada e codificada</param>
		/// <param name="textEncoding">O <c>Encoding</c> que deve ser utililizado nos bytes
		/// para transformá-los em <c>String</c>.</param>
		/// <returns>A string, descodificada e descompactada</returns>
		public string Decompress(string stringToDecompress, Encoding textEncoding)
		{
			string outString;
			if (stringToDecompress == null)
				throw new ArgumentNullException("stringToDecompress", "You tried to use an empty string");

			try
			{
				byte[] inArr = Convert.FromBase64String(stringToDecompress.Trim());
				outString = textEncoding.GetString(Decompress(inArr));
			}
			catch (NullReferenceException nEx)
			{
				return nEx.Message;
			}
			return outString;
		}

		/// <summary>
		/// Descompacta um conjunto de bytes.
		/// </summary>
		/// <param name="bytesToDecompress">O conjunto de bytes a ser descompactado</param>
		/// <returns>Um conjunto de bytes com os dados descompactados</returns>
		public byte[] Decompress(byte[] bytesToDecompress)
		{
			byte[] writeData = new byte[4096];
			using (MemoryStream inStream = new MemoryStream(bytesToDecompress))
			using (Stream s2 = InputStream(inStream))
			using (MemoryStream outStream = new MemoryStream())
			{
				int size;
				do
				{
					size = s2.Read(writeData, 0, writeData.Length);
					if (size > 0)
						outStream.Write(writeData, 0, size);
				} while (size > 0);

				return outStream.ToArray();
			}
		}
	}
}