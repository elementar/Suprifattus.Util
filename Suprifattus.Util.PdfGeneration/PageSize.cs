using System;

namespace Suprifattus.Util.PdfGeneration
{
	[Flags]
	[CLSCompliant(false)]
	public enum PageSize : uint
	{
		Portrait = 0x00000000,
		Landscape = 0x10000000,

		Custom = 0x00000001,
		A4 = 0x00000002,
		Letter = 0x00000004,
	}

	public enum TextAlignment
	{
		Left,
		Center,
		Right
	}
}