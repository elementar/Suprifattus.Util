using System;
using System.Collections;
using System.Diagnostics;

namespace Suprifattus.Util.AccessControl.Impl
{
	/// <summary>
	/// Representa uma permissão
	/// </summary>
	[Serializable]
	public class Permission : IPermission
	{
		private static readonly Hashtable allPerms = new Hashtable();

		private readonly string id;
		private string name;

		private Permission(string id)
		{
			Debug.WriteLine("Criada nova permissão " + id);
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
		/// Obtém o objeto referente à permissão com o código especificado.
		/// Caso a permissão não exista, é criada uma nova.
		/// </summary>
		/// <param name="id">O código da permissão</param>
		/// <returns>O objeto <see cref="Permission"/> existente, ou um novo</returns>
		public static IPermission GetPermission(string id)
		{
			var perm = (IPermission) allPerms[id];
			if (perm == null)
				allPerms[id] = perm = new Permission(id);

			return perm;
		}

		/// <summary>
		/// Define os dados de uma permissão. Caso a permissão ainda não exista,
		/// cria a mesma.
		/// </summary>
		/// <param name="id">O código da permissão</param>
		/// <param name="name">O nome da permissão</param>
		/// <returns>A permissão criada ou modificada.</returns>
		public static IPermission SetPermission(string id, string name)
		{
			IPermission perm = GetPermission(id);
			if (perm is Permission)
			{
				((Permission) perm).name = name;
				Debug.WriteLine("Atribuído nome à permissão " + id + ": " + name);
			}
			return perm;
		}

		/// <summary>
		/// Retorna o código da permissão.
		/// </summary>
		public string ID
		{
			get { return id; }
		}

		/// <summary>
		/// Retorna o nome da permissão.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Verifica se uma permissão é igual à outra.
		/// </summary>
		/// <param name="that">A outra permissão</param>
		/// <returns>Verdadeiro se as permissões tem o mesmo código, falso caso contrário.</returns>
		public override bool Equals(object that)
		{
			if (that is string)
				return this.id == (string) that;

			if (that is IPermission)
				return this.id == ((IPermission) that).ID;

			return false;
		}

		/// <summary>
		/// Retorna o hashcode da permissão.
		/// </summary>
		/// <returns>O hashcode da permissão</returns>
		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		/// <summary>
		/// Retorna a representação string da permissão.
		/// </summary>
		/// <returns>A representação string da permissão.</returns>
		public override string ToString()
		{
			return (name ?? id);
		}
	}
}