using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Suprifattus.Util.Collections
{
	[Serializable]
	[Obsolete("Incompleto", true)]
	public class WeakDictionary : Hashtable
	{
		public WeakDictionary()
		{
		}

		protected WeakDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override void Add(object key, object value)
		{
			WeakReference weakVal = new WeakReference(value);
			base.Add(key, weakVal);
		}

		public override object this[object key]
		{
			get
			{
				WeakReference val = (WeakReference) base[key];
				return val.Target;
			}
			set
			{
				if (value != null)
					value = new WeakReference(value);
				base[key] = value;
			}
		}

		protected override bool KeyEquals(object item, object key)
		{
			return base.KeyEquals(item, ((WeakReference) key).Target);
		}

		protected override int GetHash(object key)
		{
			return base.GetHash(((WeakReference) key).Target);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}