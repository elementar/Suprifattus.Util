using System;
using System.Collections.Generic;
using System.Threading;

namespace Suprifattus.Util.Threading
{
	public sealed class CustomThreadPool : IDisposable
	{
		public delegate void CustomRunDelegate(RunDelegate proceed);

		public delegate void RunDelegate();

		private static int lastPoolId;
		private readonly int poolId = Interlocked.Increment(ref lastPoolId);

		private readonly Semaphore workWaiting;
		private readonly Queue<WaitQueueItem> queue;
		private List<Thread> threads;
		private int running;

		public CustomThreadPool(int numThreads)
			: this(numThreads, null)
		{
		}

		public CustomThreadPool(int numThreads, CustomRunDelegate customRun)
		{
			if (numThreads <= 0)
				throw new ArgumentOutOfRangeException("numThreads");

			threads = new List<Thread>(numThreads);
			queue = new Queue<WaitQueueItem>();
			workWaiting = new Semaphore(0, int.MaxValue);

			for (int i = 0; i < numThreads; i++)
			{
				ThreadStart ts;
				if (customRun != null)
					ts = delegate { customRun(InternalRun); };
				else
					ts = InternalRun;

				var t = new Thread(ts) { IsBackground = true };
				t.Name = String.Format("p{0}.{1}", poolId, t.ManagedThreadId);
				threads.Add(t);
				t.Start();
			}
		}

		public int PooledCount
		{
			get { return running + queue.Count; }
		}

		/// <summary>
		/// Waits for all queued work to finish, or until 
		/// <paramref name="millis"/> time-out.
		/// </summary>
		/// <param name="millis">The number of milliseconds to wait</param>
		/// <returns>
		/// <c>true</c> if all queued work is finished, 
		/// <c>false</c> if <paramref name="millis"/> timed-out.
		/// </returns>
		public bool WaitAll(int millis)
		{
			while (PooledCount > 0 && millis > 0)
			{
				int wait = Math.Min(150, millis);
				Thread.Sleep(wait);
				millis -= wait;
			}

			return PooledCount == 0;
		}

		/// <summary>
		/// Waits indefinitely for all queued work to finish.
		/// </summary>
		public void WaitAll()
		{
			while (PooledCount > 0)
				Thread.Sleep(150);
		}

		public void ClearQueue()
		{
			lock (queue) queue.Clear();
		}

		/// <summary>
		/// Interrupts and releases all threads.
		/// </summary>
		public void Dispose()
		{
			if (threads == null)
				return;

			threads.ForEach(t => t.Interrupt());
			threads = null;
		}

		public void QueueUserWorkItem(WaitCallback callback)
		{
			QueueUserWorkItem(callback, null);
		}

		public void QueueUserWorkItem(WaitCallback callback, object state)
		{
			if (threads == null)
				throw new ObjectDisposedException(GetType().Name);
			if (callback == null) throw new ArgumentNullException("callback");

			var item = new WaitQueueItem { Callback = callback, State = state, Context = ExecutionContext.Capture() };

			lock (queue) queue.Enqueue(item);
			workWaiting.Release();
		}

		private void InternalRun()
		{
			try
			{
				while (true)
				{
					workWaiting.WaitOne();

					WaitQueueItem item;
					lock (queue) item = queue.Dequeue();
					try
					{
						Interlocked.Increment(ref running);
						ExecutionContext.Run(item.Context,
						                     new ContextCallback(item.Callback), item.State);
					}
					finally
					{
						Interlocked.Decrement(ref running);
					}
				}
			}
			catch (ThreadInterruptedException)
			{
			}
		}

		private class WaitQueueItem
		{
			public WaitCallback Callback;
			public object State;
			public ExecutionContext Context;
		}
	}
}