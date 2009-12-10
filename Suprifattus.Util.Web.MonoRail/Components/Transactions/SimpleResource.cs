using System;

using Castle.Services.Transaction;

namespace Suprifattus.Util.Web.MonoRail.Components.Transactions
{
	public class SimpleResource : IResource
	{
		public SimpleResource(Action commitAction, Action rollbackAction)
		{
			this.CommitAction = commitAction;
			this.RollbackAction = rollbackAction;
		}

		public SimpleResource(Action commitAction)
		{
			this.CommitAction = commitAction;
		}

		public Action StartAction { get; set; }
		public Action CommitAction { get; set; }
		public Action RollbackAction { get; set; }

		public void Start()
		{
			if (StartAction != null)
				StartAction();
		}

		public void Commit()
		{
			if (CommitAction != null)
				CommitAction();
		}

		public void Rollback()
		{
			if (RollbackAction != null)
				RollbackAction();
		}
	}
}