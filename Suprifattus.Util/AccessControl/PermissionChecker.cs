#undef DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Suprifattus.Util.AccessControl.Impl;
using Suprifattus.Util.Collections;

namespace Suprifattus.Util.AccessControl
{
	/// <summary>
	/// Verifica permiss�es.
	/// </summary>
	[Obsolete("Utilizado apenas no projeto CCS")]
	public class PermissionChecker
	{
		/// <summary>
		/// Retorna as permiss�es demandadas pelo fluxo de execu��o atual -
		/// classes, m�todos e assemblies.
		/// </summary>
		/// <returns>Um vetor com as permiss�es necess�rias para execu��o do c�digo atual.</returns>
		public static IPermission[] GetDemandedPermissions()
		{
			return GetDemandedPermissions(0);
		}

		/// <summary>
		/// Verifica se o usu�rio tem uma permiss�o espec�fica.
		/// </summary>
		/// <param name="user">O usu�rio</param>
		/// <param name="perm">A permiss�o</param>
		/// <param name="msgFormat">Formato da mensagem de erro. Utilizar {0} para nome do usu�rio e {1} para nome da permiss�o.</param>
		public static void CheckPermission(IExtendedPrincipal user, IPermission perm, string msgFormat)
		{
			if (!HasPermission(user, perm))
				throw new AccessDeniedException(user, perm, msgFormat);
		}

		/// <summary>
		/// Verifica se o usu�rio satisfaz as permiss�es demandadas.
		/// Caso o usu�rio n�o tenha permiss�o, lan�a uma exce��o de seguran�a.
		/// </summary>
		/// <param name="user">Usu�rio</param>
		/// <param name="msgFormat">Formato da mensagem de erro. Utilizar {0} para nome do usu�rio e {1} para nome da permiss�o.</param>
		/// <exception cref="AccessDeniedException">Caso o usu�rio n�o tenha as permiss�es necess�rias no contexto atual.</exception>
		public static void CheckPermissions(IExtendedPrincipal user, string msgFormat)
		{
			CheckPermissions(null, user, msgFormat);
		}

		/// <summary>
		/// Verifica se o usu�rio satisfaz as permiss�es demandadas.
		/// Caso o usu�rio n�o tenha permiss�o, lan�a uma exce��o de seguran�a.
		/// </summary>
		/// <param name="t">O tipo para verificar.</param>
		/// <param name="user">Usu�rio</param>
		/// <param name="msgFormat">Formato da mensagem de erro. Utilizar {0} para nome do usu�rio e {1} para nome da permiss�o.</param>
		/// <exception cref="AccessDeniedException">Caso o usu�rio n�o tenha as permiss�es necess�rias no contexto atual.</exception>
		public static void CheckPermissions(Type t, IExtendedPrincipal user, string msgFormat)
		{
			if (user == null)
				throw new ArgumentNullException("user", "User must not be NULL.");

			foreach (IPermission perm in GetDemandedPermissions(t, 1))
				CheckPermission(user, perm, msgFormat);
		}

		/// <summary>
		/// Verifica se o usu�rio satisfaz as permiss�es demandadas.
		/// Caso o usu�rio n�o tenha permiss�o, lan�a uma exce��o de seguran�a.
		/// </summary>
		/// <param name="en">As permiss�es ou attributos <see cref="DemandPermissionsAttribute"/> a verificar.</param>
		/// <param name="user">Usu�rio</param>
		/// <param name="msgFormat">Formato da mensagem de erro. Utilizar {0} para nome do usu�rio e {1} para nome da permiss�o.</param>
		/// <exception cref="AccessDeniedException">Caso o usu�rio n�o tenha as permiss�es necess�rias no contexto atual.</exception>
		public static void CheckPermissions(IExtendedPrincipal user, IEnumerable en, string msgFormat)
		{
			if (user == null)
				throw new ArgumentNullException("user", "User must not be NULL.");

			foreach (object obj in en)
			{
				if (obj is DemandPermissionsAttribute)
					foreach (IPermission perm in ((DemandPermissionsAttribute) obj).Permissions)
						CheckPermission(user, perm, msgFormat);

				if (obj is IPermission)
					CheckPermission(user, (IPermission) obj, msgFormat);
			}
		}

		/// <summary>
		/// Verifica se o usu�rio tem todas as permiss�es necess�rias
		/// no contexto atual.
		/// </summary>
		/// <param name="user">O usu�rio</param>
		/// <exception cref="AccessDeniedException">Caso o usu�rio n�o tenha as permiss�es necess�rias no contexto atual.</exception>
		public static void CheckPermissions(IExtendedPrincipal user)
		{
			CheckPermissions(user, null);
		}

		/// <summary>
		/// Verifica se o usu�rio tem uma permiss�o espec�fica.
		/// </summary>
		/// <param name="user">O usu�rio</param>
		/// <param name="perm">O c�digo da permiss�o a ser verificada</param>
		/// <returns>Verdadeiro se o usu�rio tem a permiss�o, falso caso contr�rio</returns>
		public static bool HasPermission(IExtendedPrincipal user, string perm)
		{
			return HasPermission(user, Permission.GetPermission(perm));
		}

		/// <summary>
		/// Verifica se o usu�rio tem uma permiss�o espec�fica.
		/// </summary>
		/// <param name="user">O usu�rio</param>
		/// <param name="perm">A permiss�o a ser verificada</param>
		/// <returns>Verdadeiro se o usu�rio tem a permiss�o, falso caso contr�rio</returns>
		public static bool HasPermission(IExtendedPrincipal user, IPermission perm)
		{
			foreach (IRole role in user.Roles)
				if (role.HasPermission(perm))
					return true;

			return false;
		}

		/// <summary>
		/// Retorna as permiss�es demandadas pelo fluxo de execu��o atual -
		/// classes, m�todos e assemblies.
		/// </summary>
		/// <param name="skipFrames">A quantidade de m�todos a ignorar. �til para encapsulamento
		/// da verifica��o de permiss�es. Se voc� n�o sabe do que se trata, utilize zero (0).</param>
		/// <returns>Um vetor com as permiss�es necess�rias para execu��o do c�digo atual.</returns>
		public static IPermission[] GetDemandedPermissions(int skipFrames)
		{
			return GetDemandedPermissions(null, skipFrames);
		}

		/// <summary>
		/// Retorna as permiss�es demandadas pelo fluxo de execu��o atual -
		/// classes, m�todos e assemblies.
		/// </summary>
		/// <param name="t">O tipo para verificar</param>
		/// <param name="skipFrames">A quantidade de m�todos a ignorar. �til para encapsulamento
		/// da verifica��o de permiss�es. Se voc� n�o sabe do que se trata, utilize zero (0).</param>
		/// <returns>Um vetor com as permiss�es necess�rias para execu��o do c�digo atual.</returns>
		public static IPermission[] GetDemandedPermissions(Type t, int skipFrames)
		{
#if DEBUG
			Debug.WriteLine("GetDemandedPermissions");
			Debug.IndentLevel++;
#endif

			var st = new StackTrace(2 + skipFrames, false);
			var permSet = new HashSet<IPermission>();

			if (t != null)
			{
				var fromType = GetAttribute(t);
				if (fromType != null)
					foreach (var perm in fromType.Permissions)
						permSet.Add(perm);
			}

			for (var i = 0; i < st.FrameCount; i++)
			{
				var frame = st.GetFrame(i);
				var method = frame.GetMethod();

				DemandPermissionsAttribute
					fromMethod = GetAttribute(method),
					fromClass = GetAttribute(method.DeclaringType),
					fromAssembly = GetAttribute(method.DeclaringType.Assembly);

				if (fromMethod != null) permSet.AddRange(fromMethod.Permissions);
				if (fromClass != null) permSet.AddRange(fromClass.Permissions);
				if (fromAssembly != null) permSet.AddRange(fromAssembly.Permissions);
			}

#if DEBUG
			Debug.IndentLevel--;
#endif

			return (IPermission[]) CollectionUtils.ToArray(typeof(IPermission), permSet);
		}

		private static DemandPermissionsAttribute GetAttribute(MethodBase mi)
		{
#if DEBUG
			Debug.WriteLine("Checking method " + mi);
#endif
			return (DemandPermissionsAttribute) Attribute.GetCustomAttribute(mi, typeof(DemandPermissionsAttribute));
		}

		private static DemandPermissionsAttribute GetAttribute(Type t)
		{
#if DEBUG
			Debug.WriteLine("Checking type " + t);
#endif
			return (DemandPermissionsAttribute) Attribute.GetCustomAttribute(t, typeof(DemandPermissionsAttribute));
		}

		private static DemandPermissionsAttribute GetAttribute(Assembly asm)
		{
#if DEBUG
			Debug.WriteLine("Checking assembly " + asm);
#endif
			return (DemandPermissionsAttribute) Attribute.GetCustomAttribute(asm, typeof(DemandPermissionsAttribute));
		}
	}
}