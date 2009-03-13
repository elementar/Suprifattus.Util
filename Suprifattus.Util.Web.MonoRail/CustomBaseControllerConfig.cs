using System;

namespace Suprifattus.Util.Web.MonoRail
{
	public class CustomBaseControllerConfig
	{
		public static readonly CustomBaseControllerConfig Instance = new CustomBaseControllerConfig();
		
		int pageSize = 10;

		public int PageSize
		{
			get { return pageSize; }
			set { pageSize = value; }
		}
	}
}
