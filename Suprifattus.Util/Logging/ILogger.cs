using System;

namespace Suprifattus.Util.Logging
{
	public interface ILogger
	{
		void Log(LogLevel level, string format, params object[] args);
		void Log(LogLevel level, object message);
		void Log(LogLevel level, Exception ex, string format, params object[] args);
		void Log(LogLevel level, Exception ex, object message);
		
		void Debug(string format, params object[] args);
		void Info(string format, params object[] args);
		void Warn(string format, params object[] args);
		void Error(string format, params object[] args);
		void Fatal(string format, params object[] args);

		void Debug(Exception ex, string format, params object[] args);
		void Info(Exception ex, string format, params object[] args);
		void Warn(Exception ex, string format, params object[] args);
		void Error(Exception ex, string format, params object[] args);
		void Fatal(Exception ex, string format, params object[] args);

		void Debug(object message);
		void Info(object message);
		void Warn(object message);
		void Error(object message);
		void Fatal(object message);

		void Debug(Exception ex, object message);
		void Info(Exception ex, object message);
		void Warn(Exception ex, object message);
		void Error(Exception ex, object message);
		void Fatal(Exception ex, object message);
	}

}
