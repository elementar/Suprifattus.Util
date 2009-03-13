using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace Suprifattus.Util.Web.MonoRail.Components
{
	/// <summary>
	/// Pode ser utilizado com <see cref="XmlDocument"/>,
	/// <see cref="XslCompiledTransform"/> ou qualquer outro objeto que tenha
	/// um construtor padrão e um método <c>Load(String)</c>, para realizar 
	/// recarregamento automático na alteração do arquivo.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Reloadable<T>
		where T: class
	{
		private T target;
		private readonly DateTime lastModification = DateTime.MinValue;
		private readonly MethodInfo loadMethod;
		private readonly ConstructorInfo constructor;

		public Reloadable(T initial, string fileName)
			: this(fileName)
		{
			this.target = initial;
		}

		public Reloadable(string fileName)
		{
			this.FileName = fileName;

			this.constructor = typeof(T).GetConstructor(new Type[0]);
			this.loadMethod = typeof(T).GetMethod("Load", new[] { typeof(string) });
		}

		public string FileName { get; set; }

		public T Target
		{
			get
			{
				CheckReload();
				return target;
			}
		}

		private void CheckReload()
		{
			var mod = File.GetLastWriteTime(FileName);
			if (target == null && lastModification == mod)
				return;

			lock (this)
			{
				Reload();
			}
		}

		private void Reload()
		{
			this.target = (T) this.constructor.Invoke(new object[0]);
			this.loadMethod.Invoke(this.target, new object[] { this.FileName });
		}
	}
}