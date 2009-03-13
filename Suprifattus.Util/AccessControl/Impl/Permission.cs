using System;
using System.Collections;
using System.Diagnostics;

namespace Suprifattus.Util.AccessControl.Impl
{
	/// <summary>
	/// Representa uma permiss�o
	/// </summary>
	[Serializable]
	public class Permission : IPermission
	{
		private static readonly Hashtable allPerms = new Hashtable();

		private readonly string id;
		private string name;

		private Permission(string id)
		{
			Debug.WriteLine("Criada nova permiss�o " + id);
			this.id = id;
		}

		[Obsolete("Utilizado apenas no projeto CCS")]
		public void Check(IExtendedPrincipal user)
		{
			PermissionChecker.CheckPermission(user, this, null);
		}

		[Obsolete("Utilizado apenas no projeto CCS")]
		public bool Test(IExtendedPrincipal user)
		{
			return PermissionChecker.HasPermission(user, this);
		}

		/// <summary>
		/// Obt�m o objeto referente � permiss�o com o c�digo especificado.
		/// Caso a permiss�o n�o exista, � criada uma nova.
		/// </summary>
		/// <param name="id">O c�digo da permiss�o</param>
		/// <returns>O objeto <see cref="Permission"/> existente, ou um novo</returns>
		public static IPermission GetPermission(string id)
		{
			var perm = (IPermission) allPerms[id];
			if (perm == null)
				allPerms[id] = perm = new Permission(id);

			return perm;
		}

		/// <summary>
		/// Define os dados de uma permiss�o. Caso a permiss�o ainda n�o exista,
		/// cria a mesma.
		/// </summary>
		/// <param name="id">O c�digo da permiss�o</param>
		/// <param name="name">O nome da permiss�o</param>
		/// <returns>A permiss�o criada ou modificada.</returns>
		public static IPermission SetPermission(string id, string name)
		{
			IPermission perm = GetPermission(id);
			if (perm is Permission)
			{
				((Permission) perm).name = name;
				Debug.WriteLine("Atribu�do nome � permiss�o " + id + ": " + name);
			}
			return perm;
		}

		/// <summary>
		/// Retorna o c�digo da permiss�o.
		/// </summary>
		public string ID
		{
			get { return id; }
		}

		/// <summary>
		/// Retorna o nome da permiss�o.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Verifica se uma permiss�o � igual � outra.
		/// </summary>
		/// <param name="that">A outra permiss�o</param>
		/// <returns>Verdadeiro se as permiss�es tem o mesmo c�digo, falso caso contr�rio.</returns>
		public override bool Equals(object that)
		{
			if (that is string)
				return this.id == (string) that;

			if (that is IPermission)
				return this.id == ((IPermission) that).ID;

			return false;
		}

		/// <summary>
		/// Retorna o hashcode da permiss�o.
		/// </summary>
		/// <returns>O hashcode da permiss�o</returns>
		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		/// <summary>
		/// Retorna a representa��o string da permiss�o.
		/// </summary>
		/// <returns>A representa��o string da permiss�o.</returns>
		public override string ToString()
		{
			return (name ?? id);
		}
	}
}