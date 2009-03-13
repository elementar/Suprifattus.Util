using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

using Castle.MonoRail.Framework.Helpers;

namespace Suprifattus.Util.Web.MonoRail.Helpers
{
	public class RuntimeInfoHelper : AbstractHelper
	{
		private static readonly Regex
			appAssemblies = new Regex(@"^(Suprifattus\.(SGLab|GED)|GraficLaser|AudiContas)", RegexOptions.Compiled),
			notLibraryAssemblies = new Regex(@"^(Suprifattus\.(SGLab|GED)|GraficLaser|AudiContas|RegexAssembly|DynamicAssemblyProxyGen|App_|[a-z0-9_-]{8})", RegexOptions.Compiled);

		private static string softwareVersion;

		public Version FrameworkVersion
		{
			get { return Environment.Version; }
		}

		public OperatingSystem OperationalSystem
		{
			get { return Environment.OSVersion; }
		}

		public long MemoryAllocated
		{
			get { return Environment.WorkingSet / 1024 / 1024; }
		}

		public long MemoryUsed
		{
			get { return GC.GetTotalMemory(false) / 1024 / 1024; }
		}

		public DateTime CurrentTime
		{
			get { return DateTime.Now; }
		}

		public string SoftwareVersion
		{
			get
			{
				if (softwareVersion == null)
				{
					string[] fullPath = Controller.Context.Server.MapPath("~/").Split('/', '\\');
					string ver = fullPath[fullPath.Length - 2].Replace("-release", "");
					if (ver.IndexOf('.') == -1)
						ver = "(dev)";

					softwareVersion = ver;
				}
				return softwareVersion;
			}
		}

		public Assembly[] LibraryAssemblies
		{
			get
			{
				var asms = new ArrayList(50);
				Assembly[] all = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly asm in all)
					if (!notLibraryAssemblies.IsMatch(asm.FullName))
						asms.Add(asm);

				asms.Sort(AsmComparer.Instance);
				return (Assembly[]) asms.ToArray(typeof(Assembly));
			}
		}

		public Assembly[] ApplicationAssemblies
		{
			get
			{
				var asms = new ArrayList(20);
				Assembly[] all = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly asm in all)
					if (appAssemblies.IsMatch(asm.FullName))
						asms.Add(asm);

				asms.Sort(AsmComparer.Instance);
				return (Assembly[]) asms.ToArray(typeof(Assembly));
			}
		}

		#region AsmComparer
		private class AsmComparer : IComparer
		{
			public static readonly AsmComparer Instance = new AsmComparer();

			private AsmComparer()
			{
			}

			public int Compare(object x, object y)
			{
				Assembly asm1 = (Assembly) x, asm2 = (Assembly) y;
				return String.Compare(asm1.GetName().Name, asm2.GetName().Name);
			}
		}
		#endregion
	}
}