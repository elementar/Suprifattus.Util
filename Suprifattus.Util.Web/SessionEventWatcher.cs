using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace Suprifattus.Util.Web
{
	/// <summary>
	/// Captura eventos de sess�o de forma isolada, lan�ando o evento
	/// apenas no momento do evento da sess�o espec�fica de quando o
	/// <see cref="EventHandler"/> foi vinculado ao 
	/// <see cref="ISessionEventWatcher"/>. Ou seja: vincula o evento
	/// � sess�o do contexto de quem a adicionou.
	/// </summary>
	public interface ISessionEventWatcher
	{
		/// <summary>
		/// Evento de in�cio de sess�o.
		/// </summary>
		event EventHandler Start;

		/// <summary>
		/// Evento de final de sess�o.
		/// </summary>
		event EventHandler End;
	}

	/// <summary>
	/// Implementa��o b�sica de <see cref="ISessionEventWatcher"/>.
	/// <seealso cref="ISessionEventWatcher"/>
	/// </summary>
	public class SessionEventWatcher : ISessionEventWatcher
	{
		private readonly IDictionary<string, EventHandler>
			start = new Dictionary<string, EventHandler>(),
			end = new Dictionary<string, EventHandler>();

		/// <summary>
		/// Cria um novo <see cref="SessionEventWatcher"/>
		/// para a aplica��o web especificada.
		/// </summary>
		/// <param name="app">A aplica��o web</param>
		public SessionEventWatcher(HttpApplication app)
		{
			foreach (string moduleKey in app.Modules.AllKeys)
			{
				var sessionModule
					= app.Modules[moduleKey] as SessionStateModule;

				if (sessionModule != null)
				{
					sessionModule.Start += delegate { CallEventHandlers(start); };
					sessionModule.End += delegate { CallEventHandlers(end); };
				}
			}
		}

		/// <summary>
		/// Evento de in�cio de sess�o.
		/// </summary>
		public event EventHandler Start
		{
			add { AddHandler(start, value); }
			remove { RemoveHandler(start, value); }
		}

		/// <summary>
		/// Evento de fim de sess�o.
		/// </summary>
		public event EventHandler End
		{
			add { AddHandler(end, value); }
			remove { RemoveHandler(end, value); }
		}

		private void AddHandler(IDictionary<string, EventHandler> col, EventHandler h)
		{
			lock (col)
			{
				string sid = GetSessionId();
				if (sid == null)
					return;
				if (col.ContainsKey(sid))
					col[sid] += h;
				else
					col.Add(sid, h);
			}
		}

		private void RemoveHandler(IDictionary<string, EventHandler> col, EventHandler h)
		{
			lock (col)
			{
				string sid = GetSessionId();
				if (sid == null)
					return;
				if (col.ContainsKey(sid))
					col[sid] -= h;
			}
		}

		private void CallEventHandlers(IDictionary<string, EventHandler> col)
		{
			lock (col)
			{
				string sid = GetSessionId();
				if (sid == null)
					return;
				if (col.ContainsKey(sid))
				{
					EventHandler h = col[sid];
					if (h != null)
						h(this, EventArgs.Empty);
				}
			}
		}

		private static string GetSessionId()
		{
			HttpContext ctx = HttpContext.Current;

			if (ctx == null || ctx.Session == null)
				return null;

			return ctx.Session.SessionID;
		}
	}
}