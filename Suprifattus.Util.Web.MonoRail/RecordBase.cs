using System;
using System.Collections;
using System.Reflection;
using System.Text;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Internal;
using Castle.Core.Logging;

using NHibernate.Expression;

using Suprifattus.Util.Text;

namespace Suprifattus.Util.Web.MonoRail
{
	public interface IRecord
	{
		object Id { get; }
	}

	[ActiveRecordSkip]
	public abstract class RecordBase : ActiveRecordValidationBase, IEscapable
	{
		protected static string Quote(string s)
		{
			return s.Replace("'", "''");
		}

		/// <summary>
		/// Deve ser sobrescrito nas classes que devem verificar por
		/// registros únicos. Ao encontrar um registro duplicado,
		/// lançar um <see cref="DuplicatedRecordException"/>.
		/// </summary>
		protected virtual void CheckUniqueness()
		{
		}

		public override void Save()
		{
			try
			{
				CheckUniqueness();
				base.Save();
			}
/*
			catch (ValidationException ex)
			{
				throw new AppValidationException(ex, this);
			}
*/
			catch (ActiveRecordException ex)
			{
				Exception baseEx = ex.GetBaseException();
				if (baseEx != null)
				{
					string msg = baseEx.Message;

					// PostgreSQL: duplicate key violates unique constraint
					if (msg.Contains("ERROR: 23505"))
						if (msg.Contains("_pkey"))
							throw new DuplicatedRecordException("Não foi possível salvar os dados. Já existe um registro com esta chave primária. Verifique as 'sequences'.", ex);
						else
							throw new DuplicatedRecordException("Não foi possível salvar os dados. Já existe um registro com estas informações.", ex);

					// SQL Server
					if (msg.StartsWith("Cannot insert duplicate key row"))
						throw new DuplicatedRecordException("Não foi possível salvar os dados. Já existe um registro com estas informações.", ex);
				}

				throw;
			}
		}

		protected static void DumpToLog(ILogger log, string prefix, Type type, object obj)
		{
			if (!log.IsDebugEnabled)
				return;

			try
			{
				log.Debug(prefix + ": " + Dump(type, obj));
			}
			catch (Exception ex)
			{
				log.Error("Erro ao realizar Dump do AR", ex);
			}
		}

		protected static string Dump(Type type, object obj)
		{
			StringBuilder sb = new StringBuilder();

			ActiveRecordModel model = ActiveRecordModel.GetModel(type);
			sb.Append(model.Type.Name);
			sb.Append(" { ");

			if (model.PrimaryKey != null)
				sb.AppendFormat("{0} = '{1}'; ", model.PrimaryKey.Property.Name, model.PrimaryKey.Property.GetValue(obj, null));

			foreach (PropertyModel m in model.Properties)
				sb.AppendFormat("{0} = '{1}'; ", m.Property.Name, m.Property.GetValue(obj, null));

			foreach (NestedModel nm in model.Components)
			{
				object n = nm.Property.GetValue(obj, null);
				if (n == null)
					sb.AppendFormat("{0} = (null); ", nm.Property.Name);
				else
					foreach (PropertyModel m in nm.Model.Properties)
						sb.AppendFormat("{0}.{1} = '{2}'; ", nm.Property.Name, m.Property.Name, m.Property.GetValue(n, null));
			}

			foreach (BelongsToModel m in model.BelongsTo)
			{
				sb.AppendFormat("{0} = ", m.Property.Name);

				object val = m.Property.GetValue(obj, null);
				if (val == null)
					sb.Append("(null)");
				else
				{
					sb.Append("[");
					ActiveRecordModel vm = ActiveRecordModel.GetModel(m.BelongsToAtt.Type);
					if (vm.PrimaryKey != null)
						sb.AppendFormat("{0} = '{1}'", vm.PrimaryKey.Property.Name, vm.PrimaryKey.Property.GetValue(val, null));
					sb.Append("]");
				}

				sb.Append("; ");
			}

//			foreach (HasManyModel m in model.HasMany)
//			{
//				sb.AppendFormat("{0} = (", m.Property.Name);
//				IEnumerable v = (IEnumerable) m.Property.GetValue(obj, null);
//				if (v != null)
//				{
//					ActiveRecordModel vm = ActiveRecordModel.GetModel(m.HasManyAtt.MapType);
//					foreach (object relObject in v)
//					{
//						sb.Append("[");
//						foreach (PrimaryKeyModel pkModel in vm.Ids)
//							sb.AppendFormat("{0} = '{1}'; ", pkModel.Property.Name, pkModel.Property.GetValue(relObject, null));
//						sb.Append("]; ");
//					}
//				}
//				sb.Append("); ");
//			}

			sb.Append("}");

			return sb.ToString();
		}

		#region FindFirst
		protected virtual internal object FindFirst(params ICriterion[] criterias)
		{
			return FindFirst(GetType(), criterias);
		}

		protected virtual internal object FindFirst(Order[] orders, params ICriterion[] criterias)
		{
			return FindFirst(GetType(), orders, criterias);
		}
		#endregion

		protected internal static bool Exists(Type t, object pk, ICriterion criteria)
		{
			PrimaryKeyModel pkModel = ActiveRecordModel.GetModel(t).PrimaryKey;
			string pkField = pkModel.Property.Name;
			object o = FindFirst(t, Expression.And(criteria, Expression.Not(Expression.Eq(pkField, pk))));
			return o != null;
		}

		#region DeleteAll
		protected static int DeleteAll(Type t, ICollection pks)
		{
			const string CouldNotDeleteMessage =
				"Não foi possível excluir o[:s] registro[:s] selecionado[:s].\n" +
				"Verifique se esse[:s] registro[:s] est[á:ão] relacionado[:s] " +
				"a outro[:s] dado[:s] do sistema.";

			try
			{
				return ActiveRecordBase.DeleteAll(t, pks);
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException is ActiveRecordException)
					throw new CouldNotDeleteException(PluralForm.Format(pks.Count, CouldNotDeleteMessage), ex);

				throw;
			}
			catch (ActiveRecordException ex)
			{
				throw new CouldNotDeleteException(PluralForm.Format(pks.Count, CouldNotDeleteMessage), ex);
			}
#if !DO_NOT_USE_CUSTOM_MESSAGES
			catch (CouldNotDeleteException ex)
			{
				throw new CouldNotDeleteException(PluralForm.Format(pks.Count, CouldNotDeleteMessage), ex);
			}
#endif
		}
		#endregion

		public override string ToString()
		{
			return GetType().Name + "#" + ActiveRecordModel.GetModel(GetType()).PrimaryKey.Property.GetValue(this, null);
		}
	}
}