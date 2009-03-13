using System;
using System.Collections;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Internal;

using Suprifattus.Util.Collections;

namespace Suprifattus.Util.Web.MonoRail
{
	public class ARLoader : IEnumerable
	{
		Type arType;
		IEnumerable keys;

		public ARLoader(Type arType, IEnumerable keys)
		{
			this.arType = arType;
			this.keys = keys;
		}

		public IEnumerator GetEnumerator()
		{
			return new ARLoaderEnumerator(arType, keys);
		}

		public class ARLoaderEnumerator : EnumeratorBase
		{
			PrimaryKeyModel pkModel;
			Type arType;
			object currentObject;

			public ARLoaderEnumerator(Type arType, IEnumerable keys)
				: base(keys.GetEnumerator())
			{
				this.pkModel = ActiveRecordModel.GetModel(arType).PrimaryKey;
				this.arType = arType;
			}

			public override bool MoveNext()
			{
				if (!base.MoveNext())
					return false;

				this.currentObject = LoadCurrentObject();
				return true;
			}

			private object LoadCurrentObject()
			{
				return ActiveRecordMediator.FindByPrimaryKey(arType, GetCurrentPK());
			}

			private object GetCurrentPK()
			{
				return Convert.ChangeType(base.Current, pkModel.Property.PropertyType);
			}

			public override object Current
			{
				get { return currentObject; }
			}
		}
	}
}