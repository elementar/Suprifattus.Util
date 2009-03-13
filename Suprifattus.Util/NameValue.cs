using System;
using System.Xml.Serialization;

namespace Suprifattus.Util
{
	/// <summary>
	/// Representa um par chave/valor.
	/// </summary>
	[CLSCompliant(false)]
	public class KeyValue : KeyValue<string, string>
	{
		public KeyValue() : this(null, null)
		{
		}

		public KeyValue(string key, string value)
		{
			this.Key = key;
			this.Value = value;
		}
	}

	/// <summary>
	/// Representa um par chave/valor.
	/// </summary>
	/// <typeparam name="K">O tipo da variável de chave</typeparam>
	/// <typeparam name="V">O tipo da variável de valor</typeparam>
	[CLSCompliant(false)]
	public class KeyValue<K, V>
	{
		/// <summary>
		/// Cria um novo par chave/valor.
		/// </summary>
		public KeyValue()
		{
		}

		/// <summary>
		/// Cria um novo par chave/valor.
		/// </summary>
		/// <param name="key">Chave</param>
		/// <param name="val">Valor</param>
		[CLSCompliant(false)]
		public KeyValue(K key, V val)
		{
			this.Key = key;
			this.Value = val;
		}

		/// <summary>
		/// Nome
		/// </summary>
		[XmlAttribute("Key")]
		public K Key { get; set; }

		/// <summary>
		/// Valor
		/// </summary>
		[XmlAttribute("Value")]
		public V Value { get; set; }
	}
}