using System;
using System.Diagnostics;
using System.Reflection;

namespace Suprifattus.Util.Reflection
{
	/// <summary>
	/// Uma condição.
	/// </summary>
	public abstract class Condition : IDisposable
	{
		/// <summary>
		/// Cria uma nova condição.
		/// </summary>
		protected Condition()
		{
		}

		/// <summary>
		/// Verifica se esta condição é satisfeita para o objeto <paramref name="obj"/>
		/// </summary>
		/// <param name="obj">O objeto a ser verificado</param>
		/// <returns>Verdadeiro se a condição é safisfeita, falso caso contrário</returns>
		public abstract bool Satisfied(object obj);

		/// <summary>
		/// Cria uma nova <see cref="Condition"/> comparando as duas condições fornecidas,
		/// utilizando o operador <see cref="Operation.Or"/>.
		/// </summary>
		/// <param name="c1">Uma condição</param>
		/// <param name="c2">Outra condição</param>
		/// <returns>Uma nova condição, comparando as condições <paramref name="c1"/>
		/// e <paramref name="c2"/> utilizando o operator <see cref="Operation.Or"/></returns>
		public static Condition operator |(Condition c1, Condition c2)
		{
			return new ComplexCondition(Operation.Or, c1, c2);
		}

		/// <summary>
		/// Cria uma nova <see cref="Condition"/> comparando as duas condições fornecidas,
		/// utilizando o operador <see cref="Operation.And"/>.
		/// </summary>
		/// <param name="c1">Uma condição</param>
		/// <param name="c2">Outra condição</param>
		/// <returns>Uma nova condição, comparando as condições <paramref name="c1"/>
		/// e <paramref name="c2"/> utilizando o operator <see cref="Operation.And"/></returns>
		public static Condition operator &(Condition c1, Condition c2)
		{
			return new ComplexCondition(Operation.And, c1, c2);
		}

		/// <summary>
		/// Cria uma nova <see cref="Condition"/> negando a condição fornecida.
		/// </summary>
		/// <param name="c">A condição</param>
		/// <returns>Uma nova condição que nega a condição fornecida em <paramref name="c"/></returns>
		public static Condition operator !(Condition c)
		{
			return new NegatedCondition(c);
		}

		/// <summary>
		/// Libera o objeto de condição.
		/// </summary>
		public virtual void Dispose() {
		}
	}
}
