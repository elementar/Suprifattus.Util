using System;

namespace Suprifattus.Util.Data.Sql
{
	/// <summary>
	/// Interface que deve ser implementada por consultas SQL.
	/// </summary>
	public interface ISqlQuery
	{
		/// <summary>
		/// Os campos
		/// </summary>
		SqlFieldCollection Fields { get; }
		/// <summary>
		/// A ordem
		/// </summary>
		SqlOrder Order { get; set; }

		/// <summary>
		/// Retorna a representa��o string da consulta.
		/// </summary>
		/// <returns>A representa��o string da consulta.</returns>
		string ToString();
	}
}
