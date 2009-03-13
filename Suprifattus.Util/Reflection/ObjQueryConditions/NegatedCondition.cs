using System;
using System.Diagnostics;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// Condição negada: uma condição que só é satisfeita se a condição agregada
	/// for falsa.
	/// </summary>
	internal class NegatedCondition : Condition
	{
		private Condition cond;

		/// <summary>
		/// Cria uma nova condição negada.
		/// </summary>
		/// <param name="cond">A condição a ser negada</param>
		public NegatedCondition(Condition cond)
		{
			this.cond = cond;
		}

		/// <summary>
		/// Verifica se esta condição é satisfeita para o objeto <paramref name="obj"/>
		/// </summary>
		/// <param name="obj">O objeto a ser verificado</param>
		/// <returns>Verdadeiro se a condição é safisfeita, falso caso contrário</returns>
		public override bool Satisfied(object obj)
		{
			return !cond.Satisfied(obj);
		}

		/// <summary>
		/// Libera o objeto de condição.
		/// </summary>
		public override void Dispose()
		{
			if (this.cond != null)
				this.cond.Dispose();

			this.cond = null;
			base.Dispose();
		}
	}
}
