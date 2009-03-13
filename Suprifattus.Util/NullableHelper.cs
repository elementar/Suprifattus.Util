using System;

namespace Suprifattus.Util
{
	/// <summary>
	/// Classe utilit�ria para lidar com <c>Nullables</c>
	/// no .NET 2.0 e na biblioteca personalizada Nullables.
	/// </summary>
	public sealed class NullableHelper
	{
		/// <summary>
		/// Verifica se um valor � nulo, considerando <c>Nullables</c>.
		/// </summary>
		public static bool IsNull(object obj)
		{
			return obj == null || obj is DBNull;
		}

		/// <summary>
		/// Obt�m o valor.
		/// </summary>
		public static object GetValue(object nullableObject)
		{
			if (IsNull(nullableObject))
				return null;
			return nullableObject;
		}

		/// <summary>
		/// Verifica se um tipo especificado � <c>Nullable</c> ou n�o.
		/// </summary>
		public static bool IsNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
	}
}