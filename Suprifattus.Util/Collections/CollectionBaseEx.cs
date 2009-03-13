using System;
using System.Collections;
using System.ComponentModel;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Uma CollectionBase mais fácil de extender. Basta definir o tipo permitido
	/// em um atributo <see cref="ValueConditionAttribute"/>, sob o id "<c>value</c>".
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(CollectionConverter))]
	public abstract class CollectionBaseEx : CollectionBase
	{
		/// <summary>
		/// Valida se o item pode ser inserido na coleção ou não.
		/// </summary>
		/// <param name="value">O item a ser validado.</param>
		protected override void OnValidate(object value)
		{
			if (!ValueConditionAttribute.CheckValue("value", value))
				throw new ArgumentException("Invalid value for this collection.");
		}
	}
}
