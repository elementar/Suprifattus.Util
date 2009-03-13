using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;
using System.Text;
using System.Xml;

namespace Suprifattus.Util.Web.Logger
{
	/// <summary>
	/// Logger assíncrono.
	/// </summary>
	public class AsyncLogger
	{
		public static AsyncLogger Start(FileInfo logFile) 
		{
			AsyncLogger logger = new AsyncLogger(logFile);
			logger.LogThread.Start();
			return logger;
		}
		
		public static AsyncLogger Start(string connectionString) 
		{
			AsyncLogger logger = new AsyncLogger(connectionString);
			logger.LogThread.Start();
			return logger;
		}
		
		public Exception LastException;

		public readonly DateTime StartDate = DateTime.Now;
		public readonly Thread LogThread;
		public readonly IOutputFilter OutputFilter;
		
		readonly Queue queue = new Queue(20);
		volatile bool halt = false;
		
		private AsyncLogger() 
		{
			this.LogThread = new Thread(new ThreadStart(LogLoop));
			this.LogThread.Name = "AsyncLogger";
		}
		
		private AsyncLogger(FileInfo logFile)
			: this()
		{
			this.OutputFilter = new OutputFilters.XmlOutputFilter(logFile);
		}

		private AsyncLogger(string connectionString)
			: this()
		{
			this.OutputFilter = new OutputFilters.OleDbOutputFilter(connectionString);
		}

		private void LogLoop() 
		{
			try 
			{
				LogEntry entry;
			
				while (!halt) 
				{
					while (queue.Count <= 0)
						Thread.Sleep(500);

					HttpRequest req = queue.Dequeue() as HttpRequest;
					
					HttpCookie cookieAspNet = req.Cookies["ASP.NET_SessionId"];
					
					entry = new LogEntry(
						DateTime.Now,
						req.RequestType,
						req.Url,
						req.UrlReferrer,
						(cookieAspNet == null ? null : cookieAspNet.Value),
						req.UserHostName,
						req.UserHostAddress,
						req.UserAgent
					);
					
					try 
					{
						OutputFilter.Write(entry);
					}
					catch (Exception ex) 
					{
						LastException = ex;
					}
				}
			}
			catch (Exception ex)
			{
				LastException = ex;
				throw ex;
			}
		}

		public void Log(HttpRequest req) 
		{
			queue.Enqueue(req);
		}
		
		public void Halt() 
		{
			halt = true;
		}
	}
}
