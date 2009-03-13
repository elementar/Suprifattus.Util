using System;

namespace Suprifattus.Util.Web.MonoRail.Contracts
{
	public interface IObjectFormatter
	{
		object Format(object arg);
	}
}