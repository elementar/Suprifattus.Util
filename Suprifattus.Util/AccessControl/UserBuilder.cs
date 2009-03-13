using System;
using System.Collections.Generic;

using Suprifattus.Util.AccessControl.Impl;

namespace Suprifattus.Util.AccessControl
{
	/// <summary>
	/// Construtor de usuários.
	/// </summary>
	public class UserBuilder : IDisposable
	{
		private int id;
		private string name, login;
		private bool auth;
		private Dictionary<string, HashSet<IPermission>> roles = new Dictionary<string, HashSet<IPermission>>();
		private Dictionary<string, object> props = new Dictionary<string, object>();

		/// <summary>
		/// Constrói um novo construtor de usuários.
		/// </summary>
		/// <param name="id">ID do usuário</param>
		/// <param name="auth">Se o usuário estará autenticado ou não.</param>
		/// <param name="name">Nome do usuário</param>
		public UserBuilder(int id, bool auth, string name)
		{
			this.id = id;
			this.name = name;
			this.auth = auth;
		}

		/// <summary>
		/// Constrói um novo construtor de usuários.
		/// </summary>
		/// <param name="id">ID do usuário</param>
		/// <param name="auth">Se o usuário estará autenticado ou não.</param>
		public UserBuilder(int id, bool auth)
			: this(id, auth, null)
		{
		}

		/// <summary>
		/// O código do usuário.
		/// </summary>
		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		/// <summary>
		/// O nome do usuário.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// O login do usuário.
		/// </summary>
		public string Login
		{
			get { return login; }
			set { login = value; }
		}

		/// <summary>
		/// Se o usuário está autenticado ou não.
		/// </summary>
		public bool Authenticated
		{
			get { return auth; }
			set { auth = value; }
		}

		/// <summary>
		/// Adiciona um papel simples.
		/// </summary>
		public void AddRole(string roleName)
		{
			roles.Add(roleName, null);
		}

		/// <summary>
		/// Adiciona uma permissão.
		/// </summary>
		/// <param name="role">O papel da permissão</param>
		/// <param name="perm">A permissão</param>
		public void AddPermission(string role, IPermission perm)
		{
			HashSet<IPermission> perms;
			if (!roles.TryGetValue(role, out perms))
				roles.Add(role, perms = new HashSet<IPermission>());
			perms.Add(perm);
		}

		/// <summary>
		/// Define uma propriedade do objeto de usuário.
		/// </summary>
		/// <param name="propertyName">O nome da propriedade</param>
		/// <param name="propertyValue">O valor da propriedade</param>
		public void SetProperty(string propertyName, object propertyValue)
		{
			props.Add(propertyName, propertyValue);
		}

		/// <summary>
		/// Constrói o usuário.
		/// </summary>
		/// <returns>O usuário.</returns>
		public UserBase Build()
		{
			var roleArray = new RoleBase[roles.Count];
			var i = 0;
			foreach (var roleEntry in roles)
				roleArray[i++] = new RoleBase(roleEntry.Key, roleEntry.Value);

			var user = new UserBase(id, auth, name, login, roleArray);
			foreach (var di in props)
				user.Properties.Add(di.Key, di.Value);

			return user;
		}

		/// <summary>
		/// Libera os recursos utilizados pelo <see cref="UserBuilder"/>.
		/// </summary>
		public void Dispose()
		{
			if (roles != null) roles.Clear();
			if (roles is IDisposable) ((IDisposable) roles).Dispose();

			if (props != null) props.Clear();
			if (props is IDisposable) ((IDisposable) props).Dispose();

			roles = null;
			props = null;
			name = null;
			login = null;

			GC.SuppressFinalize(this);
		}
	}
}