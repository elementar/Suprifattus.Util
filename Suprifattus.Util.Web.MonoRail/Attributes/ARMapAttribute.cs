using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Internal;
using Castle.MonoRail.Framework;

namespace Suprifattus.Util.Web.MonoRail.Attributes
{
	/// <summary>
	/// Mapeia objetos a valores digitados na tela.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public class ARMapAttribute : Attribute, IParameterBinder
	{
		private readonly Regex expr;

		/// <summary>
		/// Mapeia objetos a valores digitados na tela.
		/// </summary>
		/// <param name="expr">A expressão utilizada para mapear objetos.
		/// O grupo de captura <c>$1</c> deve resolver para um inteiro, que será a
		/// chave primária do objeto chave do dicionário.</param>
		public ARMapAttribute(string expr)
		{
			this.expr = new Regex(expr, RegexOptions.Compiled);
		}

		public bool AllowDuplicates { get; set; }

		int IParameterBinder.CalculateParamPoints(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			return controller.Params.Keys.OfType<string>().Any(expr.IsMatch) ? 20 : 0;
		}

		object IParameterBinder.Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			var genArgs = parameterInfo.ParameterType.GetGenericArguments();

			var keyType = genArgs[0];
			var valType = genArgs[1];

			var keyModel = ActiveRecordModel.GetModel(keyType);
			var valModel = ActiveRecordModel.GetModel(valType);

			var keyPKType = keyModel == null ? NormalizeNullable(keyType) : keyModel.PrimaryKey.Property.PropertyType;
			var valPKType = valModel == null ? NormalizeNullable(valType) : valModel.PrimaryKey.Property.PropertyType;

			var dict = (IDictionary) Activator.CreateInstance(parameterInfo.ParameterType);

			foreach (string requestKey in controller.Params.Keys)
			{
				var m = expr.Match(requestKey);
				if (!m.Success)
					continue;

				var keyContents = Convert.ChangeType(m.Groups[1].Value, keyPKType);
				if (keyModel != null)
					keyContents = ActiveRecordMediator.FindByPrimaryKey(keyType, keyContents);

				var valContents = (object) controller.Params[requestKey];
				if (String.Empty.Equals(valContents))
					valContents = null;

				if (valContents != null)
				{
					valContents = Convert.ChangeType(valContents, valPKType);
					if (valModel != null)
						valContents = ActiveRecordMediator.FindByPrimaryKey(valType, valContents);
				}

				if (AllowDuplicates)
					dict[keyContents] = valContents;
				else
					dict.Add(keyContents, valContents);
			}

			return dict;
		}

		private Type NormalizeNullable(Type valType)
		{
			if (valType.IsGenericType && valType.GetGenericTypeDefinition() == typeof(Nullable<>))
				valType = Nullable.GetUnderlyingType(valType);
			return valType;
		}
	}
}