using System;

namespace Suprifattus.Util.DesignPatterns
{
	/// <summary>
	/// Command Design Pattern.
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// Executes the command action.
		/// </summary>
		void Execute();
	}
}
