using System;
using System.Collections.Generic;

using Suprifattus.Util.AccessControl.Impl;

namespace Suprifattus.Util.AccessControl
{
	/// <summary>
	/// Construtor de usu�rios.
	/// </summary>
	public class UserBuilder : IDisposable
	{
		private int id;
		private string name, login;
		private bool auth;
		private Dictionary<string, HashSet<IPermission>> roles = new Dictionary<string, HashSet<IPermission>>();
		private Dictionary<string, object> props = new Dictionary<string, object>();

		/// <summary>
		/// Constr�i um novo construtor de usu�rios.
		/// </summary>
		/// <param name="id">ID do usu�rio</param>
		/// <param name="auth">Se o usu�rio estar� autenticado ou n�o.</param>
		/// <param name="name">Nome do usu�rio</param>
		public UserBuilder(int id, bool auth, string name)
		{
			this.id = id;
			this.name = name;
			this.auth = auth;
		}

		/// <summary>
		/// Constr�i um novo construtor de usu�rios.
		/// </summary>
		/// <param name="id">ID do usu�rio</param>
		/// <param name="auth">Se o usu�rio estar� autenticado ou n�o.</param>
		public UserBuilder(int id, bool auth)
			: this(id, auth, null)
		{
		}

		/// <summary>
		/// O c�digo do usu�rio.
		/// </summary>
		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		/// <summary>
		/// O nome do usu�rio.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// O login do usu�rio.
		/// </summary>
		public string Login
		{
			get { return login; }
			set { login = value; }
		}

		/// <summary>
		/// Se o usu�rio est� autenticado ou n�o.
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
		/// Adiciona uma permiss�o.
		/// </summary>
		/// <param name="role">O papel da permiss�o</param>
		/// <param name="perm">A permiss�o</param>
		public void AddPermission(string role, IPermission perm)
		{
			HashSet<IPermission> perms;
			if (!roles.TryGetValue(role, out perms))
				roles.Add(role, perms = new HashSet<IPermission>());
			perms.Add(perm);
		}

		/// <summary>
		/// Define uma propriedade do objeto de usu�rio.
		/// </summary>
		/// <param name="propertyName">O nome da propriedade</param>
		/// <param name="propertyValue">O valor da propriedade</param>
		public void SetProperty(string propertyName, object propertyValue)
		{
			props.Add(propertyName, propertyValue);
		}

		/// <summary>
		/// Constr�i o usu�rio.
		/// </summary>
		/// <returns>O usu�rio.</returns>
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