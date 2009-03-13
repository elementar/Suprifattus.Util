using System;
using System.Runtime.Serialization;

namespace Suprifattus.Util
{
	/// <summary>
	/// Tristate boolean
	/// </summary>
	[Serializable]
	public struct bool2 : ISerializable
	{
		/// <summary>
		/// Neither True, nor False
		/// </summary>
		public static readonly bool2 None = new bool2(vNone);

		/// <summary>
		/// False
		/// </summary>
		public static readonly bool2 False = new bool2(vFalse);

		/// <summary>
		/// True
		/// </summary>
		public static readonly bool2 True = new bool2(vTrue);

		private const short vNone = -1, vFalse = 0, vTrue = 1;
		private readonly short b;

		private bool2(short b)
		{
			this.b = b;
		}

		/// <summary>
		/// Converts a <see cref="bool"/> value into a <see cref="bool2"/>.
		/// </summary>
		/// <param name="value">The <see cref="bool"/> value</param>
		/// <returns>The respective <see cref="bool2"/> value.</returns>
		public static implicit operator bool2(bool value)
		{
			return value ? True : False;
		}

		/// <summary>
		/// Converts a <see cref="bool2"/> value into a <see cref="bool"/>.
		/// </summary>
		/// <param name="value">The <see cref="bool2"/> value</param>
		/// <returns>The respective <see cref="bool"/> value, if possible.</returns>
		/// <exception cref="InvalidOperationException">If the <see cref="bool2"/> is <see cref="bool2.None"/>.</exception>
		public static implicit operator bool(bool2 value)
		{
			switch (value.b)
			{
				case vTrue:
					return true;
				case vFalse:
					return false;
				default:
					throw new InvalidOperationException("bool2 value of type 'None' can't be converted to plain bool");
			}
		}

		/// <summary>
		/// Gets the hashcode.
		/// </summary>
		/// <returns>The hashcode.</returns>
		public override int GetHashCode()
		{
			return b.GetHashCode();
		}

		/// <summary>
		/// Compares this <see cref="bool2"/> value with another.
		/// </summary>
		/// <param name="obj">The other value</param>
		/// <returns>True if <paramref name="obj"/> is of type <see cref="bool2"/> and equal, 
		/// false otherwise</returns>
		public override bool Equals(object obj)
		{
			if (obj is bool2)
				return ((bool2) obj) == this;
			return false;
		}

		/// <summary>
		/// Compares two <see cref="bool2"/> values.
		/// </summary>
		/// <param name="a">The first value</param>
		/// <param name="b">The second value</param>
		/// <returns>False if values are equal, true otherwise</returns>
		public static bool operator !=(bool2 a, bool2 b)
		{
			return a.b != b.b;
		}

		/// <summary>
		/// Compares two <see cref="bool2"/> values.
		/// </summary>
		/// <param name="a">The first value</param>
		/// <param name="b">The second value</param>
		/// <returns>True if values are equal, false otherwise</returns>
		public static bool operator ==(bool2 a, bool2 b)
		{
			return a.b == b.b;
		}

		/// <summary>
		/// Builds a <see cref="bool2"/> object from two boolean checks.
		/// </summary>
		/// <param name="bTrue"></param>
		/// <param name="bFalse"></param>
		/// <returns></returns>
		public static bool2 FromBooleans(bool bTrue, bool bFalse)
		{
			if (bTrue) return True;
			if (bFalse) return False;
			return None;
		}

		/// <summary>
		/// Construtor utilizado na deserialização.
		/// </summary>
		/// <param name="info">O objeto <see cref="SerializationInfo"/></param>
		/// <param name="context">A enumeração <see cref="StreamingContext"/></param>
		public bool2(SerializationInfo info, StreamingContext context)
		{
			this.b = info.GetInt16("b");
		}

		/// <summary>
		/// Serializa a instância do objeto.
		/// </summary>
		/// <param name="info">O objeto <see cref="SerializationInfo"/></param>
		/// <param name="context">A enumeração <see cref="StreamingContext"/></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("b", b);
		}
	}
}