using System;
using System.Collections;
using System.Data;

namespace Suprifattus.Util.PdfGeneration.Objects.Tables
{
#if GENERICS
	using NullableInt32 = Nullable<Int32>;
	using NullableInt16 = Nullable<Int16>;
	using NullableInt64 = Nullable<Int64>;
	using NullableDouble = Nullable<Double>;
	using NullableSingle = Nullable<Single>;
	using NullableSByte = Nullable<SByte>;
	using NullableByte = Nullable<Byte>;
	using NullableDecimal = Nullable<Decimal>;
	using NullableDateTime = Nullable<DateTime>;
	using NullableBoolean = Nullable<Boolean>;

#else
	using Nullables;
#endif

	/// <summary>
	/// Calcula a largura das colunas de dados.
	/// </summary>
	internal class ColumnStyleInferer
	{
		private readonly ColumnCollection columns;
		private readonly ColumnMetadataHelper mdHelper;

		/// <summary>
		/// Cria um novo objeto responsável por inferir os estilos das colunas, 
		/// com o auxílio de um <see cref="ColumnMetadataHelper"/>.
		/// </summary>
		/// <param name="mdHelper">O objeto responsável por auxiliar a formatar colunas específicas.</param>
		/// <param name="columns">A coleção contendo as colunas</param>
		public ColumnStyleInferer(ColumnMetadataHelper mdHelper, ColumnCollection columns)
		{
			this.mdHelper = mdHelper;
			this.columns = columns;
		}

		/// <summary>
		/// Os estilos das colunas.
		/// Prefira adicionar os estilos através do método <see cref="Infer(DataColumn)"/>.
		/// </summary>
		public ColumnCollection Columns
		{
			get { return columns; }
		}

		/// <summary>
		/// Limpa o objeto e o devolve ao estado inicial.
		/// </summary>
		public void Reset()
		{
			Columns.Clear();
		}

		/// <summary>
		/// Infere o estilo de uma coluna, baseado em seu nome e tipo de dados.
		/// </summary>
		/// <param name="columnName">O nome da coluna</param>
		/// <param name="columnDataType">O tipo de dados da coluna</param>
		/// <returns>O novo estilo adicionado.</returns>
		public Column Infer(string columnName, Type columnDataType)
		{
			if (columnDataType == null)
				throw new ArgumentNullException("columnDataType", "Column type can't be null");

			return InternalInfer(columnName, columnDataType);
		}

		/// <summary>
		/// Infere o estilo de uma coluna, baseado em seus metadados de coluna
		/// (objeto <see cref="DataColumn"/>).
		/// </summary>
		/// <param name="col">O objeto <see cref="DataColumn"/></param>
		/// <returns>O novo estilo adicionado.</returns>
		public Column Infer(DataColumn col)
		{
			if (col == null)
				throw new ArgumentNullException("col", "DataColumn can't be null");

			return InternalInfer(col.ColumnName, col.DataType);
		}

		#region InternalInfer
		private Column InternalInfer(string columnName, Type columnDataType)
		{
			var column = new Column(columnName);

			if (IsSimpleType(columnDataType))
			{
				if (IsShortNumeric(columnDataType))
				{
					column.Width = 5;
					column.Style.Alignment = TextAlignment.Center;
				}
				else if (IsLongNumeric(columnDataType))
				{
					column.Width = 10;
					column.Style.Alignment = TextAlignment.Right;
				}
				else if (IsCurrency(columnDataType))
				{
					column.Width = 10;
					column.FormatString = "R$ {0:0.00}";
					column.Style.Alignment = TextAlignment.Right;
				}
				else if (IsDate(columnDataType))
				{
					column.Width = 10;
					column.FormatString = "{0:dd/MM/yyyy}";
					column.Style.Alignment = TextAlignment.Center;
				}
				else if (IsBoolean(columnDataType))
				{
					column.Width = 5;
					column.MapValue("", "--");
					column.MapValue("True", "Sim");
					column.MapValue("False", "Não");
					column.Style.Alignment = TextAlignment.Center;
				}
			}

			if (mdHelper != null)
				mdHelper.ComplementStyle(column);

			return column;
		}
		#endregion

		/// <summary>
		/// Calcula a largura física real das colunas fixas, 
		/// e distribui o espaço restante entre os elementos
		/// que não tem largura fixa.
		/// </summary>
		/// <param name="aproxCharLength">A largura aproximada dos caracteres na página.</param>
		/// <param name="maxWidth">A largura máxima</param>
		public void ScaleBy(double aproxCharLength, double maxWidth)
		{
			double totalFixed = 0;
			var countFlex = 0;

			foreach (Column col in Columns)
				if (col.Width >= 0)
				{
					col.Width *= aproxCharLength;
					totalFixed += col.Width;
				}
				else
					countFlex++;

			if (countFlex > 0)
			{
				// se há flexíveis, distribui o espaço que sobra entre eles.
				var undefinedWidth = (maxWidth - totalFixed) / countFlex;

				foreach (Column col in Columns)
					if (col.Width < 0)
						col.Width = undefinedWidth;
			}
			else
			{
				// se todos são fixos, expande igualmente todas as colunas
				var missingWidth = maxWidth - totalFixed;
				double c = Columns.Count;
				foreach (Column style in Columns)
				{
					var x = missingWidth / c;
					style.Width += x;
					missingWidth -= x;
					c--;
				}
			}
		}

		#region Type Checking
		private bool IsSimpleType(Type columnDataType)
		{
#if !GENERICS
			return columnDataType.IsValueType || typeof(INullableType).IsAssignableFrom(columnDataType);
#else
			return columnDataType.IsValueType || NullableHelper.IsNullableType(columnDataType);
#endif
		}

		private bool IsShortNumeric(Type t)
		{
			return
				t == typeof(short) || t == typeof(NullableInt16) ||
				t == typeof(ushort) ||
				t == typeof(sbyte) || t == typeof(NullableSByte) ||
				t == typeof(byte) || t == typeof(NullableByte);
		}

		private bool IsLongNumeric(Type t)
		{
			return
				t == typeof(int) || t == typeof(uint) || t == typeof(NullableInt32) ||
				t == typeof(long) || t == typeof(ulong) || t == typeof(NullableInt64) ||
				t == typeof(double) || t == typeof(float) || t == typeof(NullableDouble);
		}

		private bool IsCurrency(Type t)
		{
			return
				t == typeof(Decimal) || t == typeof(NullableDecimal);
		}

		private bool IsDate(Type t)
		{
			return
				t == typeof(DateTime) || t == typeof(NullableDateTime);
		}

		private bool IsBoolean(Type t)
		{
			return
				t == typeof(bool) || t == typeof(bool2) || t == typeof(NullableBoolean);
		}
		#endregion

		public IEnumerator GetEnumerator()
		{
			return Columns.GetEnumerator();
		}
	}
}