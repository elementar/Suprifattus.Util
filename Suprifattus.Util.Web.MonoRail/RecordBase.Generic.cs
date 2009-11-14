using System;
using System.Collections;
using System.Text;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Internal;

using NHibernate.Expression;

using Suprifattus.Util.Exceptions;

namespace Suprifattus.Util.Web.MonoRail
{
	[ActiveRecordSkip]
	public abstract class RecordBase<T> : ActiveRecordBase<T>
		where T: class
	{
		[Obsolete("Não é necessário com NHibernate 1.2")] protected const string GenericAcessorCamelCase = "NHibernate.Generics.GenericAccessor+CamelCase, NHibernate.Generics";

		#region Dump
		public virtual string Dump()
		{
			var sb = new StringBuilder();

			ActiveRecordModel model = ActiveRecordModel.GetModel(typeof(T));
			sb.Append(model.Type.Name);
			sb.Append(" { ");

			if (model.PrimaryKey != null)
				sb.AppendFormat("{0} = '{1}'; ", model.PrimaryKey.Property.Name, model.PrimaryKey.Property.GetValue(this, null));

			DumpProperties(sb, this, null, model.Properties);

			foreach (NestedModel nm in model.Components)
			{
				object n = nm.Property.GetValue(this, null);
				if (n == null)
					sb.AppendFormat("{0}.* = (null); ", nm.Property.Name);
				else
					DumpProperties(sb, n, nm, nm.Model.Properties);
			}
			sb.Append("}");

			return sb.ToString();
		}

		private void DumpProperties(StringBuilder sb, object obj, NestedModel nested, IList properties)
		{
			foreach (PropertyModel m in properties)
			{
				if (nested != null)
					sb.AppendFormat("{0}.", nested.Property.Name);

				if ((m.PropertyAtt.ColumnType ?? "").EndsWith("Blob"))
					sb.AppendFormat("{0} = (blob); ", m.Property.Name);
				else
				{
					object value = m.Property.GetValue(obj, null);
					if (NullableHelper.IsNull(value))
						sb.AppendFormat("{0} = (null); ", m.Property.Name);
					else
						sb.AppendFormat("{0} = '{1}'; ", m.Property.Name, value);
				}
			}
		}
		#endregion

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
			Save(true);
		}

		public virtual void Save(bool flush)
		{
			try
			{
				CheckUniqueness();
				if (flush)
					base.SaveAndFlush();
				else
					base.Save();
			}
			catch (ActiveRecordException ex)
			{
				Exception baseEx = ex;
				while ((baseEx = baseEx.InnerException) != null)
					CheckSaveException(ex, baseEx);
				throw;
			}
		}

		public override void Delete()
		{
			this.Delete(true);
		}

		public virtual void Delete(bool flush)
		{
			if (flush)
				base.DeleteAndFlush();
			else
				base.Delete();
		}

		public static T Find(object id, bool throwOnNotFound)
		{
			return FindByPrimaryKey(id, throwOnNotFound);
		}

		public static T[] FindAll(params Order[] orders)
		{
			return FindAll(orders, new ICriterion[0]);
		}

		public static T[] FindAll(ICriterion criterion, params Order[] orders)
		{
			return FindAll(orders, criterion);
		}

		public static T[] FindAll(ICriterion[] criterion, params Order[] orders)
		{
			return FindAll(orders, criterion);
		}

		protected static R Execute<R>(NHibernateDelegate call)
		{
			return (R) Execute(call, null);
		}

		public override string ToString()
		{
			return GetType().Name + "#" + ActiveRecordModel.GetModel(GetType()).PrimaryKey.Property.GetValue(this, null);
		}

		protected virtual void CheckSaveException(Exception rootEx, Exception ex)
		{
			if (ex.Message.Contains("Cannot insert duplicate key "))
				throw new BusinessRuleViolationException("Registro Duplicado", "Não foi possível salvar os dados. Já existe um registro com estas informações.", rootEx);

			if (ex.GetType().Name == "NpgsqlException" && ex.Message.Contains("ERROR: 23505"))
				throw new BusinessRuleViolationException("Registro Duplicado", "Não foi possível salvar os dados. Já existe um registro com estas informações.", rootEx);
		}
	}
}