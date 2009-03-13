using System;
using System.Web;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Suprifattus.Util.Web.Handlers
{
	public class SQLProxyHandler : IHttpHandler
	{
		public bool IsReusable { get { return true; } }

		public static HttpResponse Response { get { return HttpContext.Current.Response; } }
		
		public void ProcessRequest(HttpContext ctx)
		{
			int port;
			string sqlHost = null;
			int sqlPort = 1433;

			try 
			{
				port = Convert.ToInt32(ctx.Request.QueryString.ToString());
			}
			catch 
			{
				port = Convert.ToInt32(ctx.Request["port"]);
				string[] ss = ctx.Request.QueryString["sql"].Split(':');
				sqlHost = ss[0];
				if (ss.Length > 1)
					sqlPort = Convert.ToInt32(ss[1]);
			}

			Response.ContentType = "text/plain";
			BaseAnswer.Response = Response;
			BaseAnswer.dropAll = false;

			TcpListener l = null;
			
			try 
			{
				l = new TcpListener(IPAddress.Any, port);
				Response.Write("Bound to " + l.LocalEndpoint + " at port " + port + "\n");
				Response.Flush();

				if (sqlHost != null)
				{
					Response.Write("Proxy for SQL at " + sqlHost + ", using port " + sqlPort + "\n");
					Response.Flush();
				}

				l.Start();

				while (Response.IsClientConnected) 
				{
					if (!l.Pending())
					{
						if (BaseAnswer.Count == 0) 
						{
							Response.Write(".");
							Response.Flush();
						}
						Thread.Sleep(1000);
						continue;
					}
					else
					{
						Response.Write("\n");
						Response.Flush();
						if (sqlHost != null)
							new Thread(new ThreadStart(new SQLAnswer(l.AcceptTcpClient(), sqlHost, sqlPort).Answer)).Start();
						else
							new Thread(new ThreadStart(new DemoAnswer(l.AcceptTcpClient()).Answer)).Start();
					}
				}

				Response.Write("Web client disconnected\n");
				Response.Flush();
			}
			catch (Exception ex)
			{
				Response.Write("EXCEPTION: " + ex + "\n\n");
			}
			finally 
			{
				BaseAnswer.dropAll = true;

				if (l != null) l.Stop();
				Response.Write("Unbound\r\n");
				Response.Flush();
			}
		}
	}

	[CLSCompliant(false)]
	public abstract class BaseAnswer
	{
		protected static int nextId = 1;
		public static int Count = 0;
		public static HttpResponse Response;
		public static volatile bool dropAll = false;

		protected TcpClient cli;
		int id = nextId++;

		public BaseAnswer(TcpClient cli)
		{
			Count++;
			this.cli = cli;
			Response.Write("Client #" + id +  " connected.\n");
			Response.Flush();
		}
		
		public abstract void Answer();

		public int ID { get { return id; } }
	}
	
	[CLSCompliant(false)]
	public class DemoAnswer : BaseAnswer
	{
		public DemoAnswer(TcpClient cli)
			: base(cli)
		{
		}

		public override void Answer()
		{
			NetworkStream scli = cli.GetStream();
			byte[] data;

			try
			{
				int p, b;
				
				data = Encoding.ASCII.GetBytes("Hello, SQL world!\r\n");
				scli.Write(data, 0, data.Length);
					
				data = new byte[81];
				p = 0;
				bool exit = false;
				while (!exit && !dropAll) 
				{
					while (!exit && !dropAll && !scli.DataAvailable)
						Thread.Sleep(10);

					if (exit || dropAll)
						break;

					b = scli.ReadByte();

					if (p >= 79) b = 13;
					switch (b) 
					{
						case 0: goto case 27;
						case 27: 
							exit = true;
							break;
						case 13: 
							data[p++] = (byte) '\r';
							data[p++] = (byte) '\n';
							scli.Write(data, 0, p); 
							Response.Write("Client #" + ID + "> " + Encoding.ASCII.GetString(data, 0, p));
							Response.Flush();
							p = 0; 
							break;
						case 10:
							break;
						default: 
							data[p++] = (byte) b; 
							break;
					}
				}
			}
			finally 
			{
				if (!dropAll) 
				{
					Response.Write("Client #" + ID + " disconnected.\n");
					Response.Flush();
				}
				else
				{
					data = Encoding.ASCII.GetBytes("\r\nConnection reset by peer.");
					scli.Write(data, 0, data.Length);
				}
				
				data = Encoding.ASCII.GetBytes("\r\nBye!");
				scli.Write(data, 0, data.Length);

				scli.Close();
				cli.Close();
				Count--;
			}
		}
	}

	[CLSCompliant(false)]
	public class SQLAnswer : BaseAnswer
	{
		private string sqlHost;
		private int sqlPort;

		public SQLAnswer(TcpClient cli, string sqlHost, int sqlPort)
			: base(cli)
		{
			this.sqlHost = sqlHost;
			this.sqlPort = sqlPort;
		}

		public override void Answer()
		{
			NetworkStream scli = cli.GetStream();
			TcpClient sql = new TcpClient();
			NetworkStream ssql = null;

			try 
			{
				sql.Connect(sqlHost, sqlPort);
				ssql = sql.GetStream();
			}
			catch (Exception ex)
			{
				Response.Write("Client #" + ID + " disconnected: Connection to SQL Server failed.\n");
				Response.Write("EXCEPTION: " + ex + "\n");
				Response.Flush();

				scli.Close();
				cli.Close();
				Count--;
				return;
			}
			
			try
			{
				while (!dropAll) 
				{
					while (!dropAll && !scli.DataAvailable)
						Thread.Sleep(1);

					if (dropAll)
						break;

					scli.WriteByte((byte) ssql.ReadByte());
				}
			}
			finally 
			{
				if (!dropAll) 
				{
					Response.Write("Client #" + ID + " disconnected.\n");
					Response.Flush();
				}
				
				ssql.Close();
				sql.Close();
				
				scli.Close();
				cli.Close();
				Count--;
			}
		}
	}
}
