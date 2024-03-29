using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Suprifattus.Util.Graphics
{
	/// <summary>
	/// Classe utilitária para tratamento de imagens
	/// </summary>
	public class ImageUtil
	{
		public static Image MakeCroppedThumbnail(Image b, SizeF cropSize, float scale)
		{
			var r = new Bitmap((int) (cropSize.Width * scale), (int) (cropSize.Height * scale));

			var p = new PointF((b.Width / 2f) - (cropSize.Width / 2f), (b.Height / 2f) - (cropSize.Height / 2f));

			using (var g = System.Drawing.Graphics.FromImage(r))
			{
				g.InterpolationMode = InterpolationMode.HighQualityBicubic; // bicubic = sharp, bilinear = smooth
				g.DrawImage(b, new RectangleF(0, 0, r.Width, r.Height), new RectangleF(p, cropSize), GraphicsUnit.Pixel);

				g.Flush(FlushIntention.Flush);
			}

			return r;
		}

		/// <summary>
		/// Monta um <em>Thumbnail</em> de uma imagem.
		/// </summary>
		/// <param name="b">A imagem original</param>
		/// <param name="s">O tamanho do <em>Thumbnail</em></param>
		/// <param name="margin">A margem que deve ser deixada internamente</param>
		/// <returns>A nova imagem</returns>
		public static Image MakeThumbnail(Image b, SizeF s, int margin)
		{
			var r = new Bitmap((int) s.Width, (int) s.Height);

			var divisor = Math.Max(b.Width / (float) r.Width, b.Height / (float) r.Height);
			var newSize = new SizeF(b.Width / divisor - margin * 2, b.Height / divisor - margin * 2);
			var pos = new PointF((r.Width - newSize.Width) / 2, (r.Height - newSize.Height) / 2);
			var resRect = new RectangleF(new Point(0, 0), s);

			var c1 = Color.FromArgb(50, 50, 50);
			var c2 = Color.FromArgb(200, 200, 200);

			using (var g = System.Drawing.Graphics.FromImage(r))
			{
				g.FillRectangle(new LinearGradientBrush(resRect, c1, c2, 45.0f), resRect);
				g.InterpolationMode = InterpolationMode.HighQualityBicubic; // bicubic = sharp, bilinear = smooth
				g.DrawImage(b, new RectangleF(pos, newSize));

				g.Flush(FlushIntention.Flush);
			}
			return r;
		}
	}
}