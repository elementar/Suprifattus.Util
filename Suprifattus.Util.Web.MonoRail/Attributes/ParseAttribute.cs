using System;
using System.Reflection;

using Castle.MonoRail.Framework;

namespace Suprifattus.Util.Web.MonoRail.Attributes
{
	/// <summary>
	/// When used in a <see cref="Controller"/> parameter,
	/// calls the parameter type's <c>ParseArray</c> or <c>ParseElement</c> method.
	/// </summary>
	/// <remarks>
	/// The methods shoud use the following signature:
	/// <code>public static ParseElement(String)</code>
	/// and
	/// <code>public static ParseArray(String[])</code>
	/// They will be called depending on the <see cref="Controller"/>'s parameter
	/// usage.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Parameter)]
	public class ParseAttribute : Attribute, IParameterBinder
	{
		string parameterName;

		public string ParameterName
		{
			get { return parameterName; }
			set { parameterName = value; }
		}

		public int CalculateParamPoints(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			string val = GetParameterValue(controller, parameterInfo);
			return String.IsNullOrEmpty(val) ? 0 : 10;
		}

		/// <summary>
		/// The signature of the <c>ParseElement</c> method.
		/// </summary>
		static readonly Type[] ParseElementSignature = {typeof(String)};

		/// <summary>
		/// The signature of the <c>ParseArray</c> method.
		/// </summary>
		static readonly Type[] ParseArraySignature = { typeof(String[]) };

		/// <summary>
		/// The binding flags of the <c>ParseElement</c> and <c>ParseArray</c> methods.
		/// </summary>
		const BindingFlags ParseBindingFlags = BindingFlags.Static | BindingFlags.Public;

		/// <summary>
		/// Binds a controller's parameter to a value.
		/// </summary>
		/// <param name="controller">The controller</param>
		/// <param name="parameterInfo">The parameter</param>
		/// <returns>The parsed value</returns>
		/// <exception cref="RailsException">
		/// If it could not find the <c>ParseElement</c> or <c>ParseArray</c> method.
		/// </exception>
		public object Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			Type type = parameterInfo.ParameterType;
			bool isArray = type.HasElementType;
			if (isArray)
				type = type.GetElementType();

			string methodName = "Parse" + (isArray ? "Array" : "Element");
			Type[] methodSig = isArray ? ParseArraySignature : ParseElementSignature;
			MethodInfo mi = type.GetMethod(methodName, ParseBindingFlags, null, methodSig, null);
			if (mi == null)
				throw new RailsException(String.Format("Parsing {0} is not supported by the {1} class. Make sure the class implements a public static {2}(String) method.", (isArray ? "an array" : "an element"), type, methodName));

			object vals = isArray
			              	? (object) GetParameterValues(controller, parameterInfo)
			              	: GetParameterValue(controller, parameterInfo);
			return mi.Invoke(null, new object[] {vals});
		}

		private string[] GetParameterValues(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			return controller.Request.Params.GetValues(GetParameterName(parameterInfo));
		}

		private string GetParameterValue(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			return controller.Request.Params.Get(GetParameterName(parameterInfo));
		}

		private string GetParameterName(ParameterInfo p)
		{
			return ParameterName ?? p.Name;
		}
	}
}