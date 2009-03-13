using System;

namespace Suprifattus.Util.Timing
{
	public interface IClock
	{
		DateTime Get();
	}

	public sealed class SystemClock : IClock
	{
		public static readonly SystemClock Instance = new SystemClock();

		public DateTime Get()
		{
			return DateTime.Now;
		}
	}

	public class StaleClock : IClock
	{
		private DateTime dateTime;

		public StaleClock()
			: this(DateTime.MinValue)
		{
		}

		public StaleClock(DateTime dateTime)
		{
			Set(dateTime);
		}

		public void Set(DateTime dateTime)
		{
			this.dateTime = dateTime;
		}

		public DateTime Get()
		{
			return dateTime;
		}
	}
}