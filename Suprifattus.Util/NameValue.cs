using System;
using System.Xml.Serialization;

namespace Suprifattus.Util
{
	/// <summary>
	/// Representa um par chave/valor.
	/// </summary>
#if !GENERICS
	[Serializable]
	public class KeyValue
	{
		private string key, val;
#else
		[CLSCompliant(false)]
		public class KeyValue : KeyValue<string, string>
    {
			public KeyValue() : this(null, null) { }

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
		private K key;
		private V val;
#endif

		/// <summary>
		/// Cria um novo par chave/valor.
		/// </summary>
		public KeyValue() { }

		/// <summary>
		/// Cria um novo par chave/valor.
		/// </summary>
		/// <param name="key">Chave</param>
		/// <param name="val">Valor</param>
#if GENERICS
		[CLSCompliant(false)]
		public KeyValue(K key, V val)
#else
		public KeyValue(string key, string val)
#endif
		{
			this.key = key;
			this.val = val;
		}

		/// <summary>
		/// Nome
		/// </summary>
#if GENERICS
		[XmlAttribute("Key")]
		public K Key
#else
		[XmlAttribute("Key")]
		public string Key
#endif
		{ get { return key; } set { key = value; } }

		/// <summary>
		/// Valor
		/// </summary>
#if GENERICS
		[XmlAttribute("Value")]
		public V Value
#else
		[XmlAttribute("Value")]
		public string Value 
#endif
		{ get { return val; } set { val = value; } }
	}
}