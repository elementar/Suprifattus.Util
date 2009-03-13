using System;
using System.Collections.Specialized;

using Castle.MonoRail.Framework.Helpers;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	public class MathHelper : AbstractHelper
	{
		public int Min(int n1, int n2)
		{
			return Math.Min(n1, n2);
		}

		public double Add(params double[] nums)
		{
			double r = 0;
			foreach (double d in nums)
				r += d;

			return r;
		}

		public float Add(params float[] nums)
		{
			float r = 0;
			foreach (float d in nums)
				r += d;

			return r;
		}

		public int Add(params int[] nums)
		{
			int r = 0;
			foreach (int d in nums)
				r += d;

			return r;
		}

		public DateTime AddDays(DateTime dt, double days)
		{
			return dt.AddDays(days);
		}

		#region Var
		private HybridDictionary vars;

		public Variable Var(string id)
		{
			if (vars == null)
				vars = new HybridDictionary();

			var var = (Variable) vars[id];
			if (var == null)
				vars.Add(id, var = new Variable());

			return var;
		}

		public class Variable
		{
			private decimal value;

			public Variable Clear()
			{
				this.value = 0;
				return this;
			}

			public Variable Set(object value)
			{
				if (value != null)
					this.value = Convert.ToDecimal(value);
				return this;
			}

			public Variable Add(object value)
			{
				if (value != null)
					this.value += Convert.ToDecimal(value);
				return this;
			}

			public Variable Mul(object value)
			{
				if (value != null)
					this.value *= Convert.ToDecimal(value);
				return this;
			}

			public Variable Div(object value)
			{
				if (value != null)
					this.value /= Convert.ToDecimal(value);
				return this;
			}

			public string Show()
			{
				return Convert.ToString(this.value);
			}

			public string Show(string format)
			{
				return this.value.ToString(format);
			}

			public override string ToString()
			{
				return String.Empty;
			}
		}
		#endregion
	}
}