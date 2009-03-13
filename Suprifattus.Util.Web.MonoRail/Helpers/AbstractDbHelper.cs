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
	/// Esta implementa��o utiliza o <see cref="DbMetadataCache"/>
	/// do <c>Suprifattus.Util.Data</c>, que por sua vez utiliza
	/// o m�todo <see cref="System.Data.IDataReader.GetSchemaTable"/>
	/// para obter os metadados.
	/// 
	/// Esta implementa��o tamb�m utiliza apenas um 
	/// <see cref="DbMetadataCache"/> para todas as tabelas. Caso
	/// haja duas conex�es diferentes, � necess�rio alterar a
	/// implementa��o para utilizar um
	/// <see cref="DbMetadataCache"/> para cada conex�o.
	/// </remarks>
	public abstract class AbstractDbHelper : AbstractHelper
	{
		private static readonly object staticSync = new object();

		private static HybridDictionary type2lengths = new HybridDictionary();
		private static DbMetadataCache mdCache = null;

		/// <summary>
		/// Obt�m o tamanho m�ximo de um campo no banco de dados.
		/// </summary>
		/// <param name="qualifiedField">O campo e a tabela</param>
		/// <returns>
		/// O tamanho m�ximo do campo, ou 
		/// <c>null</c> caso n�o seja
		/// poss�vel obter o tamanho m�ximo.
		/// </returns>
		public NullableInt32 MaxLength(string qualifiedField)
		{
			string[] s = qualifiedField.Split(new char[] {'.'}, 2);
			return MaxLength(s[0], s[1]);
		}

		/// <summary>
		/// Obt�m o tamanho m�ximo de um campo no banco de dados.
		/// </summary>
		/// <param name="table">A tabela</param>
		/// <param name="field">O campo</param>
		/// <returns>
		/// O tamanho m�ximo do campo, ou 
		/// <c>null</c> caso n�o seja
		/// poss�vel obter o tamanho m�ximo.
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
		/// Obt�m o <see cref="Type"/> referente � tabela especificada.
		/// </summary>
		/// <param name="table">O nome da tabela, no <see cref="Castle.ActiveRecord"/></param>
		/// <returns>O <see cref="Type"/></returns>
		protected abstract Type GetModelType(string table);

		/// <summary>
		/// Garante que o <see cref="mdCache"/> estar� criado.
		/// </summary>
		/// <param name="t">
		/// O tipo, para cria��o do 
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
		/// Implementa��o de <see cref="IDbConnectionFactory"/>,
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