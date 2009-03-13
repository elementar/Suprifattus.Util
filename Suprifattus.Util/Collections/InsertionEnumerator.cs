using System;
using System.Collections;

namespace Suprifattus.Util.Collections
{
	public class InsertionEnumerator : EnumeratorBase
	{
		private bool inserting = false;
		private object currentInsertion;
		private Queue insertionQueue;

		public InsertionEnumerator(IEnumerator en)
			: base(en)
		{
		}

		protected void Insert(object o)
		{
			if (insertionQueue == null)
				insertionQueue = new Queue();

			insertionQueue.Enqueue(o);
		}

		public override bool MoveNext()
		{
			if (insertionQueue != null && insertionQueue.Count > 0)
			{
				currentInsertion = insertionQueue.Dequeue();
				inserting = true;
				return true;
			}
			else
				inserting = false;

			return base.MoveNext();
		}

		public override object Current
		{
			get { return (inserting ? currentInsertion : base.Current); }
		}

	}
}
