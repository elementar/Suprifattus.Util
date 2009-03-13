using System;
using System.Collections;
using System.Collections.Specialized;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Classe auxiliar para criar dicionários rápidos,
	/// geralmente para passagem de parâmetros.
	/// </summary>
	public class Dict : HybridDictionary
	{
		/// <summary>
		/// Cria um novo <c>Dict</c>.
		/// </summary>
		public Dict()
		{
		}

		/// <summary>
		/// Cria um novo <c>Dict</c> baseado em outro <see cref="IDictionary"/>.
		/// </summary>
		public Dict(IDictionary source)
		{
			foreach (DictionaryEntry de in source)
				base.Add(de.Key, de.Value);
		}

		/// <summary>
		/// Cria um novo <c>Dict</c> baseado em um <see cref="NameValueCollection"/>.
		/// </summary>
		public Dict(NameValueCollection col)
		{
			foreach (string key in col.Keys)
				base.Add(key, col[key]);
		}

		/// <summary>
		/// Adiciona um item.
		/// </summary>
		public new Dict Add(object key, object value)
		{
			base.Add(key, value);
			return this;
		}
		
		/// <summary>
		/// Atribui um item.
		/// </summary>
		public Dict Set(object key, object value)
		{
			base[key] = value;
			return this;
		}
	}
}