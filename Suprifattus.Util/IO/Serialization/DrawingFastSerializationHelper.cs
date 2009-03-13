using System;
using System.Collections.Specialized;
using System.Drawing;

namespace Suprifattus.Util.Serialization
{
	public class DrawingFastSerializationHelper: IFastSerializationTypeSurrogate
	{
		#region Static
		internal static readonly int ColorIsKnown = BitVector32.CreateMask();
		internal static readonly int ColorHasName = BitVector32.CreateMask(ColorIsKnown);
		internal static readonly int ColorHasValue = BitVector32.CreateMask(ColorHasName);
		internal static readonly int ColorHasRed = BitVector32.CreateMask(ColorHasValue);
		internal static readonly int ColorHasGreen = BitVector32.CreateMask(ColorHasRed);
		internal static readonly int ColorHasBlue = BitVector32.CreateMask(ColorHasGreen);
		internal static readonly int ColorHasAlpha = BitVector32.CreateMask(ColorHasBlue);
		#endregion Static

		#region IFastSerialization
		public bool SupportsType(Type type)
		{
			if (type == typeof(Color))
				return true;
			return false;
		}

		public void Serialize(SerializationWriter writer, object value)
		{
			Type type = value.GetType();

			if (type == typeof(Color))
				Serialize(writer, (Color) value);

			else
			{
				throw new InvalidOperationException(string.Format("{0} does not support Type: {1}", GetType(), type));
			}
		}

		public object Deserialize(SerializationReader reader, Type type)
		{
			if (type == typeof(Color))
			{
				return DeserializeColor(reader);
			}

			else
			{
				throw new InvalidOperationException(string.Format("{0} does not support Type: {1}", GetType(), type));
			}
		}
		#endregion IFastSerialization

		#region Color
		public static void Serialize(SerializationWriter writer, Color color)
		{
			BitVector32 flags = new BitVector32();

			if (color.IsKnownColor)
				flags[ColorIsKnown] = true;
			else if (color.IsNamedColor)
				flags[ColorHasName] = true;
			else if (!color.IsEmpty)
			{
				flags[ColorHasValue] = true;
				flags[ColorHasRed] = color.R != 0;
				flags[ColorHasGreen] = color.G != 0;
				flags[ColorHasBlue] = color.B != 0;
				flags[ColorHasAlpha] = color.A != 0;
			}
			writer.WriteOptimized(flags);

			if (color.IsKnownColor)
				writer.WriteOptimized((int) color.ToKnownColor());
			else if (color.IsNamedColor)
				writer.WriteOptimized(color.Name);
			else if (!color.IsEmpty)
			{
				byte component;
				if ( (component = color.R) != 0) writer.Write(component);	
				if ( (component = color.G) != 0) writer.Write(component);	
				if ( (component = color.B) != 0) writer.Write(component);	
				if ( (component = color.A) != 0) writer.Write(component);	
			}
		}

		public static Color DeserializeColor(SerializationReader reader)
		{
			BitVector32 flags = reader.ReadOptimizedBitVector32();
			if (flags[ColorIsKnown])
				return Color.FromKnownColor((KnownColor) reader.ReadOptimizedInt32());
			else if (flags[ColorHasName])
				return Color.FromName(reader.ReadOptimizedString());
			else if (!flags[ColorHasValue])
				return Color.Empty;
			else
			{
				byte red = flags[ColorHasRed] ? reader.ReadByte() : (byte) 0;
				byte green = flags[ColorHasGreen] ? reader.ReadByte() : (byte) 0;
				byte blue = flags[ColorHasBlue] ? reader.ReadByte() : (byte) 0;
				byte alpha = flags[ColorHasAlpha] ? reader.ReadByte() : (byte) 0;
				return Color.FromArgb(alpha, red, green, blue);
			}
		}
		#endregion Color
	}
}