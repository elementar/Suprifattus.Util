using System;

namespace Suprifattus.Util.Web.Logger
{
	public interface IOutputFilter
	{
		void Write(LogEntry entry);
	}
}
