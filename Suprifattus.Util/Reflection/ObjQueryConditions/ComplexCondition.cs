using System;
using System.Diagnostics;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// As operações possíveis entre condições.
	/// </summary>
	public enum Operation
	{
		/// <summary>
		/// E
		/// </summary>
		And,
		/// <summary>
		/// Ou
		/// </summary>
		Or,
	}

	/// <summary>
	/// Condição complexa: uma condição composta por outras condições aninhadas.
	/// </summary>
	internal class ComplexCondition : Condition
	{
		private Condition[] conds;
		private Operation op;

		/// <summary>
		/// Cria uma nova condição complexa.
		/// </summary>
		/// <param name="op">Um operador, de acordo com a enumeração <see cref="Operation"/></param>
		/// <param name="conds">As condições a serem aninhadas</param>
		public ComplexCondition(Operation op, params Condition[] conds)
		{
			this.op = op;
			this.conds = conds;
		}

		/// <summary>
		/// Verifica se esta condição é satisfeita para o objeto <paramref name="obj"/>
		/// </summary>
		/// <param name="obj">O objeto a ser verificado</param>
		/// <returns>Verdadeiro se a condição é safisfeita, falso caso contrário</returns>
		public override bool Satisfied(object obj)
		{
			bool result = (op == Operation.And ? true : false);
			foreach (Condition cond in conds)
			{
				Debug.Assert(!Object.ReferenceEquals(this, cond), "Recursion detected!");

				switch (op)
				{
					case Operation.And:
						result = result && cond.Satisfied(obj);
						if (!result)
							return result;
						break;
					case Operation.Or:
						result = result || cond.Satisfied(obj);
						if (result)
							return result;
						break;
				}
			}

			return result;
		}

		/// <summary>
		/// Libera o objeto de condição.
		/// </summary>
		public override void Dispose()
		{
			this.conds = null;
			base.Dispose();
		}
	}
}
