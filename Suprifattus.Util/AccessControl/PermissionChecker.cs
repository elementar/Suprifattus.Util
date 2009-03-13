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
	/// Verifica permissões.
	/// </summary>
	[Obsolete("Utilizado apenas no projeto CCS")]
	public class PermissionChecker
	{
		/// <summary>
		/// Retorna as permissões demandadas pelo fluxo de execução atual -
		/// classes, métodos e assemblies.
		/// </summary>
		/// <returns>Um vetor com as permissões necessárias para execução do código atual.</returns>
		public static IPermission[] GetDemandedPermissions()
		{
			return GetDemandedPermissions(0);
		}

		/// <summary>
		/// Verifica se o usuário tem uma permissão específica.
		/// </summary>
		/// <param name="user">O usuário</param>
		/// <param name="perm">A permissão</param>
		/// <param name="msgFormat">Formato da mensagem de erro. Utilizar {0} para nome do usuário e {1} para nome da permissão.</param>
		public static void CheckPermission(IExtendedPrincipal user, IPermission perm, string msgFormat)
		{
			if (!HasPermission(user, perm))
				throw new AccessDeniedException(user, perm, msgFormat);
		}

		/// <summary>
		/// Verifica se o usuário satisfaz as permissões demandadas.
		/// Caso o usuário não tenha permissão, lança uma exceção de segurança.
		/// </summary>
		/// <param name="user">Usuário</param>
		/// <param name="msgFormat">Formato da mensagem de erro. Utilizar {0} para nome do usuário e {1} para nome da permissão.</param>
		/// <exception cref="AccessDeniedException">Caso o usuário não tenha as permissões necessárias no contexto atual.</exception>
		public static void CheckPermissions(IExtendedPrincipal user, string msgFormat)
		{
			CheckPermissions(null, user, msgFormat);
		}

		/// <summary>
		/// Verifica se o usuário satisfaz as permissões demandadas.
		/// Caso o usuário não tenha permissão, lança uma exceção de segurança.
		/// </summary>
		/// <param name="t">O tipo para verificar.</param>
		/// <param name="user">Usuário</param>
		/// <param name="msgFormat">Formato da mensagem de erro. Utilizar {0} para nome do usuário e {1} para nome da permissão.</param>
		/// <exception cref="AccessDeniedException">Caso o usuário não tenha as permissões necessárias no contexto atual.</exception>
		public static void CheckPermissions(Type t, IExtendedPrincipal user, string msgFormat)
		{
			if (user == null)
				throw new ArgumentNullException("user", "User must not be NULL.");

			foreach (IPermission perm in GetDemandedPermissions(t, 1))
				CheckPermission(user, perm, msgFormat);
		}

		/// <summary>
		/// Verifica se o usuário satisfaz as permissões demandadas.
		/// Caso o usuário não tenha permissão, lança uma exceção de segurança.
		/// </summary>
		/// <param name="en">As permissões ou attributos <see cref="DemandPermissionsAttribute"/> a verificar.</param>
		/// <param name="user">Usuário</param>
		/// <param name="msgFormat">Formato da mensagem de erro. Utilizar {0} para nome do usuário e {1} para nome da permissão.</param>
		/// <exception cref="AccessDeniedException">Caso o usuário não tenha as permissões necessárias no contexto atual.</exception>
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
		/// Verifica se o usuário tem todas as permissões necessárias
		/// no contexto atual.
		/// </summary>
		/// <param name="user">O usuário</param>
		/// <exception cref="AccessDeniedException">Caso o usuário não tenha as permissões necessárias no contexto atual.</exception>
		public static void CheckPermissions(IExtendedPrincipal user)
		{
			CheckPermissions(user, null);
		}

		/// <summary>
		/// Verifica se o usuário tem uma permissão específica.
		/// </summary>
		/// <param name="user">O usuário</param>
		/// <param name="perm">O código da permissão a ser verificada</param>
		/// <returns>Verdadeiro se o usuário tem a permissão, falso caso contrário</returns>
		public static bool HasPermission(IExtendedPrincipal user, string perm)
		{
			return HasPermission(user, Permission.GetPermission(perm));
		}

		/// <summary>
		/// Verifica se o usuário tem uma permissão específica.
		/// </summary>
		/// <param name="user">O usuário</param>
		/// <param name="perm">A permissão a ser verificada</param>
		/// <returns>Verdadeiro se o usuário tem a permissão, falso caso contrário</returns>
		public static bool HasPermission(IExtendedPrincipal user, IPermission perm)
		{
			foreach (IRole role in user.Roles)
				if (role.HasPermission(perm))
					return true;

			return false;
		}

		/// <summary>
		/// Retorna as permissões demandadas pelo fluxo de execução atual -
		/// classes, métodos e assemblies.
		/// </summary>
		/// <param name="skipFrames">A quantidade de métodos a ignorar. Útil para encapsulamento
		/// da verificação de permissões. Se você não sabe do que se trata, utilize zero (0).</param>
		/// <returns>Um vetor com as permissões necessárias para execução do código atual.</returns>
		public static IPermission[] GetDemandedPermissions(int skipFrames)
		{
			return GetDemandedPermissions(null, skipFrames);
		}

		/// <summary>
		/// Retorna as permissões demandadas pelo fluxo de execução atual -
		/// classes, métodos e assemblies.
		/// </summary>
		/// <param name="t">O tipo para verificar</param>
		/// <param name="skipFrames">A quantidade de métodos a ignorar. Útil para encapsulamento
		/// da verificação de permissões. Se você não sabe do que se trata, utilize zero (0).</param>
		/// <returns>Um vetor com as permissões necessárias para execução do código atual.</returns>
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