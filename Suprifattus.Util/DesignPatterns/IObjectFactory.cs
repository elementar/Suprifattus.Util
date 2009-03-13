using System;

namespace Suprifattus.Util.DesignPatterns
{
#if GENERICS
	/// <summary>
	/// Cria outros objetos do tipo <typeparamref name="T"/>
	/// </summary>
	/// <typeparam name="T">O tipo de objeto que é criado por este <see cref="IObjectFactory&lt;T&gt;"/></typeparam>
	[CLSCompliant(false)]
	public interface IObjectFactory<T>
	{
		/// <summary>
		/// Cria um novo objeto do tipo <typeparamref name="T"/>
		/// </summary>
		/// <returns>Uma instância de um objeto do tipo <typeparamref name="T"/></returns>
		T Create();
	}
	
	public class ObjectFactory<T> : IObjectFactory<T>
	{
		public delegate T CreateDelegate();
		private CreateDelegate del;

		public ObjectFactory(CreateDelegate del)
		{
			this.del = del;
		}

		public T Create()
		{
			return del();
		}
	}

	public class ObjectFactory<T,P> : IObjectFactory<T,P>
	{
		public delegate T CreateDelegate(P parameter);
		private CreateDelegate del;

		public ObjectFactory(CreateDelegate del)
		{
			this.del = del;
		}

		public T Create(P parameter)
		{
			return del(parameter);
		}
	}

	/// <summary>
	/// Cria outros objetos do tipo <typeparamref name="T"/>, recebendo 
	/// como parâmetro objetos do tipo <typeparamref name="P"/>.
	/// </summary>
	/// <typeparam name="T">O tipo de objeto que é criado por este <see cref="IObjectFactory&lt;T&gt;"/></typeparam>
	/// <typeparam name="P">O tipo de objeto que é utilizado para criar objetos do tipo <typeparamref name="T"/></typeparam>
	[CLSCompliant(false)]
	public interface IObjectFactory<T, P>
	{
		/// <summary>
		/// Cria um novo objeto do tipo <typeparamref name="T"/>
		/// </summary>
		/// <param name="param">O parâmetro do tipo <typeparamref name="P"/> utilizado para criar a instância do objeto do tipo <typeparamref name="T"/></param>
		/// <returns>Uma instância de um objeto do tipo <typeparamref name="T"/></returns>
		T Create(P param);
	}
#endif

	/// <summary>
	/// Cria outros objetos.
	/// </summary>
	public interface IObjectFactory
	{
		/// <summary>
		/// Cria um novo objeto.
		/// </summary>
		/// <returns>Uma instância de um objeto.</returns>
		object Create();
	}
}
