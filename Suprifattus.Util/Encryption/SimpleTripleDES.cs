using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

using Suprifattus.Util.IO;

namespace Suprifattus.Util.Encryption
{
	/// <summary>
	/// Classe que simplifica o uso de 3-DES.
	/// </summary>
	/// <remarks>
	/// "Roubada" de: http://www.codeproject.com/vb/net/VB_NET_TripleDES.asp
	/// </remarks>
	public class SimpleTripleDES : IDisposable
	{
		// define the triple des provider
		private readonly TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

		private readonly Encoding textEncoding = Encoding.UTF8;

		// define the local property arrays
		private readonly byte[] key = new byte[24];
		private readonly byte[] iv = new byte[8];

		public SimpleTripleDES(string stringKey)
		{
			byte[] md5hash;
			byte[] saltedHash;

			using (HashAlgorithm md5 = MD5.Create())
			{
				md5hash = md5.ComputeHash(Encoding.UTF8.GetBytes(stringKey));
				saltedHash = md5.ComputeHash(Encoding.UTF8.GetBytes(stringKey));
				saltedHash = md5.ComputeHash(Encoding.UTF8.GetBytes(stringKey + Encoding.UTF8.GetString(saltedHash)));
			}

			Array.Copy(saltedHash, key, saltedHash.Length);

			int startcount = saltedHash.Length; /* always 128 */
			int midcount = md5hash.Length / 2; /* always 64 */

			for (int i = midcount; i < md5hash.Length; i++)
			{
				key[startcount + (i - midcount)] = md5hash[i];
				iv[i - midcount] = md5hash[i - midcount];
			}
		}

		public SimpleTripleDES(byte[] key, byte[] iv)
		{
			this.key = key;
			this.iv = iv;
		}

		public byte[] Encrypt(byte[] input)
		{
			using (MemoryStream msOut = new MemoryStream())
			{
				using (MemoryStream msIn = new MemoryStream(input))
				using (CryptoStream cryptStream = new CryptoStream(msOut, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
				{
					// transform the bytes as requested
					Streams.SaveStream(msIn, cryptStream);
					cryptStream.FlushFinalBlock();
				}

				// hand back the encrypted buffer
				return msOut.ToArray();
			}
		}

		public byte[] Decrypt(byte[] input)
		{
			using (MemoryStream msOut = new MemoryStream())
			{
				using (MemoryStream msIn = new MemoryStream(input))
				using (CryptoStream cryptStream = new CryptoStream(msIn, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
				{
					// transform the bytes as requested
					Streams.SaveStream(cryptStream, msOut);
				}

				// hand back the encrypted buffer
				return msOut.ToArray();
			}
		}

		/// <summary>
		/// Accepts any string, encodes using the encoding set on <see cref="textEncoding"/>,
		/// then encrypts and encodes as Base64.
		/// </summary>
		public string Encrypt(string text)
		{
			byte[] input = textEncoding.GetBytes(text);
			byte[] output = Encrypt(input);
			return Convert.ToBase64String(output);
		}

		/// <summary>
		/// Accepts a Base64 encoded array of bytes, and decrypt as a string.
		/// </summary>
		public string Decrypt(string text)
		{
			byte[] input = Convert.FromBase64String(text);
			byte[] output = Decrypt(input);
			return textEncoding.GetString(output);
		}

		/// <summary>
		/// Accepts any serializable object, encrypts and encodes as Base64.
		/// </summary>
		public string EncryptObject(object obj)
		{
			BinaryFormatter fmt = new BinaryFormatter();

			using (MemoryStream ms = new MemoryStream())
			{
				fmt.Serialize(ms, obj);
				return Convert.ToBase64String(Encrypt(ms.ToArray()));
			}
		}

		/// <summary>
		/// Accepts a Base64 encoded array of bytes, and decrypt as an object.
		/// </summary>
		public object DecryptObject(string text)
		{
			BinaryFormatter fmt = new BinaryFormatter();

			using (MemoryStream ms = new MemoryStream(Decrypt(Convert.FromBase64String(text))))
				return fmt.Deserialize(ms);
		}

		/// <summary>
		/// Accepts a Base64 encoded array of bytes, and decrypt as an object.
		/// </summary>
		public T DecryptObject<T>(string text)
		{
			return (T) DecryptObject(text);
		}

		public void Dispose()
		{
		}
	}
}