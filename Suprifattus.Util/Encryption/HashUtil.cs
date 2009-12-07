using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Suprifattus.Util.Encryption
{
	/// <summary>
	/// Classe utilitária para uso de hashing.
	/// </summary>
	public class HashUtil
	{
		private HashUtil()
		{
		}

		/// <summary>
		/// Criptografa uma string utilizando o algoritmo <see cref="MD5"/>.
		/// </summary>
		/// <param name="str">A string de origem</param>
		/// <returns>A string criptografada com MD5</returns>
		public static string GetMD5Hash(string str)
		{
			return GetHash(HashAlgorithm.Create("MD5"), str);
		}

		/// <summary>
		/// Criptografa bytes utilizando o algoritmo <see cref="MD5"/>.
		/// </summary>
		/// <param name="source">Os dados de origem</param>
		/// <returns>A string criptografada com MD5</returns>
		public static string GetMD5Hash(byte[] source)
		{
			return GetHash(HashAlgorithm.Create("MD5"), source);
		}

		/// <summary>
		/// Criptografa bytes utilizando o algoritmo <see cref="MD5"/>.
		/// </summary>
		/// <param name="source">Os dados de origem</param>
		/// <returns>A string criptografada com o algoritmo especificado</returns>
		public static string GetMD5Hash(Stream source)
		{
			return GetHash(HashAlgorithm.Create("MD5"), source);
		}

		/// <summary>
		/// Criptografa uma string utilizando o algoritmo especificado em
		/// <paramref name="hashAlg"/>.
		/// </summary>
		/// <param name="hashAlg">O algoritmo de hashing.</param>
		/// <param name="str">A string de origem</param>
		/// <returns>A string criptografada com o algoritmo especificado</returns>
		public static string GetHash(HashAlgorithm hashAlg, string str)
		{
			int len = str.Length;
			byte[] source = new byte[len];
			for (int i = 0; i < str.Length; i++)
				source[i] = (byte) str[i];

			return GetHash(hashAlg, source);
		}

		/// <summary>
		/// Criptografa bytes utilizando o algoritmo especificado em
		/// <paramref name="hashAlg"/>.
		/// </summary>
		/// <param name="hashAlg">O algoritmo de hashing.</param>
		/// <param name="source">Os dados de origem</param>
		/// <returns>A string criptografada com o algoritmo especificado</returns>
		public static string GetHash(HashAlgorithm hashAlg, Stream source)
		{
			byte[] encoded = hashAlg.ComputeHash(source);
			return HashToString(encoded);
		}

		/// <summary>
		/// Criptografa bytes utilizando o algoritmo especificado em
		/// <paramref name="hashAlg"/>.
		/// </summary>
		/// <param name="hashAlg">O algoritmo de hashing.</param>
		/// <param name="source">Os bytes de origem</param>
		/// <returns>A string criptografada com o algoritmo especificado</returns>
		public static string GetHash(HashAlgorithm hashAlg, byte[] source)
		{
			byte[] encoded = hashAlg.ComputeHash(source);
			return HashToString(encoded);
		}

		/// <summary>
		/// Retorna a representação string (hexadecimal) dos bytes especificados.
		/// </summary>
		/// <param name="encoded">Os bytes</param>
		/// <returns>A string</returns>
		public static string HashToString(byte[] encoded)
		{
			var sb = new StringBuilder(encoded.Length * 2);
			foreach (byte b in encoded)
				sb.Append(b.ToString("x2"));

			return sb.ToString();
		}
	}
}