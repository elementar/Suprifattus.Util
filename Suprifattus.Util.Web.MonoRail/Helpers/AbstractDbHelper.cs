using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Internal;
using Castle.MonoRail.Framework.Helpers;

using NHibernate;

using Suprifattus.Util.Data;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
#if GENERICS
	using NullableInt32 = Nullable<int>;
#else
	using Nullables;
#endif

	/// <summary>
	/// Utilizada para facilitar a busca de metadados do banco de dados.
	/// </summary>
	/// <remarks>
	/// Esta implementação utiliza o <see cref="DbMetadataCache"/>
	/// do <c>Suprifattus.Util.Data</c>, que por sua vez utiliza
	/// o método <see cref="System.Data.IDataReader.GetSchemaTable"/>
	/// para obter os metadados.
	/// 
	/// Esta implementação também utiliza apenas um 
	/// <see cref="DbMetadataCache"/> para todas as tabelas. Caso
	/// haja duas conexões diferentes, é necessário alterar a
	/// implementação para utilizar um
	/// <see cref="DbMetadataCache"/> para cada conexão.
	/// </remarks>
	public abstract class AbstractDbHelper : AbstractHelper
	{
		private static readonly object staticSync = new object();

		private static HybridDictionary type2lengths = new HybridDictionary();
		private static DbMetadataCache mdCache = null;

		/// <summary>
		/// Obtém o tamanho máximo de um campo no banco de dados.
		/// </summary>
		/// <param name="qualifiedField">O campo e a tabela</param>
		/// <returns>
		/// O tamanho máximo do campo, ou 
		/// <c>null</c> caso não seja
		/// possível obter o tamanho máximo.
		/// </returns>
		public NullableInt32 MaxLength(string qualifiedField)
		{
			string[] s = qualifiedField.Split(new char[] {'.'}, 2);
			return MaxLength(s[0], s[1]);
		}

		/// <summary>
		/// Obtém o tamanho máximo de um campo no banco de dados.
		/// </summary>
		/// <param name="table">A tabela</param>
		/// <param name="field">O campo</param>
		/// <returns>
		/// O tamanho máximo do campo, ou 
		/// <c>null</c> caso não seja
		/// possível obter o tamanho máximo.
		/// </returns>
		public NullableInt32 MaxLength(string table, string field)
		{
			Type t = GetModelType(table);
			if (t == null) return null;

			EnsureMetadataCache(t);

			ActiveRecordModel model = ActiveRecordModel.GetModel(t);
			if (model == null) return null;

			IDictionary type2len = (IDictionary) type2lengths[t];
			if (type2len == null)
			{
				lock (type2lengths.SyncRoot)
				{
					type2len = new HybridDictionary(true);

					string physicalTableName = model.ActiveRecordAtt.Table;

					foreach (PropertyModel p in model.Properties)
					{
						string physicalColumnName = p.PropertyAtt.Column;
						type2len.Add(p.Property.Name, mdCache.MaxLength(physicalTableName, physicalColumnName));
					}

					type2lengths[t] = type2len;
				}
			}

			if (!type2len.Contains(field))
				return null;

			return (int) type2len[field];
		}

		/// <summary>
		/// Obtém o <see cref="Type"/> referente à tabela especificada.
		/// </summary>
		/// <param name="table">O nome da tabela, no <see cref="Castle.ActiveRecord"/></param>
		/// <returns>O <see cref="Type"/></returns>
		protected abstract Type GetModelType(string table);

		/// <summary>
		/// Garante que o <see cref="mdCache"/> estará criado.
		/// </summary>
		/// <param name="t">
		/// O tipo, para criação do 
		/// <see cref="NHibernateDbConnectionFactory"/>.
		/// </param>
		private static void EnsureMetadataCache(Type t)
		{
			if (mdCache == null)
				lock (staticSync)
				{
					NHibernateDbConnectionFactory factory =
						new NHibernateDbConnectionFactory(ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(t));
					mdCache = new DbMetadataCache(factory);
				}
		}

		/// <summary>
		/// Implementação de <see cref="IDbConnectionFactory"/>,
		/// que utiliza o 
		/// </summary>
		private class NHibernateDbConnectionFactory : IDbConnectionFactory
		{
			private ISessionFactory fac;

			public NHibernateDbConnectionFactory(ISessionFactory fac)
			{
				this.fac = fac;
			}

			public IDbConnection GetConnection()
			{
				return fac.ConnectionProvider.GetConnection();
			}
		}
	}
}