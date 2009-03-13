using System;
using System.Threading;

namespace Suprifattus.Util.Threading
{
	/// <summary>
	/// Gerencia se��es cr�ticas de c�digo.
	/// </summary>
	public class CriticalSectionHandler
	{
		internal readonly ManualResetEvent resetEvent = new ManualResetEvent(true);

		/// <summary>
		/// Entra em uma se��o cr�tica.
		/// </summary>
		public CriticalSection Enter()
		{
			return new CriticalSection(this);
		}
	}

	/// <summary>
	/// Representa uma se��o cr�tica do c�digo.
	/// Apenas uma CriticalSection pode estar rodando ao mesmo tempo.
	/// <seealso cref="CriticalSectionHandler"/>
	/// </summary>
	public class CriticalSection : IDisposable
	{
		private readonly CriticalSectionHandler handler;
		private bool left;

		internal CriticalSection(CriticalSectionHandler handler)
		{
			this.handler = handler;
			Initialize();
		}

		private void Initialize()
		{
			while (!handler.resetEvent.WaitOne(100, false))
				Thread.Sleep(100);

			handler.resetEvent.Reset();
		}

		/// <summary>
		/// Abandona a se��o cr�tica.
		/// </summary>
		public void Leave()
		{
			if (!left)
			{
				left = true;
				handler.resetEvent.Set();
			}
		}

		void IDisposable.Dispose()
		{
			Leave();
		}
	}
}