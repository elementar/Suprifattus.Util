using System;
using System.Collections;

using Suprifattus.Util.Data.XBind;

namespace Suprifattus.Util.Collections
{
	/// <summary>
	/// Um enumerador que percorre outro, agrupando pelo valor de uma propriedade.
	/// </summary>
	public class GroupByEnumerable : IEnumerable
	{
		private readonly IEnumerable source;
		private readonly string field;
		private readonly int maxPerGroup;

		/// <summary>
		/// Cria um <see cref="GroupByEnumerable"/>, percorrendo os dados em
		/// <paramref name="source"/>, e agrupando pelo campo em <paramref name="field"/>.
		/// </summary>
		public GroupByEnumerable(IEnumerable source, string field)
			: this(source, field, -1)
		{
		}

		/// <summary>
		/// Cria um <see cref="GroupByEnumerable"/>, percorrendo os dados em
		/// <paramref name="source"/>, agrupando pelo campo em <paramref name="field"/>,
		/// com no máximo <paramref name="maxPerGroup"/> itens por grupo.
		/// </summary>
		public GroupByEnumerable(IEnumerable source, string field, int maxPerGroup)
		{
			this.source = source;
			this.field = field;
			this.maxPerGroup = maxPerGroup;
		}

		/// <summary>
		/// Enumera.
		/// </summary>
		public IEnumerator GetEnumerator()
		{
			return new GroupByEnumerator(source, field, maxPerGroup);
		}
	}

	/// <summary>
	/// Enumerador que agrupa os itens.
	/// Útil em linguagens de template, como NVelocity.
	/// </summary>
	public class GroupByEnumerator : IEnumerator
	{
		private static readonly XBindContext xbind = new XBindContext();
		private readonly IEnumerable source;
		private readonly string field;
		private readonly int maxItensPerGroup = -1;

		private IEnumerator en;
		private GroupEnumerator currentGroup;

		/// <summary>
		/// Cria um novo <see cref="GroupByEnumerator"/>.
		/// </summary>
		/// <param name="source">O enumerável original</param>
		/// <param name="field">A propriedade do enumerável que representa o grupo</param>
		public GroupByEnumerator(IEnumerable source, string field)
		{
			this.source = source;
			this.field = field;

			Reset();
		}

		/// <summary>
		/// Cria um novo <see cref="GroupByEnumerator"/>.
		/// </summary>
		/// <param name="source">O enumerável original</param>
		/// <param name="field">A propriedade do enumerável que representa o grupo</param>
		/// <param name="maxItensPerGroup">O número máximo de itens por grupo</param>
		public GroupByEnumerator(IEnumerable source, string field, int maxItensPerGroup)
		{
			this.source = source;
			this.field = field;
			this.maxItensPerGroup = maxItensPerGroup;

			Reset();
		}

		/// <summary>
		/// Move para o próximo item.
		/// </summary>
		public bool MoveNext()
		{
			// se não tem mais elementos, não tem mais grupos
			if (currentGroup == null && !en.MoveNext())
				return false;

			this.currentGroup = new GroupEnumerator(this);
			return true;
		}

		/// <summary>
		/// Reinicia o enumerador.
		/// </summary>
		public void Reset()
		{
			this.en = source.GetEnumerator();
			this.currentGroup = null;
		}

		/// <summary>
		/// Obtém o valor atual da propriedade pela qual é realizado o agrupamento.
		/// </summary>
		public object GroupValue
		{
			get { return currentGroup.GroupValue; }
		}

		/// <summary>
		/// Obtém o grupo atual.
		/// </summary>
		public object Current
		{
			get { return currentGroup; }
		}

		#region GroupEnumerator inner class
		public class GroupEnumerator : IEnumerator, IEnumerable
		{
			private readonly IEnumerator en;
			private readonly string field;
			private readonly object groupValue;
			private readonly GroupByEnumerator g;
			private static readonly XBindContext xbind = GroupByEnumerator.xbind;

			private int skip = 1;
			private int count;
			private bool got;

			public GroupEnumerator(GroupByEnumerator g)
			{
				this.g = g;
				this.en = g.en;
				this.field = g.field;
				this.groupValue = xbind.Resolve(en.Current, field);
			}

			#region GroupValue and synonyms
			public object GroupValue
			{
				get { return groupValue; }
			}

			public object Value
			{
				get { return groupValue; }
			}

			public object Group
			{
				get { return groupValue; }
			}

			public object Object
			{
				get { return groupValue; }
			}
			#endregion

			public bool MoveNext()
			{
				if (skip > 0)
				{
					skip--;
					return true;
				}

				if (!en.MoveNext())
				{
					g.currentGroup = null;
					return false;
				}

				if ((g.maxItensPerGroup != -1 && ++count >= g.maxItensPerGroup) || !Equals(GroupValue, xbind.Resolve(Current, field)))
					return false;

				return true;
			}

			public IEnumerator GetEnumerator()
			{
				if (!got)
				{
					got = true;
					return this;
				}

				throw new InvalidOperationException("Could not call GetEnumerator twice on a GroupEnumerator");
			}

			public void Reset()
			{
				throw new InvalidOperationException();
			}

			public object Current
			{
				get { return en.Current; }
			}
		}
		#endregion
	}
}