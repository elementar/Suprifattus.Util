using System;
using System.Threading;

namespace Suprifattus.Util.Threading
{
	/// <summary>
	/// Gerencia seções críticas de código.
	/// </summary>
	public class CriticalSectionHandler
	{
		internal readonly ManualResetEvent resetEvent = new ManualResetEvent(true);

		/// <summary>
		/// Entra em uma seção crítica.
		/// </summary>
		public CriticalSection Enter()
		{
			return new CriticalSection(this);
		}
	}

	/// <summary>
	/// Representa uma seção crítica do código.
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
		/// Abandona a seção crítica.
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