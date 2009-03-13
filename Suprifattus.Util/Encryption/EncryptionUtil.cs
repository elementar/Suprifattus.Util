using System;
using System.IO;
using System.Security.Cryptography;

namespace Suprifattus.Util.Encryption
{
	/// <summary>
	/// Classe utilitária para uso de criptografia e Hashing.
	/// </summary>
	[Obsolete("Utilize HashUtil")]
	public class EncryptionUtil
	{
		private EncryptionUtil()
		{
		}

		/// <summary>
		/// Criptografa uma string utilizando o algoritmo <see cref="MD5"/>.
		/// </summary>
		/// <param name="str">A string de origem</param>
		/// <returns>A string criptografada com MD5</returns>
		public static string Encrypt(string str) 
		{
			return HashUtil.GetMD5Hash(str);
		}

		/// <summary>
		/// Criptografa uma string utilizando o algoritmo especificado em
		/// <paramref name="hashAlg"/>
		/// </summary>
		/// <param name="hashAlg">O algoritmo de hashing.</param>
		/// <param name="str">A string de origem</param>
		/// <returns>A string criptografada com o algoritmo especificado</returns>
		public static string Encrypt(HashAlgorithm hashAlg, string str) 
		{
			return HashUtil.GetHash(hashAlg, str);
		}

		/// <summary>
		/// Criptografa bytes utilizando o algoritmo especificado em
		/// <paramref name="hashAlg"/>
		/// </summary>
		/// <param name="hashAlg">O algoritmo de hashing.</param>
		/// <param name="source">Os dados de origem</param>
		/// <returns>A string criptografada com o algoritmo especificado</returns>
		public static string Encrypt(HashAlgorithm hashAlg, Stream source)
		{
			return HashUtil.GetHash(hashAlg, source);
		}
		
		/// <summary>
		/// Criptografa bytes utilizando o algoritmo especificado em
		/// <paramref name="hashAlg"/>
		/// </summary>
		/// <param name="hashAlg">O algoritmo de hashing.</param>
		/// <param name="source">Os bytes de origem</param>
		/// <returns>A string criptografada com o algoritmo especificado</returns>
		public static string Encrypt(HashAlgorithm hashAlg, byte[] source)
		{
			return HashUtil.GetHash(hashAlg, source);
		}

		/// <summary>
		/// Retorna a representação string (hexadecimal) dos bytes especificados.
		/// </summary>
		/// <param name="encoded">Os bytes</param>
		/// <returns>A string</returns>
		public static string HashToString(byte[] encoded)
		{
			return HashUtil.HashToString(encoded);
		}
	}
}