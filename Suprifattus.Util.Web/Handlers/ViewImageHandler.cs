using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

using Suprifattus.Util.Graphics;

namespace Suprifattus.Util.Web.Handlers
{
	public class ViewImageHandler : IHttpHandler
	{
		public bool IsReusable { get { return true; } }

		static readonly Regex rxSize = new Regex(@"(\d+),(\d+)", RegexOptions.Compiled);

		public void ProcessRequest(HttpContext ctx)
		{
			string sizeString = ctx.Request.QueryString["s"];
			string imageFile  = ctx.Request.QueryString["f"];
			if (sizeString == null)
				return;

			Match m = rxSize.Match(sizeString);

			if (!m.Success)
				return;

			FileInfo fi = new FileInfo(ctx.Request.MapPath(imageFile));

			ctx.Response.ContentType = "image/jpeg";
			ctx.Response.Cache.SetCacheability(HttpCacheability.Private);
			ctx.Response.Cache.SetLastModified(fi.LastWriteTime);
			ctx.Response.Cache.SetOmitVaryStar(true);
			
			Size s = new Size(Convert.ToInt32(m.Groups[1].Value), Convert.ToInt32(m.Groups[2].Value));

			using (Image original = Image.FromFile(fi.FullName))
			using (Image thumbnail = ImageUtil.MakeThumbnail(original, s, 4))
				thumbnail.Save(ctx.Response.OutputStream, ImageFormat.Jpeg);
		}
	}
}
